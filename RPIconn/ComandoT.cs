using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Renci.SshNet;

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

			public ComandoTxt()
				{
				Comando = Risultato = Errore = string.Empty;
				}
			public ComandoTxt(string comando)
				{
				Comando = comando;
				Risultato = Errore = string.Empty;
				}
			}

		string _comando;
		
		List<ComandoTxt> _comandiBash;
		ConnectionData _cd;
		Task _cmdTask;

#warning La gestione del task deve essere inclusa nell'oggetto ComandoT !!!
#warning A ComandoT.StartCommand...() passare i delegate per ContinueWith() e altro.
#warning A ComandoT.StartCommand...() usa i token di cancellazione internamente


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
			// _running = false;
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
		/// Execute command through ssh
		/// </summary>
		/// <param name="commandName"></param>
		/// <returns></returns>
		void ExecuteCommand()
			{	
			SshCommand scm;
			try
				{
				foreach(ComandoTxt c in _comandiBash)
					{
					scm = _cd.sshClient.CreateCommand(c.Comando);
					scm.Execute();
					c.Risultato = scm.Result;
					c.Errore = scm.Error;
					}
				}
			catch(Exception ex)
				{
				Messaggi.AddMessage(Messaggi.ERR.ERROR, Messaggi.Tipo.Errori, ex.Message);
				}
			return;
			}
		
		void AfterCommand(Task t)
			{
			#if DEBUG
			Messaggi.AddMessage($"Command {_cmdTask.Id} is completed {t.Status.ToString()}");
			#endif
			_cmdTask = null;
			}

		/// <summary>
		/// Start task
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

		#endregion

		}
	}
