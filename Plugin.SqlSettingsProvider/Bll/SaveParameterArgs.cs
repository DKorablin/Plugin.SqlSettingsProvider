using System;
using System.Xml.Serialization;

namespace Plugin.SqlSettingsProvider.Bll
{
	internal class SaveParameterArgs
	{
		[XmlIgnore]
		public SqlDataSource Data { get; set; }
		public Int32 FlagId { get; set; }
		public Int32 UserId { get; set; }
		public Int32 ApplicationId { get; set; }
		public Int32 PluginId { get; set; }
		public String ValueName { get; set; }
		public Byte[] Value { get; set; }

		public SaveParameterArgs(SqlDataSource data, Int32 flagId, Int32 userId, Int32 applicationId, Int32 pluginId, String valueName, Byte[] value)
		{
			this.Data = data;
			this.FlagId = flagId;
			this.UserId = userId;
			this.ApplicationId = applicationId;
			this.PluginId = pluginId;
			this.ValueName = valueName;
			this.Value = value;
		}
	}
}