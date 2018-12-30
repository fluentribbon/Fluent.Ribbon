using System.Reflection;

[assembly: AssemblyDescription("")]

#if !NET_CORE_3_0
[assembly: NUnit.Framework.Apartment(System.Threading.ApartmentState.STA)]
#endif