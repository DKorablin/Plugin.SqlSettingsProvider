/****** Object:  Table [sal].[UserApplication]    Script Date: 01.06.2013 0:57:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [sal].[UserApplication](
	[UserID] [int] NOT NULL,
	[ApplicationID] [int] NOT NULL,
 CONSTRAINT [PK_UserApplication] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[ApplicationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [sal].[UserApplication]  WITH CHECK ADD  CONSTRAINT [FK_UserApplication_Application] FOREIGN KEY([ApplicationID])
REFERENCES [sal].[Application] ([ApplicationID])
GO

ALTER TABLE [sal].[UserApplication] CHECK CONSTRAINT [FK_UserApplication_Application]
GO

ALTER TABLE [sal].[UserApplication]  WITH CHECK ADD  CONSTRAINT [FK_UserApplication_Users] FOREIGN KEY([UserID])
REFERENCES [sal].[Users] ([UserID])
GO

ALTER TABLE [sal].[UserApplication] CHECK CONSTRAINT [FK_UserApplication_Users]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Идентификатор пользователя' , @level0type=N'SCHEMA',@level0name=N'sal', @level1type=N'TABLE',@level1name=N'UserApplication', @level2type=N'COLUMN',@level2name=N'UserID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Идентификатор приложения' , @level0type=N'SCHEMA',@level0name=N'sal', @level1type=N'TABLE',@level1name=N'UserApplication', @level2type=N'COLUMN',@level2name=N'ApplicationID'
GO