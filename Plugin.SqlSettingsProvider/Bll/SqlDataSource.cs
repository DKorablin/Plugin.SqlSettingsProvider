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
		/// <summary>Размер буфера</summary>
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

		/// <summary>Получить идентификатор плагина</summary>
		/// <param name="plugin">Интерфейс плагина, ряд по которому необходимо найти</param>
		/// <returns>Идентификатор плагина</returns>
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

		/// <summary>Сохранить значение плагина</summary>
		/// <param name="plugin">Интерфейс плагина, параметр по которому необходимо сохранить</param>
		/// <param name="key">Ключ значения в плагине по которому сохранить значение</param>
		/// <param name="value">Значение ключа</param>
		public void SavePluginParameter(IPluginDescription plugin, String key, Byte[] value)
			=> this.SavePluginParameter(this.UserID, this.ApplicationID, this[plugin], key, value);

		/// <summary>Загрузить значение плагина</summary>
		/// <param name="plugin">Интерфейс плагина, параметры по которому необходимо загрузить</param>
		/// <param name="key">Ключ по которому надо получить значение</param>
		/// <returns>Массив параметров</returns>
		public IEnumerable<KeyValuePair<String,Byte[]>> LoadPluginParameters(IPluginDescription plugin, String key)
			=> this.LoadPluginParameters(this.UserID, this.ApplicationID, this[plugin], key);

		/// <summary>Сохранить значение плагина</summary>
		/// <param name="userId">Идентификатор пользователя</param>
		/// <param name="applicationId">Идентификатор приложения</param>
		/// <param name="pluginId">Идентификатор плагина</param>
		/// <param name="valueName">Ключ плагина значение которого надо сохранить</param>
		/// <param name="value">Значение ключа плагина</param>
		/// <returns>Резултат сохранения</returns>
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

		/// <summary>Загрузить значение плагина</summary>
		/// <param name="userId">Идентификатор пользователя</param>
		/// <param name="applicationId">Идентификатор приложения</param>
		/// <param name="pluginId">Идентификатор плагина</param>
		/// <param name="keyName">Ключ по которому надо получить значение</param>
		/// <returns>Массив байт</returns>
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
						if(!reader.IsDBNull(1))//Только если значение определно. Т.к. источник данных может прислать список всех параметров без исключения null
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
								if(count < SqlDataSource.BufferLength)//TODO: Зачем это надо?
									Array.Resize(ref buffer, (Int32)count);

								bytes.AddRange(buffer);
							} while(count >= SqlDataSource.BufferLength);
							result.Add(key, bytes.ToArray());
						}
					return result;
				}
			}
		}

		/// <summary>Получить идентификатор плагина</summary>
		/// <param name="userId">Идентификатор пользователя</param>
		/// <param name="applicationId">Идентификатор приложения</param>
		/// <param name="pluginName">Идентификатор плагина</param>
		/// <returns>Идентификатор плагина из БД</returns>
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

		/// <summary>Инициализация внутренних переменных источника данных</summary>
		private void InitApplication()
		{
			if(this._applicationId.HasValue && this._userId.HasValue)
				return;
			else
			{
				String userName = Thread.CurrentPrincipal.Identity.IsAuthenticated
					? Thread.CurrentPrincipal.Identity.Name
					: Environment.UserName;

				String applicationName = String.Join("|", this._plugin.Host.Plugins.FindPluginType<IPluginKernel>().Select(p => p.Name).OrderBy(p => p).ToArray());

				if(applicationName.Length == 0)
					applicationName = "<Empty>";

				this._pluginsIdentity = this.GetApplicationPlugins(userName, applicationName, out Int32 userId, out Int32 applicationId);

				this._userId = userId;
				this._applicationId = applicationId;
			}
		}

		/// <summary>Получение параметров приложения для ускоренной загрузки значений плагинов</summary>
		/// <param name="login">Логин</param>
		/// <param name="applicationName">Наименование приложения</param>
		/// <param name="userId">Идентификатор логина в БД</param>
		/// <param name="applicationId">Издентификатор приложения в БД</param>
		/// <returns>Массив изентификаторов и наименований плагинов</returns>
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