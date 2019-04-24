using NLog;
using PDI.Properties;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PDI
{
    public class Menu
    {
        public bool IgnoreDriveType = false;
        public event EventHandler Disk_Clicked;
        public event EventHandler Exit_Clicked;
        
        private Logger logger = LogManager.GetCurrentClassLogger();

        public ContextMenuStrip Create(ref PhysicalDisk currentDisk)
        {
            try
            {
                ToolStripMenuItem disks = new ToolStripMenuItem(Resources.SelectDisk);
                List<string> instanceNames = PhysicalDisk.GetPerformableDiskInstanceNames(IgnoreDriveType);

                foreach (string instanceName in instanceNames)
                {
                    ToolStripRadioButtonMenuItem disk = new ToolStripRadioButtonMenuItem(Regex.Replace(instanceName, @"\d\s", ""));
                    disk.Click += Disk_Clicked;
                    disk.Tag = instanceName;

                    if ((string.IsNullOrEmpty(Settings.Default.LastSelectedDrive) && disks.DropDownItems.Count == 0) ||
                        (Settings.Default.LastSelectedDrive == instanceName))
                    {
                        disk.Checked = true;
                        currentDisk = new PhysicalDisk(instanceName);
                    }

                    disks.DropDownItems.Add(disk);
                }

                if (disks.DropDownItems.Count > 0 &&
                    instanceNames.Count > 0 &&
                    string.IsNullOrEmpty(currentDisk.InstanceName))
                {
                    ((ToolStripRadioButtonMenuItem)disks.DropDownItems[0]).Checked = true;
                    currentDisk = new PhysicalDisk(instanceNames[0]);
                    Settings.Default.LastSelectedDrive = currentDisk.InstanceName;
                    Settings.Default.Save();
                }

                ToolStripMenuItem interval = new ToolStripMenuItem(Resources.UpdateInterval);
                
                for(int i = 100; i <= 1000; i += 100)
                {
                    ToolStripRadioButtonMenuItem updateIntervalItem = new ToolStripRadioButtonMenuItem(string.Format("{0:#,0}", i));
                    updateIntervalItem.Checked = i == Settings.Default.UpdateInterval;
                    updateIntervalItem.Tag = i;
                    updateIntervalItem.Click += UpdateIntervalItem_Clicked;
                    interval.DropDownItems.Add(updateIntervalItem);
                }

                ToolStripMenuItem ignoreDriveType = new ToolStripMenuItem(Resources.IgnoreDriveType);
                ignoreDriveType.Checked = Settings.Default.IgnoreDriveType;
                ignoreDriveType.Click += IgnoreDriveType_Clicked;

                ToolStripMenuItem allowMultipleInstances = new ToolStripMenuItem(Resources.AllowMultipleInstances);
                allowMultipleInstances.Checked = Settings.Default.AllowMutipleInstances;
                allowMultipleInstances.Click += AllowMultipleInstances_Clicked;

                //ToolStripMenuItem addShortcutToStartup = new ToolStripMenuItem(Resources.AddShortcutToStartup);
                //addShortcutToStartup.Checked = Settings.Default.AddShortcutToStartup;
                //addShortcutToStartup.Click += AddShortcutToStartup_Clicked;
                //if (addShortcutToStartup.Checked) AutoStart.CreateStartupShortcut(); else AutoStart.RemoveStartupShortcut();

                ToolStripMenuItem addRunKeyToRegistry = new ToolStripMenuItem(Resources.AddRunKeyToRegistry);
                addRunKeyToRegistry.Checked = Settings.Default.AddRunKeyToRegistry;
                addRunKeyToRegistry.Click += AddRunKeyToRegistry_Clicked;
                if (addRunKeyToRegistry.Checked) AutoStart.CreateRunKey(); else AutoStart.RemoveRunKey();

                ToolStripMenuItem settings = new ToolStripMenuItem(Resources.Settings);
                settings.DropDownItems.AddRange(new ToolStripItem[] { interval, ignoreDriveType, allowMultipleInstances, addRunKeyToRegistry });

                ToolStripMenuItem exit = new ToolStripMenuItem(Resources.Exit);
                exit.Click += Exit_Clicked;

                ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
                contextMenuStrip.Items.AddRange(new ToolStripItem[] { disks, settings, new ToolStripSeparator(), exit });

                return contextMenuStrip;
            }
            catch
            {
                throw;
            }
        }

        private void UpdateIntervalItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                ToolStripRadioButtonMenuItem updateIntervalItem = (ToolStripRadioButtonMenuItem)sender;
                int updateInterval = (int)updateIntervalItem.Tag;
                Settings.Default.UpdateInterval = updateInterval;
                Settings.Default.Save();

                logger.Info(string.Format("Update interval is changed to {0:#,0}ms.", updateInterval));
            }
            catch(Exception exception)
            {
                logger.Error(exception);
                if (Exit_Clicked != null) Exit_Clicked(this, e);
            }
        }

        private void IgnoreDriveType_Clicked(object sender, EventArgs e)
        {
            try
            {
                ToolStripMenuItem ignoreDriveType = (ToolStripMenuItem)sender;
                ignoreDriveType.Checked = !ignoreDriveType.Checked;
                Settings.Default.IgnoreDriveType = ignoreDriveType.Checked;
                Settings.Default.Save();
                logger.Info(string.Format("Drive type is {0}.", ignoreDriveType.Checked ? "ignored" : "considered"));
            }
            catch(Exception exception)
            {
                logger.Error(exception);
                if (Exit_Clicked != null) Exit_Clicked(this, e);
            }
        }

        private void AllowMultipleInstances_Clicked(object sender, EventArgs e)
        {
            try
            {
                ToolStripMenuItem allowMultipleInstances = (ToolStripMenuItem)sender;
                allowMultipleInstances.Checked = !allowMultipleInstances.Checked;
                Settings.Default.AllowMutipleInstances = allowMultipleInstances.Checked;
                Settings.Default.Save();
                logger.Info(string.Format("Multiple instances are {0}.", allowMultipleInstances.Checked ? "allowed" : "forbidden"));
            }
            catch(Exception exception)
            {
                logger.Error(exception);
                if (Exit_Clicked != null) Exit_Clicked(this, e);
            }
        }

        private void AddShortcutToStartup_Clicked(object sender, EventArgs e)
        {
            try
            {
                ToolStripMenuItem addShortcutToStartup = (ToolStripMenuItem)sender;
                addShortcutToStartup.Checked = !addShortcutToStartup.Checked;
                Settings.Default.AddShortcutToStartup = addShortcutToStartup.Checked;
                Settings.Default.Save();
                if (addShortcutToStartup.Checked) AutoStart.CreateStartupShortcut(); else AutoStart.RemoveStartupShortcut();
                logger.Info(string.Format("Add Shortcut to startup folder is {0}.", addShortcutToStartup.Checked ? "enabled" : "disabled"));
            }
            catch(Exception exception)
            {
                logger.Error(exception);
                if(Exit_Clicked != null) Exit_Clicked(this, e);
            }
        }

        private void AddRunKeyToRegistry_Clicked(object sender, EventArgs e)
        {
            try
            {
                ToolStripMenuItem addRunKeyToRegistry = (ToolStripMenuItem)sender;
                addRunKeyToRegistry.Checked = !addRunKeyToRegistry.Checked;
                Settings.Default.AddRunKeyToRegistry = addRunKeyToRegistry.Checked;
                Settings.Default.Save();
                if (addRunKeyToRegistry.Checked) AutoStart.CreateRunKey(); else AutoStart.RemoveRunKey();
                logger.Info(string.Format("Add run key to registry is {0}.", addRunKeyToRegistry.Checked ? "enabled" : "disabled"));
            }
            catch(Exception exception)
            {
                logger.Error(exception);
                if (Exit_Clicked != null) Exit_Clicked(this, e);
            }
        }
    }
}
