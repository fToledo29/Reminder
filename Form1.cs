using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reminder
{
    public partial class frm_Reminder : Form
    {
        public frm_Reminder()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Builds the command that is going to call the PowerShell Script to create the new Corgee environment.
        /// </summary>
        /// <param name="parameters">Represents the parameters used on the PowerShell call</param>
        /// <returns>String</returns>
        private string BuildScript(string ps1Location)
        {
            string result = "";
            try
            {                
                result = "powershell.exe -ExecutionPolicy ByPass -File \"" + ps1Location + "\";";
                ProgBar.PerformStep();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                return null;
            }

            return result;
        }

        /// <summary>
        /// Executes the PowerShell Script which is passed as parameter
        /// </summary>
        /// <param name="cmdArg">Command that is going to be executed by PowerShell</param>
        /// <returns>PSObject Collection  </returns>
        private Collection<PSObject> PowerScript(string cmdArg)
        {
            Collection<PSObject> psresults = null;
            try
            {
                using (Runspace _runspace = RunspaceFactory.CreateRunspace())
                {
                    _runspace.ThreadOptions = PSThreadOptions.ReuseThread;
                    _runspace.Open();
                    using (PowerShell powershell = PowerShell.Create())
                    {

                        powershell.Runspace = _runspace;
                        powershell.AddScript(cmdArg);
                        psresults = powershell.Invoke();
                    }
                    _runspace.Close();
                    ProgBar.PerformStep();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            return psresults;
        }
        public void GetProgressBarSettings()
        {
            // Display the ProgressBar control.
            ProgBar.Visible = true;
            // Set Minimum to 1 to represent the first file being copied.
            ProgBar.Minimum = 0;
            // Set Maximum to the total number of files to copy.
            ProgBar.Maximum = 10;
            // Set the initial value of the ProgressBar.
            ProgBar.Value = 1;
            // Set the Step property to a value of 1 to represent each file being copied.
            ProgBar.Step = 1;
        }

        /// <summary>
        /// Appends the given result to the textbox object
        /// </summary>
        /// <param name="psresults">Powershell Object collection</param>
        /// <returns>boolean</returns>
        public bool WriteResult(Collection<PSObject> psresults)
        {
            bool result = false;
            try
            {
                string psresu = psresults.ToString();
                txt_Result.Text = "";
                foreach (var item in psresults)
                {
                    txt_Result.AppendText(item.ToString());
                    txt_Result.AppendText(Environment.NewLine);
                }
                result = true;
                ProgBar.PerformStep();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Saves the PowerShell Scripts where the Schedule task will point.
        /// </summary>
        public bool InstallScripts()
        {
            bool result = false;
            try
            {
                GetProgressBarSettings();

                string pathLocation = "C:\\Program Files\\Reminder\\";

                //  Build path for the Alerts Script
                string fullPathAlerts = pathLocation + "Alerts.ps1";
                ProgBar.PerformStep();
                //  Save the Alerts Script
                File.WriteAllBytes(fullPathAlerts, Properties.Resources.Alerts);
                ProgBar.PerformStep();

                //  Build path for the Reminder Script
                string fullPathReminder = pathLocation + "Reminder.ps1";
                ProgBar.PerformStep();
                //  Save the Reminder Script
                File.WriteAllBytes(fullPathReminder, Properties.Resources.Reminder);
                ProgBar.PerformStep();

                //  Build path for the ScheduleTask Script
                string fullPathScheduleTask = pathLocation + "ScheduleTask.ps1";
                ProgBar.PerformStep();
                //  Save the ScheduleTask Script
                File.WriteAllBytes(fullPathScheduleTask, Properties.Resources.ScheduleTask);
                ProgBar.PerformStep();


                string arguments = BuildScript(fullPathReminder);
                Collection<PSObject> psresults = PowerScript(arguments);
                if (psresults != null)
                {
                    result = WriteResult(psresults);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                result = false;
            }
            return result;
        }

        private void frm_Reminder_Load(object sender, EventArgs e)
        {
            // Start the Scripts Installation.
            InstallScripts();
            System.Threading.Thread.Sleep(8000);
            //System.Environment.Exit(1);
        }
    }
}
