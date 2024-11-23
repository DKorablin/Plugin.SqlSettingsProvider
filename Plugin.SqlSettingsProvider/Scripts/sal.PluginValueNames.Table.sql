/****** Object:  Table [sal].[PluginValueNames]    Script Date: 25.05.2016 20:31:39 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [sal].[PluginValueNames](
	[ValueID] [int] IDENTITY(1,1) NOT NULL,
	[PluginID] [int] NOT NULL,
	[ValueName] [nvarchar](255) NOT NULL,
	[Value] [varbinary](max) NULL,
 CONSTRAINT [PK_PluginValueNames] PRIMARY KEY CLUSTERED 
(
	[ValueID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [sal].[PluginValueNames]  WITH CHECK ADD  CONSTRAINT [FK_PluginValueNames_Plugins] FOREIGN KEY([PluginID])
REFERENCES [sal].[Plugins] ([PluginID])
GO

ALTER TABLE [sal].[PluginValueNames] CHECK CONSTRAINT [FK_PluginValueNames_Plugins]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Идентификатор значения параметра плагина' , @level0type=N'SCHEMA',@level0name=N'sal', @level1type=N'TABLE',@level1name=N'PluginValueNames', @level2type=N'COLUMN',@level2name=N'ValueID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Идентификатор плагина' , @level0type=N'SCHEMA',@level0name=N'sal', @level1type=N'TABLE',@level1name=N'PluginValueNames', @level2type=N'COLUMN',@level2name=N'PluginID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Наименование значения плагина' , @level0type=N'SCHEMA',@level0name=N'sal', @level1type=N'TABLE',@level1name=N'PluginValueNames', @level2type=N'COLUMN',@level2name=N'ValueName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Значение параметра плагина по умолчанию' , @level0type=N'SCHEMA',@level0name=N'sal', @level1type=N'TABLE',@level1name=N'PluginValueNames', @level2type=N'COLUMN',@level2name=N'Value'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Наименования значений параметров плагина' , @level0type=N'SCHEMA',@level0name=N'sal', @level1type=N'TABLE',@level1name=N'PluginValueNames'
GO