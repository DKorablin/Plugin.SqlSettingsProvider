/****** Object:  StoredProcedure [sal].[ModifyPlugins]    Script Date: 01.06.2013 0:57:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		AlphaOmega
-- Create date:	26.02.2011
-- Description:	Modify plugin array in the DB
-- =============================================
CREATE PROCEDURE [sal].[ModifyPlugins] 
	@FlagID TinyInt, 
	@PluginID int = null,
	@PluginName NVarChar(255)=null,
	@PluginData VarBinary(MAX)=null
AS
BEGIN
	SET NOCOUNT ON;

	If @FlagID=0 Begin--Add new plugin
		INSERT INTO sal.Plugins(PluginName,PluginData) VAlUES(@PluginName,@PluginData)
		return Scope_Identity();
	End
	If @FlagID=1 Begin--Remove plugin
		DELETE FROM sal.Plugins WHERE PluginID=@PluginID;
		return @@ROWCOUNT;
	End
END

GO