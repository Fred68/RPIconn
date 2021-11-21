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
		#region DEFs
		public static readonly string CFG_FILENAME = "conn.cfg";	// Configuration file (same folder)
		public static readonly string IP = "ip";					// Login: ip address...
		public static readonly string USR = "usr";					// ...username...
		public static readonly string PWD = "pwd";					// ...password.
		public static readonly float CONN_REFRESH = 1;				// Connection display status refresh (seconds)
		public static readonly int CANCEL_PAUSE = 300;				// Wait task to stop (msec)
		
		public static readonly string CMD_FILENAME = "cmd.cfg";		// Command file (same folder)
		public static readonly string CMD = "->CMD";				// Start command
		public static readonly string END = "<-";					// End command
		public static readonly string REM = "#";					// Comment line


		#endregion

		// DA FARE
		//			Provare disconnessione (RPI spenta durante una sessione)
		//			Aggiungere evento OnClose() per chiudere le connessione
		//			Se chiamato Disconnect prima del termine del Task Connect, si blocca.
		//			L'interfaccia non può esser modificata dai Task. Usare BeginInvoke o Invoke()
		//			Vedere se BeginInvoke richiede un EndInvoke. Non necessario !
		#warning	Occhio al ciclo di lettura della lista dei messaggi (yield retunn). Aggiungere lock() o impedire la cancellazione di messaggi (solo sotto lock): fare una classe derivata ?
		#warning	Capire come leggere le risposte in modo sincrono (leggere i nomi delle variabili con Back12.sh -i)
		//			Capire come leggere le risposte in modo asincrono: non serve al momento, eseguendo dei comandi relativamente semplici.
		#warning	File di configurazione: come codificare nel file la lettura dei dati ricevuti
		#warning	Permettere 1 solo avvio in multithread di un singolo comando
		//			Usare il Dictionary dei comandi, con un oggetto ComandoTask con i dati del comando
		//			L'oggetto ComandoTask contiene: lista comandi e dei risultati, Task, dati (output, callback), stato (per es. se è già avviato...)
		#warning	Modificare, all'avvio e alla chiusura di un comando, la sua visualizzazione nella listbox
		#warning	Studiare listbox derivate (con i colori) e con oggetti (invece che con stringhe). L'oggetto è un ComandoTask.
		#warning	Aggiungere un lock alla list box ? No, usare BeginInvoke() dal thread del comando al thread principale (main UI thread).
		//			Lista di Task. Non serve, con Task.Factory.StartNew(...) si crea un Task nel ThreadPool gestito dal Common Language Runtime
		#warning	Capire come leggere le informazioni dei file (ultima modifica e dimensione) su Windows
		#warning	Capire come leggere le informazioni dei file (ultima modifica e dimensione) su Linux
		#warning	Capire come eseguire il trasferimento di file in ssh, da programma

		// DA FARE IN FUTURO
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
				{IP,""},
				{USR,""},
				{PWD,""},
				{CMD,""}
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


		#region PROPERTIES

		/// <summary>
		/// Enum with connection stat
		/// </summary>
		public enum ConnectionStatEnum {Disconnected, Connecting, Disconnecting, Connected};

		/// <summary>
		/// Connection stat property (read only)
		/// </summary>
		public ConnectionStatEnum ConnectionStatus
			{
			get
				{
				ConnectionStatEnum cstat = ConnectionStatEnum.Disconnected;
				
				if(connData.connectTask != null)
					{
					if(!connData.ct.IsCancellationRequested)
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
		/// Check if ssh connection is active
		/// </summary>
		/// <returns></returns>
		public bool IsConnected
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
		/// Get connection Task status (if running or not)
		/// </summary>
		public bool IsConnRunning
			{
			get
				{
				bool run = false;
				if(connData.connectTask != null)
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
		/// Get connection username and host (IP)
		/// </summary>
		public Tuple<string,string> ConnStat
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

		#endregion


		#region SSH COMMAND DATA

		/// <summary>
		/// Command dictionary
		/// </summary>
		Dictionary<string,ComandoT> comandi;

		#endregion


		#region MAIN FORM

		#region GUI
		Font lbFont = new Font("Courier New", 9);
		SolidBrush lbBrushAvail = new SolidBrush(Color.Blue);
		SolidBrush lbBrushUnavai = new SolidBrush(Color.DarkRed);
		#endregion
		
		
		/// <summary>
		/// Main form ctor
		/// </summary>
		public MainForm()
			{
			InitializeComponent();
			configFile = CFG_FILENAME;
			connData = new ConnectionData();
			//sshClient = null;
			//connectTask = null;
			pendingQuit = false;
			
			commandFile = CMD_FILENAME;

			comandi = new Dictionary<string, ComandoT>();

			}

		/// <summary>
		/// OnLoad
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MainForm_Load(object sender,EventArgs e)
			{
			lbFont = new Font("Courier New", 10);
			lbBrushAvail = new SolidBrush(Color.Blue);
			lbBrushUnavai = new SolidBrush(Color.DarkRed);

			refreshTimer.Interval = (int)(CONN_REFRESH*1000f);
			refreshTimer.Enabled = true;
			
			if(!ReadCommands(commandFile, ref comandi))
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

			UpdateStatus();
			}

		/// <summary>
		/// Update main form with up-to-date connection data
		/// </summary>
		public void UpdateStatus()
			{

			lbStat.Text = this.ConnectionStatus.ToString();
			lbQuitPnd.Text = pendingQuit ? "Closing program" : string.Empty;

			Tuple<string,string> cinfo = ConnStat;
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
		/// Connect to server (not asynchronous)
		/// </summary>
		SshClient Connect()
			{
			SshClient sshc = null;
			Messaggi.Clear();
			try
				{
				sshc = new SshClient(cfg[IP], cfg[USR], cfg[PWD]);
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
		/// Start connection task
		/// and prepare callback function
		/// </summary>
		Task<SshClient> StartConnection()
			{
			Task<SshClient> task = null;
			if(ReadConnPar(configFile, ref cfg))
				{
				if(!IsConnected)
					{
					connData.cts = new CancellationTokenSource();
					connData.ct = connData.cts.Token;
					if( (connData.ct != CancellationToken.None) && (connData.cts != null))		// If cancellation toke and sourc are not null...
						 {
						 connData.ct.Register( () =>										// Register callback function, when task is canceled
							{
							#if DEBUG
							MessageBox.Show(Messaggi.MSG.TASK_CANCELED);
							#endif
							connData.cts.Dispose();										// Clear object (no more valid, token expired)
							panel1.BeginInvoke(new Action( () =>	{
																	UpdateStatus();
																	}
															));	// Update status (when UI thread is ready).
							#if DEBUG
							Messaggi.AddMessage("Arresto in corso del task di connessione");
							#endif
							}
							);
						 }
					task = Task<SshClient>.Factory.StartNew( () => Connect(), connData.ct);
					if(task != null)
						{
						task.ContinueWith(AfterConnection);
						}
					}
				}
			return task;
			}

		/// <summary>
		/// Callback function, after connection task is completed
		/// </summary>
		/// <param name="t"></param>
		void AfterConnection(Task<SshClient> t)
			{
			SshClient res = t.Result;		// Read SshClient return value from delegate function started in the task
			if(res != null)
				{
				connData.sshClient = res;			// Saves connection data
				}
			#if DEBUG
			Messaggi.AddMessage($"Task is completed {t.Status.ToString()}");
			#endif
			connData.connectTask = null;			// Clear the task
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
		void Disconnect()
			{
			if(!CheckActiveTask())								// Ask, if there is an active task
				return;
			if(connData.connectTask != null)					// If still connecting (i.e.: double click on disconnect, token already used)
				{
				if(!connData.ct.IsCancellationRequested)		// If no task cancellation request pending...
					{
					connData.cts.Cancel();						// ...request connection task cancellation
					#if DEBUG
					Messaggi.AddMessage("Richiesta cancellazione del task di connessione");
					#endif
					Thread.Sleep(CANCEL_PAUSE);		// Wait task to stop (msec)
					}
				else								// If a cancellation request is pending...
					{
					return;							// ...do nothing and exit function
					}
				bool taskcompleted = true;			// Salva lo stato del task
				try
					{
					taskcompleted = connData.connectTask.IsCompleted;	// In try block: connectTask might have become null
					}
				catch
					{
					taskcompleted = true;
					}
				if(!taskcompleted)					// If still connecting... This test waits for the task completion...!
					{
					#if DEBUG
					Messaggi.AddMessage("Il task di connessione non è ancora completato");
					#endif
					// connectTask.Wait();			// ...wait for task completion. NO: continue executionn
					// ...connectTask.IsCompleted.ToString()}... This command waits for task completion. NO: continue execution.
					return;
					}
				else
					{
					connData.connectTask = null;					// At the end, reset task to null, anyway.
					#if DEBUG
					Messaggi.AddMessage($"Task di connessione azzerato");
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
				if((dict[IP].Length==0)||(dict[USR].Length==0)||(dict[PWD].Length==0))
					throw new Exception($"Some parameter is empty: {IP}={dict[IP]}, {USR}={dict[USR]}, {PWD}={dict[PWD]}");
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


		#region SSH COMMAND FUNCTIONS

		/// <summary>
		/// Read command file and fill command dictionary
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="dict"></param>
		/// <returns></returns>
		bool ReadCommands(string filename, ref Dictionary<string, ComandoT> dict)
			{
			bool ok = true;
			StreamReader sr = null;
			try
				{
				sr = File.OpenText(filename);

				bool inCommand = false;					// flag, reading lines inside a command
				int lc = 0;								// line counter
				string cmd;								// Active command
				List<ComandoT.ComandoTxt> cmdList;		// Active command list

				cmd = string.Empty;
				cmdList = new List<ComandoT.ComandoTxt>();

				while(sr.Peek() >= 0)
					{
					string line = sr.ReadLine();
					lc++;
					if( (line.Length>0) && (!line.StartsWith(REM)) )		// Se riga non nulla e non di commento
						{
						if(line.StartsWith(CMD))							// Nuovo comando
							{
							cmd = line.Substring(CMD.Length+1);				// Estrae il comando
							if(cmd.Length == 0)								// Comando nullo
								{
								throw new Exception($"Empty command in line {lc}");
								}
							#if DEBUG
							Messaggi.AddMessage($"Opening new command: [{cmd}]");
							#endif

							if(!inCommand)									// Nuovo comando
								{
								inCommand = true;
								cmdList = new List<ComandoT.ComandoTxt>();	// Crea una lista di comandi
								}
							else											// Errore, è già iniziato un nuovo comando
								{
								throw new Exception($"Previous command non closed in line {lc}");
								}
							}
						else if(line.StartsWith(END))						// Chiude il comando attivo
							{
							inCommand = false;
							if(cmdList.Count>0)
								{
								ComandoT cmdT = new ComandoT(connData, cmd);	// Crea un oggetto con i comandi
								cmdT.ComandiBash = cmdList;						// Assegna la lista dei comandi
								comandi.Add(cmd, cmdT);							// Lo aggiunge al dizionario
								#if DEBUG
								Messaggi.AddMessage($"Added [{cmd}] command !");
								#endif
								}
							cmd = string.Empty;
							}
						else if(inCommand)
							{
							cmdList.Add(new ComandoT.ComandoTxt(line));
							#if DEBUG
							Messaggi.AddMessage($"[{cmd}] reading line: [{line}] ");
							#endif
							}
						}
					}
				sr.Close();
				sr = null;
				//	throw new Exception($"Command file read NOT IMPLEMENTED, yet");
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
			// comandi.Add("TEST", new List<string> {"ls", "pwd" });
			return ok;
			}

		#endregion


		#region EVENT HANDLERS
		
		/// <summary>
		/// Connect button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bConnect_Click(object sender,EventArgs e)
			{
			if(connData.connectTask == null)
				{
				connData.connectTask = StartConnection();
				}
			UpdateStatus();
			}

		/// <summary>
		/// Connection info button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bConnInfo_Click(object sender,EventArgs e)
			{
			UpdateStatus();
			if(connData.sshClient != null)
				{
				StringBuilder strb = new StringBuilder();
				strb.AppendLine($"Host: {connData.sshClient.ConnectionInfo.Host}");
				strb.AppendLine($"Port: {connData.sshClient.ConnectionInfo.Port.ToString()}");
				strb.AppendLine($"Username: {connData.sshClient.ConnectionInfo.Username}");
				strb.AppendLine($"Server: {connData.sshClient.ConnectionInfo.ServerVersion}");
				strb.AppendLine($"Client: {connData.sshClient.ConnectionInfo.ClientVersion}");
				MessageBox.Show(strb.ToString());
				}
			}

		/// <summary>
		/// Disconnect button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bDisconnect_Click(object sender,EventArgs e)
			{
			if(CheckActiveTask())
				Disconnect();
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
			if(ConnectionStatus != ConnectionStatEnum.Disconnected)		// If not disconnected...
					{
					pendingQuit = true;			// ...request quit after connection task is finished
					Disconnect();
					e.Cancel = true;
					UpdateStatus();
					}
			else								// If disconnected, ask and close
				{
				if(MessageBox.Show(Messaggi.GUI.MSG.USCIRE, Messaggi.GUI.TIT.USCIRE, MessageBoxButtons.YesNo) != DialogResult.Yes)
					{
					pendingQuit = false;
					e.Cancel = true;
					}
				}
			if((ConnectionStatus == ConnectionStatEnum.Connected) && (RunningTasks > 0))
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
				e.Graphics.DrawString(selitem, lbFont, ct.IsRunning ? lbBrushUnavai : lbBrushAvail, left, top);
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


		}
	}
