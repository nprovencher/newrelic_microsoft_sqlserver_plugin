﻿using System.ServiceProcess;
using NewRelic.Microsoft.SqlServer.Plugin.Properties;

namespace NewRelic.Microsoft.SqlServer.Plugin.Core
{
	/// <summary>
	/// Windows Service wrapper class.
	/// </summary>
	public class SqlMonitorService : ServiceBase
	{
		private SqlMonitor _monitor;

		public SqlMonitorService()
		{
			ServiceName = ServiceConstants.ServiceName;
		}

		protected override void OnStart(string[] args)
		{
			if (_monitor == null)
			{
				// TODO Get values from config
				_monitor = new SqlMonitor(".", "master");
			}

			_monitor.Start();
		}

		protected override void OnStop()
		{
			if (_monitor != null)
			{
				_monitor.Stop();
			}

			_monitor = null;
		}
	}
}
