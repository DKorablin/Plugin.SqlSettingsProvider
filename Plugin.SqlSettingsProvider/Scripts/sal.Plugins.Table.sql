/****** Object:  Table [sal].[Plugins]    Script Date: 01.06.2013 0:57:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [sal].[Plugins](
	[PluginID] [int] IDENTITY(1,1) NOT NULL,
	[PluginName] [nvarchar](255) NOT NULL,
	[PluginData] [varbinary](max) NULL,
	[AddedDate] [smalldatetime] NOT NULL CONSTRAINT [DF_Plugins_AddedDate]  DEFAULT (getdate()),
 CONSTRAINT [PK_Plugins] PRIMARY KEY CLUSTERED 
(
	[PluginID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_Plugins] UNIQUE NONCLUSTERED 
(
	[PluginName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Plugin identifier' , @level0type=N'SCHEMA',@level0name=N'sal', @level1type=N'TABLE',@level1name=N'Plugins', @level2type=N'COLUMN',@level2name=N'PluginID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Plugin name' , @level0type=N'SCHEMA',@level0name=N'sal', @level1type=N'TABLE',@level1name=N'Plugins', @level2type=N'COLUMN',@level2name=N'PluginName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Plugin information stored in the database' , @level0type=N'SCHEMA',@level0name=N'sal', @level1type=N'TABLE',@level1name=N'Plugins', @level2type=N'COLUMN',@level2name=N'PluginData'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Plugin added date' , @level0type=N'SCHEMA',@level0name=N'sal', @level1type=N'TABLE',@level1name=N'Plugins', @level2type=N'COLUMN',@level2name=N'AddedDate'
GO