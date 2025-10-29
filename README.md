# SQL Settings plugin provider
[![Auto build](https://github.com/DKorablin/Plugin.SqlSettingsProvider/actions/workflows/release.yml/badge.svg)](https://github.com/DKorablin/Plugin.SqlSettingsProvider/releases/latest)

Despite the fact that the provider is written in pure ADO.NET stored procedures are used to interact with tables so it's unlikely that the provider will work with another DBMS other than Microsoft SQL Server.

To interact with data source stored procedures are used. That's why you can add personal logic or expand specific settings for plugins proceeding from user and plugin.

## Getting Started
To install the SQL Settings Provider Plugin, follow these steps:
1. Download the latest release from the [Releases](https://github.com/DKorablin/Plugin.SqlSettingsProvider/releases)
2. Extract the downloaded ZIP file to a desired location.
3. Add tables and stored procedures to your SQL Server database using the provided script files.
4. Use the provided [Flatbed.Dialog (Lite)](https://dkorablin.github.io/Flatbed-Dialog-Lite) executable or download one of the supported host applications:
	- [Flatbed.Dialog](https://dkorablin.github.io/Flatbed-Dialog)
	- [Flatbed.MDI](https://dkorablin.github.io/Flatbed-MDI)
	- [Flatbed.MDI (WPF)](https://dkorablin.github.io/Flatbed-MDI-Avalon)
5. [Configure](https://github.com/DKorablin/Plugin.Configuration/releases) the SQL Settings Provider Plugin in your host application by specifying the connection string and other required parameters.