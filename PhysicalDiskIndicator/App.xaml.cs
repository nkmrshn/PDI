using NLog;
using PDI.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Forms;

namespace PDI
{
    public partial class App : System.Windows.Application
    {
        private Mutex mutex;
        private Logger logger = LogManager.GetCurrentClassLogger();
        private PhysicalDisk currentDisk;
        private NotifyIcon notifyIcon;
        private BackgroundWorker worker;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                string guid = ((GuidAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GuidAttribute), true)[0]).Value;
                mutex = new Mutex(false, guid);
                
                if (!Settings.Default.AllowMutipleInstances && !mutex.WaitOne(0, false))
                {
                    Shutdown();
                    return;
                }

                logger.Info("Started");

                Menu menu = new Menu();
                menu.IgnoreDriveType = Settings.Default.IgnoreDriveType;
                menu.Disk_Clicked += Disk_Clicked;
                menu.Exit_Clicked += Exit_Clicked;

                notifyIcon = new NotifyIcon();
                notifyIcon.ContextMenuStrip = menu.Create(ref currentDisk);
                notifyIcon.Icon = PDI.Properties.Resources.InActive;
                notifyIcon.Visible = true;

                if (currentDisk == null)
                {
                    logger.Error("Disk not found");
                    Shutdown();
                    return;
                }

                worker = new BackgroundWorker();
                worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
                worker.ProgressChanged += new ProgressChangedEventHandler(Worker_ProgressChanged);
                worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
                worker.WorkerReportsProgress = true;
                worker.WorkerSupportsCancellation = true;
                worker.RunWorkerAsync();
            }
             catch (Exception exception)
            {
                logger.Error(exception);
                Shutdown();
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Cleanup();
        }

        private void Cleanup()
        {
            try
            {
                if (mutex != null) mutex.Close();
                if (worker != null) worker.Dispose();
                if (currentDisk != null) currentDisk.Dispose();
                if (notifyIcon != null) notifyIcon.Dispose();
                logger.Info("Finished");
            }
            catch (Exception exception)
            {
                logger.Error(exception);
            }
        }

        private void Worker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                if (currentDisk == null) return;
                notifyIcon.Text = currentDisk.Text;

                while (!worker.CancellationPending)
                {
                    currentDisk.SetValues();
                    worker.ReportProgress(0);

                    Thread.Sleep(Settings.Default.UpdateInterval);
                }
            }
            catch (Exception exception)
            {
                logger.Error(exception);
                Shutdown();
            }
        }

        private void Worker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            try
            {
                Icon currentIcon = currentDisk.GetStatusIcon();

                if (notifyIcon.Icon != currentIcon)
                {
                    if (notifyIcon.Icon != null) notifyIcon.Icon.Dispose();
                    notifyIcon.Icon = currentIcon;
                }
            }
            catch(Exception exception)
            {
                logger.Error(exception);
                Shutdown();
            }
        }

        private void Worker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (currentDisk != null) worker.RunWorkerAsync();
            }
            catch (Exception exception)
            {
                logger.Error(exception);
                Shutdown();
            }
        }

        private void Disk_Clicked(object sender, EventArgs e)
        {
            try
            {
                ToolStripRadioButtonMenuItem disk = (ToolStripRadioButtonMenuItem)sender;
                if (currentDisk.InstanceName == (string)disk.Tag) return;
                string tmp = currentDisk.InstanceName;
                if (worker != null) worker.CancelAsync();
                if (currentDisk != null) currentDisk.Dispose();
                currentDisk = new PhysicalDisk((string)disk.Tag);
                Settings.Default.LastSelectedDrive = currentDisk.InstanceName;
                Settings.Default.Save();
                logger.Info(string.Format("Changed to {0}", currentDisk.InstanceName));
            }
            catch (Exception exception)
            {
                logger.Error(exception);
                Shutdown();
            }
        }

        private void Exit_Clicked(object sender, EventArgs e)
        {
            Shutdown();
        }

        private void Application_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            Cleanup();
        }
    }
}
