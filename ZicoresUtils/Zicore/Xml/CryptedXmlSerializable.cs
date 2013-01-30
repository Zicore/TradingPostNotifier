using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zicore.Xml;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Zicore.Encryption;

namespace Zicore.Xml
{
    public enum AesKeySize
    {
        Size128 = 128,
        Size192 = 192,
        Size256 = 256
    };

    public enum AesHashType
    {
        MD5 = 0,
        SHA1 = 1
    };

    public class CryptedXmlSerializable : XmlSerializable
    {
        public CryptedXmlSerializable()
        {

        }

        public CryptedXmlSerializable(String password)
        {
            this.password = password;
        }

        String saltValue = "SALT";
        String initVector = "@6B2G3D4ehF6g7h8"; // 16 Bytes
        Int32 passwordIterations = 16;

        public override void LoadFrom(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    byte[] file = RijndaelSimple.DecryptFile(File.ReadAllBytes(path), password, saltValue, AesHashType.MD5.ToString(), passwordIterations, initVector, (int)AesKeySize.Size128);

                    try
                    {
                        using (MemoryStream ms = new MemoryStream(file))
                        {
                            XmlTextReader xr = new XmlTextReader(ms);
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
                                Encode(c);
                            }

                            xr.Close();
                            ms.Close();
                        }
                    }
                    catch
                    {
                        throw;
                    }
                }
                catch
                {
                    throw new System.Security.Cryptography.CryptographicException("Could not decrypt file!");
                }
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        public override void SaveAs(string path)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                XmlSerializer s = new XmlSerializer(this.GetType());
                Encode(this);
                s.Serialize(ms, this);
                Decode(this);

                Byte[] file = ms.ToArray();
                ms.Close();
                file = RijndaelSimple.EncryptFile(file, password, saltValue, AesHashType.MD5.ToString(), passwordIterations, initVector, (int)AesKeySize.Size128);

                FileStream fs = new FileStream(path, FileMode.CreateNew);
                fs.Write(file, 0, file.Length);
                fs.Close();
            }
        }

        String password = "";

        public String Password
        {
            get { return password; }
            set { password = value; }
        }
    }
}
