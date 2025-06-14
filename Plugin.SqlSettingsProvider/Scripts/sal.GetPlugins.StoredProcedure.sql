/****** Object:  StoredProcedure [sal].[GetPlugins]    Script Date: 01.06.2013 0:57:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:				AlphaOmega
-- Create date:			26.02.2011
-- Description:			Gets all information on all plugins
-- Get Applications:	EXEC sal.GetPlugins @FlagID=0, @UserID=1
-- Get Plugins:			EXEC sal.GetPlugins @FlagID=1, @UserID=1, @ApplicationID=1
-- =============================================
CREATE PROCEDURE [sal].[GetPlugins]
	@FlagID TinyInt,
	@UserID Int,
	@ApplicationID Int=null
AS
BEGIN
	SET NOCOUNT ON;

	If @FlagID=0 Begin--Gets the list of all applications availabe for the user
		SELECT
			a.ApplicationID,a.Name,a.Description
		FROM sal.[Application] a With(nolock)
			INNER JOIN sal.[UserApplication] ua With(nolock) ON ua.ApplicationID=a.ApplicationID
			INNER JOIN sal.[Users] u With(nolock) ON u.UserID=ua.UserID
		WHERE ua.UserID=@UserID
	End
	If @FlagID=1 Begin
		SELECT
			p.PluginID,p.PluginName,p.PluginData
		FROM sal.[Plugins] p WITH(nolock)
			INNER JOIN sal.[ApplicationPlugins] ap WITH(nolock) ON ap.PluginID=p.PluginID
			INNER JOIN sal.[UserApplication] ua WITH(nolock) ON ua.ApplicationID=ap.ApplicationID AND ua.UserID=ua.UserID
			INNER JOIN sal.[UserPlugin] up WITH(nolock) ON up.PluginID=p.PluginID
			INNER JOIN sal.[Users] u WITH(nolock) ON u.UserID=up.UserID
		WHERE up.UserID=@UserID AND ap.ApplicationID=@ApplicationID
    End
END

GO