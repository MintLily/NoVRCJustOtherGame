using System.Reflection;
using MelonLoader;

[assembly: AssemblyTitle(NoVRCJustChilloutVR.BuildInfo.Description)]
[assembly: AssemblyDescription(NoVRCJustChilloutVR.BuildInfo.Description)]
[assembly: AssemblyCompany(NoVRCJustChilloutVR.BuildInfo.Company)]
[assembly: AssemblyProduct(NoVRCJustChilloutVR.BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + NoVRCJustChilloutVR.BuildInfo.Author)]
[assembly: AssemblyTrademark(NoVRCJustChilloutVR.BuildInfo.Company)]
[assembly: AssemblyVersion(NoVRCJustChilloutVR.BuildInfo.Version)]
[assembly: AssemblyFileVersion(NoVRCJustChilloutVR.BuildInfo.Version)]
[assembly: MelonInfo(typeof(NoVRCJustChilloutVR.NoVRCJustChilloutVR), NoVRCJustChilloutVR.BuildInfo.Name, NoVRCJustChilloutVR.BuildInfo.Version, NoVRCJustChilloutVR.BuildInfo.Author, NoVRCJustChilloutVR.BuildInfo.DownloadLink)]


// Create and Setup a MelonGame to mark a Mod as Universal or Compatible with specific Games.
// If no MelonGameAttribute is found or any of the Values for any MelonGame on the Mod is null or empty it will be assumed the Mod is Universal.
// Values for MelonMame can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonGame("VRChat", "VRChat")]