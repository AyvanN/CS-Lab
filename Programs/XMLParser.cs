using System;
using System.Reflection;
using System.Xml;

namespace FileWatcher
{
    class XMLParser : IParser
    {
        private readonly string path;

        private readonly Type Type;

        public XMLParser(string path, Type Type)
        {
            this.path = path;
            this.Type = Type;
        }

        private void Deserialization(PropertyInfo param, object parent, XmlNode parentNode)
        {
            foreach (XmlNode node in parentNode.ChildNodes)
            {
                if (node.Name == param.Name)
                {
                    if (param.PropertyType.IsPrimitive || param.PropertyType == typeof(string))
                    {
                        param.SetValue(parent, Convert.ChangeType(node.InnerText, param.PropertyType));
                    }
                    else if (param.PropertyType.IsEnum)
                    {
                        param.SetValue(parent, Enum.Parse(param.PropertyType, node.InnerText));
                    }
                    else
                    {
                        Type subType = param.PropertyType;
                        object subObj = Activator.CreateInstance(subType);

                        param.SetValue(parent, subObj);

                        PropertyInfo[] subparams = subType.GetProperties();
                        foreach (PropertyInfo sparam in subparams)
                        {
                            Deserialization(sparam, subObj, node);
                        }
                    }
                }
            }
        }


        public T Parse<T>()
        {
            object result = Activator.CreateInstance(typeof(T));

            if (result is null)
            {
                throw new ArgumentNullException($"{nameof(result)} is null");
            }

            PropertyInfo[] properties = typeof(T).GetProperties();

            foreach (PropertyInfo param in properties)
            {
                Deserialization(param, result, FindNode<T>());
            }

            return (T)result;
        }

        private XmlNode FindNode<T>()
        {
            XmlDocument doc = new XmlDocument();

            doc.Load(path);

            if (typeof(T) == Type)
            {
                return doc.DocumentElement;
            }

            PropertyInfo[] properties = Type.GetProperties();

            XmlNode result = null;

            foreach (PropertyInfo param in properties)
            {
                RecursiveNode<T>(param, doc.DocumentElement, ref result);
            }

            if (result is null)
            {
                throw new ArgumentNullException($"{nameof(result)} is null");
            }

            return result;
        }

        private void RecursiveNode<T>(PropertyInfo param, XmlNode parentNode, ref XmlNode result)
        {
            foreach (XmlNode node in parentNode.ChildNodes)
            {
                if (node.Name == param.Name && param.PropertyType == typeof(T) && result == null)
                {
                    result = node;

                    if (!param.PropertyType.IsPrimitive && !(param.PropertyType == typeof(string)))
                    {
                        Type subType = param.PropertyType;

                        PropertyInfo[] subparams = subType.GetProperties();
                        foreach (PropertyInfo sparam in subparams)
                        {
                            RecursiveNode<T>(sparam, node, ref result);
                        }
                    }
                }
            }
        }
    }

}
