using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Globalization;

namespace FileWatcher
{
    class FileLogger
    {
        private EncryptTools encrypter = new EncryptTools();
        private ArchiveBuilder archiver = new ArchiveBuilder();
        private readonly object obj = new object();
        private FileSystemWatcher watcher;
        private readonly StringBuilder messages = new StringBuilder();
        private readonly Tools tools;
        bool activation = true;
        public FileLogger(Tools tools)
        {
            this.tools = tools;

            if (!Directory.Exists(this.tools.SourcePath))
            {
                Directory.CreateDirectory(this.tools.SourcePath);
            }

            if (!Directory.Exists(this.tools.TargetPath))
            {
                Directory.CreateDirectory(this.tools.TargetPath);
            }

            watcher = new FileSystemWatcher(this.tools.SourcePath);
            watcher.Filter = "*.txt";
            watcher.Deleted += Delete;
            watcher.Created += Create;
            watcher.Changed += Change;
            watcher.Renamed += Rename;
            watcher.Created += FileTransfer;
        }

        public void WriteToFile(string message)
        {
            if (!Directory.Exists(tools.SourcePath))
            {
                Directory.CreateDirectory(tools.SourcePath);

                watcher = new FileSystemWatcher(tools.SourcePath);
                watcher.Deleted += Delete;
                watcher.Created += Create;
                watcher.Changed += Change;
                watcher.Renamed += Rename;

                watcher.EnableRaisingEvents = true;
            }

            if (!Directory.Exists(tools.TargetPath))
            {
                Directory.CreateDirectory(tools.TargetPath);
            }

            lock (obj)
            {
                using (StreamWriter sw = new StreamWriter(tools.LogFilePath, true))
                {
                    sw.Write(message);
                }
            }
        }

        public void StartService()
        {
            WriteToFile($"Service was started at {DateTime.Now:dd/MM/yyyy HH:mm:ss}\n");
            watcher.EnableRaisingEvents = true;
            while (activation)
            {
                Thread.Sleep(1000);
            }
        }

        public void StopService()
        {
            watcher.EnableRaisingEvents = false;
            activation = false;
            messages.Clear();
            WriteToFile($"Service was stopped at {DateTime.Now:dd/MM/yyyy HH:mm:ss}\n");
        }

        private void Create(object sender, FileSystemEventArgs File)
        {
            string Path = File.FullPath;
            string Event = "created";
            AddMessage(Path, Event);
        }

        private void Delete(object sender, FileSystemEventArgs File)
        {
            string Path = File.FullPath;
            string Event = "deleted";
            AddMessage(Path, Event);
        }

