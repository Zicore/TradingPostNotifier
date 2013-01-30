using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace ZicoresUtils.Zicore.Configuration
{
    public static class ConfigHelper
    {
        public static string GetConfigFolder()
        {
            try
            {
                String path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                AssemblyName asmName = Assembly.GetEntryAssembly().GetName(); // name of the main executing project
                path = System.IO.Path.Combine(path, asmName.Name);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
            catch
            {
                throw;
            }
        }

        public static string GetProgramDataFolder()
        {
            try
            {
                String path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                AssemblyName asmName = Assembly.GetEntryAssembly().GetName(); // name of the main executing project
                path = System.IO.Path.Combine(path, asmName.Name);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
            catch
            {
                throw;
            }
        }

        public static string GetConfigFile(Object obj, String extension)
        {
            String className = obj.GetType().Name;
            return GetConfigFile(className, extension);
        }

        public static string GetConfigFile(Object obj)
        {
            return GetConfigFile(obj, "xml");
        }

        public static string GetConfigFile(String name, String extension)
        {
            String className = String.Format("{0}.{1}", name, extension);
            String path = ConfigHelper.GetConfigFolder();
            return Path.Combine(path, className);
        }

        public static string GetConfigFile(String name)
        {
            String className = String.Format("{0}", name);
            String path = ConfigHelper.GetConfigFolder();
            return Path.Combine(path, className);
        }
    }
}
