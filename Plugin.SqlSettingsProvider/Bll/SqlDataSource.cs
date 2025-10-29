using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using AlphaOmega.Data;
using SAL.Flatbed;

namespace Plugin.SqlSettingsProvider.Bll
{
	public class SqlDataSource
	{
		/// <summary>Maximum buffer size</summary>
		private const Int32 BufferLength = 1024;
		private readonly Plugin _plugin;
		private readonly Object _identityLock = new Object();
		private Int32? _userId;
		private Int32? _applicationId;
		private Dictionary<String, Int32> _pluginsIdentity;

		public Int32 UserID
		{
			get
			{
				if(!this._userId.HasValue)
					this.InitApplication();
				return this._userId.Value;
			}
		}

		public Int32 ApplicationID
		{
			get
			{
				if(!this._applicationId.HasValue)
					this.InitApplication();
				return this._applicationId.Value;
			}
		}

		private Dictionary<String, Int32> PluginsIdentity
		{
			get
			{
				if(this._pluginsIdentity == null)
					this.InitApplication();
				return this._pluginsIdentity;
			}
		}

		public SqlDataSource(Plugin plugin)
			=> this._plugin = plugin;

		/// <summary>Get the plugin ID</summary>
		/// <param name="plugin">The plugin interface for which to find the row</param>
		/// <returns>The plugin ID</returns>
		public Int32 this[IPluginDescription plugin]
		{
			get
			{
				String pluginId = plugin.ID;

				if(this.PluginsIdentity.TryGetValue(pluginId, out Int32 result))
					return result;

				lock(this._identityLock)
				{
					if(this.PluginsIdentity.TryGetValue(pluginId, out result))
						return result;

					result = this.GetPluginId(this.UserID, this.ApplicationID, pluginId);
					this.PluginsIdentity.Add(pluginId, result);
				}
				return result;
			}
		}

		/// <summary>Save plugin value</summary>
		/// <param name="plugin">Plugin interface for which to save</param>
		/// <param name="key">Plugin value key for which to save</param>
		/// <param name="value">Key value</param>
		public void SavePluginParameter(IPluginDescription plugin, String key, Byte[] value)
			=> this.SavePluginParameter(this.UserID, this.ApplicationID, this[plugin], key, value);

		/// <summary>Load plugin value</summary>
		/// <param name="plugin">Plugin interface for loading parameters</param>
		/// <param name="key">Key for getting the value</param>
		/// <returns>Parameter array</returns>
		public IEnumerable<KeyValuePair<String,Byte[]>> LoadPluginParameters(IPluginDescription plugin, String key)
			=> this.LoadPluginParameters(this.UserID, this.ApplicationID, this[plugin], key);

		/// <summary>Save plugin value</summary>
		/// <param name="userId">User ID</param>
		/// <param name="applicationId">Application ID</param>
		/// <param name="pluginId">Plugin ID</param>
		/// <param name="valueName">Plugin key whose value should be saved</param>
		/// <param name="value">Plugin key value</param>
		/// <returns>Save result</returns>
		private void SavePluginParameter(Int32 userId, Int32 applicationId, Int32 pluginId, String valueName, Byte[] value)
		{
			ThreadPool.QueueUserWorkItem(SavePluginParameterAsync, new SaveParameterArgs(this, 0, userId, applicationId, pluginId, valueName, value));

			/*using(DbConnection connection = this.Plugin.SettingsInternal.CreateConnection())
			using(DbCommand command = this.Plugin.SettingsInternal.CreateCommand(connection))
			{
				command.CommandText = "settings.SavePluginParameter";

				command.CreateParameters(MethodInfo.GetCurrentMethod(),
					userId, applicationId, pluginId, valueName, value);

				try
				{
					command.Connection.Open();
					return command.ExecuteNonQuery() > 0;
				} finally
				{
					command.Connection.Close();
				}
			}*/
		}

		private static void SavePluginParameterAsync(Object state)
		{
			SaveParameterArgs args = (SaveParameterArgs)state;
			using(DbConnector connector = args.Data._plugin.Settings.CreateConnector())
			{
				connector.Command.CommandText = "settings.SavePluginParameter";
				connector.CreateParameters(args);

				try
				{
					connector.Connection.Open();
					connector.Command.ExecuteNonQuery();
				} catch(Exception exc)
				{
					args.Data._plugin.Trace.TraceData(TraceEventType.Error, 1, exc);
				}
			}
		}

