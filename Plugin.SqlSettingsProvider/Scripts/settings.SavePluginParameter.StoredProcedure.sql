/****** Object:  StoredProcedure [settings].[SavePluginParameter]    Script Date: 01.06.2013 0:57:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		AlphaOmega
-- Create date: 22.08.2012
-- Description:	Сохранение значения плагина
-- Test:		EXEC [settings].[SavePluginParameter] @FlagId=0, @UserId=1, @applicationId=2, @pluginId=20, @valueName='UseDefaultCredentials', @value=0x00000100
-- Delete:		EXEC [settings].[SavePluginParameter] @FlagId=0, @UserId=1, @ApplicationId=1, @PluginId=47, @ValueName='MachineNames', @Value=null
-- Set Default:	EXEC [settings].[SavePluginParameter] @FlagId=1, @UserId=1, @ApplicationId=1, @PluginId=3, @ValueName='ConnectionString', @Value=null
-- =============================================
ALTER PROCEDURE [settings].[SavePluginParameter]
	@FlagId Int=0,
	@UserId int,
	@ApplicationId int,
	@PluginId int,
	@ValueName NVarChar(255),
	@Value VarBinary(MAX)=null
AS
BEGIN
	If @FlagId=0 Begin--Сохранение значения плагина
		If @Value IS NULL Begin--Удаление значения
			DELETE FROM settings.PluginValues2
				FROM settings.PluginValues2 AS pv
			INNER JOIN sal.PluginValueNames pvn on pvn.ValueID=pv.ValueID
			WHERE pv.ApplicationID=@ApplicationId
				AND pv.UserID=@UserId
				AND pv.PluginID=@PluginId
				AND pvn.ValueName=ISNULL(@ValueName,pvn.ValueName)
			return @@ROWCOUNT;
		End
		Else Begin--Изменение значения

			DECLARE @ValueId Int;--Получить идентификатор параметра плагина
			SELECT @ValueId=pvn.ValueId FROM sal.PluginValueNames pvn WHERE pvn.PluginID=@PluginId AND pvn.ValueName=@ValueName

			If @ValueId IS NULL BEGIN--Накой параметра плагина не найден. Необходимо добавить
				INSERT INTO sal.PluginValueNames(PluginID,ValueName) VALUES(@PluginId,@ValueName)
				SET @ValueId=SCOPE_IDENTITY();
			END

			--Сохранение параметра плагина
			MERGE INTO settings.PluginValues2 AS pv
			USING (SELECT @ApplicationId, @UserId, @PluginId, @valueId, @value) AS src (ApplicationId, UserId, PluginId, ValueId, Value)
			ON pv.ApplicationID=src.ApplicationId
				AND pv.UserID=src.UserId
				AND pv.PluginId=src.PluginId
				AND pv.ValueId=src.ValueId
			WHEN MATCHED THEN
				UPDATE SET pv.Value=src.Value
			WHEN NOT MATCHED THEN
				INSERT (ApplicationID,UserID,PluginID,ValueID,Value)
				VALUES (src.ApplicationId, src.UserId, src.PluginId, src.ValueId, src.Value);
			return @@ROWCOUNT;
		End
	End--If @FlagId=0
	If @FlagId=1 Begin--Выставить значение плагина по умолчанию от определённого пользователя
		UPDATE sal.PluginValueNames
		SET sal.PluginValueNames.Value=ISNULL(@Value, v.Value)--Если не передано в параметре, то взять из параметра пользователя
		FROM sal.PluginValueNames
			INNER JOIN settings.PluginValues2 v ON v.ValueID=sal.PluginValueNames.ValueID
		WHERE
			v.PluginID=@PluginId
			AND v.UserID=@UserId
			AND v.ApplicationID=@ApplicationId
			AND sal.PluginValueNames.ValueName=@ValueName
	End--If @FlagId=1
END