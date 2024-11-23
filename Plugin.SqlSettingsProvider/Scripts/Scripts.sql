-- Перенос значения параметров плагина от одного клиента в параметры плагина по умолчанию
UPDATE sal.PluginValueNames
SET
	sal.PluginValueNames.Value=settings.PluginValues2.Value
FROM sal.PluginValueNames
INNER JOIN settings.PluginValues2
ON sal.PluginValueNames.ValueID=settings.PluginValues2.ValueID
WHERE
	sal.PluginValueNames.PluginID=30 --Идентификатор плагина значения которого необходимо перенести
	AND settings.PluginValues2.UserID=1--Идентификатор пользователя от которого необходимо перенести значения
	AND settings.PluginValues2.ApplicationID=5--Идентификатор приложения из которого перенести значения