
namespace RPIconn
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
			if(disposing && (components != null))
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
			this.components = new System.ComponentModel.Container();
			this.bSshConnect = new System.Windows.Forms.Button();
			this.bConnInfo = new System.Windows.Forms.Button();
			this.lbIP = new System.Windows.Forms.Label();
			this.LbUsr = new System.Windows.Forms.Label();
			this.lbSshStat = new System.Windows.Forms.Label();
			this.refreshTimer = new System.Windows.Forms.Timer(this.components);
			this.lbActive = new System.Windows.Forms.Label();
			this.lbMsg = new System.Windows.Forms.Label();
			this.bViewMsg = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.lbSftpStat = new System.Windows.Forms.Label();
			this.lbQuitPnd = new System.Windows.Forms.Label();
			this.btCommand = new System.Windows.Forms.Button();
			this.lbCommands = new System.Windows.Forms.ListBox();
			this.btViewResult = new System.Windows.Forms.Button();
			this.cbComandi = new System.Windows.Forms.CheckBox();
			this.cbRisultati = new System.Windows.Forms.CheckBox();
			this.cbErrori = new System.Windows.Forms.CheckBox();
			this.btViewVariables = new System.Windows.Forms.Button();
			this.bSftpConnect = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.bTestSftp = new System.Windows.Forms.Button();
			this.tbPath = new System.Windows.Forms.TextBox();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// bSshConnect
			// 
			this.bSshConnect.Location = new System.Drawing.Point(56, 91);
			this.bSshConnect.Name = "bSshConnect";
			this.bSshConnect.Size = new System.Drawing.Size(100, 35);
			this.bSshConnect.TabIndex = 3;
			this.bSshConnect.Text = "Connect";
			this.bSshConnect.UseVisualStyleBackColor = true;
			this.bSshConnect.Click += new System.EventHandler(this.bSshConnect_Click);
			// 
			// bConnInfo
			// 
			this.bConnInfo.Location = new System.Drawing.Point(56, 40);
			this.bConnInfo.Name = "bConnInfo";
			this.bConnInfo.Size = new System.Drawing.Size(100, 35);
			this.bConnInfo.TabIndex = 4;
			this.bConnInfo.Text = "Conn Info";
			this.bConnInfo.UseVisualStyleBackColor = true;
			this.bConnInfo.Click += new System.EventHandler(this.bSshConnInfo_Click);
			// 
			// lbIP
			// 
			this.lbIP.AutoSize = true;
			this.lbIP.Location = new System.Drawing.Point(135, 9);
			this.lbIP.Name = "lbIP";
			this.lbIP.Size = new System.Drawing.Size(100, 15);
			this.lbIP.TabIndex = 6;
			this.lbIP.Text = "XXX.XXX.XXX.XXX";
			// 
			// LbUsr
			// 
			this.LbUsr.AutoSize = true;
			this.LbUsr.Location = new System.Drawing.Point(263, 9);
			this.LbUsr.Name = "LbUsr";
			this.LbUsr.Size = new System.Drawing.Size(12, 15);
			this.LbUsr.TabIndex = 7;
			this.LbUsr.Text = "-";
			// 
			// lbSshStat
			// 
			this.lbSshStat.AutoSize = true;
			this.lbSshStat.Location = new System.Drawing.Point(14, 9);
			this.lbSshStat.Name = "lbSshStat";
			this.lbSshStat.Size = new System.Drawing.Size(16, 15);
			this.lbSshStat.TabIndex = 8;
			this.lbSshStat.Text = "...";
			// 
			// refreshTimer
			// 
			this.refreshTimer.Tick += new System.EventHandler(this.timer_Tick);
			// 
			// lbActive
			// 
			this.lbActive.AutoSize = true;
			this.lbActive.Location = new System.Drawing.Point(462, 9);
			this.lbActive.Name = "lbActive";
			this.lbActive.Size = new System.Drawing.Size(12, 15);
			this.lbActive.TabIndex = 10;
			this.lbActive.Text = "-";
			// 
			// lbMsg
			// 
			this.lbMsg.AutoSize = true;
			this.lbMsg.Location = new System.Drawing.Point(352, 9);
			this.lbMsg.Name = "lbMsg";
			this.lbMsg.Size = new System.Drawing.Size(12, 15);
			this.lbMsg.TabIndex = 11;
			this.lbMsg.Text = "-";
			// 
			// bViewMsg
			// 
			this.bViewMsg.Location = new System.Drawing.Point(162, 40);
			this.bViewMsg.Name = "bViewMsg";
			this.bViewMsg.Size = new System.Drawing.Size(100, 35);
			this.bViewMsg.TabIndex = 12;
			this.bViewMsg.Text = "View Messages";
			this.bViewMsg.UseVisualStyleBackColor = true;
			this.bViewMsg.Click += new System.EventHandler(this.bViewMsg_Click);
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
			this.panel1.Controls.Add(this.lbSftpStat);
			this.panel1.Controls.Add(this.lbQuitPnd);
			this.panel1.Controls.Add(this.lbSshStat);
			this.panel1.Controls.Add(this.lbActive);
			this.panel1.Controls.Add(this.lbMsg);
			this.panel1.Controls.Add(this.lbIP);
			this.panel1.Controls.Add(this.LbUsr);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(662, 34);
			this.panel1.TabIndex = 13;
			// 
			// lbSftpStat
			// 
			this.lbSftpStat.AutoSize = true;
			this.lbSftpStat.Location = new System.Drawing.Point(61, 9);
			this.lbSftpStat.Name = "lbSftpStat";
			this.lbSftpStat.Size = new System.Drawing.Size(16, 15);
			this.lbSftpStat.TabIndex = 15;
			this.lbSftpStat.Text = "...";
			// 
			// lbQuitPnd
			// 
			this.lbQuitPnd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lbQuitPnd.AutoSize = true;
			this.lbQuitPnd.Location = new System.Drawing.Point(511, 9);
			this.lbQuitPnd.Name = "lbQuitPnd";
			this.lbQuitPnd.Size = new System.Drawing.Size(12, 15);
			this.lbQuitPnd.TabIndex = 14;
			this.lbQuitPnd.Text = "-";
			this.lbQuitPnd.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// btCommand
			// 
			this.btCommand.Location = new System.Drawing.Point(374, 91);
			this.btCommand.Name = "btCommand";
			this.btCommand.Size = new System.Drawing.Size(100, 35);
			this.btCommand.TabIndex = 14;
			this.btCommand.Text = "Execute";
			this.btCommand.UseVisualStyleBackColor = true;
			this.btCommand.Click += new System.EventHandler(this.btCommand_Click);
			// 
			// lbCommands
			// 
			this.lbCommands.FormattingEnabled = true;
			this.lbCommands.ItemHeight = 15;
			this.lbCommands.Location = new System.Drawing.Point(162, 91);
			this.lbCommands.Name = "lbCommands";
			this.lbCommands.Size = new System.Drawing.Size(206, 109);
			this.lbCommands.TabIndex = 15;
			this.lbCommands.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lbCommands_DrawItem);
			// 
			// btViewResult
			// 
			this.btViewResult.Location = new System.Drawing.Point(374, 131);
			this.btViewResult.Name = "btViewResult";
			this.btViewResult.Size = new System.Drawing.Size(100, 69);
			this.btViewResult.TabIndex = 16;
			this.btViewResult.Text = "Results";
			this.btViewResult.UseVisualStyleBackColor = true;
			this.btViewResult.Click += new System.EventHandler(this.btViewCommand_Click);
			// 
			// cbComandi
			// 
			this.cbComandi.AutoSize = true;
			this.cbComandi.Checked = true;
			this.cbComandi.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbComandi.Location = new System.Drawing.Point(480, 132);
			this.cbComandi.Name = "cbComandi";
			this.cbComandi.Size = new System.Drawing.Size(75, 19);
			this.cbComandi.TabIndex = 17;
			this.cbComandi.Text = "Comandi";
			this.cbComandi.UseVisualStyleBackColor = true;
			// 
			// cbRisultati
			// 
			this.cbRisultati.AutoSize = true;
			this.cbRisultati.Checked = true;
			this.cbRisultati.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbRisultati.Location = new System.Drawing.Point(480, 157);
			this.cbRisultati.Name = "cbRisultati";
			this.cbRisultati.Size = new System.Drawing.Size(68, 19);
			this.cbRisultati.TabIndex = 18;
			this.cbRisultati.Text = "Risultati";
			this.cbRisultati.UseVisualStyleBackColor = true;
			// 
			// cbErrori
			// 
			this.cbErrori.AutoSize = true;
			this.cbErrori.Location = new System.Drawing.Point(480, 182);
			this.cbErrori.Name = "cbErrori";
			this.cbErrori.Size = new System.Drawing.Size(54, 19);
			this.cbErrori.TabIndex = 19;
			this.cbErrori.Text = "Errori";
			this.cbErrori.UseVisualStyleBackColor = true;
			// 
			// btViewVariables
			// 
			this.btViewVariables.Location = new System.Drawing.Point(268, 40);
			this.btViewVariables.Name = "btViewVariables";
			this.btViewVariables.Size = new System.Drawing.Size(100, 35);
			this.btViewVariables.TabIndex = 20;
			this.btViewVariables.Text = "Variables";
			this.btViewVariables.UseVisualStyleBackColor = true;
			this.btViewVariables.Click += new System.EventHandler(this.btViewVariables_Click);
			// 
			// bSftpConnect
			// 
			this.bSftpConnect.Location = new System.Drawing.Point(56, 229);
			this.bSftpConnect.Name = "bSftpConnect";
			this.bSftpConnect.Size = new System.Drawing.Size(100, 35);
			this.bSftpConnect.TabIndex = 21;
			this.bSftpConnect.Text = "Connect";
			this.bSftpConnect.UseVisualStyleBackColor = true;
			this.bSftpConnect.Click += new System.EventHandler(this.bSftpConnect_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(14, 101);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(25, 15);
			this.label1.TabIndex = 22;
			this.label1.Text = "Ssh";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(14, 239);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(28, 15);
			this.label2.TabIndex = 23;
			this.label2.Text = "Sftp";
			// 
			// bTestSftp
			// 
			this.bTestSftp.Location = new System.Drawing.Point(202, 229);
			this.bTestSftp.Name = "bTestSftp";
			this.bTestSftp.Size = new System.Drawing.Size(93, 46);
			this.bTestSftp.TabIndex = 24;
			this.bTestSftp.Text = "Dir ?";
			this.bTestSftp.UseVisualStyleBackColor = true;
			this.bTestSftp.Click += new System.EventHandler(this.bTestSftp_Click);
			// 
			// tbPath
			// 
			this.tbPath.Location = new System.Drawing.Point(300, 235);
			this.tbPath.Name = "tbPath";
			this.tbPath.Size = new System.Drawing.Size(290, 23);
			this.tbPath.TabIndex = 25;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(662, 363);
			this.Controls.Add(this.tbPath);
			this.Controls.Add(this.bTestSftp);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.bSftpConnect);
			this.Controls.Add(this.btViewVariables);
			this.Controls.Add(this.cbErrori);
			this.Controls.Add(this.cbRisultati);
			this.Controls.Add(this.cbComandi);
			this.Controls.Add(this.btViewResult);
			this.Controls.Add(this.lbCommands);
			this.Controls.Add(this.btCommand);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.bViewMsg);
			this.Controls.Add(this.bConnInfo);
			this.Controls.Add(this.bSshConnect);
			this.Name = "MainForm";
			this.Text = "RPI connect";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

			}

		#endregion
		private System.Windows.Forms.Button bSshConnect;
		private System.Windows.Forms.Button bConnInfo;
		private System.Windows.Forms.Label lbIP;
		private System.Windows.Forms.Label LbUsr;
		private System.Windows.Forms.Label lbSshStat;
		private System.Windows.Forms.Timer refreshTimer;
		private System.Windows.Forms.Label lbActive;
		private System.Windows.Forms.Label lbMsg;
		private System.Windows.Forms.Button bViewMsg;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label lbQuitPnd;
		private System.Windows.Forms.Button btCommand;
		private System.Windows.Forms.ListBox lbCommands;
		private System.Windows.Forms.Button btViewResult;
		private System.Windows.Forms.CheckBox cbComandi;
		private System.Windows.Forms.CheckBox cbRisultati;
		private System.Windows.Forms.CheckBox cbErrori;
		private System.Windows.Forms.Button btViewVariables;
		private System.Windows.Forms.Label lbSftpStat;
		private System.Windows.Forms.Button bSftpConnect;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button bTestSftp;
		private System.Windows.Forms.TextBox tbPath;
		}
	}

