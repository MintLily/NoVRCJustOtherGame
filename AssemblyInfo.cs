using System.Reflection;
using MelonLoader;

[assembly: AssemblyTitle(NoVRCJustNeosVR.BuildInfo.Description)]
[assembly: AssemblyDescription(NoVRCJustNeosVR.BuildInfo.Description)]
[assembly: AssemblyCompany(NoVRCJustNeosVR.BuildInfo.Company)]
[assembly: AssemblyProduct(NoVRCJustNeosVR.BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + NoVRCJustNeosVR.BuildInfo.Author)]
[assembly: AssemblyTrademark(NoVRCJustNeosVR.BuildInfo.Company)]
[assembly: AssemblyVersion(NoVRCJustNeosVR.BuildInfo.Version)]
[assembly: AssemblyFileVersion(NoVRCJustNeosVR.BuildInfo.Version)]
[assembly: MelonInfo(typeof(NoVRCJustNeosVR.NoVRCJustNeosVR),
NoVRCJustNeosVR.BuildInfo.Name,
NoVRCJustNeosVR.BuildInfo.Version,
NoVRCJustNeosVR.BuildInfo.Author,
NoVRCJustNeosVR.BuildInfo.DownloadLink)]


// Create and Setup a MelonGame to mark a Mod as Universal or Compatible with specific Games.
// If no MelonGameAttribute is found or any of the Values for any MelonGame on the Mod is null or empty it will be assumed the Mod is Universal.
// Values for MelonMame can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonGame("VRChat", "VRChat")]