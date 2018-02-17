using System;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows;

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyCompany("https://github.com/fluentribbon/Fluent.Ribbon")]
[assembly: AssemblyProduct("Fluent.Ribbon")]
[assembly: AssemblyCopyright("Copyright © 2012 - 2018 Bastian Schmidt; Copyright © 2009 - 2012 Degtyarev Daniel, Rikker Serg")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: AssemblyVersion("7.0.0.0")]
[assembly: AssemblyFileVersion("7.0.0.0")]
[assembly: AssemblyInformationalVersion("SRC")]

[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]

[assembly: NeutralResourcesLanguage("en-US")]
[assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)]