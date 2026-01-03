namespace FTPPUPLOAD
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnDeleteServer = new System.Windows.Forms.Button();
            this.btnTestAndSave = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.txtServerName = new System.Windows.Forms.TextBox();
            this.btnLoadServer = new System.Windows.Forms.Button();
            this.cmbSavedServers = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtFtpHost = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtFolderPath = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnUpload = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lstLog = new System.Windows.Forms.ListBox();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.lblStatus = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnForceDeleteCorrupted = new System.Windows.Forms.Button();
            this.btnRename = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.btnDeleteAll = new System.Windows.Forms.Button();
            this.btnDeleteSelected = new System.Windows.Forms.Button();
            this.lstFtpFiles = new System.Windows.Forms.CheckedListBox();
            this.btnListFiles = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnDeleteServer);
            this.groupBox1.Controls.Add(this.btnTestAndSave);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.txtServerName);
            this.groupBox1.Controls.Add(this.btnLoadServer);
            this.groupBox1.Controls.Add(this.cmbSavedServers);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.txtPort);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtPassword);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtUsername);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtFtpHost);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(9, 10);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Size = new System.Drawing.Size(495, 162);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "FTP Bilgileri";
            // 
            // btnDeleteServer
            // 
            this.btnDeleteServer.BackColor = System.Drawing.Color.Crimson;
            this.btnDeleteServer.ForeColor = System.Drawing.Color.White;
            this.btnDeleteServer.Location = new System.Drawing.Point(330, 102);
            this.btnDeleteServer.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnDeleteServer.Name = "btnDeleteServer";
            this.btnDeleteServer.Size = new System.Drawing.Size(60, 22);
            this.btnDeleteServer.TabIndex = 20;
            this.btnDeleteServer.Text = "Sil";
            this.btnDeleteServer.UseVisualStyleBackColor = false;
            this.btnDeleteServer.Click += new System.EventHandler(this.btnDeleteServer_Click);
            // 
            // btnTestAndSave
            // 
            this.btnTestAndSave.BackColor = System.Drawing.Color.ForestGreen;
            this.btnTestAndSave.ForeColor = System.Drawing.Color.White;
            this.btnTestAndSave.Location = new System.Drawing.Point(262, 132);
            this.btnTestAndSave.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnTestAndSave.Name = "btnTestAndSave";
            this.btnTestAndSave.Size = new System.Drawing.Size(105, 22);
            this.btnTestAndSave.TabIndex = 19;
            this.btnTestAndSave.Text = "Kaydet";
            this.btnTestAndSave.UseVisualStyleBackColor = false;
            this.btnTestAndSave.Click += new System.EventHandler(this.btnTestAndSave_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(11, 134);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 13);
            this.label8.TabIndex = 17;
            this.label8.Text = "Sunucu Adı:";
            // 
            // txtServerName
            // 
            this.txtServerName.Location = new System.Drawing.Point(98, 134);
            this.txtServerName.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtServerName.Name = "txtServerName";
            this.txtServerName.Size = new System.Drawing.Size(160, 20);
            this.txtServerName.TabIndex = 18;
            // 
            // btnLoadServer
            // 
            this.btnLoadServer.Location = new System.Drawing.Point(262, 102);
            this.btnLoadServer.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnLoadServer.Name = "btnLoadServer";
            this.btnLoadServer.Size = new System.Drawing.Size(60, 22);
            this.btnLoadServer.TabIndex = 16;
            this.btnLoadServer.Text = "Yükle";
            this.btnLoadServer.UseVisualStyleBackColor = true;
            this.btnLoadServer.Click += new System.EventHandler(this.btnLoadServer_Click);
            // 
            // cmbSavedServers
            // 
            this.cmbSavedServers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSavedServers.FormattingEnabled = true;
            this.cmbSavedServers.Location = new System.Drawing.Point(98, 103);
            this.cmbSavedServers.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cmbSavedServers.Name = "cmbSavedServers";
            this.cmbSavedServers.Size = new System.Drawing.Size(158, 21);
            this.cmbSavedServers.TabIndex = 15;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(11, 106);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(88, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Kayıtlı Sunucular:";
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(308, 65);
            this.txtPort.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(76, 20);
            this.txtPort.TabIndex = 7;
            this.txtPort.Text = "21";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(270, 67);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Port:";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(308, 39);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(174, 20);
            this.txtPassword.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(270, 41);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Şifre:";
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(308, 16);
            this.txtUsername.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(174, 20);
            this.txtUsername.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(270, 19);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "K.Adı:";
            // 
            // txtFtpHost
            // 
            this.txtFtpHost.Location = new System.Drawing.Point(85, 17);
            this.txtFtpHost.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtFtpHost.Name = "txtFtpHost";
            this.txtFtpHost.Size = new System.Drawing.Size(181, 20);
            this.txtFtpHost.TabIndex = 1;
            this.txtFtpHost.TextChanged += new System.EventHandler(this.txtFtpHost_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 19);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "FTP Sunucu:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnBrowse);
            this.groupBox2.Controls.Add(this.txtFolderPath);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Location = new System.Drawing.Point(9, 177);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox2.Size = new System.Drawing.Size(495, 65);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Dosya Seçimi";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(412, 24);
            this.btnBrowse.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(68, 24);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "Gözat...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtFolderPath
            // 
            this.txtFolderPath.Location = new System.Drawing.Point(75, 27);
            this.txtFolderPath.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtFolderPath.Name = "txtFolderPath";
            this.txtFolderPath.ReadOnly = true;
            this.txtFolderPath.Size = new System.Drawing.Size(324, 20);
            this.txtFolderPath.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 29);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Klasör Yolu:";
            // 
            // btnUpload
            // 
            this.btnUpload.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnUpload.Location = new System.Drawing.Point(9, 247);
            this.btnUpload.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(495, 37);
            this.btnUpload.TabIndex = 2;
            this.btnUpload.Text = "FTP\'ye Yükle";
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(9, 288);
            this.progressBar.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(495, 19);
            this.progressBar.TabIndex = 3;
            // 
            // lstLog
            // 
            this.lstLog.FormattingEnabled = true;
            this.lstLog.HorizontalScrollbar = true;
            this.lstLog.Location = new System.Drawing.Point(9, 333);
            this.lstLog.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.lstLog.Name = "lstLog";
            this.lstLog.Size = new System.Drawing.Size(496, 147);
            this.lstLog.TabIndex = 4;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(9, 313);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(40, 13);
            this.lblStatus.TabIndex = 5;
            this.lblStatus.Text = "Hazır...";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnForceDeleteCorrupted);
            this.groupBox3.Controls.Add(this.btnRename);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.btnDeleteAll);
            this.groupBox3.Controls.Add(this.btnDeleteSelected);
            this.groupBox3.Controls.Add(this.lstFtpFiles);
            this.groupBox3.Controls.Add(this.btnListFiles);
            this.groupBox3.Location = new System.Drawing.Point(518, 10);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox3.Size = new System.Drawing.Size(285, 439);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "FTP Yönetimi";
            // 
            // btnForceDeleteCorrupted
            // 
            this.btnForceDeleteCorrupted.BackColor = System.Drawing.Color.Purple;
            this.btnForceDeleteCorrupted.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnForceDeleteCorrupted.ForeColor = System.Drawing.Color.White;
            this.btnForceDeleteCorrupted.Location = new System.Drawing.Point(11, 404);
            this.btnForceDeleteCorrupted.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnForceDeleteCorrupted.Name = "btnForceDeleteCorrupted";
            this.btnForceDeleteCorrupted.Size = new System.Drawing.Size(262, 28);
            this.btnForceDeleteCorrupted.TabIndex = 6;
            this.btnForceDeleteCorrupted.Text = "⚠ ZORLA SİL (Bozuk Klasör İçin)";
            this.btnForceDeleteCorrupted.UseVisualStyleBackColor = false;
            this.btnForceDeleteCorrupted.Click += new System.EventHandler(this.btnForceDeleteCorrupted_Click);
            // 
            // btnRename
            // 
            this.btnRename.BackColor = System.Drawing.Color.RoyalBlue;
            this.btnRename.ForeColor = System.Drawing.Color.White;
            this.btnRename.Location = new System.Drawing.Point(11, 370);
            this.btnRename.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnRename.Name = "btnRename";
            this.btnRename.Size = new System.Drawing.Size(262, 28);
            this.btnRename.TabIndex = 5;
            this.btnRename.Text = "Seçileni Yeniden Adlandır";
            this.btnRename.UseVisualStyleBackColor = false;
            this.btnRename.Click += new System.EventHandler(this.btnRename_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 61);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(151, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "FTP Sunucusundaki Dosyalar:";
            // 
            // btnDeleteAll
            // 
            this.btnDeleteAll.BackColor = System.Drawing.Color.DarkRed;
            this.btnDeleteAll.ForeColor = System.Drawing.Color.White;
            this.btnDeleteAll.Location = new System.Drawing.Point(146, 337);
            this.btnDeleteAll.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnDeleteAll.Name = "btnDeleteAll";
            this.btnDeleteAll.Size = new System.Drawing.Size(128, 28);
            this.btnDeleteAll.TabIndex = 4;
            this.btnDeleteAll.Text = "Tümünü Sil";
            this.btnDeleteAll.UseVisualStyleBackColor = false;
            this.btnDeleteAll.Click += new System.EventHandler(this.btnDeleteAll_Click);
            // 
            // btnDeleteSelected
            // 
            this.btnDeleteSelected.BackColor = System.Drawing.Color.OrangeRed;
            this.btnDeleteSelected.ForeColor = System.Drawing.Color.White;
            this.btnDeleteSelected.Location = new System.Drawing.Point(11, 337);
            this.btnDeleteSelected.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnDeleteSelected.Name = "btnDeleteSelected";
            this.btnDeleteSelected.Size = new System.Drawing.Size(128, 28);
            this.btnDeleteSelected.TabIndex = 3;
            this.btnDeleteSelected.Text = "Seçilenleri Sil";
            this.btnDeleteSelected.UseVisualStyleBackColor = false;
            this.btnDeleteSelected.Click += new System.EventHandler(this.btnDeleteSelected_Click);
            // 
            // lstFtpFiles
            // 
            this.lstFtpFiles.CheckOnClick = true;
            this.lstFtpFiles.FormattingEnabled = true;
            this.lstFtpFiles.HorizontalScrollbar = true;
            this.lstFtpFiles.Location = new System.Drawing.Point(11, 77);
            this.lstFtpFiles.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.lstFtpFiles.Name = "lstFtpFiles";
            this.lstFtpFiles.Size = new System.Drawing.Size(264, 244);
            this.lstFtpFiles.TabIndex = 2;
            // 
            // btnListFiles
            // 
            this.btnListFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnListFiles.Location = new System.Drawing.Point(11, 20);
            this.btnListFiles.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnListFiles.Name = "btnListFiles";
            this.btnListFiles.Size = new System.Drawing.Size(262, 32);
            this.btnListFiles.TabIndex = 0;
            this.btnListFiles.Text = "FTP\'deki Dosyaları Listele";
            this.btnListFiles.UseVisualStyleBackColor = true;
            this.btnListFiles.Click += new System.EventHandler(this.btnListFiles_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(814, 488);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lstLog);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btnUpload);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FTP Dosya Yükleme Otomasyonu";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtFtpHost;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtFolderPath;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.ListBox lstLog;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnListFiles;
        private System.Windows.Forms.CheckedListBox lstFtpFiles;
        private System.Windows.Forms.Button btnDeleteSelected;
        private System.Windows.Forms.Button btnDeleteAll;
        private System.Windows.Forms.Button btnRename;
        private System.Windows.Forms.Button btnForceDeleteCorrupted;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cmbSavedServers;
        private System.Windows.Forms.Button btnLoadServer;
        private System.Windows.Forms.TextBox txtServerName;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnTestAndSave;
        private System.Windows.Forms.Button btnDeleteServer;
    }
}

