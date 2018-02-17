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
GitVersion(new GitVersionSettings { OutputType = GitVersionOutput.BuildServer });
GitVersion gitVersion;

// Define directories.
var buildDir = Directory("./bin");

var username = "";
var password = "";

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Setup(context =>
{
    gitVersion = GitVersion(new GitVersionSettings { OutputType = GitVersionOutput.Json });
    Information("Informational Version: {0}", gitVersion.InformationalVersion);
    Information("SemVer Version: {0}", gitVersion.SemVer);

    Information(Figlet("Fluent.Ribbon"));
});

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
});

Task("Restore")
    //.IsDependentOn("Clean")
    .Does(() =>
{
    // Is temporarily needed for sdk-csproj. Otherwise some files are missing.
    DotNetCoreRestore();

    PaketRestore();
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
      MSBuild("./Fluent.Ribbon.sln", settings => 
        settings
            .SetMaxCpuCount(0)
            .SetConfiguration(configuration)
            // .SetVerbosity(verbosity)
            .SetVerbosity(Verbosity.Minimal)
            );
    }
});

Task("EnsurePublishDirectory")
    .Does(() =>
{
    EnsureDirectoryExists("./Publish");
});

Task("Pack")    
    .IsDependentOn("EnsurePublishDirectory")
    .Does(() =>
{	
	PaketPack("./Publish", new PaketPackSettings { Version = gitVersion.NuGetVersion });
});

Task("Zip")    
    .IsDependentOn("EnsurePublishDirectory")
    .Does(() =>
{
    Zip("./bin/Fluent.Ribbon.Showcase/" + configuration, "./Publish/Fluent.Ribbon.Showcase-v" + gitVersion.NuGetVersion + ".zip");
});

Task("Tests")    
    .Does(() =>
{
    NUnit3(
        "./bin/Fluent.Ribbon.Tests/**/" + configuration + "/**/*.Tests.dll",
        new NUnit3Settings { ToolPath = "./packages/cake/NUnit.ConsoleRunner/tools/nunit3-console.exe" }
    );
});

Task("GetCredentials")
    .Does(() =>
{
    username = EnvironmentVariable("GITHUB_USERNAME_FLUENT_RIBBON");
    password = EnvironmentVariable("GITHUB_PASSWORD_FLUENT_RIBBON");
});

Task("CreateReleaseNotes")    
    .IsDependentOn("EnsurePublishDirectory")
    //.IsDependentOn("GetCredentials")
    .Does(() =>
{
    // GitReleaseManagerExport(username, password, "fluentribbon", "Fluent.Ribbon", "./Publish/releasenotes.md", new GitReleaseManagerExportSettings {
    //     TagName         = "v6.0.0",
    //     TargetDirectory = "./Publish",
    //     LogFilePath     = "./Publish/grm.log"
    // });
    // GitReleaseManagerCreate(username, password, "fluentribbon", "Fluent.Ribbon", new GitReleaseManagerCreateSettings {
    //     Milestone         = gitVersion.MajorMinorPatch,
    //     Name              = gitVersion.SemVer,
    //     Prerelease        = false,
    //     TargetCommitish   = "master",
    //     WorkingDirectory  = "./"
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
