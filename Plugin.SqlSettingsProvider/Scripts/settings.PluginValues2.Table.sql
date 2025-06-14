/****** Object:  Table [settings].[PluginValues2]    Script Date: 25.05.2016 20:27:32 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [settings].[PluginValues2](
	[UserID] [int] NOT NULL,
	[ApplicationID] [int] NOT NULL,
	[PluginID] [int] NOT NULL,
	[ValueID] [int] NOT NULL,
	[Value] [varbinary](max) NULL,
 CONSTRAINT [PK_PluginValues2] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[ApplicationID] ASC,
	[PluginID] ASC,
	[ValueID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [settings].[PluginValues2]  WITH CHECK ADD  CONSTRAINT [FK_PluginValues2_Application] FOREIGN KEY([ApplicationID])
REFERENCES [sal].[Application] ([ApplicationID])
GO

ALTER TABLE [settings].[PluginValues2] CHECK CONSTRAINT [FK_PluginValues2_Application]
GO

ALTER TABLE [settings].[PluginValues2]  WITH CHECK ADD  CONSTRAINT [FK_PluginValues2_Plugins] FOREIGN KEY([PluginID])
REFERENCES [sal].[Plugins] ([PluginID])
GO

ALTER TABLE [settings].[PluginValues2] CHECK CONSTRAINT [FK_PluginValues2_Plugins]
GO

ALTER TABLE [settings].[PluginValues2]  WITH CHECK ADD  CONSTRAINT [FK_PluginValues2_PluginValueNames] FOREIGN KEY([ValueID])
REFERENCES [sal].[PluginValueNames] ([ValueID])
GO

ALTER TABLE [settings].[PluginValues2] CHECK CONSTRAINT [FK_PluginValues2_PluginValueNames]
GO

ALTER TABLE [settings].[PluginValues2]  WITH CHECK ADD  CONSTRAINT [FK_PluginValues2_Users] FOREIGN KEY([UserID])
REFERENCES [sal].[Users] ([UserID])
GO

ALTER TABLE [settings].[PluginValues2] CHECK CONSTRAINT [FK_PluginValues2_Users]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Идентификатор пользователя' , @level0type=N'SCHEMA',@level0name=N'settings', @level1type=N'TABLE',@level1name=N'PluginValues2', @level2type=N'COLUMN',@level2name=N'UserID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Идентификатор приложения' , @level0type=N'SCHEMA',@level0name=N'settings', @level1type=N'TABLE',@level1name=N'PluginValues2', @level2type=N'COLUMN',@level2name=N'ApplicationID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Идентификатор плагина' , @level0type=N'SCHEMA',@level0name=N'settings', @level1type=N'TABLE',@level1name=N'PluginValues2', @level2type=N'COLUMN',@level2name=N'PluginID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Идентификатор значения параметра плагина' , @level0type=N'SCHEMA',@level0name=N'settings', @level1type=N'TABLE',@level1name=N'PluginValues2', @level2type=N'COLUMN',@level2name=N'ValueID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Значение параметра плагина' , @level0type=N'SCHEMA',@level0name=N'settings', @level1type=N'TABLE',@level1name=N'PluginValues2', @level2type=N'COLUMN',@level2name=N'Value'
GO


