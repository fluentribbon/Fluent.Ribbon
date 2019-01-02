//////////////////////////////////////////////////////////////////////
// TOOLS / ADDINS
//////////////////////////////////////////////////////////////////////

#tool GitVersion.CommandLine&version=4.0.0
#addin Cake.Figlet&version=1.2.0

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var verbosity = Argument("verbosity", Verbosity.Normal);

var PROJECT_DIR = Context.Environment.WorkingDirectory.FullPath + "/";
var PACKAGE_DIR = Directory(Argument("artifact-dir", PROJECT_DIR + "package") + "/");

//////////////////////////////////////////////////////////////////////
// SET ERROR LEVELS
//////////////////////////////////////////////////////////////////////

var ErrorDetail = new List<string>();

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

var local = BuildSystem.IsLocalBuild;

// Set build version
if (local == false
    || verbosity == Verbosity.Verbose)
{
    GitVersion(new GitVersionSettings { OutputType = GitVersionOutput.BuildServer });
}
GitVersion gitVersion = GitVersion(new GitVersionSettings { OutputType = GitVersionOutput.Json });

var isPullRequest = AppVeyor.Environment.PullRequest.IsPullRequest;
var branchName = gitVersion.BranchName;
var isDevelopBranch = StringComparer.OrdinalIgnoreCase.Equals("develop", branchName);
var isReleaseBranch = StringComparer.OrdinalIgnoreCase.Equals("master", branchName);
var isTagged = AppVeyor.Environment.Repository.Tag.IsTag;

// Define directories.
var buildDir = Directory("./bin");
var solutionFile = File("./Fluent.Ribbon.sln");
var testResultsDir = Directory("./TestResults");

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(context =>
{
    if (!IsRunningOnWindows())
    {
        throw new NotImplementedException("Fluent.Ribbon will only build on Windows because it's not possible to target WPF and Windows Forms from UNIX.");
    }

    Information(Figlet("Fluent.Ribbon"));

    Information("Informational Version  : {0}", gitVersion.InformationalVersion);
    Information("SemVer Version         : {0}", gitVersion.SemVer);
    Information("AssemblySemVer Version : {0}", gitVersion.AssemblySemVer);
    Information("MajorMinorPatch Version: {0}", gitVersion.MajorMinorPatch);
    Information("NuGet Version          : {0}", gitVersion.NuGetVersion);
    Information("IsLocalBuild           : {0}", local);
    Information("Branch                 : {0}", branchName);
    Information("Configuration          : {0}", configuration);
});

Teardown(ctx =>
{
});

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
    CleanDirectories("./**/obj");
});

Task("Restore")
    .Does(() =>
{
    // MSBuild(solutionFile, settings => 
    //     settings
    //         .SetConfiguration(configuration)
    //         .SetVerbosity(Verbosity.Minimal)
    //         .WithRestore()
    //         );
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{    
    // Use MSBuild
    MSBuild(solutionFile, settings => 
        settings
            .SetMaxCpuCount(0)
            .SetConfiguration(configuration)
            // .SetVerbosity(verbosity)
            .SetVerbosity(Verbosity.Minimal)
            .WithRestore()
            .WithProperty("Version", isReleaseBranch ? gitVersion.MajorMinorPatch : gitVersion.NuGetVersion)
            .WithProperty("AssemblyVersion", gitVersion.AssemblySemVer)
            .WithProperty("FileVersion", gitVersion.MajorMinorPatch)
            .WithProperty("InformationalVersion", gitVersion.InformationalVersion)
            );
});

Task("EnsurePackageDirectory")
    .Does(() =>
{
    EnsureDirectoryExists(PACKAGE_DIR);
});

Task("Pack")
    .IsDependentOn("EnsurePackageDirectory")
    .Does(() =>
{
    var msBuildSettings = new MSBuildSettings {
        Verbosity = Verbosity.Minimal,
        Configuration = configuration
    };
    var project = "./Fluent.Ribbon/Fluent.Ribbon.csproj";

    MSBuild(project, msBuildSettings
      .WithTarget("pack")
      .WithProperty("PackageOutputPath", MakeAbsolute(PACKAGE_DIR).ToString())
      .WithProperty("RepositoryBranch", branchName)
      .WithProperty("RepositoryCommit", gitVersion.Sha)
      .WithProperty("Version", isReleaseBranch ? gitVersion.MajorMinorPatch : gitVersion.NuGetVersion)
      .WithProperty("AssemblyVersion", gitVersion.AssemblySemVer)
      .WithProperty("FileVersion", gitVersion.AssemblySemFileVer)
      .WithProperty("InformationalVersion", gitVersion.InformationalVersion)
    );
});

Task("Zip")    
    .IsDependentOn("EnsurePackageDirectory")
    .Does(() =>
{
    Zip(buildDir.ToString() + "/Fluent.Ribbon/" + configuration, PACKAGE_DIR.ToString() + "/Fluent.Ribbon-v" + gitVersion.NuGetVersion + ".zip");
    Zip(buildDir.ToString() + "/Fluent.Ribbon.Showcase/" + configuration, PACKAGE_DIR.ToString() + "/Fluent.Ribbon.Showcase-v" + gitVersion.NuGetVersion + ".zip");
});

Task("Test")    
    .Does(() =>
{
    CleanDirectory(testResultsDir);

    var settings = new DotNetCoreTestSettings
        {
            Configuration = configuration,
            NoBuild = true,
            NoRestore = true,
            Logger = "trx",
            ResultsDirectory = testResultsDir,
            Verbosity = DotNetCoreVerbosity.Normal
        };

    DotNetCoreTest("./Fluent.Ribbon.Tests/Fluent.Ribbon.Tests.csproj", settings);
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Build");

Task("CI")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .IsDependentOn("Pack")
    .IsDependentOn("Zip");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);