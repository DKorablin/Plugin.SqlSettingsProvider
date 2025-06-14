/****** Object:  Table [sal].[UserPlugin]    Script Date: 01.06.2013 0:57:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [sal].[UserPlugin](
	[UserID] [int] NOT NULL,
	[PluginID] [int] NOT NULL,
 CONSTRAINT [PK_UserPlugin] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[PluginID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [sal].[UserPlugin]  WITH CHECK ADD  CONSTRAINT [FK_UserPlugin_Plugins] FOREIGN KEY([PluginID])
REFERENCES [sal].[Plugins] ([PluginID])
GO

ALTER TABLE [sal].[UserPlugin] CHECK CONSTRAINT [FK_UserPlugin_Plugins]
GO

ALTER TABLE [sal].[UserPlugin]  WITH CHECK ADD  CONSTRAINT [FK_UserPlugin_Users] FOREIGN KEY([UserID])
REFERENCES [sal].[Users] ([UserID])
GO

ALTER TABLE [sal].[UserPlugin] CHECK CONSTRAINT [FK_UserPlugin_Users]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Идентификатор пользователя' , @level0type=N'SCHEMA',@level0name=N'sal', @level1type=N'TABLE',@level1name=N'UserPlugin', @level2type=N'COLUMN',@level2name=N'UserID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Идентификатор плагина' , @level0type=N'SCHEMA',@level0name=N'sal', @level1type=N'TABLE',@level1name=N'UserPlugin', @level2type=N'COLUMN',@level2name=N'PluginID'
GO