/****** Object:  StoredProcedure [settings].[GetPluginId]    Script Date: 01.06.2013 0:57:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		AlphaOmega
-- Create date: 22.07.2012
-- Description:	Получение идентификатора плагина
/* Пример:
	DECLARE @PluginId Int;
	EXEC @PluginId = [settings].[GetPluginId] @UserId=1, @ApplicationId=1,@PluginName='Plugin.IISControl'
	SELECT @PluginId;
*/
-- =============================================
CREATE PROCEDURE [settings].[GetPluginId]
	@UserId int, 
	@ApplicationId int,
	@PluginName NVarChar(255)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @PluginId Int;
	
	SELECT @PluginId=p.PluginID
	FROM sal.Plugins p With(nolock)
		INNER JOIN sal.ApplicationPlugins ap With(nolock) ON ap.PluginID=p.PluginID
	WHERE p.PluginName=@PluginName
		AND ap.ApplicationID=@ApplicationId
	
	If @PluginId IS NULL Begin--Нет полного пути плагина
	
		SELECT @PluginId=p.PluginID
		FROM sal.Plugins p With(nolock)
		WHERE p.PluginName=@PluginName
		
		If @PluginId IS NULL Begin
			INSERT INTO sal.Plugins(PluginName) VALUES(@PluginName);
			SET @PluginId=SCOPE_IDENTITY();
		End
		If @PluginId IS NOT NULL--Отсутствует связка с приложением
			INSERT INTO sal.ApplicationPlugins(ApplicationID,PluginID) VALUES(@ApplicationId,@PluginId);
	End
	
	RETURN @PluginId;
END

GO