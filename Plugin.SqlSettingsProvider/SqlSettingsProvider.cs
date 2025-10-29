using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using SAL.Flatbed;

namespace Plugin.SqlSettingsProvider
{
	public class SqlSettingsProvider : ISettingsProvider
	{
		private readonly Plugin _pluginHost;
		private readonly IPluginDescription _pluginWrapper;
		private INotifyPropertyChanged _settingsChanged;

		internal SqlSettingsProvider(Plugin pluginHost, IPluginDescription pluginWrapper)
		{
			this._pluginHost = pluginHost;
			this._pluginWrapper = pluginWrapper;
		}

		private void settings_PropertyChanged(Object sender, PropertyChangedEventArgs e)
		{
			String propertyName = e.PropertyName;
			IPluginSettings settings = (IPluginSettings)this._pluginWrapper.Instance;
			PropertyInfo property = settings.Settings.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

			if(property == null)
				this._pluginHost.Trace.TraceEvent(TraceEventType.Verbose, 9, String.Format("Property {0} not found in plugin {1} ", e.PropertyName, this._pluginWrapper.ID));
			else if(SqlSettingsProvider.CanSaveProperty(property))
			{
				Object value = property.GetValue(settings.Settings, null);
				this.SaveAssemblyParameterI(this._pluginWrapper, propertyName, value);
			}
		}

		#region ISettingsProvider
		Stream ISettingsProvider.LoadAssemblyBlob(String key)
		{
			if(String.IsNullOrEmpty(key))
				throw new ArgumentNullException(nameof(key));

			foreach(KeyValuePair<String, Byte[]> result in this._pluginHost.DataSource.LoadPluginParameters(this._pluginWrapper, key))
				return result.Key == key
					? new MemoryStream(result.Value)
					: throw new ArgumentException($"Получен ключ {result.Key} вместо {key}");
			return null;
		}

		Object ISettingsProvider.LoadAssemblyParameter(String key)
			=> String.IsNullOrEmpty(key)
				? throw new ArgumentNullException(nameof(key))
				: this.LoadAssemblyParameterI(key);

		IEnumerable<KeyValuePair<String, Object>> ISettingsProvider.LoadAssemblyParameters()
		{
			foreach(KeyValuePair<String, Byte[]> pair in this._pluginHost.DataSource.LoadPluginParameters(this._pluginWrapper, null))
			{
				Object value;
				try
				{
					value = Utils.DeserializeObject(pair.Value);
				} catch(Exception exc)
				{
					this._pluginHost.Trace.TraceData(TraceEventType.Error, 10, exc);
					continue;
				}
				yield return new KeyValuePair<String, Object>(pair.Key, value);
			}
		}

		void ISettingsProvider.LoadAssemblyParameters<T>(T settings)
		{
			_ = settings ?? throw new ArgumentNullException(nameof(settings));

			if(this._settingsChanged != null)
				this._settingsChanged.PropertyChanged -= settings_PropertyChanged;

			try
			{
				PropertyInfo[] properties = settings.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
				foreach(PropertyInfo property in properties)
					if(property.CanWrite)
					{//TODO: It looks like there's a database query for each field here...
						Object value = this.LoadAssemblyParameterI(property.Name);
						try
						{
							property.SetValue(settings, value, null);
						} catch(ArgumentException exc)
						{//If you change the field type, you must continue loading the parameters.
							exc.Data.Add(property.Name, value == null ? "null" : value.ToString());
							this._pluginHost.Trace.TraceData(TraceEventType.Error, 10, exc);
						}
					}
			} finally
			{
				if(settings is INotifyPropertyChanged onSettingsChanged)
				{
					this._settingsChanged = onSettingsChanged;
					this._settingsChanged.PropertyChanged += settings_PropertyChanged;
				}
			}
		}

		/*public static void SetValueForValueType<T>(this FieldInfo field, ref T item, Object value) where T : struct
		{
			field.SetValueDirect(__makeref(item), value);
		}*/

