-- ������� �������� ���������� ������� �� ������ ������� � ��������� ������� �� ���������
UPDATE sal.PluginValueNames
SET
	sal.PluginValueNames.Value=settings.PluginValues2.Value
FROM sal.PluginValueNames
INNER JOIN settings.PluginValues2
ON sal.PluginValueNames.ValueID=settings.PluginValues2.ValueID
WHERE
	sal.PluginValueNames.PluginID=30 --������������� ������� �������� �������� ���������� ���������
	AND settings.PluginValues2.UserID=1--������������� ������������ �� �������� ���������� ��������� ��������
	AND settings.PluginValues2.ApplicationID=5--������������� ���������� �� �������� ��������� ��������