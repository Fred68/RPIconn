using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
			/// Sftp connection
			/// </summary>
			public SftpClient sftpClient {get; set;}

			/// <summary>
			/// Ssh connection task
			/// </summary>
			public Task<SshClient> sshConnectTask {get; set;}

			/// <summary>
			/// Sftp connection task
			/// </summary>
			public Task<SftpClient> sftpConnectTask {get; set;}

			/// <summary>
			/// Cancellation token source (for ssh connection Task)
			/// </summary>
			public CancellationTokenSource ssh_cts {get; set;}

			/// <summary>
			/// Cancellation token  (for ssh connection Task)
			/// </summary>
			public CancellationToken ssh_ct {get; set;}


			/// <summary>
			/// Cancellation token source (for sftp connection Task)
			/// </summary>
			public CancellationTokenSource sftp_cts {get; set;}

			/// <summary>
			/// Cancellation token  (for sftp connection Task)
			/// </summary>
			public CancellationToken sftp_ct {get; set;}

			public ConnectionData()
				{
				sshClient = null;
				sshConnectTask = null;

				sftpClient = null;
				sftpConnectTask = null;

				ssh_cts = null;
				ssh_ct = CancellationToken.None;

				sftp_cts = null;
				sftp_ct = CancellationToken.None;
				}
			}
	}