		/// <summary>Load plugin value</summary>
		/// <param name="userId">User ID</param>
		/// <param name="applicationId">Application ID</param>
		/// <param name="pluginId">Plugin ID</param>
		/// <param name="keyName">Key to retrieve the value by</param>
		/// <returns>Byte array</returns>
		private IEnumerable<KeyValuePair<String, Byte[]>> LoadPluginParameters(Int32 userId, Int32 applicationId, Int32 pluginId, String keyName)
		{
			using(DbConnector connector = this._plugin.Settings.CreateConnector())
			{
				connector.Command.CommandText = "settings.LoadPluginParameter";

				connector.CreateParameters(MethodInfo.GetCurrentMethod(),
					userId, applicationId, pluginId, keyName);

				connector.Connection.Open();
				using(DbDataReader reader = connector.Command.ExecuteReader(CommandBehavior.CloseConnection))
				{
					Dictionary<String, Byte[]> result = new Dictionary<String, Byte[]>();
					while(reader.Read())
						if(!reader.IsDBNull(1))//Only if the value is defined. Because the data source can send a list of all parameters without exception, null
						{
							List<Byte> bytes = new List<Byte>();
							Byte[] buffer = new Byte[SqlDataSource.BufferLength];
							Int64 position = 0;
							Int64 count = 0;
							String key = reader.GetString(0);
							do
							{
								count = reader.GetBytes(1, position, buffer, 0, buffer.Length);
								position += count;
								if(count < SqlDataSource.BufferLength)
									Array.Resize(ref buffer, (Int32)count);

								bytes.AddRange(buffer);
							} while(count >= SqlDataSource.BufferLength);
							result.Add(key, bytes.ToArray());
						}
					return result;
				}
			}
		}

		/// <summary>Get plugin ID</summary>
		/// <param name="userId">User ID</param>
		/// <param name="applicationId">Application ID</param>
		/// <param name="pluginName">Plugin ID</param>
		/// <returns>Plugin ID from the database</returns>
		private Int32 GetPluginId(Int32 userId, Int32 applicationId, String pluginName)
		{
			using(DbConnector connector = this._plugin.Settings.CreateConnector())
			{
				connector.Command.CommandText = "settings.GetPluginId";
				connector.CreateParameters(MethodInfo.GetCurrentMethod(),
					userId, applicationId, pluginName);
				return (Int32)connector.ExecuteNonQueryWithReturn();
			}
		}

		/// <summary>Initialization of internal variables of the data source</summary>
		private void InitApplication()
		{
			if(this._applicationId.HasValue && this._userId.HasValue)
				return;

			//.NET5+
			//AppDomain.CurrentDomain.SetPrincipalPolicy(System.Security.Principal.PrincipalPolicy.UnauthenticatedPrincipal)
			String userName = Thread.CurrentPrincipal?.Identity.IsAuthenticated == true
				? Thread.CurrentPrincipal.Identity.Name
				: Environment.UserName;

			String applicationName = String.Join("|", this._plugin.Host.Plugins.FindPluginType<IPluginKernel>().Select(p => p.Name).OrderBy(p => p).ToArray());

			if(applicationName.Length == 0)
				applicationName = "<Empty>";

			this._pluginsIdentity = this.GetApplicationPlugins(userName, applicationName, out Int32 userId, out Int32 applicationId);

			this._userId = userId;
			this._applicationId = applicationId;
		}

		/// <summary>Getting application parameters for faster loading of plugin values</summary>
		/// <param name="login">Login</param>
		/// <param name="applicationName">Application name</param>
		/// <param name="userId">Login ID in the database</param>
		/// <param name="applicationId">Application ID in the database</param>
		/// <returns>Array of plugin identifiers and names</returns>
		private Dictionary<String, Int32> GetApplicationPlugins(String login, String applicationName, out Int32 userId, out Int32 applicationId)
		{
			using(DbConnector connector = this._plugin.Settings.CreateConnector())
			{
				connector.Command.CommandText = "settings.GetAplicationParameters";
				connector.CreateParameters(MethodInfo.GetCurrentMethod(),
					login, applicationName);

				Dictionary<String, Int32> result = new Dictionary<String, Int32>();
				connector.Connection.Open();
				using(DbDataReader reader = connector.Command.ExecuteReader(CommandBehavior.CloseConnection))
					while(reader.Read())
						result.Add(reader.GetString(1), reader.GetInt32(0));

				Object[] returnParams = connector.GetOutputParameters(MethodInfo.GetCurrentMethod()).ToArray();
				userId = (Int32)returnParams[0];
				applicationId = (Int32)returnParams[1];
				return result;
			}
		}
	}
}