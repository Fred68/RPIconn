using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Renci.SshNet;
using System.IO;

namespace RPIconn
	{
	/// <summary>
	/// ComandoT with command name, list of commands, task data etc...
	/// </summary>
	public class ComandoT
		{

		[Flags]
		public enum Formats	{
							None			= 0,			
							Comandi			= 1 << 0,
							Risultati		= 1 << 2,
							Errori			= 1 << 3
							}
		

		/// <summary>
		/// Comando, risultato ed errore
		/// </summary>
		public class ComandoTxt
			{
			public string Comando;
			public string Risultato;
			public string Errore;

			/// <summary>
			/// Ctor
			/// </summary>
			public ComandoTxt()
				{
				Comando = Risultato = Errore = string.Empty;
				}
			/// <summary>
			/// Ctor with command name
			/// </summary>
			/// <param name="comando"></param>
			public ComandoTxt(string comando)
				{
				Comando = comando;
				Clear();
				}

			/// <summary>
			/// Clear command results
			/// Command instruction is not changed
			/// </summary>
			public void Clear()
				{
				Risultato = Errore = string.Empty;
				}
			}

		delegate void Comandospeciale(string risultati);

		
		#region STATIC MEMBERS
		
		static Dictionary<string, Comandospeciale> _dictComSpeciali;
		static DictionaryWithLock<string> _variabili;
		
		#endregion


		string _comando;
		List<ComandoTxt> _comandiBash;
		ConnectionData _cd;
		Task _cmdTask;

		//	La gestione del task deve essere inclusa nell'oggetto ComandoT !!!
		//	A ComandoT.StartCommand...() passare i delegate per ContinueWith() e altro.
		#warning A ComandoT.StartCommand...() deve usare gli eventuali token di cancellazione internamente

		
		/// <summary>
		/// Static Ctor
		/// </summary>
		static ComandoT()
			{
			_dictComSpeciali = new Dictionary<string,Comandospeciale>();
			_dictComSpeciali.Add(Defs.COMM_READ,Read_COM);
			}

		/// <summary>
		/// Set variables 
		/// </summary>
		/// <param name="variabili"></param>
		public static void SetVariabili(DictionaryWithLock<string> variabili)
			{
			_variabili = variabili;
			}


		/// <summary>
		/// Ctor (with commmand name)
		/// </summary>
		/// <param name="comando"></param>
		public ComandoT(ConnectionData cd, string comando)
			{
			_comando = comando;
			_cd = cd;
			Init();
			}

		#region PROPERTIES
		/// <summary>
		/// Comando name
		/// </summary>
		public string Comando
			{
			get {return _comando;}
			}
		
		public bool IsRunning
			{
			get
				{
				return (_cmdTask != null);
				}
			}

		public bool IsConnected
			{
			get
				{
				bool conn = false;
				if(_cd.sshClient != null)
					if(_cd.sshClient.IsConnected)
						conn = true;
				return conn;
				}
			}

		/// <summary>
		/// Access commands List<string> 
		/// </summary>
		public List<ComandoTxt> ComandiBash
			{
			get {return _comandiBash;}
			set {_comandiBash = value;}
			}

		#endregion


		#region FUNZIONI


		/// <summary>
		/// Initialize (used in ctor)
		/// </summary>
		void Init()
			{
			_comandiBash = new List<ComandoTxt>();
			_cmdTask = null;
			}

		/// <summary>
		/// To String()
		/// Override. To display in a list box
		/// </summary>
		/// <returns></returns>
		public override string ToString()
			{
			return _comando;
			}

		public string ToString(Formats f)
			{
			StringBuilder strb = new StringBuilder();
			foreach(ComandoTxt ctx in _comandiBash)
				{
				if((f & Formats.Comandi) != 0)
					{
					strb.Append(ctx.Comando);
					strb.Append(Environment.NewLine);
					}
				if((f & Formats.Risultati) != 0)
					{
					strb.AppendLine(ctx.Risultato);
					strb.Append(Environment.NewLine);
					}
				if((f & Formats.Errori) != 0)
					{
					strb.AppendLine(ctx.Errore);
					strb.Append(Environment.NewLine);
					}
				}
			return strb.Replace("\n", Environment.NewLine).ToString();
			}

		/// <summary>
		/// Clear all command results
		/// Command instruction not changed
		/// </summary>
		public void Clear()
			{
			foreach(ComandoTxt cmd in _comandiBash)
				cmd.Clear();
			}
	
		/// <summary>
		/// Execute command through ssh
		/// </summary>
		/// <param name="commandName"></param>
		/// <returns></returns>
		void ExecuteCommand()
			{	
			SshCommand scm;
			try
				{
				int indx, lastBashindx;
				Clear();																// Clear command results
				for(indx = 0, lastBashindx = -1; indx < _comandiBash.Count; indx++)		// Cycle through bash command list; lastBash index is -1 if none.
					{
					ComandoTxt c = _comandiBash[indx];
					if(!c.Comando.StartsWith(Defs.SPEC))								// Bash command
						{
						scm = _cd.sshClient.CreateCommand(c.Comando);
						scm.Execute();
						c.Risultato = scm.Result;
						c.Errore = scm.Error;
						lastBashindx = indx;
						}
					else																// Special operation (non bash) on last bash command
						{
						ExecuteSpecial(indx, lastBashindx);
						}
					}
				}
			catch(Exception ex)
				{
				Messaggi.AddMessage(Messaggi.ERR.ERROR, Messaggi.Tipo.Errori, ex.Message);
				}
			return;
			}
	
		/// <summary>
		/// Execute special command
		/// </summary>
		/// <param name="iSpec">index of special command</param>
		/// <param name="iBash">index ofthe last Bash command</param>
		void ExecuteSpecial(int iSpec, int iBash)
			{
			if(iBash >= 0)
				{
				string cmd, res;
				cmd = _comandiBash[iSpec].Comando;
				if(cmd.StartsWith(Defs.SPEC))
					cmd = cmd.Substring(Defs.SPEC.Length);
				res = _comandiBash[iBash].Risultato;
				
				if(_dictComSpeciali.ContainsKey(cmd))		// Chiama il delegate del dizionario
					{
					_dictComSpeciali[cmd](res);
					}

				}
			else
				Messaggi.AddMessage("Errore", Messaggi.Tipo.Errori, $"Comando: {_comandiBash[iSpec].Comando}, ultimo bash: inesistente");
			}

		/// <summary>
		/// Start task with this command
		/// </summary>
		/// <returns>IsRunning?</returns>
		public Task StartTask()
			{
			if(_cmdTask == null)
				{
				if(IsConnected)
					{
					_cmdTask = Task.Factory.StartNew( () => ExecuteCommand());
					if(_cmdTask != null)
						{
						_cmdTask.ContinueWith(AfterCommand);
						}
					}
				else
					{
					Messaggi.AddMessage(Messaggi.ERR.NOT_CONNECTED, Messaggi.Tipo.Errori);
					}
				}
			else
				{
				#if DEBUG
				Messaggi.AddMessage(Messaggi.MSG.TASK_RUNNING, Messaggi.Tipo.Messaggi);
				#endif
				}
			return _cmdTask;
			}

		/// <summary>
		/// After command task completed
		/// </summary>
		/// <param name="t"></param>
		void AfterCommand(Task t)
			{
			#if DEBUG
			Messaggi.AddMessage($"Command {_cmdTask.Id} is completed {t.Status.ToString()}");
			#endif
			_cmdTask = null;
			}

		/// <summary>
		/// Read command file and fill command dictionary
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="cd"></param>
		/// <param name="cmdd"></param>
		/// <returns></returns>
		public static bool ReadCommands(string filename, ConnectionData cd, DictionaryWithLock<ComandoT> cmdd)
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
					if( (line.Length>0) && (!line.StartsWith(Defs.REM)) )		// Se riga non nulla e non di commento
						{
						if(line.StartsWith(Defs.CMD))							// Nuovo comando
							{
							cmd = line.Substring(Defs.CMD.Length+1);			// Estrae il comando
							if(cmd.Length == 0)									// Comando nullo
								{
								throw new Exception($"Empty command in line {lc}");
								}
							#if DEBUG
							Messaggi.AddMessage($"Opening new command: [{cmd}]");
							#endif

							if(!inCommand)										// Nuovo comando
								{
								inCommand = true;
								cmdList = new List<ComandoT.ComandoTxt>();		// Crea una lista di comandi
								}
							else												// Errore, è già iniziato un nuovo comando
								{
								throw new Exception($"Previous command non closed in line {lc}");
								}
							}
						else if(line.StartsWith(Defs.END))						// Chiude il comando attivo
							{
							inCommand = false;
							if(cmdList.Count>0)
								{
								ComandoT cmdT = new ComandoT(cd, cmd);			// Crea un oggetto con i comandi
								cmdT.ComandiBash = cmdList;						// Assegna la lista dei comandi
								cmdd.Add(cmd, cmdT);							// Lo aggiunge al dizionario
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


		#region SPECIAL COMMAND DELEGATES

		/// <summary>
		/// ***READ command
		/// </summary>
		/// <param name="res">risultato</param>
		static void Read_COM(string res)
			{
			if(ComandoT._variabili == null)
				{
				Messaggi.AddMessage("Errore", Messaggi.Tipo.Errori, $"Manca il riferimento al dizionario delle variabili");
				}

			string[] lines = res.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
			
			foreach(string line in lines)
				{
				string var_name, var_val;

				int indx = line.IndexOf(Defs.COMM_ASSIGN);	// Search 1st occurrence of separatore Defs.COMM_ASSIGN (default '=').
				if(indx > 1)								// If found and after the first character
					{
					var_name = line.Substring(0,indx);
					var_val = line.Substring(indx + 1);	

					//#if DEBUG
					//Messaggi.AddMessage("Speciale", Messaggi.Tipo.Messaggi, $"{var_name}={var_val}");
					//#endif
					
					ComandoT._variabili[var_name] = var_val;
					}
				// Defs.COMM_ASSIGN;
				}
			
			#if DEBUG
			StringBuilder sb = new StringBuilder();	
			foreach(string line in lines)
				{
				sb.AppendLine(line);
				}
			Messaggi.AddMessage("Speciale", Messaggi.Tipo.Messaggi, $"Risultato ultimo bash=\n{sb.ToString()}");
			#endif


			}


		#endregion

		}
	}
