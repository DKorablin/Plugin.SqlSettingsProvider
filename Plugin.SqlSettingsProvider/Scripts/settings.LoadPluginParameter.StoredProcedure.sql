/****** Object:  StoredProcedure [settings].[LoadPluginParameter]    Script Date: 01.06.2013 0:57:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		AlphaOmega
-- Create date: 22.07.2011
-- Description:	Загрузка параметра из источника данных
-- Example:		EXEC [Confer].[settings].[LoadPluginParameter] @UserId=1, @ApplicationId=2, @PluginId=41
-- =============================================
CREATE PROCEDURE [settings].[LoadPluginParameter]
	@UserId int,
	@ApplicationId int,
	@PluginId int,
	@ValueName NVarChar(255)=null
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
		pvn.ValueName,
		ISNULL((SELECT pv.Value FROM settings.PluginValues2 pv WHERE pv.PluginID=pvn.PluginID AND pv.ValueID=pvn.ValueID AND pv.ApplicationID=@ApplicationId AND pv.UserID=@UserId),pvn.Value)
	FROM sal.PluginValueNames pvn
	WHERE pvn.PluginID=@PluginID
		AND (EXISTS(SELECT 1 FROM settings.PluginValues2 pv WHERE pv.PluginID=pvn.PluginID AND pv.ValueID=pvn.ValueID AND pv.ApplicationID=@ApplicationId AND pv.UserID=@UserId) OR pvn.Value is not null)--TODO (03.07.2013): Временный фикс т.к. не знаю как отрезать значения с null. Накостылено в SqlSettingsProvider
		AND pvn.ValueName = ISNULL(@ValueName,pvn.ValueName)
		COLLATE SQL_Latin1_General_CP1_CS_AS

	/*SELECT
		ISNULL(pv.ValueName,pvd.ValueName) as ValueName,
		ISNULL(pv.Value,pvd.Value) as Value
	FROM settings.PluginValues pv With(nolock)
	LEFT JOIN settings.PluginValuesDefault pvd With(nolock)
		ON (pvd.ApplicationID is null OR pv.ApplicationID=pvd.ApplicationID)
		AND pv.PluginID=pvd.PluginID
		AND pv.ValueName=pvd.ValueName
	WHERE
		(pv.UserID = @UserId OR (pv.UserID is null AND pvd.PluginID=@PluginId))
		AND pv.ApplicationID = @ApplicationId
		AND pv.PluginID=@PluginId
		AND pv.ValueName = ISNULL(@ValueName,pv.ValueName)
	COLLATE SQL_Latin1_General_CP1_CS_AS*/
END

GO