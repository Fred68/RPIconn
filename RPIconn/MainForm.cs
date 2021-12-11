using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

using Renci.SshNet;
using System.IO;

namespace RPIconn
	{
	public partial class MainForm:Form
		{

		//		FATTO:
		//			Provare disconnessione (RPI spenta durante una sessione)
		//			Aggiungere evento OnClose() per chiudere le connessione
		//			Se chiamato Disconnect prima del termine del Task Connect, si blocca.
		//			L'interfaccia non può esser modificata dai Task. Usare BeginInvoke o Invoke()
		//			Vedere se BeginInvoke richiede un EndInvoke. Non necessario !
		//			Capire come leggere le risposte in modo asincrono: non serve al momento, eseguendo dei comandi relativamente semplici.
		//			Lista di Task. Non serve, con Task.Factory.StartNew(...) si crea un Task nel ThreadPool gestito dal Common Language Runtime
		//			Usare il Dictionary dei comandi, con un oggetto ComandoTask con i dati del comando
		//			L'oggetto ComandoTask contiene: lista comandi e dei risultati, Task, dati (output, callback), stato (per es. se è già avviato...)
		//			Capire come leggere le risposte in modo sincrono (leggere i nomi delle variabili con Back12.sh -i). Leggere l'output.

		#warning DA FARE:

		//			Aggiungere connessione sftp

		#warning	Vedere: https://ourcodeworld.com/articles/read/369/how-to-access-a-sftp-server-using-ssh-net-sync-and-async-with-c-in-winforms

		#warning	Occhio al ciclo di lettura della lista dei messaggi (yield retunn). Aggiungere lock() o impedire la cancellazione di messaggi (solo sotto lock): fare una classe derivata ?
			
		#warning	File di configurazione: come codificare nel file la lettura dei dati ricevuti. Per ora comando READ per leggere le variabili
		#warning	Permettere 1 solo avvio in multithread di un singolo comando
		
		#warning	Modificare, all'avvio e alla chiusura di un comando, la sua visualizzazione nella listbox
		#warning	Studiare listbox derivate (con i colori) e con oggetti (invece che con stringhe). L'oggetto è un ComandoTask.
		#warning	Aggiungere un lock alla list box ? No, usare BeginInvoke() dal thread del comando al thread principale (main UI thread).
		
		#warning	Capire come leggere le informazioni dei file (ultima modifica e dimensione) su Windows
		#warning	Capire come leggere le informazioni dei file (ultima modifica e dimensione) su Linux
		#warning	Capire come eseguire il trasferimento di file in ssh, da programma

		#warning	In futuro: Aggiungere form per inserire i dati per il collegamento
		#warning	In futuro: salvare la password crittografata (chiave nelle impostazioni di configurazione utente del programma)


		#region CONFIGURATION DATA

		/// <summary>
		/// Configuration file name
		/// </summary>
		string configFile;

		/// <summary>
		/// Command file name
		/// </summary>
		string commandFile;

		/// <summary>
		/// Keywords and value in configuration file
		/// </summary>
		Dictionary<string, string> cfg = new Dictionary<string, string>()
			{
				{Defs.IP,""},
				{Defs.USR,""},
				{Defs.PWD,""},
				{Defs.CMD,""}
			};


		#endregion


		#region CONNECTION DATA

		/// <summary>
		/// Connection data
		/// </summary>
		ConnectionData connData;

		/// <summary>
		/// Flag if quit requested during connection
		/// </summary>
		bool pendingQuit;

		#endregion

		#region COMMAND AND VARIABLES DATA

		/// <summary>
		/// Command dictionary
		/// </summary>
		DictionaryWithLock<ComandoT> comandi;

		/// <summary>
		/// Variables dictionary
		/// </summary>
		DictionaryWithLock<string> variabili;
		
		#endregion



		#region PROPERTIES

		/// <summary>
		/// Enum with connection stat
		/// </summary>
		public enum ConnectionStatEnum {Disconnected = 0, Connecting, Disconnecting, Connected};

		/// <summary>
		/// Ssh connection stat property (read only)
		/// </summary>
		public ConnectionStatEnum SshConnectionStatus
			{
			get
				{
				ConnectionStatEnum cstat = ConnectionStatEnum.Disconnected;
				
				if(connData.sshConnectTask != null)
					{
					if(!connData.ssh_ct.IsCancellationRequested)
						cstat = ConnectionStatEnum.Connecting;
					else
						cstat = ConnectionStatEnum.Disconnecting;
					}
				else
					{
					if(connData.sshClient != null)
						if(connData.sshClient.IsConnected)
							cstat = ConnectionStatEnum.Connected;
					}
				return cstat;
				}
			}

		/// <summary>
		/// Sftp connection stat property (read only)
		/// </summary>
		public ConnectionStatEnum SftpConnectionStatus
			{
			get
				{
				ConnectionStatEnum cstat = ConnectionStatEnum.Disconnected;
				
				if(connData.sftpConnectTask != null)
					{
					if(!connData.ssh_ct.IsCancellationRequested)
						cstat = ConnectionStatEnum.Connecting;
					else
						cstat = ConnectionStatEnum.Disconnecting;
					}
				else
					{
					if(connData.sftpClient != null)
						if(connData.sftpClient.IsConnected)
							cstat = ConnectionStatEnum.Connected;
					}
				return cstat;
				}
			}			

		/// <summary>
		/// Check if ssh connection is active
		/// </summary>
		/// <returns></returns>
		public bool IsSshConnected
			{
			get
				{
				bool conn = false;
				if(connData.sshClient != null)
					if(connData.sshClient.IsConnected)
						conn = true;
				return conn;
				}
			}

		/// <summary>
		/// Check if sftp connection is active
		/// </summary>
		/// <returns></returns>
		public bool IsSftpConnected
			{
			get
				{
				bool conn = false;
				if(connData.sftpClient != null)
					if(connData.sftpClient.IsConnected)
						conn = true;
				return conn;
				}
			}
		
		/// <summary>
		/// Get ssh connection Task status (if running or not)
		/// </summary>
		public bool IsSshConnRunning
			{
			get
				{
				bool run = false;
				if(connData.sshConnectTask != null)
					{
					// Calling task.IsCompleted stop the current thread until the task is really completed
					//if(!connectTask.IsCompleted)
					//	run = true;
					run = true;
					}
				return run;
				}
			}

		/// <summary>
		/// Get sftp connection Task status (if running or not)
		/// </summary>
		public bool IsSftpConnRunning
			{
			get
				{
				bool run = false;
				if(connData.sftpConnectTask != null)
					{
					// Calling task.IsCompleted stop the current thread until the task is really completed
					//if(!connectTask.IsCompleted)
					//	run = true;
					run = true;
					}
				return run;
				}
			}

		/// <summary>
		/// Get ssh connection username and host (IP)
		/// </summary>
		public Tuple<string,string> SshConnStat
			{
			get
				{
				string host = "-";
				string user = "-";
				if(connData.sshClient != null)
					if(connData.sshClient.IsConnected)
						{
						ConnectionInfo cinfo = connData.sshClient.ConnectionInfo;
						host = cinfo.Host;
						user = cinfo.Username;
						}
				return new Tuple<string,string>(host,user);
				}
			}

		/// <summary>
		/// Get sftp connection username and host (IP)
		/// </summary>
		public Tuple<string,string> SftpConnStat
			{
			get
				{
				string host = "-";
				string user = "-";
				if(connData.sftpClient != null)
					if(connData.sftpClient.IsConnected)
						{
						ConnectionInfo cinfo = connData.sftpClient.ConnectionInfo;
						host = cinfo.Host;
						user = cinfo.Username;
						}
				return new Tuple<string,string>(host,user);
				}
			}

		/// <summary>
		/// Number of running tasks
		/// </summary>
		public int RunningTasks
			{
			get
				{
				int i= 0;
				foreach(ComandoT cm in comandi.Values)
					{
					if(cm.IsRunning)
						i++;
					}
				return i;	
				}
			}

		/// <summary>
		/// Connection Data
		/// </summary>
		public ConnectionData ConnData
			{
			get {return connData;}
			}

		/// <summary>
		/// Command dictionary
		/// </summary>
		public DictionaryWithLock<ComandoT> Comandi
			{
			get {return comandi;}
			}

		/// <summary>
		/// Variables dictionary
		/// </summary>
		public DictionaryWithLock<string> Variabili
			{
			get {return variabili;}
			}
		#endregion


		


		#region MAIN FORM
		#region GUI
		Font lbFont;	
		SolidBrush[] brushes;

		SolidBrush lbBrushAvail;
		SolidBrush lbBrushUnavail;
		#endregion
		
		
		/// <summary>
		/// Main form ctor
		/// </summary>
		public MainForm()
			{
			InitializeComponent();
			configFile = Defs.CFG_FILENAME;
			connData = new ConnectionData();
			pendingQuit = false;
			
			commandFile = Defs.CMD_FILENAME;

			comandi = new DictionaryWithLock<ComandoT>();
			variabili = new DictionaryWithLock<string>();

			ComandoT.SetVariabili(variabili);

			}

		/// <summary>
		/// OnLoad
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MainForm_Load(object sender,EventArgs e)
			{
			lbFont = new Font(Defs.FONT, Defs.FONT_SIZE);

			brushes = new SolidBrush[Enum.GetNames(typeof(ConnectionStatEnum)).Length];
			brushes[(int)ConnectionStatEnum.Disconnected] = new SolidBrush(Color.DarkRed);
			brushes[(int)ConnectionStatEnum.Connecting] = new SolidBrush(Color.Yellow);
			brushes[(int)ConnectionStatEnum.Disconnecting] = new SolidBrush(Color.Orange);
			brushes[(int)ConnectionStatEnum.Connected] = new SolidBrush(Color.Green);
			
			lbBrushAvail = brushes[(int)ConnectionStatEnum.Connected];
			lbBrushUnavail = brushes[(int)ConnectionStatEnum.Disconnected];

			refreshTimer.Interval = (int)(Defs.CONN_REFRESH*1000f);
			refreshTimer.Enabled = true;
			
			if(!ComandoT.ReadCommands(commandFile, ConnData, Comandi))
				{
				MessageBox.Show(Messaggi.ERR.COMMANDS);
				}

			lbCommands.Items.Clear();
			lbCommands.SelectionMode = SelectionMode.One;

			foreach(ComandoT s in comandi.Values)
				{
				lbCommands.Items.Add(s);
				}

			lbCommands.DrawMode = DrawMode.OwnerDrawFixed;

			lbSshStat.Text = "Ssh";
			lbSftpStat.Text = "Sftp";

			UpdateStatus();
			}

		/// <summary>
		/// Update main form with up-to-date connection data
		/// </summary>
		public void UpdateStatus()
			{

			lbSshStat.ForeColor = brushes[(int)this.SshConnectionStatus].Color;
			lbSftpStat.ForeColor = brushes[(int)this.SftpConnectionStatus].Color;

			switch(SshConnectionStatus)
				{
				case ConnectionStatEnum.Connected:
					{
					bSshConnect.Text = Messaggi.GUI.MSG.DISCONNECT;
					}
					break;
				case ConnectionStatEnum.Disconnected:
					{
					bSshConnect.Text = Messaggi.GUI.MSG.CONNECT;
					}
					break;
				default:
					{
					bSshConnect.Text = this.SshConnectionStatus.ToString()+"...";
					}
					break;
				}

			switch(SftpConnectionStatus)
				{
				case ConnectionStatEnum.Connected:
					{
					bSftpConnect.Text = Messaggi.GUI.MSG.DISCONNECT;
					}
					break;
				case ConnectionStatEnum.Disconnected:
					{
					bSftpConnect.Text = Messaggi.GUI.MSG.CONNECT;
					}
					break;
				default:
					{
					bSftpConnect.Text = this.SftpConnectionStatus.ToString()+"...";
					}
					break;
				}
			
			lbQuitPnd.Text = pendingQuit ? "Closing program" : string.Empty;

			Tuple<string,string> cinfo = SshConnStat;
			lbIP.Text = cinfo.Item1;
			LbUsr.Text = cinfo.Item2;
			
			if(Messaggi.HasMessages(Messaggi.Tipo.Errori))
				lbMsg.Text = "E";
			else if(Messaggi.HasMessages(Messaggi.Tipo.Messaggi))
				lbMsg.Text = "M";
			else
				lbMsg.Text = "";

			if(lbActive.Text == "/")
				lbActive.Text = "\\";
			else
				lbActive.Text = "/";
			
			Invalidate();
			}

		/// <summary>
		/// Return selected CommandT in list box
		/// </summary>
		/// <returns>CommandT or null if not selected or not found</returns>
		public ComandoT ComandoSelezionato()
			{
			ComandoT cmd = null;
			string c = string.Empty;

			if(lbCommands.SelectedIndex != -1)
				{
				c = lbCommands.SelectedItem.ToString();
				if(!comandi.ContainsKey(c))
					{
					Messaggi.AddMessage(Messaggi.ERR.COMMAND_NOT_FOUND, Messaggi.Tipo.Errori, $"{c}=?");
					}
				else 
					{
					if(comandi[c].ComandiBash.Count == 0)
						{
						Messaggi.AddMessage(Messaggi.ERR.COMMAND_EMPTY, Messaggi.Tipo.Errori, $"{c}=?");
						}
					else
						{
						cmd = comandi[c];
						}
					}
				}
			return cmd;
			}
		#endregion


		#region CONNECTION

		/// <summary>
		/// Connect to ssh server (not asynchronous)
		/// </summary>
		SshClient SshConnect()
			{
			SshClient sshc = null;
			Messaggi.Clear();
			try
				{
				sshc = new SshClient(cfg[Defs.IP], cfg[Defs.USR], cfg[Defs.PWD]);
				sshc.Connect();
				}
			catch(Exception ex)
				{
				Messaggi.AddMessage(Messaggi.ERR.ERROR, Messaggi.Tipo.Errori, ex.Message);
				if(sshc != null)
					sshc.Disconnect();
				sshc = null;
				}
			return sshc;
			}

		/// <summary>
		/// Connect to sftp server (not asynchronous)
		/// </summary>
		SftpClient SftpConnect()
			{
			SftpClient sftpc = null;
			Messaggi.Clear();
			try
				{
				sftpc = new SftpClient(cfg[Defs.IP], cfg[Defs.USR], cfg[Defs.PWD]);
				sftpc.Connect();
				}
			catch(Exception ex)
				{
				Messaggi.AddMessage(Messaggi.ERR.ERROR, Messaggi.Tipo.Errori, ex.Message);
				if(sftpc != null)
					sftpc.Disconnect();
				sftpc = null;
				}
			return sftpc;
			}

		/// <summary>
		/// Start connection task
		/// and prepare callback function
		/// </summary>
		Task<SshClient> StartSshConnection()
			{
			Task<SshClient> task = null;
			if(ReadConnPar(configFile, ref cfg))
				{
				if(!IsSshConnected)
					{
					connData.ssh_cts = new CancellationTokenSource();
					connData.ssh_ct = connData.ssh_cts.Token;
					if( (connData.ssh_ct != CancellationToken.None) && (connData.ssh_cts != null))		// If cancellation toke and sourc are not null...
						 {
						 connData.ssh_ct.Register( () =>										// Register callback function, when task is canceled
							{
							#if DEBUG
							MessageBox.Show(Messaggi.MSG.TASK_CANCELED);
							#endif
							connData.ssh_cts.Dispose();										// Clear object (no more valid, token expired)
							panel1.BeginInvoke(new Action( () =>	{
																	UpdateStatus();
																	}
															));	// Update status (when UI thread is ready).
							#if DEBUG
							Messaggi.AddMessage("Arresto in corso del task di connessione ssh");
							#endif
							}
							);
						 }
					task = Task<SshClient>.Factory.StartNew( () => SshConnect(), connData.ssh_ct);
					if(task != null)
						{
						task.ContinueWith(AfterSshConnection);
						}
					}
				}
			return task;
			}

		/// <summary>
		/// Start connection task
		/// and prepare callback function
		/// </summary>
		Task<SftpClient> StartSftpConnection()
			{
			Task<SftpClient> task = null;
			if(ReadConnPar(configFile, ref cfg))
				{
				if(!IsSftpConnected)
					{
					connData.sftp_cts = new CancellationTokenSource();
					connData.sftp_ct = connData.sftp_cts.Token;
					if( (connData.sftp_ct != CancellationToken.None) && (connData.sftp_cts != null))		// If cancellation toke and sourc are not null...
						 {
						 connData.sftp_ct.Register( () =>										// Register callback function, when task is canceled
							{
							#if DEBUG
							MessageBox.Show(Messaggi.MSG.TASK_CANCELED);
							#endif
							connData.sftp_cts.Dispose();										// Clear object (no more valid, token expired)
							panel1.BeginInvoke(new Action( () =>	{
																	UpdateStatus();
																	}
															));	// Update status (when UI thread is ready).
							#if DEBUG
							Messaggi.AddMessage("Arresto in corso del task di connessione sftp");
							#endif
							}
							);
						 }
					task = Task<SftpClient>.Factory.StartNew( () => SftpConnect(), connData.sftp_ct);
					if(task != null)
						{
						task.ContinueWith(AfterSftpConnection);
						}
					}
				}
			return task;
			}

		/// <summary>
		/// Callback function, after connection task is completed
		/// </summary>
		/// <param name="t"></param>
		void AfterSshConnection(Task<SshClient> t)
			{
			SshClient res = t.Result;		// Read SshClient return value from delegate function started in the task
			if(res != null)
				{
				connData.sshClient = res;			// Saves connection data
				}
			#if DEBUG
			Messaggi.AddMessage($"Ssh task is completed {t.Status.ToString()}");
			#endif
			connData.sshConnectTask = null;			// Clear the task
			BeginInvoke(new Action( () =>	{
											UpdateStatus();		// Update status (when UI thread is ready) but does not stop the calling thread
											}					// EndInvoke() is not necessary (no need of a callback method).
									));		
			if(pendingQuit)
				{
				BeginInvoke(new Action( () => Close()));		// Close application windows (when UI thread is ready).
				}
			} 

		/// <summary>
		/// Callback function, after connection task is completed
		/// </summary>
		/// <param name="t"></param>
		void AfterSftpConnection(Task<SftpClient> t)
			{
			SftpClient res = t.Result;		// Read SshClient return value from delegate function started in the task
			if(res != null)
				{
				connData.sftpClient = res;			// Saves connection data
				}
			#if DEBUG
			Messaggi.AddMessage($"Sftp task is completed {t.Status.ToString()}");
			#endif
			connData.sftpConnectTask = null;			// Clear the task
			BeginInvoke(new Action( () =>	{
											UpdateStatus();		// Update status (when UI thread is ready) but does not stop the calling thread
											}					// EndInvoke() is not necessary (no need of a callback method).
									));		
			if(pendingQuit)
				{
				BeginInvoke(new Action( () => Close()));		// Close application windows (when UI thread is ready).
				}
			} 

		/// <summary>
		/// Check and ask is active stak can be stopped
		/// </summary>
		/// <returns></returns>
		bool CheckActiveTask()
			{
			bool ok = true;
			if(RunningTasks > 0)
				{
				if(MessageBox.Show(Messaggi.GUI.MSG.TASK_ATTIVI, Messaggi.GUI.TIT.TASK, MessageBoxButtons.YesNo) != DialogResult.Yes)
					{
					ok = false;
					}
				}
			return ok;
			}

		/// <summary>
		/// Disconnect
		/// </summary>
		void SshDisconnect()
			{
			if(!CheckActiveTask())								// Ask, if there is an active task
				return;
			if(connData.sshConnectTask != null)					// If still connecting (i.e.: double click on disconnect, token already used)
				{
				if(!connData.ssh_ct.IsCancellationRequested)		// If no task cancellation request pending...
					{
					connData.ssh_cts.Cancel();						// ...request connection task cancellation
					#if DEBUG
					Messaggi.AddMessage("Richiesta cancellazione del task di connessione ssh");
					#endif
					Thread.Sleep(Defs.CANCEL_PAUSE);		// Wait task to stop (msec)
					}
				else								// If a cancellation request is pending...
					{
					return;							// ...do nothing and exit function
					}
				bool taskcompleted = true;			// Salva lo stato del task
				try
					{
					taskcompleted = connData.sshConnectTask.IsCompleted;	// In try block: connectTask might have become null
					}
				catch
					{
					taskcompleted = true;
					}
				if(!taskcompleted)					// If still connecting... This test waits for the task completion...!
					{
					#if DEBUG
					Messaggi.AddMessage("Il task di connessione ssh non è ancora completato");
					#endif
					// connectTask.Wait();			// ...wait for task completion. NO: continue executionn
					// ...connectTask.IsCompleted.ToString()}... This command waits for task completion. NO: continue execution.
					return;
					}
				else
					{
					connData.sshConnectTask = null;					// At the end, reset task to null, anyway.
					#if DEBUG
					Messaggi.AddMessage($"Task di connessione ssh azzerato");
					#endif
					}
				}
			if(connData.sshClient != null)					// If sshClient is not null...
				{
				#if DEBUG
				Messaggi.AddMessage($"Disconnessione del client ssh");
				#endif
				connData.sshClient.Disconnect();				// ...disconnct, anyway.
				connData.sshClient = null;
				#if DEBUG
				Messaggi.AddMessage($"Azzeramento del Client ssh");
				#endif
				}
			#if DEBUG
			Messaggi.AddMessage($"Client ssh is {((connData.sshClient == null) ? string.Empty : "not ")}null");
			#endif
			if(pendingQuit)		// If there is a Close() request (quit program) when task or connection were active...
				Close();		// ...ask for Close() again (for the Main UI Thread)
			}

		/// <summary>
		/// Disconnect
		/// </summary>
		void SftpDisconnect()
			{
			if(!CheckActiveTask())								// Ask, if there is an active task
				return;
			if(connData.sftpConnectTask != null)					// If still connecting (i.e.: double click on disconnect, token already used)
				{
				if(!connData.sftp_ct.IsCancellationRequested)		// If no task cancellation request pending...
					{
					connData.sftp_cts.Cancel();						// ...request connection task cancellation
					#if DEBUG
					Messaggi.AddMessage("Richiesta cancellazione del task di connessione sftp");
					#endif
					Thread.Sleep(Defs.CANCEL_PAUSE);		// Wait task to stop (msec)
					}
				else								// If a cancellation request is pending...
					{
					return;							// ...do nothing and exit function
					}
				bool taskcompleted = true;			// Salva lo stato del task
				try
					{
					taskcompleted = connData.sftpConnectTask.IsCompleted;	// In try block: connectTask might have become null
					}
				catch
					{
					taskcompleted = true;
					}
				if(!taskcompleted)					// If still connecting... This test waits for the task completion...!
					{
					#if DEBUG
					Messaggi.AddMessage("Il task di connessione sftp non è ancora completato");
					#endif
					// connectTask.Wait();			// ...wait for task completion. NO: continue executionn
					// ...connectTask.IsCompleted.ToString()}... This command waits for task completion. NO: continue execution.
					return;
					}
				else
					{
					connData.sftpConnectTask = null;					// At the end, reset task to null, anyway.
					#if DEBUG
					Messaggi.AddMessage($"Task di connessione sftp azzerato");
					#endif
					}
				}
			if(connData.sftpClient != null)					// If sftpClient is not null...
				{
				#if DEBUG
				Messaggi.AddMessage($"Disconnessione del client sftp");
				#endif
				connData.sftpClient.Disconnect();				// ...disconnct, anyway.
				connData.sftpClient = null;
				#if DEBUG
				Messaggi.AddMessage($"Azzeramento del Client sftp");
				#endif
				}
			#if DEBUG
			Messaggi.AddMessage($"Client sftp is {((connData.sftpClient == null) ? string.Empty : "not ")}null");
			#endif
			if(pendingQuit)		// If there is a Close() request (quit program) when task or connection were active...
				Close();		// ...ask for Close() again (for the Main UI Thread)
			}

		/// <summary>
		/// Read config file with connection parameters and fill dictionary
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		bool ReadConnPar(string filename, ref Dictionary<string, string> dict)
			{
			bool ok = true;
			StreamReader sr = null;
			try
				{
				sr = File.OpenText(filename);
				while(sr.Peek() >= 0)
					{
					string line = sr.ReadLine();
					foreach(string key in cfg.Keys)
						{
						if(line.StartsWith(key+"="))
							dict[key] = line.Substring(key.Length+1);
						}
					}
				sr.Close();
				sr = null;
				if((dict[Defs.IP].Length==0)||(dict[Defs.USR].Length==0)||(dict[Defs.PWD].Length==0))
					throw new Exception($"Some parameter is empty: {Defs.IP}={dict[Defs.IP]}, {Defs.USR}={dict[Defs.USR]}, {Defs.PWD}={dict[Defs.PWD]}");
				}
			catch(Exception ex)
				{
				ok = false;
				Messaggi.AddMessage(Messaggi.ERR.ERROR, Messaggi.Tipo.Errori, ex.Message);
				}
			finally
				{
				if(sr != null)
					{
					sr.Close();
					}
				}
			return ok;
			}

		#endregion



		#region EVENT HANDLERS
		
		/// <summary>
		/// Ssh connect button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bSshConnect_Click(object sender,EventArgs e)
			{
			switch(this.SshConnectionStatus)
				{
				case ConnectionStatEnum.Disconnected:
					{
					connData.sshConnectTask = StartSshConnection();
					}
					break;
				case ConnectionStatEnum.Connected:
					{
					if(CheckActiveTask())
						SshDisconnect();
					}
					break;
				default:
					break;
				}
			UpdateStatus();
			
			}

		/// <summary>
		/// Sftp connect button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bSftpConnect_Click(object sender,EventArgs e)
			{
			switch(this.SftpConnectionStatus)
				{
				case ConnectionStatEnum.Disconnected:
					{
					connData.sftpConnectTask = StartSftpConnection();
					}
					break;
				case ConnectionStatEnum.Connected:
					{
					if(CheckActiveTask())
						SftpDisconnect();
					}
					break;
				default:
					break;
				}
			UpdateStatus();
			
			}

		/// <summary>
		/// Connection info button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bSshConnInfo_Click(object sender,EventArgs e)
			{
			UpdateStatus();
			StringBuilder strb = new StringBuilder();
			if(connData.sshClient != null)
				{
				strb.AppendLine("SSH");
				strb.AppendLine($"Host: {connData.sshClient.ConnectionInfo.Host}");
				strb.AppendLine($"Port: {connData.sshClient.ConnectionInfo.Port.ToString()}");
				strb.AppendLine($"Username: {connData.sshClient.ConnectionInfo.Username}");
				strb.AppendLine($"Server: {connData.sshClient.ConnectionInfo.ServerVersion}");
				strb.AppendLine($"Client: {connData.sshClient.ConnectionInfo.ClientVersion}");
				}
			if(connData.sftpClient != null)
				{
				strb.AppendLine("SFTP");
				strb.AppendLine($"Host: {connData.sftpClient.ConnectionInfo.Host}");
				strb.AppendLine($"Port: {connData.sftpClient.ConnectionInfo.Port.ToString()}");
				strb.AppendLine($"Username: {connData.sftpClient.ConnectionInfo.Username}");
				strb.AppendLine($"Server: {connData.sftpClient.ConnectionInfo.ServerVersion}");
				strb.AppendLine($"Client: {connData.sftpClient.ConnectionInfo.ClientVersion}");
				}
			if(strb.Length > 1)
				MessageBox.Show(strb.ToString());
			}

		/// <summary>
		/// Disconnect button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bSshDisconnect_Click(object sender,EventArgs e)
			{
			if(CheckActiveTask())
				SshDisconnect();
			UpdateStatus();
			}

		/// <summary>
		/// Timer event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void timer_Tick(object sender,EventArgs e)
			{
			UpdateStatus();
			}

		/// <summary>
		/// OnClose()
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MainForm_FormClosing(object sender,FormClosingEventArgs e)
			{

			// Ask to close connection
			if((SshConnectionStatus != ConnectionStatEnum.Disconnected) || (SftpConnectionStatus != ConnectionStatEnum.Disconnected))
				{
				if(MessageBox.Show(Messaggi.GUI.MSG.STOP_CONN, Messaggi.GUI.TIT.EXIT, MessageBoxButtons.YesNo) != DialogResult.Yes)
					{
					pendingQuit = false;
					e.Cancel = true;
					return;
					}
				}

			if(SshConnectionStatus != ConnectionStatEnum.Disconnected)		// If not disconnected...
					{
					pendingQuit = true;			// ...request quit after connection task is finished
					SshDisconnect();
					e.Cancel = true;
					UpdateStatus();
					}
			else if(SftpConnectionStatus != ConnectionStatEnum.Disconnected)		// If not disconnected...
					{
					pendingQuit = true;			// ...request quit after connection task is finished
					SftpDisconnect();
					e.Cancel = true;
					UpdateStatus();
					}
			else								// If disconnected, ask and close
				{
				if(MessageBox.Show(Messaggi.GUI.MSG.EXIT, Messaggi.GUI.TIT.EXIT, MessageBoxButtons.YesNo) != DialogResult.Yes)
					{
					pendingQuit = false;
					e.Cancel = true;
					}
				}
			if(((SshConnectionStatus == ConnectionStatEnum.Connected) || (SftpConnectionStatus == ConnectionStatEnum.Connected)) && (RunningTasks > 0))
				{
				Messaggi.AddMessage(Messaggi.MSG.TASK_RUNNING,Messaggi.Tipo.Messaggi);
				e.Cancel = true;
				}
			}

		/// <summary>
		/// View messages button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bViewMsg_Click(object sender,EventArgs e)
			{
			if(Messaggi.HasMessages())
				{
				MessageBox.Show(Messaggi.MessaggiCompleti(), Messaggi.GUI.TIT.MESSAGGI);
				Messaggi.Clear();
				UpdateStatus();
				}
			}

		/// <summary>
		/// Custom list box
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lbCommands_DrawItem(object sender, DrawItemEventArgs e)
			{
			if((e.Index < lbCommands.Items.Count) && !(e.Index < 0))
				{
				ComandoT ct = (ComandoT) lbCommands.Items[e.Index];
			
				string selitem = lbCommands.Items[e.Index].ToString();
			
				int left = e.Bounds.Left;
				int top = e.Bounds.Top;
				e.DrawBackground();
				e.Graphics.DrawString(selitem, lbFont, ct.IsRunning ? lbBrushUnavail : lbBrushAvail, left, top);
				}
			}

		/// <summary>
		/// Execute selected command
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btCommand_Click(object sender,EventArgs e)
			{
			ComandoT comSelezionato = ComandoSelezionato();

			if(comSelezionato != null)
				{
				comSelezionato.StartTask();
				}

			}

		/// <summary>
		/// Show selected command details
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btViewCommand_Click(object sender,EventArgs e)
			{
			ComandoT comSelezionato = ComandoSelezionato();
			ComandoT.Formats f =	(cbComandi.Checked ? ComandoT.Formats.Comandi : ComandoT.Formats.None) |
									(cbRisultati.Checked ? ComandoT.Formats.Risultati : ComandoT.Formats.None) |
									(cbErrori.Checked ? ComandoT.Formats.Errori : ComandoT.Formats.None);
			if((comSelezionato != null)&&(f != ComandoT.Formats.None))
				{
				string testo = comSelezionato.ToString(f);
				#if false
				MessageBox.Show(testo);
				#endif
				TextBoxOutput tbo = new TextBoxOutput();
				tbo.Text = comSelezionato.ToString();
				testo.Replace("\n",	Environment.NewLine);
				tbo.InnerText = testo;
				tbo.Show();
				
				}
			}



		#endregion

		/// <summary>
		/// Vie variables
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btViewVariables_Click(object sender,EventArgs e)
			{
			if(variabili.Values.Count > 0)
				{
				StringBuilder strb = new StringBuilder();
				foreach(string cmd in variabili.Keys)
					{
					strb.AppendLine($"{cmd}={variabili[cmd]}");
					}
				MessageBox.Show(strb.ToString());
				}
			}

		private void bTestSftp_Click(object sender,EventArgs e)
			{
			if(SftpConnectionStatus == ConnectionStatEnum.Connected)
				{
				string path = tbPath.Text;
				try
					{
					StringBuilder strb = new StringBuilder();
					strb.AppendLine($"Directory of '{path}':");
					IEnumerable<Renci.SshNet.Sftp.SftpFile> files = connData.sftpClient.ListDirectory(path);
					foreach(Renci.SshNet.Sftp.SftpFile f in files)
						{
						strb.AppendLine(f.Name);
						}
					Messaggi.AddMessage(strb.ToString());
					}
				catch(Exception ex)
					{
					Messaggi.AddMessage(Messaggi.ERR.ERROR, Messaggi.Tipo.Errori, ex.Message);
					}
				}
			}
		}
	}
