using System.Reflection;
using System.Runtime.InteropServices;

[assembly: ComVisible(false)]
[assembly: Guid("30c832aa-37b3-4748-80c4-01604b5434ad")]
[assembly: System.CLSCompliant(true)]

#if NETSTANDARD
[assembly: AssemblyMetadata("ProjectUrl", "https://dkorablin.ru/project/Default.aspx?File=103")]
#else

[assembly: AssemblyTitle("Plugin.SqlSettingsProvider")]
[assembly: AssemblyDescription("SQL Settings provider plugin")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyCompany("Danila Korablin")]
[assembly: AssemblyProduct("Plugin.SqlSettingsProvider")]
[assembly: AssemblyCopyright("Copyright © Danila Korablin 2012-2024")]
#endif