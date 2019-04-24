using PDI.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;

namespace PDI
{
    public class PhysicalDisk : IDisposable
    {
        public readonly string InstanceName;
        public string Text { get { return Regex.Replace(InstanceName, @"\d\s", ""); } }
        public float Read { get { return read; } }
        public float Write { get { return write; } }

        private PerformanceCounter reading;
        private PerformanceCounter writing;
        private float read;
        private float write;

        public PhysicalDisk(string instanceName)
        {
            try
            {
                InstanceName = instanceName;
                reading = new PerformanceCounter("PhysicalDisk", "Disk Read Bytes/sec", instanceName);
                writing = new PerformanceCounter("PhysicalDisk", "Disk Write Bytes/sec", instanceName);
            }
            catch
            {
                throw;
            }
        }

        public void SetValues()
        {
            try
            {
                read = reading.NextValue();
                write = writing.NextValue();
            }
            catch
            {
                throw;
            }
        }

        public Icon GetStatusIcon()
        {
            if (read > 0 && write > 0)
            {
                return Resources.Active;
            }
            else if (read > 0)
            {
                return Resources.Read;
            }
            else if (write > 0)
            {
                return Resources.Write;
            }
            else
            {
                return Resources.InActive;
            }
        }

        public static List<string> GetPerformableDiskInstanceNames(bool ignoreDriveType = false)
        {
            try
            {
                if (PerformanceCounterCategory.Exists("PhysicalDisk"))
                {
                    List<string> instanceNames = new List<string>();
                    PerformanceCounterCategory category = new PerformanceCounterCategory("PhysicalDisk");

                    foreach (string instanceName in category.GetInstanceNames())
                    {
                        if (instanceName.LastIndexOf(':') == -1) continue;
                        string[] logicalDrives = Regex.Replace(instanceName, @"\d\s", "").Split(' ');
                        DriveInfo driveInfo = new DriveInfo(Regex.Replace(logicalDrives[0], ":", ""));
                        if (ignoreDriveType || !ignoreDriveType && driveInfo.DriveType == DriveType.Fixed) instanceNames.Add(instanceName);
                    }

                    return instanceNames;
                }
            }
            catch
            {
                throw;
            }

            return null;
        }

        public void Dispose()
        {
            if (reading != null) reading.Dispose();
            if (writing != null) writing.Dispose();
        }
    }
}
