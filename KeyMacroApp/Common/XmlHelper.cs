using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace KeyMacroApp.Common
{
    internal class XmlSerializerHelper<T>
    {
        private Type _type;

        public XmlSerializerHelper()
        {
            _type = typeof(T);
        }

        public static void Save(string path, object obj)
        {
            try
            {
                using (var writer = new StreamWriter(path))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(writer, obj);
                }
            }
            catch (Exception ex)
            {
                Debug.Print($"[Error]: XmlSerializerHelper<{typeof(T).Name}>.Save({path})\n{ex}");
            }
        }

        public static T? Read(string path)
        {
            T? data;
            try
            {
                using (var reader = new StreamReader(path))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    data = (T?)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                Debug.Print($"[Error]: XmlSerializerHelper<{typeof(T).Name}>.Read({path})\n{ex}");
                data = default;
            }
            return data;
        }
    }
}