		Boolean ISettingsProvider.RemoveAssemblyBlob(String key)
		{
			if(String.IsNullOrEmpty(key))
				throw new ArgumentNullException(nameof(key));

			this._pluginHost.DataSource.SavePluginParameter(this._pluginWrapper, key, null);
			return true;
		}

		void ISettingsProvider.RemoveAssemblyParameter()
			=> this._pluginHost.DataSource.SavePluginParameter(this._pluginWrapper, null, null);

		Boolean ISettingsProvider.RemoveAssemblyParameter(String key)
		{
			if(String.IsNullOrEmpty(key))
				throw new ArgumentNullException(nameof(key));

			this._pluginHost.DataSource.SavePluginParameter(this._pluginWrapper, key, null);
			return true;
		}

		void ISettingsProvider.SaveAssemblyBlob(String key, Stream value)
		{
			if(String.IsNullOrEmpty(key))
				throw new ArgumentNullException(nameof(key));

			Byte[] bytes = null;
			if(value != null)
			{
				bytes = new Byte[value.Length];
				value.Position = 0;
				value.Read(bytes, 0, bytes.Length);
			}
			this._pluginHost.DataSource.SavePluginParameter(this._pluginWrapper, key, bytes);
		}

		void ISettingsProvider.SaveAssemblyParameter(String key, Object value)
		{
			if(String.IsNullOrEmpty(key))
				throw new ArgumentNullException(nameof(key));

			this.SaveAssemblyParameterI(this._pluginWrapper, key, value);
		}

		/// <summary>Saving build parameters to storage</summary>
		/// <param name="settings">Plugin settings object</param>
		void ISettingsProvider.SaveAssemblyParameters()
		{
			if(this._settingsChanged != null)
				this._pluginHost.Trace.TraceEvent(TraceEventType.Verbose, 9, $"Plugin {this._pluginWrapper.ID} supports INotifyPropertyChanged. Properties saved automatically");

			IPluginSettings settings = (IPluginSettings)this._pluginWrapper.Instance;

			PropertyInfo[] properties = settings.Settings.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
			foreach(PropertyInfo property in properties)
				if(SqlSettingsProvider.CanSaveProperty(property))
				{
					Object value = property.GetValue(settings.Settings, null);
					this.SaveAssemblyParameterI(this._pluginWrapper, property.Name, value);
				}
		}
		#endregion ISettingsProvider

		private Object LoadAssemblyParameterI(String key)
		{
			foreach(KeyValuePair<String, Byte[]> result in this._pluginHost.DataSource.LoadPluginParameters(this._pluginWrapper, key))
				if(result.Key == key)
					try
					{
						return Utils.DeserializeObject(result.Value);
					} catch(Exception exc)
					{
						exc.Data.Add(nameof(key), key);
						exc.Data.Add("Plugin", this._pluginWrapper.Name);
						this._pluginHost.Trace.TraceData(TraceEventType.Error, 10, exc);
						return null;
					} else
					throw new ArgumentException($"Got key {result.Key} instead of {key}");
			return null;
			//return Plugin.DeserializeObject(this.Sql.LoadPluginParameter(plugin, key));
		}

		private void SaveAssemblyParameterI(IPluginDescription plugin, String key, Object value)
		{
			if(value != null)//Try to save enum as underlying type
			{
				Type valueType = value.GetType();
				if(valueType.IsEnum)
					value = Convert.ChangeType(value, Enum.GetUnderlyingType(valueType));
			}

			Byte[] bytes = null;
			if(value != null)
				bytes = Utils.SerializeObject(value);
			this._pluginHost.DataSource.SavePluginParameter(plugin, key, bytes);
		}

		/// <summary>Checking if a property can be saved in the SettingsProvider</summary>
		/// <param name="property">Property information</param>
		/// <returns></returns>
		private static Boolean CanSaveProperty(PropertyInfo property)
			=> property.CanRead && property.CanWrite && property.GetSetMethod(false) != null;
	}
}