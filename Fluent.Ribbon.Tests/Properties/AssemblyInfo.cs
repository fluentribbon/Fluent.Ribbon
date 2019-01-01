using System.Reflection;

[assembly: AssemblyDescription("")]

#if !NETCOREAPP3_0
[assembly: NUnit.Framework.Apartment(System.Threading.ApartmentState.STA)]
#endif