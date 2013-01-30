using System;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Xml.Serialization;
using System.Text;
using ZicoresUtils.Zicore.Configuration;

namespace Zicore.Xml
{
    public abstract class XmlSerializable : ISavableLoadable
    {
        public event EventHandler Saving;
        public event EventHandler Loading;

        public virtual void Save()
        {

        }

        public virtual void Load()
        {

        }

        public virtual void Save(String name)
        {
            if (Directory.Exists(ConfigHelper.GetConfigFolder()))
            {
                SaveAs(Path.Combine(ConfigHelper.GetConfigFolder(), name));
            }
            else
            {
                try
                {
                    Directory.CreateDirectory(ConfigHelper.GetConfigFolder());
                }
                catch
                {
                    Console.Write("Couldn't create directory");
                }
            }
        }

        bool useEncoding = false;

        public bool UseEncoding
        {
            get { return useEncoding; }
            set { useEncoding = value; }
        }

        public virtual void SaveAs(string path)
        {
            try
            {
                using (StreamWriter w = new StreamWriter(path))
                {
                    XmlSerializer s = new XmlSerializer(this.GetType());
                    if (UseEncoding)
                        Encode(this);

                    s.Serialize(w, this);


                    if (UseEncoding)
                        Decode(this);
                    w.Close();
                }
            }
            catch
            {
                throw;
            }
        }

        public static void Encode(Object obj)
        {
            //Type t = obj.GetType();
            //PropertyInfo[] properties = t.GetProperties();
            //foreach (PropertyInfo p in properties)
            //{
            //    if (p.PropertyType == typeof(string))
            //    {
            //        String value = "";
            //        value = p.GetValue(obj, null) as String;
            //        if (value != null)
            //        {
            //            p.SetValue(obj, System.Web.HttpUtility.HtmlEncode(value), null);
            //        }
            //    }
            //}
        }

        public static void Decode(Object obj)
        {
            //Type t = obj.GetType();
            //PropertyInfo[] properties = t.GetProperties();
            //foreach (PropertyInfo p in properties)
            //{
            //    if (p.PropertyType == typeof(string))
            //    {
            //        String value = "";
            //        value = p.GetValue(obj, null) as String;
            //        if (value != null)
            //        {
            //            p.SetValue(obj, System.Web.HttpUtility.HtmlDecode(value), null);
            //        }
            //    }
            //}
        }

        public virtual string ToXmlString()
        {

            using (MemoryStream ms = new MemoryStream())
            {
                using (StreamWriter xmlSink = new StreamWriter(ms))
                {
                    XmlSerializer s = new XmlSerializer(this.GetType());

                    //if (UseEncoding) 
                    //    Encode(this);

                    s.Serialize(xmlSink, this);
                    //if (UseEncoding)
                    //    Decode(this);

                    String str = GetStringFromMemoryStream(ms);
                    return str;
                }
            }

        }

        public static string GetStringFromMemoryStream(MemoryStream m)
        {
            if (m == null || m.Length == 0)
                return null;

            m.Flush();
            m.Position = 0;
            using (StreamReader sr = new StreamReader(m))
            {
                string s = sr.ReadToEnd();

                return s;
            }
        }

        public virtual void Load(String name, bool raiseEvent = true)
        {
            if (raiseEvent && Loading != null)
            {
                Loading(this, new EventArgs());
            }
            try
            {
                if (Directory.Exists(ConfigHelper.GetConfigFolder()))
                {
                    LoadFrom(Path.Combine(ConfigHelper.GetConfigFolder(), name));
                }
            }
            catch (FileNotFoundException)
            {

            }
        }

        public virtual void LoadFrom(string path)
        {
            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    XmlTextReader xr = new XmlTextReader(sr);
                    XmlSerializer xs = new XmlSerializer(this.GetType());
                    object c;
                    if (xs.CanDeserialize(xr))
                    {
                        c = xs.Deserialize(xr);
                        Type t = this.GetType();
                        PropertyInfo[] properties = t.GetProperties();
                        foreach (PropertyInfo p in properties)
                        {
                            p.SetValue(this, p.GetValue(c, null), null);
                        }
                        if (UseEncoding)
                            Encode(c);
                    }

                    xr.Close();
                    sr.Close();
                }
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        public static object Load(string type, string xmlData)
        {
            using (StringReader sr = new StringReader(xmlData))
            {
                using (XmlTextReader xr = new XmlTextReader(sr))
                {
                    XmlSerializer xs = new XmlSerializer(Type.GetType(type));
                    object c;
                    if (xs.CanDeserialize(xr))
                    {
                        try
                        {
                            c = xs.Deserialize(xr);
                            xr.Close();
                            //XmlSerializable.Decode(c);
                            return c;
                        }
                        catch
                        {
                            return null;
                        }
                    }
                }
            }
            return null;
        }
    }
}