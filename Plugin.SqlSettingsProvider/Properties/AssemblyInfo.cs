using System.Reflection;
using System.Runtime.InteropServices;

[assembly: Guid("30c832aa-37b3-4748-80c4-01604b5434ad")]
[assembly: System.CLSCompliant(true)]

#if NETSTANDARD
[assembly: AssemblyMetadata("ProjectUrl", "https://dkorablin.ru/project/Default.aspx?File=103")]
#else

[assembly: AssemblyDescription("SQL Settings provider plugin")]
[assembly: AssemblyCopyright("Copyright © Danila Korablin 2012-2025")]
#endif