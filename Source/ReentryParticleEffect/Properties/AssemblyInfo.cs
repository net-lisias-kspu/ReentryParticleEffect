using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Reentry Particle Effect /L Unleashed")]
[assembly: AssemblyDescription("Activates an unused stock particle effect for reentry, featuring a plasma trail and sparks.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(ReentryParticleEffect.LegalMamboJambo.Company)]
[assembly: AssemblyProduct(ReentryParticleEffect.LegalMamboJambo.Product)]
[assembly: AssemblyCopyright(ReentryParticleEffect.LegalMamboJambo.Copyright)]
[assembly: AssemblyTrademark(ReentryParticleEffect.LegalMamboJambo.Trademark)]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("5f534982-a697-4006-81ed-74f5a5868a10")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion(ReentryParticleEffect.Version.Number)]
[assembly: AssemblyFileVersion(ReentryParticleEffect.Version.Number)]
[assembly: KSPAssembly("ReentryParticleEffect", ReentryParticleEffect.Version.major, ReentryParticleEffect.Version.minor)]
[assembly: KSPAssemblyDependency("KSPe", 2, 4)]
[assembly: KSPAssemblyDependency("KSPe.UI", 2, 4)]
