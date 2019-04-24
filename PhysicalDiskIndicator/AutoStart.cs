using IWshRuntimeLibrary;
using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;

namespace PDI
{
    public class AutoStart
    {
        private static Assembly assembly = Assembly.GetExecutingAssembly();
        private static string product = ((AssemblyProductAttribute)assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), true)[0]).Product;
        private static string subKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        public static void CreateStartupShortcut()
        {
            try
            {
                IWshShortcut shortcut = (IWshShortcut)(new WshShell()).CreateShortcut(GetStartupPath());
                shortcut.Description = ((AssemblyDescriptionAttribute)assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), true)[0]).Description;
                shortcut.TargetPath = assembly.Location;
                shortcut.Save();
            }
            catch
            {
                throw;
            }
        }

        public static void RemoveStartupShortcut()
        {
            try
            {
                string path = GetStartupPath();
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            }
            catch
            {
                throw;
            }
        }

        public static void CreateRunKey()
        {
            try
            {
                RegistryKey runKey = Registry.CurrentUser.OpenSubKey(subKey, true);
                runKey.SetValue(product, assembly.Location);
                runKey.Close();
            }
            catch
            {
                throw;
            }
        }

        public static void RemoveRunKey()
        {
            try
            {
                RegistryKey runKey = Registry.CurrentUser.OpenSubKey(subKey, true);
                if (runKey.GetValue(product) != null) runKey.DeleteValue(product);
                runKey.Close();
            }
            catch
            {
                throw;
            }
        }

        private static string GetStartupPath()
        {
            try
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), product + ".lnk");
            }
            catch
            {
                throw;
            }
        }
    }
}
