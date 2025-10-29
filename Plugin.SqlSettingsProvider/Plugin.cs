using System;
using System.Diagnostics;
using System.Linq;
using Plugin.SqlSettingsProvider.Bll;
using SAL.Flatbed;

namespace Plugin.SqlSettingsProvider
{
	/// <summary>Plugin for saving and loading settings from MSSQL</summary>
	public class Plugin : ISettingsPluginProvider, IPluginSettings<PluginSettings>
	{
		private TraceSource _trace;
		private PluginSettings _settings;
		private SqlDataSource _dataSource;

		internal TraceSource Trace
			=> this._trace ?? (this._trace = Plugin.CreateTraceSource<Plugin>());

		internal IHost Host { get; }

		Object IPluginSettings.Settings
			=> this.Settings;

		public PluginSettings Settings
		{
			get
			{
				if(this._settings == null)
				{
					ISettingsProvider provider = this.Host.Plugins.Settings(this);
					if(provider != null)
					{
						this._settings = new PluginSettings(this);
						provider.LoadAssemblyParameters(this._settings);
					}
				}
				return this._settings;
			}
		}

		internal SqlDataSource DataSource
		{
			get
			{
				if(this._dataSource == null
					&& this.Settings != null
					&& !String.IsNullOrEmpty(this.Settings.ConnectionString)
					&& !String.IsNullOrEmpty(this.Settings.ProviderName))
					this.DataSource = new SqlDataSource(this);
				return this._dataSource;
			}
			set { this._dataSource = value; }
		}

		public Plugin(IHost host)
			=> this.Host = host ?? throw new ArgumentNullException(nameof(host));

		ISettingsProvider ISettingsPluginProvider.GetSettingsProvider(IPlugin plugin)
		{
			_ = plugin ?? throw new ArgumentNullException(nameof(plugin));

			if(plugin == this)
				throw new InvalidOperationException("Cant configure self");
			else if(this.DataSource == null)
				return null;//The plugin is not configured
			else
			{
				IPluginDescription pluginWrapper = this.GetPluginWrapper(plugin);
				return new SqlSettingsProvider(this, pluginWrapper);
			}
		}

		Boolean IPlugin.OnConnection(ConnectMode mode)
		{
			if(this.Settings == null)
			{
				this.Trace.TraceEvent(TraceEventType.Error, 10, "{0} requires parent provider for storing connection settings", this.GetType());
				return false;
			} else
				return true;
		}

		Boolean IPlugin.OnDisconnection(DisconnectMode mode)
		{
			switch(mode)
			{
			case DisconnectMode.FlatbedClosed:
			case DisconnectMode.HostShutdown:
				return true;
			default:
				this.Trace.TraceEvent(TraceEventType.Error, 10, "Settings provider plugin can't be unloaded at the runtime");
				return false;
			}
		}

		private IPluginDescription GetPluginWrapper(IPlugin plugin)
		{
			IPluginDescription result = this.Host.Plugins.FirstOrDefault(p => p.Instance == plugin);
			return result ?? throw new ArgumentNullException($"Assembly {plugin.GetType().Assembly.GetName().Name} not found in list of loaded plugins");
		}

		private static TraceSource CreateTraceSource<T>(String name = null) where T : IPlugin
		{
			TraceSource result = new TraceSource(typeof(T).Assembly.GetName().Name + name);
			result.Switch.Level = SourceLevels.All;
			result.Listeners.Remove("Default");
			result.Listeners.AddRange(System.Diagnostics.Trace.Listeners);
			return result;
		}
	}
}