/****** Object:  Table [sal].[ApplicationPlugins]    Script Date: 01.06.2013 0:57:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [sal].[ApplicationPlugins](
	[ApplicationID] [int] NOT NULL,
	[PluginID] [int] NOT NULL,
 CONSTRAINT [PK_ApplicationPlugins] PRIMARY KEY CLUSTERED 
(
	[ApplicationID] ASC,
	[PluginID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [sal].[ApplicationPlugins]  WITH CHECK ADD  CONSTRAINT [FK_ApplicationPlugins_Application] FOREIGN KEY([ApplicationID])
REFERENCES [sal].[Application] ([ApplicationID])
GO

ALTER TABLE [sal].[ApplicationPlugins] CHECK CONSTRAINT [FK_ApplicationPlugins_Application]
GO

ALTER TABLE [sal].[ApplicationPlugins]  WITH CHECK ADD  CONSTRAINT [FK_ApplicationPlugins_Plugins] FOREIGN KEY([PluginID])
REFERENCES [sal].[Plugins] ([PluginID])
GO

ALTER TABLE [sal].[ApplicationPlugins] CHECK CONSTRAINT [FK_ApplicationPlugins_Plugins]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Application identifier' , @level0type=N'SCHEMA',@level0name=N'sal', @level1type=N'TABLE',@level1name=N'ApplicationPlugins', @level2type=N'COLUMN',@level2name=N'ApplicationID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Plugin identifier' , @level0type=N'SCHEMA',@level0name=N'sal', @level1type=N'TABLE',@level1name=N'ApplicationPlugins', @level2type=N'COLUMN',@level2name=N'PluginID'
GO