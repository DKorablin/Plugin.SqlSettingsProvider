/****** Object:  StoredProcedure [settings].[GetAplicationParameters]    Script Date: 01.06.2013 0:57:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		AlphaOmega
-- Create date: 22.07.2012
-- Description:	Получение идентификаторов для получния данных
-- Получение:	EXEC [settings].[GetAplicationParameters] @Login='Пользователь', @ApplicationName='Kernel.Empty', @UserId=null, @ApplicationId=null
-- =============================================
CREATE PROCEDURE [settings].[GetAplicationParameters]
	@Login NVarChar(255),
	@ApplicationName NVarChar(255),
	@PluginName NVarChar(255)=null,
	@UserId Int Out,
	@ApplicationId Int Out,
	@PluginId Int=null Out
AS
BEGIN
	SET NOCOUNT ON;
	
	--Получение идентификатора пользователя
	SELECT @UserId=UserID FROM sal.Users WHERE [Login]=@Login
	
	If @UserId IS NULL Begin
		INSERT INTO sal.Users([Login]) VALUES(@Login)
		SET @UserId=SCOPE_IDENTITY();
	End
	
	--Получение идентификатора приложения
	SELECT @ApplicationId=ApplicationID FROM sal.[Application] WHERE Name=@ApplicationName;
	
	If @ApplicationId IS NULL Begin
		INSERT INTO sal.[Application](Name) VALUES(@ApplicationName)
		SET @ApplicationId=SCOPE_IDENTITY();
		
		INSERT INTO sal.UserApplication(UserID,ApplicationID) VALUES(@UserId,@ApplicationId);
	End
	
	--Получение идентификатора плагина
	If @PluginName IS NOT NULL Begin
		SELECT @PluginId FROM sal.Plugins WHERE PluginName=@PluginName;
		
		If @PluginId IS NULL Begin
			INSERT INTO sal.Plugins(PluginName) VALUES(@PluginName)
			SET @PluginId=SCOPE_IDENTITY();
			
			INSERT INTO sal.ApplicationPlugins(ApplicationID,PluginID) VALUES(@ApplicationId,@PluginId);
		End
	End
	--Получение всех плагинов с идентификаторами
	Else If @PluginName IS NULL Begin
		SELECT
			p.PluginID,
			p.PluginName
		FROM
			sal.Plugins p WIth(nolock)
			INNER JOIN sal.ApplicationPlugins ap With(nolock) ON ap.PluginID=p.PluginID
			INNER JOIN sal.UserApplication ua With(nolock) ON ua.ApplicationID=ap.ApplicationID
		WHERE ap.ApplicationID=@ApplicationId
		AND ua.UserID=@UserId
	End
END

GO