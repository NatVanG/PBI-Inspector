using PBIXInspectorWinLibrary;
using PBIXInspectorWinLibrary.Utils;

namespace PBIXInspectorWinForm
{
    public partial class MainForm : Form
    {
        private static CLIArgs _args;

        public MainForm()
        {
            InitializeComponent();
            this.Text = AppUtils.About();
            this.FormClosing += MainForm_FormClosing;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txtConsoleOutput.Clear();
            Main.WinMessageIssued += Main_MessageIssued;
            UseSamplePBIFileStateCheck();
            UseBaseRulesCheck();
            UseTempFilesStateCheck();
            txtPBIDesktopFile.Focus();
        }

        private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            Clear();
        }

        private void Main_MessageIssued(object? sender, PBIXInspectorLibrary.MessageIssuedEventArgs e)
        {
            if (e.MessageType == PBIXInspectorLibrary.MessageTypeEnum.Dialog)
            {
                var dr = MessageBox.Show(e.Message, "Delete directory?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    e.DialogOKResponse = true;
                }
            }
            else
            {
                txtConsoleOutput.AppendText(string.Concat(e.MessageType.ToString(), ": ", e.Message, "\r\n"));
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void btnBrowsePBIDesktopFile_Click(object sender, EventArgs e)
        {
            this.openPBIDesktopFileDialog.ShowDialog(this);
        }

        private void btnBrowseRulesFile_Click(object sender, EventArgs e)
        {
            this.openRulesFileDialog.ShowDialog(this);
        }

        private void btnBrowseOutputDir_Click(object sender, EventArgs e)
        {
            if (this.outputFolderBrowserDialog.ShowDialog(this) == DialogResult.OK)
            {
                this.txtOutputDirPath.Text = this.outputFolderBrowserDialog.SelectedPath;
            }
        }

        private void UseSamplePBIFileStateCheck()
        {
            var enabled = !this.chckUseSamplePBIFile.Checked;
            if (!enabled) { this.txtPBIDesktopFile.Text = Constants.SamplePBIXFilePath; } else { this.txtPBIDesktopFile.Clear(); };
            this.txtPBIDesktopFile.Enabled = enabled;
            this.btnBrowsePBIDesktopFile.Enabled = enabled;
            this.chckVerbose.Checked = !enabled;
        }

        private void chckUseSamplePBIFile_CheckedChanged(object sender, EventArgs e)
        {
            UseSamplePBIFileStateCheck();
        }


        private void UseBaseRulesCheck()
        {
            var enabled = !this.chkUseBaseRules.Checked;
            if (!enabled) { this.txtRulesFilePath.Text = Constants.SampleRulesFilePath; } else { this.txtRulesFilePath.Clear(); }
            this.txtRulesFilePath.Enabled = enabled;
            this.btnBrowseRulesFile.Enabled = enabled;
        }

        private void chkUseBaseRules_CheckedChanged(object sender, EventArgs e)
        {
            UseBaseRulesCheck();
        }

        private void UseTempFilesStateCheck()
        {
            var enabled = !this.chckUseTempFiles.Checked;
            this.txtOutputDirPath.Clear();
            this.txtOutputDirPath.Enabled = enabled;
            this.btnBrowseOutputDir.Enabled = enabled;
        }

        private void chckUseTempFiles_CheckedChanged(object sender, EventArgs e)
        {
            UseTempFilesStateCheck();
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            Clear();
            btnRun.Enabled = false;
            var pbiFilePath = !string.IsNullOrEmpty(this.txtPBIDesktopFile.Text) &&  this.txtPBIDesktopFile.Text.ToLower().EndsWith("report.json") ? Path.GetDirectoryName(this.txtPBIDesktopFile.Text) : this.txtPBIDesktopFile.Text;
            var rulesFilePath = this.txtRulesFilePath.Text;
            var outputPath = this.txtOutputDirPath.Text;
            var verboseString = this.chckVerbose.Checked.ToString();
            var formatsString = string.Concat(this.chckJsonOutput.Checked ? "JSON" : string.Empty, ",", this.chckHTMLOutput.Checked ? "HTML" : string.Empty);
            _args = new CLIArgs { PBIFilePath = pbiFilePath, RulesFilePath = rulesFilePath, OutputPath = outputPath, FormatsString = formatsString, VerboseString = verboseString };

            Main.Run(_args);
            btnRun.Enabled = true;

        }

        internal void Clear()
        {
            txtConsoleOutput.Clear();
            if (_args != null && _args.DeleteOutputDirOnExit && Directory.Exists(_args.OutputDirPath))
            {
                Directory.Delete(_args.OutputDirPath, true);
            }
        }

        private void openPBIDesktopFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.txtPBIDesktopFile.Text = this.openPBIDesktopFileDialog.FileName;
        }

        private void openRulesFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.txtRulesFilePath.Text = this.openRulesFileDialog.FileName;
        }

        private void outputFolderBrowserDialog_HelpRequest(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void txtOutputDirPath_TextChanged(object sender, EventArgs e)
        {
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void lnkHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                AppUtils.WinOpen(Constants.ReadmePageUrl);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to open link that was clicked.");
            }
        }

        private void lnkLicense_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                AppUtils.WinOpen(Constants.LicensePageUrl);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to open link that was clicked.");
            }
        }

        private void lnkAbout_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show(AppUtils.About());
        }
    }
}