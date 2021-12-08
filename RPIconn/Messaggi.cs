using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace RPIconn
	{
	/// <summary>
	/// Singolo messaggio
	/// </summary>
	public class MessaggioErrore
		{
		public DateTime MsgTime {get; set;}
		public string Messaggio { get; set; }
		public string Dettaglio { get; set; }
		public MessaggioErrore(string msg, string det)
			{
			Messaggio = msg;
			Dettaglio = det;
			MsgTime = DateTime.Now;
			}
		public string ToLine()
			{
			return $"{Messaggio}{((Dettaglio.Length > 0) ? Messaggi.SeparatoreMsg : string.Empty)} {Dettaglio}\t[{MsgTime.ToString("hh:mm:ss.ff")}]{System.Environment.NewLine}";
			}
		}
	/// <summary>
	/// Classe con tutti i messaggi
	/// </summary>
	static public class Messaggi
		{
		static int LISTE = 2;
		static List<MessaggioErrore>[] _msg = new List<MessaggioErrore>[LISTE];
		static int[] _ultimiMsg = new int[LISTE];

		public static string SeparatoreMsg = " - ";
        /// <summary>
        /// Messaggi di errore
        /// </summary>
		public struct ERR
			{
			public const string ERROR = "Error";
			public const string FILENAME = "Error in the file name";
			public const string COMMANDS = "Errore reading commands";
			public const string NOT_CONNECTED = "Not connected";
			public const string COMMAND_NOT_FOUND = "Command not found";
			public const string COMMAND_EMPTY = "Command without instructions";
			
			}
        /// <summary>
        /// Messaggi informativi
        /// </summary>
		public struct MSG
			{
			public const string TASK_CANCELED = "Task canceled";
			public const string TASK_WAITING =  "Task waiting for task end";
			public const string TASK_RUNNING =  "Task still running";
			}
		/// <summary>
		/// Messaggi dell'interfaccia grafica (dialog box ecc...)
		/// </summary>
		public struct GUI
			{
			public struct MSG
				{
				public const string USCIRE = @"Exit program ?";
				public const string TASK_ATTIVI = @"Some commands are still running. Proceed ?";
				public const string CONNECT = @"Connect";
				public const string DISCONNECT = @"Disconnect";
				public const string BUSY = @"Busy";
				}
			public struct TIT
				{
				public const string USCIRE = @"Quitting....";
				public const string MESSAGGI = @"Messages";
				public const string TASK = @"Task";
				}
			}
		/// <summary>
		/// Messaggi dei tooltip
		/// </summary>
		
		public enum Tipo {Messaggi=0, Errori, NUM};
		/// <summary>
		/// Costruttore statico
		/// Lock superfluo, eseguito una tantum
		/// </summary>
		static Messaggi()
			{
			for(int i=0; i < (int)Tipo.NUM; i++)
				{
				_msg[i] = new List<MessaggioErrore>();
				}
			Reset();
			}
		/// <summary>
		/// Aggiunge un messaggio
		/// </summary>
		/// <param name="msg">Messaggio, string</param>
		/// <param name="dett">Dettagli, string</param>
		/// <param name="typ">Tipo: errore o messaggio</param>
		public static void AddMessage(string msg, Tipo typ = Tipo.Messaggi, string dett = "")
			{
			int i = (int)typ;
			if( (i>=0) && (i<(int)Tipo.NUM) )
				{
				lock(_msg)
					{
					lock(_ultimiMsg)
						{
						_msg[i].Add(new MessaggioErrore(msg,dett));
						_ultimiMsg[i]++;
						} 
					}
				}
			}
		/// <summary>
		/// Restituisce l'ultimo messaggio
		/// </summary>
		/// <param name="typ">Tipo.Errori o </param>
		/// <returns></returns>
		public static MessaggioErrore LastMessage(Tipo typ = Tipo.Errori)
			{
			MessaggioErrore msg = null;
			int i = (int)typ;
			if( (i>=0) && (i<(int)Tipo.NUM) )
				{
				lock(_msg)
					{
					lock(_msg)
						{
						msg = _msg[i].Last();	 // Non usa il costruttore di copia, la dellocazione è fatta dal GC
						}
					}
				}
			if(msg == null)
				msg = new MessaggioErrore(String.Empty, String.Empty);
			return msg;
			}
		/// <summary>
		/// Cancella i messaggi del tipo indicato (oppure tutti)
		/// </summary>
		/// <param name="typ"></param>
		public static void Clear(Tipo typ = Tipo.NUM)
			{
			int i = (int)typ;
			if (i == (int)Tipo.NUM)
				{
				lock(_msg)
					{
					foreach(List<MessaggioErrore> lst in _msg)
						{
						lst.Clear();
						} 
					}
				Reset();
				}
			else if ((i >= 0) && (i < (int)Tipo.NUM))
				{
				lock(_msg)
					{
					_msg[i].Clear(); 
					}
				Reset(typ);
				}
			}
		/// <summary>
		/// Azzera i contatori dei messaggi del tipo indicato (oppure tutti)
		/// </summary>
		/// <param name="typ"></param>
		public static void Reset(Tipo typ = Tipo.NUM)
			{
			int i = (int)typ;
			lock(_ultimiMsg)
				{
				if(i == (int)Tipo.NUM)
					{
					for(int ii = 0;ii < LISTE;ii++)
						{
						_ultimiMsg[ii] = 0;
						}
					}
				else if((i >= 0) && (i < (int)Tipo.NUM))
					_ultimiMsg[i] = 0; 
				}
			}
		/// <summary>
		/// Enumeratore dei messaggi
		/// </summary>
		/// <param name="typ">Tipo: errore o messaggio</param>
		/// <returns>IEnumerable MessaggioErrore</returns>
		public static IEnumerable<MessaggioErrore> Messages(Tipo typ)
			{
			int i = (int)typ;
			if((i >= 0) && (i < (int)Tipo.NUM))
				{
				foreach (MessaggioErrore m in _msg[i])		// Potenziale rischio se msg eliminati da un altro processo
					yield return m;
				}
			yield break;
			}
		/// <summary>
		/// Enumeratore degli ultimi messaggi
		/// </summary>
		/// <param name="typ">Tipo: errore o messaggio</param>
		/// <returns>IEnumerable MessaggioErrore</returns>
		public static IEnumerable<MessaggioErrore> LastMessages(Tipo typ)
			{
			MessaggioErrore m = null;
			int i = (int)typ;
			if((i >= 0) && (i < (int)Tipo.NUM))
				{
				for(int ii = 0; ii < NumLastMessages(typ); ii++)
					{
					m = (_msg[i])[_msg[i].Count-ii-1];		// Potenziale rischio se msg eliminati da un altro processo
					yield return m;
					}
				}
			yield break;
			}
		/// <summary>
		/// Numero di messaggi
		/// </summary>
		/// <param name="typ">Tipo: errori o messaggi</param>
		/// <returns>int</returns>
		public static int NumMessages(Tipo typ)
			{
			int n = 0;
			int i = (int) typ;
			if ((i >= 0) && (i < (int)Tipo.NUM))
				{
				n = _msg[i].Count;
				}
			return n;
			}
		/// <summary>
		/// Numero degli ultimi messaggi (dall'ultimo Reset)
		/// </summary>
		/// <param name="typ">Tipo: errori o messaggi</param>
		/// <returns>int</returns>
		public static int NumLastMessages(Tipo typ)
			{
			int n = 0;
			int i = (int) typ;
			if ((i >= 0) && (i < (int)Tipo.NUM))
				{
				n = _ultimiMsg[i];
				}
			return n;
			}
		/// <summary>
		/// Restituisce true se ci sono messaggi o errori
		/// </summary>
		/// <param name="typ"></param>
		/// <returns>bool</returns>
		public static bool HasMessages(Tipo typ)
			{
			bool hasMsg = false;
			if (NumMessages(typ) > 0)
				hasMsg = true;
			return hasMsg;
			}
		public static bool HasMessages()
			{
			bool hasMsg = false;
			for(int i=0; i<LISTE; i++)
				if (NumMessages((Tipo)i) > 0)
					hasMsg = true;
			return hasMsg;
			}
		/// <summary>
		/// Restiruisce true se ci sono messaggi o errori recenti
		/// </summary>
		/// <param name="typ"></param>
		/// <returns></returns>
		public static bool HasLastMessages(Tipo typ)
			{
			bool hasLastMsg = false;
			if (NumLastMessages(typ) > 0)
				hasLastMsg = true;
			return hasLastMsg;
			}
		/// <summary>
		/// Restituisce un'unica stringa con i messaggi
		/// </summary>
		/// <param name="typ"></param>
		/// <param name="lastMessages"></param>
		/// <returns></returns>
		public static string ToString(Messaggi.Tipo typ, bool lastMessages = false)
			{
			StringBuilder strb = new StringBuilder();
			List<string> lm = new List<string>();
			if(lastMessages)
				{
				foreach (MessaggioErrore msg in LastMessages(typ))
					lm.Add(msg.ToLine());
				}
			else
				{
				foreach (MessaggioErrore msg in Messages(typ))
					lm.Add(msg.ToLine());
				}
			lm = lm.Distinct().ToList();

			foreach (string str in lm)
				strb.Append(str);
			return strb.ToString();
			}
		/// <summary>
		/// Estrae i messaggi completi
		/// </summary>
		/// <returns>string</returns>
        public static string MessaggiCompleti()
            {
            StringBuilder strb = new StringBuilder();
			string s1, s2;
			s1 = Messaggi.ToString(Messaggi.Tipo.Errori);
			s2 = Messaggi.ToString(Messaggi.Tipo.Messaggi);
			if(s1.Length > 0)
				strb.Append("Errori"+ System.Environment.NewLine + s1+ Environment.NewLine);
			if(s2.Length > 0)
				strb.Append("Avvisi"+ System.Environment.NewLine + s2);
            return strb.ToString();
            }
        }
	}
