namespace PBIXInspectorWinForm
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            groupBox1 = new GroupBox();
            btnBrowseRulesFile = new Button();
            btnBrowsePBIDesktopFile = new Button();
            label2 = new Label();
            label1 = new Label();
            chkUseBaseRules = new CheckBox();
            chckUseSamplePBIFile = new CheckBox();
            txtRulesFilePath = new TextBox();
            txtPBIDesktopFile = new TextBox();
            btnBrowseOutputDir = new Button();
            label3 = new Label();
            chckUseTempFiles = new CheckBox();
            txtOutputDirPath = new TextBox();
            groupBox2 = new GroupBox();
            chckVerbose = new CheckBox();
            label4 = new Label();
            chckHTMLOutput = new CheckBox();
            chckJsonOutput = new CheckBox();
            txtConsoleOutput = new TextBox();
            btnRun = new Button();
            openPBIDesktopFileDialog = new OpenFileDialog();
            openRulesFileDialog = new OpenFileDialog();
            outputFolderBrowserDialog = new FolderBrowserDialog();
            lblMessage = new Label();
            lnkHelp = new LinkLabel();
            lnkLicense = new LinkLabel();
            lnkAbout = new LinkLabel();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(btnBrowseRulesFile);
            groupBox1.Controls.Add(btnBrowsePBIDesktopFile);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(chkUseBaseRules);
            groupBox1.Controls.Add(chckUseSamplePBIFile);
            groupBox1.Controls.Add(txtRulesFilePath);
            groupBox1.Controls.Add(txtPBIDesktopFile);
            groupBox1.Location = new Point(31, 63);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(933, 201);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Inputs";
            groupBox1.Enter += groupBox1_Enter;
            // 
            // btnBrowseRulesFile
            // 
            btnBrowseRulesFile.Location = new Point(640, 111);
            btnBrowseRulesFile.Name = "btnBrowseRulesFile";
            btnBrowseRulesFile.Size = new Size(112, 34);
            btnBrowseRulesFile.TabIndex = 10;
            btnBrowseRulesFile.Text = "Browse";
            btnBrowseRulesFile.UseVisualStyleBackColor = true;
            btnBrowseRulesFile.Click += btnBrowseRulesFile_Click;
            // 
            // btnBrowsePBIDesktopFile
            // 
            btnBrowsePBIDesktopFile.Location = new Point(640, 58);
            btnBrowsePBIDesktopFile.Name = "btnBrowsePBIDesktopFile";
            btnBrowsePBIDesktopFile.Size = new Size(112, 34);
            btnBrowsePBIDesktopFile.TabIndex = 9;
            btnBrowsePBIDesktopFile.Text = "Browse";
            btnBrowsePBIDesktopFile.UseVisualStyleBackColor = true;
            btnBrowsePBIDesktopFile.Click += btnBrowsePBIDesktopFile_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(49, 111);
            label2.Name = "label2";
            label2.Size = new Size(82, 25);
            label2.TabIndex = 7;
            label2.Text = "Rules file";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(49, 60);
            label1.Name = "label1";
            label1.Size = new Size(137, 25);
            label1.TabIndex = 6;
            label1.Text = "PBI Desktop file";
            // 
            // chkUseBaseRules
            // 
            chkUseBaseRules.AutoSize = true;
            chkUseBaseRules.Checked = true;
            chkUseBaseRules.CheckState = CheckState.Checked;
            chkUseBaseRules.Location = new Point(768, 116);
            chkUseBaseRules.Name = "chkUseBaseRules";
            chkUseBaseRules.Size = new Size(151, 29);
            chkUseBaseRules.TabIndex = 4;
            chkUseBaseRules.Text = "Use base rules";
            chkUseBaseRules.UseVisualStyleBackColor = true;
            chkUseBaseRules.CheckedChanged += chkUseBaseRules_CheckedChanged;
            // 
            // chckUseSamplePBIFile
            // 
            chckUseSamplePBIFile.AutoSize = true;
            chckUseSamplePBIFile.Location = new Point(768, 63);
            chckUseSamplePBIFile.Name = "chckUseSamplePBIFile";
            chckUseSamplePBIFile.Size = new Size(129, 29);
            chckUseSamplePBIFile.TabIndex = 3;
            chckUseSamplePBIFile.Text = "Use sample";
            chckUseSamplePBIFile.UseVisualStyleBackColor = true;
            chckUseSamplePBIFile.CheckedChanged += chckUseSamplePBIFile_CheckedChanged;
            // 
            // txtRulesFilePath
            // 
            txtRulesFilePath.Location = new Point(204, 110);
            txtRulesFilePath.Name = "txtRulesFilePath";
            txtRulesFilePath.Size = new Size(430, 31);
            txtRulesFilePath.TabIndex = 1;
            // 
            // txtPBIDesktopFile
            // 
            txtPBIDesktopFile.Location = new Point(204, 60);
            txtPBIDesktopFile.Name = "txtPBIDesktopFile";
            txtPBIDesktopFile.Size = new Size(430, 31);
            txtPBIDesktopFile.TabIndex = 0;
            // 
            // btnBrowseOutputDir
            // 
            btnBrowseOutputDir.Location = new Point(640, 35);
            btnBrowseOutputDir.Name = "btnBrowseOutputDir";
            btnBrowseOutputDir.Size = new Size(112, 34);
            btnBrowseOutputDir.TabIndex = 11;
            btnBrowseOutputDir.Text = "Browse";
            btnBrowseOutputDir.UseVisualStyleBackColor = true;
            btnBrowseOutputDir.Click += btnBrowseOutputDir_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(28, 37);
            label3.Name = "label3";
            label3.Size = new Size(144, 25);
            label3.TabIndex = 8;
            label3.Text = "Output directory";
            label3.Click += label3_Click;
            // 
            // chckUseTempFiles
            // 
            chckUseTempFiles.AutoSize = true;
            chckUseTempFiles.Location = new Point(768, 39);
            chckUseTempFiles.Name = "chckUseTempFiles";
            chckUseTempFiles.Size = new Size(150, 29);
            chckUseTempFiles.TabIndex = 5;
            chckUseTempFiles.Text = "Use temp files";
            chckUseTempFiles.UseVisualStyleBackColor = true;
            chckUseTempFiles.CheckedChanged += chckUseTempFiles_CheckedChanged;
            // 
            // txtOutputDirPath
            // 
            txtOutputDirPath.BorderStyle = BorderStyle.FixedSingle;
            txtOutputDirPath.Location = new Point(204, 37);
            txtOutputDirPath.Name = "txtOutputDirPath";
            txtOutputDirPath.ReadOnly = true;
            txtOutputDirPath.Size = new Size(430, 31);
            txtOutputDirPath.TabIndex = 2;
            txtOutputDirPath.TextChanged += txtOutputDirPath_TextChanged;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(chckVerbose);
            groupBox2.Controls.Add(label4);
            groupBox2.Controls.Add(btnBrowseOutputDir);
            groupBox2.Controls.Add(chckHTMLOutput);
            groupBox2.Controls.Add(chckJsonOutput);
            groupBox2.Controls.Add(txtOutputDirPath);
            groupBox2.Controls.Add(label3);
            groupBox2.Controls.Add(chckUseTempFiles);
            groupBox2.Location = new Point(31, 285);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(933, 183);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "Outputs";
            // 
            // chckVerbose
            // 
            chckVerbose.AutoSize = true;
            chckVerbose.Location = new Point(768, 124);
            chckVerbose.Name = "chckVerbose";
            chckVerbose.Size = new Size(102, 29);
            chckVerbose.TabIndex = 2;
            chckVerbose.Text = "Verbose";
            chckVerbose.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(28, 95);
            label4.Name = "label4";
            label4.Size = new Size(136, 25);
            label4.TabIndex = 12;
            label4.Text = "Output formats";
            label4.Click += label4_Click;
            // 
            // chckHTMLOutput
            // 
            chckHTMLOutput.AutoSize = true;
            chckHTMLOutput.Checked = true;
            chckHTMLOutput.CheckState = CheckState.Checked;
            chckHTMLOutput.Location = new Point(337, 94);
            chckHTMLOutput.Name = "chckHTMLOutput";
            chckHTMLOutput.Size = new Size(84, 29);
            chckHTMLOutput.TabIndex = 1;
            chckHTMLOutput.Text = "HTML";
            chckHTMLOutput.UseVisualStyleBackColor = true;
            // 
            // chckJsonOutput
            // 
            chckJsonOutput.AutoSize = true;
            chckJsonOutput.Location = new Point(204, 94);
            chckJsonOutput.Name = "chckJsonOutput";
            chckJsonOutput.Size = new Size(81, 29);
            chckJsonOutput.TabIndex = 0;
            chckJsonOutput.Text = "JSON";
            chckJsonOutput.UseVisualStyleBackColor = true;
            // 
            // txtConsoleOutput
            // 
            txtConsoleOutput.Location = new Point(31, 535);
            txtConsoleOutput.Multiline = true;
            txtConsoleOutput.Name = "txtConsoleOutput";
            txtConsoleOutput.ReadOnly = true;
            txtConsoleOutput.ScrollBars = ScrollBars.Vertical;
            txtConsoleOutput.Size = new Size(933, 152);
            txtConsoleOutput.TabIndex = 2;
            // 
            // btnRun
            // 
            btnRun.Location = new Point(852, 482);
            btnRun.Name = "btnRun";
            btnRun.Size = new Size(112, 34);
            btnRun.TabIndex = 3;
            btnRun.Text = "Run";
            btnRun.UseVisualStyleBackColor = true;
            btnRun.Click += btnRun_Click;
            // 
            // openPBIDesktopFileDialog
            // 
            openPBIDesktopFileDialog.Filter = "Power BI Project Report file (report.json)|report.json|Power BI Desktop file (*.pbix)|*.pbix|All Files (*.*)|*.*";
            openPBIDesktopFileDialog.FileOk += openPBIDesktopFileDialog_FileOk;
            // 
            // openRulesFileDialog
            // 
            openRulesFileDialog.FileName = "openFileDialog2";
            openRulesFileDialog.Filter = "Json files (*.json)|*.json|All files (*.*)|*.*";
            openRulesFileDialog.FileOk += openRulesFileDialog_FileOk;
            // 
            // outputFolderBrowserDialog
            // 
            outputFolderBrowserDialog.HelpRequest += outputFolderBrowserDialog_HelpRequest;
            // 
            // lblMessage
            // 
            lblMessage.AutoSize = true;
            lblMessage.Location = new Point(747, 487);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(82, 25);
            lblMessage.TabIndex = 4;
            lblMessage.Text = "Message";
            lblMessage.Visible = false;
            // 
            // lnkHelp
            // 
            lnkHelp.AutoSize = true;
            lnkHelp.Location = new Point(702, 23);
            lnkHelp.Name = "lnkHelp";
            lnkHelp.Size = new Size(81, 25);
            lnkHelp.TabIndex = 5;
            lnkHelp.TabStop = true;
            lnkHelp.Text = "Read me";
            lnkHelp.LinkClicked += lnkHelp_LinkClicked;
            // 
            // lnkLicense
            // 
            lnkLicense.AutoSize = true;
            lnkLicense.Location = new Point(808, 23);
            lnkLicense.Name = "lnkLicense";
            lnkLicense.Size = new Size(68, 25);
            lnkLicense.TabIndex = 6;
            lnkLicense.TabStop = true;
            lnkLicense.Text = "License";
            lnkLicense.LinkClicked += lnkLicense_LinkClicked;
            // 
            // lnkAbout
            // 
            lnkAbout.AutoSize = true;
            lnkAbout.Location = new Point(896, 23);
            lnkAbout.Name = "lnkAbout";
            lnkAbout.Size = new Size(62, 25);
            lnkAbout.TabIndex = 7;
            lnkAbout.TabStop = true;
            lnkAbout.Text = "About";
            lnkAbout.LinkClicked += lnkAbout_LinkClicked;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1002, 712);
            Controls.Add(lnkAbout);
            Controls.Add(lnkLicense);
            Controls.Add(lnkHelp);
            Controls.Add(lblMessage);
            Controls.Add(btnRun);
            Controls.Add(txtConsoleOutput);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MainForm";
            Text = "VisOps with PBI Inspector";
            Load += Form1_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private CheckBox chckHTMLOutput;
        private CheckBox chckJsonOutput;
        private TextBox txtConsoleOutput;
        private Button btnRun;
        private Label label3;
        private Label label2;
        private Label label1;
        private CheckBox chckUseTempFiles;
        private CheckBox chkUseBaseRules;
        private CheckBox chckUseSamplePBIFile;
        private TextBox txtOutputDirPath;
        private TextBox txtRulesFilePath;
        private TextBox txtPBIDesktopFile;
        private Button btnBrowseOutputDir;
        private Button btnBrowseRulesFile;
        private Button btnBrowsePBIDesktopFile;
        private OpenFileDialog openPBIDesktopFileDialog;
        private OpenFileDialog openRulesFileDialog;
        private FolderBrowserDialog outputFolderBrowserDialog;
        private CheckBox chckVerbose;
        private Label label4;
        private Label lblMessage;
        private LinkLabel lnkHelp;
        private LinkLabel lnkLicense;
        private LinkLabel lnkAbout;
    }
}