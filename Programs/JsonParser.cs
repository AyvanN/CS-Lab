using System;
using System.Reflection;
using System.IO;
using System.Text.Json;

namespace FileWatcher
{
    class JsonParser : IParser
    {
        private readonly string path;

        private readonly Type Type;

        public JsonParser(string path, Type Type)
        {
            this.path = path;
            this.Type = Type;
        }

        private JsonElement FindNode<T>(string json)
        {
            JsonDocument Document = JsonDocument.Parse(json);

            JsonElement root = Document.RootElement;

            if (typeof(T) == Type)
            {
                return root;
            }

            PropertyInfo[] properties = Type.GetProperties();

            JsonElement? result = null;

            foreach (PropertyInfo param in properties)
            {
                RecursiveNode<T>(param, root, ref result);
            }

            if (result is null)
            {
                throw new ArgumentNullException($"{nameof(result)} is null");
            }

            return (JsonElement)result;
        }

        public T Parse<T>()
        {
            string json;

            using (StreamReader sr = new StreamReader(path))
            {
                json = sr.ReadToEnd();
            }

            object result = Activator.CreateInstance(typeof(T));

            if (result is null)
            {
                throw new ArgumentNullException($"{nameof(result)} is null");
            }

            PropertyInfo[] properties = typeof(T).GetProperties();

            JsonElement rootNode = FindNode<T>(json);

            foreach (PropertyInfo param in properties)
            {
                Deserialization(param, result, rootNode);
            }

            return (T)result;
        }

        private void RecursiveNode<T>(PropertyInfo info, JsonElement parentNode, ref JsonElement? result)
        {
            if (result == null)
            {
                if (parentNode.TryGetProperty(info.Name, out JsonElement element) && info.PropertyType == typeof(T))
                {
                    result = (JsonElement?)element;
                    return;
                }
                else if (!info.PropertyType.IsPrimitive && info.PropertyType != typeof(string))
                {
                    Type subType = info.PropertyType;

                    PropertyInfo[] subparams = subType.GetProperties();
                    foreach (PropertyInfo sparam in subparams)
                    {
                        RecursiveNode<T>(sparam, parentNode.GetProperty(sparam.Name), ref result);
                    }
                }
            }
        }

        private void Deserialization(PropertyInfo info, object obj, JsonElement parentNode)
        {
            if (info.PropertyType.IsPrimitive || info.PropertyType == typeof(string))
            {
                info.SetValue(obj, Convert.ChangeType(parentNode.GetProperty(info.Name).GetRawText().Trim('"'), info.PropertyType));
            }
            else if (info.PropertyType.IsEnum)
            {
                info.SetValue(obj, Enum.Parse(info.PropertyType, parentNode.GetProperty(info.Name).GetRawText().Trim('"')));
            }
            else
            {
                Type subType = info.PropertyType;
                object subObj = Activator.CreateInstance(subType);

                info.SetValue(obj, subObj);

                PropertyInfo[] subparams = subType.GetProperties();
                foreach (PropertyInfo sparam in subparams)
                {
                    Deserialization(sparam, subObj, parentNode.GetProperty(info.Name));
                }
            }
        }
    }
}
