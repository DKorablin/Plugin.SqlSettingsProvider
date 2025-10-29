using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing.Design;
using System.Web.UI.Design;
using AlphaOmega.Data;
using AlphaOmega.Design;

namespace Plugin.SqlSettingsProvider
{
	public class PluginSettings
	{
		private readonly Plugin _plugin;
		private String _connectionString;
		private String _providerName;

		internal PluginSettings(Plugin plugin)
			=> this._plugin = plugin;

		[Category("Data")]
		[Description("Connection string to the data source")]
		[Editor(typeof(ConnectionStringEditor), typeof(UITypeEditor))]
		public String ConnectionString
		{
			get => this._connectionString;
			set
			{
				this._connectionString = String.IsNullOrEmpty(value)
					? null
					: value;
				this._plugin.DataSource = null;
			}
		}

		[Category("Data")]
		[Description("Data source connection provider. (.NET5+ dynamic data providers are not supported)")]
		[Editor(typeof(AdoNetProviderEditor), typeof(UITypeEditor))]
		public String ProviderName
		{
			get => this._providerName;
			set
			{
				if(String.IsNullOrEmpty(value))
				{
					this._providerName = null;
					this._plugin.DataSource = null;
				} else
				{
					var rows = DbProviderFactories.GetFactoryClasses().Rows;
					if(rows.Count > 0)//.NET 5+ FactoryClasses are not registered
					{
						Boolean isFound = false;
						foreach(DataRow row in rows)
							if(value.Equals((String)row["InvariantName"], StringComparison.InvariantCultureIgnoreCase))
							{
								isFound = true;
								break;
							}
						if(!isFound)
							throw new ArgumentException($"Provider name '{value}' invalid");
					}

					this._providerName = value;
					this._plugin.DataSource = null;
				}
			}
		}

		public Boolean IsValid => this.ProviderName != null && this.ConnectionString != null;

		/// <summary>Create a connection to a data source</summary>
		/// <returns>Connect to a data source</returns>
		internal DbConnector CreateConnector()
		{
			DbConnector result = new DbConnector(this.ProviderName, this.ConnectionString);
			result.Command.CommandType = CommandType.StoredProcedure;
			return result;
		}

		/// <summary>Data source connections</summary>
		internal DbConnection CreateConnection()
		{
			DbProviderFactory factory = DbProviderFactories.GetFactory(this.ProviderName);
			DbConnection result = factory.CreateConnection();
			result.ConnectionString = this.ConnectionString;
			return result;
		}

		/// <summary>Command to execute in the data source</summary>
		internal DbCommand CreateCommand(DbConnection connection)
		{
			DbProviderFactory factory = DbProviderFactories.GetFactory(this.ProviderName);
			DbCommand result = factory.CreateCommand();
			result.Connection = connection;
			result.CommandType = CommandType.StoredProcedure;
			return result;
		}
	}
}