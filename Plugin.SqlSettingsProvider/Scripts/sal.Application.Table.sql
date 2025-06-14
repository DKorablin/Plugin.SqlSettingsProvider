/****** Object:  Table [sal].[Application]    Script Date: 01.06.2013 0:57:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [sal].[Application](
	[ApplicationID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](1024) NULL,
	[AddedDate] [smalldatetime] NOT NULL CONSTRAINT [DF_Application_Added]  DEFAULT (getdate()),
 CONSTRAINT [PK_Application] PRIMARY KEY CLUSTERED 
(
	[ApplicationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_ApplicationName] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Application identifier' , @level0type=N'SCHEMA',@level0name=N'sal', @level1type=N'TABLE',@level1name=N'Application', @level2type=N'COLUMN',@level2name=N'ApplicationID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Application name' , @level0type=N'SCHEMA',@level0name=N'sal', @level1type=N'TABLE',@level1name=N'Application', @level2type=N'COLUMN',@level2name=N'Name'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Application description' , @level0type=N'SCHEMA',@level0name=N'sal', @level1type=N'TABLE',@level1name=N'Application', @level2type=N'COLUMN',@level2name=N'Description'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Application added date' , @level0type=N'SCHEMA',@level0name=N'sal', @level1type=N'TABLE',@level1name=N'Application', @level2type=N'COLUMN',@level2name=N'AddedDate'
GO