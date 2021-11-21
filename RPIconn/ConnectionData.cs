using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading.Tasks;
using System.Threading;
using Renci.SshNet;


namespace RPIconn
	{
	public class ConnectionData
			{
			/// <summary>
			/// Ssh connection
			/// </summary>
			public SshClient sshClient {get; set;}

			/// <summary>
			/// Connection task
			/// </summary>
			public Task<SshClient> connectTask {get; set;}

			/// <summary>
			/// Cancellation token source (for connection Task)
			/// </summary>
			public CancellationTokenSource cts {get; set;}

			/// <summary>
			/// Cancellation token  (for connection Task)
			/// </summary>
			public CancellationToken ct {get; set;}

			public ConnectionData()
				{
				sshClient = null;
				connectTask = null;
				cts = null;
				ct = CancellationToken.None;
				}
			}
	}