        private void FileTransfer(object sender, FileSystemEventArgs File)
        {
            if (!Directory.Exists(this.tools.SourcePath))
            {
                Directory.CreateDirectory(this.tools.SourcePath);
                watcher = new FileSystemWatcher(this.tools.SourcePath);
                watcher.Filter = "*.txt";
                watcher.Deleted += Delete;
                watcher.Created += Create;
                watcher.Changed += Change;
                watcher.Renamed += Rename;
                watcher.Created += FileTransfer;
            }

            if (!Directory.Exists(this.tools.TargetPath))
            {
                Directory.CreateDirectory(this.tools.TargetPath);
            }

            if (messages.Length > 0)
            {
                WriteToFile(messages.ToString());
                messages.Clear();
            }

            lock (obj)
            {
                try
                {
                    var directoryInfo = new DirectoryInfo(this.tools.SourcePath);
                    var Path = System.IO.Path.Combine(this.tools.SourcePath, File.Name);
                    var Name = File.Name;
                    var dt = DateTime.Now;
                    var AdditionalPath = System.IO.Path.Combine(dt.ToString("yyyy", DateTimeFormatInfo.InvariantInfo), dt.ToString("MM", DateTimeFormatInfo.InvariantInfo),
                                  dt.ToString("dd", DateTimeFormatInfo.InvariantInfo));
                    var newPathTarget = System.IO.Path.Combine(this.tools.TargetPath, System.IO.Path.GetFileNameWithoutExtension(Name) + "_" +
                      dt.ToString(@"yyyy_MM_dd_HH_mm_ss", DateTimeFormatInfo.InvariantInfo) + System.IO.Path.GetExtension(Name));
                    var newPath = System.IO.Path.Combine(this.tools.SourcePath, AdditionalPath, System.IO.Path.GetFileNameWithoutExtension(Name) + "_" +
                        dt.ToString(@"yyyy_MM_dd_HH_mm_ss", DateTimeFormatInfo.InvariantInfo) + System.IO.Path.GetExtension(Name));
                    var newPathSec = System.IO.Path.Combine(this.tools.SourcePath, AdditionalPath, System.IO.Path.GetFileNameWithoutExtension(Name) + "_" +
                        dt.ToString(@"yyyy_MM_dd_HH_mm_ss", DateTimeFormatInfo.InvariantInfo)+ "_1" + System.IO.Path.GetExtension(Name));
                    if (!directoryInfo.Exists)
                    {
                        directoryInfo.Create();
                    }
                    directoryInfo.CreateSubdirectory(AdditionalPath);

                    if (tools.NeedToEncrypt)
                    {
                        System.IO.File.Move(Path, newPath);                        
                        encrypter.EncryptFile(newPath, newPath);
                    }
                    else
                    {
                        System.IO.File.Move(Path, newPath);
                    }
                    
                    if(tools.Options.Compress)
                    {
                        var compPath = System.IO.Path.ChangeExtension(newPath, "gz");
                        var newCompPath = System.IO.Path.Combine(tools.TargetPath, System.IO.Path.GetFileName(compPath));
                        var decompPath = System.IO.Path.ChangeExtension(newCompPath, "txt");
                        archiver.Compress(newPath, compPath, tools.Options.Level);
                        System.IO.File.Delete(newPath);
                        System.IO.File.Move(compPath, newCompPath);
                        archiver.Decompress(newCompPath, decompPath);
                        System.IO.File.Delete(newCompPath);
                    }
                    else
                    {
                        System.IO.File.Move(newPath, System.IO.Path.Combine(tools.TargetPath, System.IO.Path.GetFileName(newPath)));               
                    }

                    var decryptPath = System.IO.Path.Combine(tools.TargetPath, System.IO.Path.GetFileName(newPath));

                    if (tools.NeedToEncrypt)
                    {
                        encrypter.DecryptFile(System.IO.Path.Combine(tools.TargetPath, System.IO.Path.GetFileName(newPath)), decryptPath);
                        System.IO.File.Delete(System.IO.Path.Combine(tools.TargetPath, System.IO.Path.GetFileName(newPath)));
                    }
                    else
                    {
                        System.IO.File.Move(System.IO.Path.Combine(tools.TargetPath, System.IO.Path.GetFileName(newPath)), decryptPath);
                    }

                    if(tools.Options.Archive)
                    {
                        archiver.Add(decryptPath, tools.TargetPath);
                        System.IO.File.Delete(decryptPath);
                    }                                
                }
                catch (Exception ex)
                {
                    using (StreamWriter sw = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Exceptions.txt"), true))
                    {
                        sw.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} Exception: {ex.Message}");
                    }
                }
            }
        }       

        private void Rename(object sender, RenamedEventArgs File)
        {
            string Path = File.OldFullPath;
            string Event = "renamed to " + File.FullPath;
            AddMessage(Path, Event);
        }

        private void Change(object sender, FileSystemEventArgs File)
        {
            string Path = File.FullPath;
            string Event = "changed";
            AddMessage(Path, Event);
        }

        void AddMessage(string Path, string Event)
        {
            messages.Append($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} file {Path} was {Event}\n");
        }

      
    }
} 

