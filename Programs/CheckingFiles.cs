using System;

namespace FileWatcher
{
    class CheckingFiles : IParser
    {
        IParser parser;

        public T Parse<T>() => parser.Parse<T>();

        public CheckingFiles(string path, Type Type)
        {
            if (path.EndsWith(".json"))
            {
                parser = new JsonParser(path, Type);
            }
            else if (path.EndsWith(".xml"))
            {
                parser = new XMLParser(path, Type);
            }
            else
            {
                throw new ArgumentNullException($"extension");
            }
        }
    }
}