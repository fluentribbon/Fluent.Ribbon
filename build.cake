//////////////////////////////////////////////////////////////////////
// TOOLS / ADDINS
//////////////////////////////////////////////////////////////////////

#tool paket:?package=GitVersion.CommandLine
#tool paket:?package=NUnit.ConsoleRunner
#addin paket:?package=Cake.Figlet
#addin paket:?package=Cake.Paket

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
if (string.IsNullOrWhiteSpace(target))
{
    target = "Default";
}

var configuration = Argument("configuration", "Release");
if (string.IsNullOrWhiteSpace(configuration))
{
    configuration = "Release";
}

var verbosity = Argument("verbosity", Verbosity.Normal);
if (string.IsNullOrWhiteSpace(configuration))
{
    verbosity = Verbosity.Normal;
}

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Set build version
if (BuildSystem.IsLocalBuild == false)
{
    GitVersion(new GitVersionSettings { OutputType = GitVersionOutput.BuildServer });
}
GitVersion gitVersion = GitVersion(new GitVersionSettings { OutputType = GitVersionOutput.Json });

var local = BuildSystem.IsLocalBuild;
var isPullRequest = AppVeyor.Environment.PullRequest.IsPullRequest;
var isDevelopBranch = StringComparer.OrdinalIgnoreCase.Equals("develop", AppVeyor.Environment.Repository.Branch);
var isReleaseBranch = StringComparer.OrdinalIgnoreCase.Equals("master", AppVeyor.Environment.Repository.Branch);
var isTagged = AppVeyor.Environment.Repository.Tag.IsTag;

// Define directories.
var buildDir = Directory("./bin");
var publishDir = Directory("./Publish");
var solutionFile = File("./Fluent.Ribbon.sln");

var username = "";
var token = "";

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Setup(context =>
{
    Information(Figlet("Fluent.Ribbon"));

    Information("Informational Version  : {0}", gitVersion.InformationalVersion);
    Information("SemVer Version         : {0}", gitVersion.SemVer);
    Information("AssemblySemVer Version : {0}", gitVersion.AssemblySemVer);
    Information("MajorMinorPatch Version: {0}", gitVersion.MajorMinorPatch);
    Information("NuGet Version          : {0}", gitVersion.NuGetVersion);
    Information("IsLocalBuild           : {0}", local);
});

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
    CleanDirectories("./**/obj");
});

Task("Restore")
    //.IsDependentOn("Clean")
    .Does(() =>
{
    MSBuild(solutionFile, settings => 
        settings
            .SetConfiguration(configuration)
            .WithTarget("restore")
            );
});

Task("Update-SolutionInfo")
    .Does(() =>
{
	var solutionInfo = "./Shared/GlobalAssemblyInfo.cs";
	GitVersion(new GitVersionSettings { UpdateAssemblyInfo = true, UpdateAssemblyInfoFilePath = solutionInfo});
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{    
    if(IsRunningOnWindows())
    {
      // Use MSBuild
      MSBuild(solutionFile, settings => 
        settings
            .SetMaxCpuCount(0)
            .SetConfiguration(configuration)
            // .SetVerbosity(verbosity)
            .SetVerbosity(Verbosity.Minimal)
            .WithProperty("AssemblyVersion", gitVersion.AssemblySemVer)
            .WithProperty("FileVersion", gitVersion.MajorMinorPatch)
            .WithProperty("InformationalVersion", gitVersion.InformationalVersion)
            );
    }
});

Task("EnsurePublishDirectory")
    .Does(() =>
{
    EnsureDirectoryExists(publishDir);
});

Task("Pack")    
    .IsDependentOn("EnsurePublishDirectory")
    .Does(() =>
{	
	PaketPack(publishDir, new PaketPackSettings { Version = isReleaseBranch ? gitVersion.MajorMinorPatch : gitVersion.NuGetVersion, BuildConfig = configuration });
});

Task("Zip")    
    .IsDependentOn("EnsurePublishDirectory")
    .Does(() =>
{
    Zip(buildDir.ToString() + "/Fluent.Ribbon/" + configuration, publishDir.ToString() + "/Fluent.Ribbon-v" + gitVersion.NuGetVersion + ".zip");
    Zip(buildDir.ToString() + "/Fluent.Ribbon.Showcase/" + configuration, publishDir.ToString() + "/Fluent.Ribbon.Showcase-v" + gitVersion.NuGetVersion + ".zip");
});

Task("Tests")    
    .Does(() =>
{
    NUnit3(
        buildDir.ToString() + "/Fluent.Ribbon.Tests/**/" + configuration + "/**/*.Tests.dll",
        new NUnit3Settings { ToolPath = "./packages/cake/NUnit.ConsoleRunner/tools/nunit3-console.exe" }
    );
});

Task("GetCredentials")
    .Does(() => 
{
    // username = EnvironmentVariable("GITHUB_USERNAME_FLUENT_RIBBON");
    // if (string.IsNullOrEmpty(username))
    // {
    //     throw new Exception("The GITHUB_USERNAME_FLUENT_RIBBON environment variable is not defined.");
    // }

    // token = EnvironmentVariable("GITHUB_TOKEN_FLUENT_RIBBON");
    // if (string.IsNullOrEmpty(token))
    // {
    //     throw new Exception("The GITHUB_TOKEN_FLUENT_RIBBON environment variable is not defined.");
    // }
});

Task("CreateReleaseNotes")
    .WithCriteria(() => !isTagged)
    .IsDependentOn("EnsurePublishDirectory")
    .IsDependentOn("GetCredentials")
    .Does(() =>
{
    // GitReleaseManagerCreate(username, token, "fluentribbon", "Fluent.Ribbon", new GitReleaseManagerCreateSettings {
    //     Milestone         = gitVersion.MajorMinorPatch,
    //     Name              = "Fluent.Ribbon" + gitVersion.SemVer,
    //     Prerelease        = false,
    //     TargetCommitish   = "master",
    //     WorkingDirectory  = "./"
    // });
});

Task("ExportReleaseNotes")
    .IsDependentOn("EnsurePublishDirectory")
    .IsDependentOn("GetCredentials")
    .Does(() =>
{
    // GitReleaseManagerExport(username, token, "fluentribbon", "Fluent.Ribbon", publishDir.ToString() + "/releasenotes.md", new GitReleaseManagerExportSettings {
    //     // TagName         = gitVersion.SemVer,
    //     TagName         = "v6.1.0",
    //     TargetDirectory = publishDir,
    //     LogFilePath     = publishDir.ToString() + "/grm.log"
    // });
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Build");

Task("appveyor")
    .IsDependentOn("Update-SolutionInfo")
    .IsDependentOn("Build")
    .IsDependentOn("Tests")
    .IsDependentOn("Pack")
    .IsDependentOn("Zip");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
