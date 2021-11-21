
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
			this.bConnect = new System.Windows.Forms.Button();
			this.bConnInfo = new System.Windows.Forms.Button();
			this.bDisconnect = new System.Windows.Forms.Button();
			this.lbIP = new System.Windows.Forms.Label();
			this.LbUsr = new System.Windows.Forms.Label();
			this.lbStat = new System.Windows.Forms.Label();
			this.refreshTimer = new System.Windows.Forms.Timer(this.components);
			this.lbActive = new System.Windows.Forms.Label();
			this.lbMsg = new System.Windows.Forms.Label();
			this.bViewMsg = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.lbQuitPnd = new System.Windows.Forms.Label();
			this.btCommand = new System.Windows.Forms.Button();
			this.lbCommands = new System.Windows.Forms.ListBox();
			this.btViewCommand = new System.Windows.Forms.Button();
			this.cbComandi = new System.Windows.Forms.CheckBox();
			this.cbRisultati = new System.Windows.Forms.CheckBox();
			this.cbErrori = new System.Windows.Forms.CheckBox();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// bConnect
			// 
			this.bConnect.Location = new System.Drawing.Point(14, 91);
			this.bConnect.Name = "bConnect";
			this.bConnect.Size = new System.Drawing.Size(100, 35);
			this.bConnect.TabIndex = 3;
			this.bConnect.Text = "Connect";
			this.bConnect.UseVisualStyleBackColor = true;
			this.bConnect.Click += new System.EventHandler(this.bConnect_Click);
			// 
			// bConnInfo
			// 
			this.bConnInfo.Location = new System.Drawing.Point(120, 91);
			this.bConnInfo.Name = "bConnInfo";
			this.bConnInfo.Size = new System.Drawing.Size(100, 35);
			this.bConnInfo.TabIndex = 4;
			this.bConnInfo.Text = "Conn Info";
			this.bConnInfo.UseVisualStyleBackColor = true;
			this.bConnInfo.Click += new System.EventHandler(this.bConnInfo_Click);
			// 
			// bDisconnect
			// 
			this.bDisconnect.Location = new System.Drawing.Point(14, 132);
			this.bDisconnect.Name = "bDisconnect";
			this.bDisconnect.Size = new System.Drawing.Size(100, 35);
			this.bDisconnect.TabIndex = 5;
			this.bDisconnect.Text = "Disconnect";
			this.bDisconnect.UseVisualStyleBackColor = true;
			this.bDisconnect.Click += new System.EventHandler(this.bDisconnect_Click);
			// 
			// lbIP
			// 
			this.lbIP.AutoSize = true;
			this.lbIP.Location = new System.Drawing.Point(192, 9);
			this.lbIP.Name = "lbIP";
			this.lbIP.Size = new System.Drawing.Size(100, 15);
			this.lbIP.TabIndex = 6;
			this.lbIP.Text = "XXX.XXX.XXX.XXX";
			// 
			// LbUsr
			// 
			this.LbUsr.AutoSize = true;
			this.LbUsr.Location = new System.Drawing.Point(327, 9);
			this.LbUsr.Name = "LbUsr";
			this.LbUsr.Size = new System.Drawing.Size(12, 15);
			this.LbUsr.TabIndex = 7;
			this.LbUsr.Text = "-";
			// 
			// lbStat
			// 
			this.lbStat.AutoSize = true;
			this.lbStat.Location = new System.Drawing.Point(14, 9);
			this.lbStat.Name = "lbStat";
			this.lbStat.Size = new System.Drawing.Size(16, 15);
			this.lbStat.TabIndex = 8;
			this.lbStat.Text = "...";
			// 
			// refreshTimer
			// 
			this.refreshTimer.Tick += new System.EventHandler(this.timer_Tick);
			// 
			// lbActive
			// 
			this.lbActive.AutoSize = true;
			this.lbActive.Location = new System.Drawing.Point(506, 9);
			this.lbActive.Name = "lbActive";
			this.lbActive.Size = new System.Drawing.Size(12, 15);
			this.lbActive.TabIndex = 10;
			this.lbActive.Text = "-";
			// 
			// lbMsg
			// 
			this.lbMsg.AutoSize = true;
			this.lbMsg.Location = new System.Drawing.Point(411, 9);
			this.lbMsg.Name = "lbMsg";
			this.lbMsg.Size = new System.Drawing.Size(12, 15);
			this.lbMsg.TabIndex = 11;
			this.lbMsg.Text = "-";
			// 
			// bViewMsg
			// 
			this.bViewMsg.Location = new System.Drawing.Point(120, 132);
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
			this.panel1.Controls.Add(this.lbQuitPnd);
			this.panel1.Controls.Add(this.lbStat);
			this.panel1.Controls.Add(this.lbActive);
			this.panel1.Controls.Add(this.lbMsg);
			this.panel1.Controls.Add(this.lbIP);
			this.panel1.Controls.Add(this.LbUsr);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(697, 34);
			this.panel1.TabIndex = 13;
			// 
			// lbQuitPnd
			// 
			this.lbQuitPnd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lbQuitPnd.AutoSize = true;
			this.lbQuitPnd.Location = new System.Drawing.Point(598, 9);
			this.lbQuitPnd.Name = "lbQuitPnd";
			this.lbQuitPnd.Size = new System.Drawing.Size(12, 15);
			this.lbQuitPnd.TabIndex = 14;
			this.lbQuitPnd.Text = "-";
			this.lbQuitPnd.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// btCommand
			// 
			this.btCommand.Location = new System.Drawing.Point(14, 200);
			this.btCommand.Name = "btCommand";
			this.btCommand.Size = new System.Drawing.Size(114, 35);
			this.btCommand.TabIndex = 14;
			this.btCommand.Text = "Command (test)";
			this.btCommand.UseVisualStyleBackColor = true;
			this.btCommand.Click += new System.EventHandler(this.btCommand_Click);
			// 
			// lbCommands
			// 
			this.lbCommands.FormattingEnabled = true;
			this.lbCommands.ItemHeight = 15;
			this.lbCommands.Location = new System.Drawing.Point(231, 93);
			this.lbCommands.Name = "lbCommands";
			this.lbCommands.Size = new System.Drawing.Size(207, 139);
			this.lbCommands.TabIndex = 15;
			this.lbCommands.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lbCommands_DrawItem);
			// 
			// btViewCommand
			// 
			this.btViewCommand.Location = new System.Drawing.Point(14, 241);
			this.btViewCommand.Name = "btViewCommand";
			this.btViewCommand.Size = new System.Drawing.Size(114, 69);
			this.btViewCommand.TabIndex = 16;
			this.btViewCommand.Text = "View command";
			this.btViewCommand.UseVisualStyleBackColor = true;
			this.btViewCommand.Click += new System.EventHandler(this.btViewCommand_Click);
			// 
			// cbComandi
			// 
			this.cbComandi.AutoSize = true;
			this.cbComandi.Checked = true;
			this.cbComandi.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbComandi.Location = new System.Drawing.Point(134, 241);
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
			this.cbRisultati.Location = new System.Drawing.Point(134, 266);
			this.cbRisultati.Name = "cbRisultati";
			this.cbRisultati.Size = new System.Drawing.Size(68, 19);
			this.cbRisultati.TabIndex = 18;
			this.cbRisultati.Text = "Risultati";
			this.cbRisultati.UseVisualStyleBackColor = true;
			// 
			// cbErrori
			// 
			this.cbErrori.AutoSize = true;
			this.cbErrori.Location = new System.Drawing.Point(134, 291);
			this.cbErrori.Name = "cbErrori";
			this.cbErrori.Size = new System.Drawing.Size(54, 19);
			this.cbErrori.TabIndex = 19;
			this.cbErrori.Text = "Errori";
			this.cbErrori.UseVisualStyleBackColor = true;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(697, 507);
			this.Controls.Add(this.cbErrori);
			this.Controls.Add(this.cbRisultati);
			this.Controls.Add(this.cbComandi);
			this.Controls.Add(this.btViewCommand);
			this.Controls.Add(this.lbCommands);
			this.Controls.Add(this.btCommand);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.bViewMsg);
			this.Controls.Add(this.bDisconnect);
			this.Controls.Add(this.bConnInfo);
			this.Controls.Add(this.bConnect);
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
		private System.Windows.Forms.Button bConnect;
		private System.Windows.Forms.Button bConnInfo;
		private System.Windows.Forms.Button bDisconnect;
		private System.Windows.Forms.Label lbIP;
		private System.Windows.Forms.Label LbUsr;
		private System.Windows.Forms.Label lbStat;
		private System.Windows.Forms.Timer refreshTimer;
		private System.Windows.Forms.Label lbActive;
		private System.Windows.Forms.Label lbMsg;
		private System.Windows.Forms.Button bViewMsg;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label lbQuitPnd;
		private System.Windows.Forms.Button btCommand;
		private System.Windows.Forms.ListBox lbCommands;
		private System.Windows.Forms.Button btViewCommand;
		private System.Windows.Forms.CheckBox cbComandi;
		private System.Windows.Forms.CheckBox cbRisultati;
		private System.Windows.Forms.CheckBox cbErrori;
		}
	}

