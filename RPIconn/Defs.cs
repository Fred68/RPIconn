using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPIconn
	{
	public static class Defs
		{
		public const string CFG_FILENAME = "conn.cfg";		// Configuration file (same folder)
		public const string IP = "ip";						// Login: ip address...
		public const string USR = "usr";					// ...username...
		public const string PWD = "pwd";					// ...password.
		public const float CONN_REFRESH = 1;				// Connection display status refresh (seconds)
		public const int CANCEL_PAUSE = 300;				// Wait task to stop (msec)
		
		public const string CMD_FILENAME = "cmd.cfg";		// Command file (same folder)
		public const string CMD = "->CMD";					// Start command
		public const string SPEC = "***";						// Prefix for special command line
		public const string END = "<-";						// End command
		public const string REM = "#";						// Comment line
		public const string COMM_READ = "READ";				// READ command
		public const string COMM_ASSIGN = "=";				// Assign operator command

		public const string FONT = "Arial";					// ListBox font
		public const int FONT_SIZE = 9;
		}
	}
