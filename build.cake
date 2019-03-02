//////////////////////////////////////////////////////////////////////
// TOOLS / ADDINS
//////////////////////////////////////////////////////////////////////

#tool GitVersion.CommandLine&version=4.0.0
#tool vswhere&version=2.5.2
#addin Cake.Figlet&version=1.2.0

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var verbosity = Argument("verbosity", Verbosity.Normal);

var PROJECT_DIR = Context.Environment.WorkingDirectory.FullPath + "/";
var PACKAGE_DIR = Directory(Argument("artifact-dir", PROJECT_DIR + "/bin/package") + "/");

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

var VSWhereLatestSettings = new VSWhereLatestSettings
{
    IncludePrerelease = true
};
var latestInstallationPath = VSWhereLatest(VSWhereLatestSettings);
var msBuildPath = latestInstallationPath.Combine("./MSBuild/Current/Bin");
var msBuildPathExe = msBuildPath.CombineWithFilePath("./MSBuild.exe");

if (FileExists(msBuildPathExe) == false)
{
    throw new NotImplementedException("You need at least Visual Studio 2019 to build this project.");
}

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

    Information("Informational   Version: {0}", gitVersion.InformationalVersion);
    Information("SemVer          Version: {0}", gitVersion.SemVer);
    Information("AssemblySemVer  Version: {0}", gitVersion.AssemblySemVer);
    Information("MajorMinorPatch Version: {0}", gitVersion.MajorMinorPatch);
    Information("NuGet           Version: {0}", gitVersion.NuGetVersion);
    Information("IsLocalBuild           : {0}", local);
    Information("Branch                 : {0}", branchName);
    Information("Configuration          : {0}", configuration);
    Information("MSBuildPath            : {0}", msBuildPath);
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
    //DotNetCoreRestore();
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{    
    var buildSettings = new DotNetCoreBuildSettings {
        Configuration = configuration,
        MSBuildSettings = new DotNetCoreMSBuildSettings()
            .SetMaxCpuCount(0)
            .SetConfiguration(configuration)
            .WithProperty("Version", isReleaseBranch ? gitVersion.MajorMinorPatch : gitVersion.NuGetVersion)
            .WithProperty("AssemblyVersion", gitVersion.AssemblySemVer)
            .WithProperty("FileVersion", gitVersion.MajorMinorPatch)
            .WithProperty("InformationalVersion", gitVersion.InformationalVersion)
    };

    DotNetCoreBuild(solutionFile, buildSettings);
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
    var project = "./Fluent.Ribbon/Fluent.Ribbon.csproj";

    var buildSettings = new DotNetCorePackSettings {
        Configuration = configuration,
        MSBuildSettings = new DotNetCoreMSBuildSettings()
            .SetMaxCpuCount(0)
            .SetConfiguration(configuration)
            .WithProperty("PackageOutputPath", MakeAbsolute(PACKAGE_DIR).ToString())
            .WithProperty("RepositoryBranch", branchName)
            .WithProperty("RepositoryCommit", gitVersion.Sha)
            .WithProperty("Version", isReleaseBranch ? gitVersion.MajorMinorPatch : gitVersion.NuGetVersion)
            .WithProperty("AssemblyVersion", gitVersion.AssemblySemVer)
            .WithProperty("FileVersion", gitVersion.MajorMinorPatch)
            .WithProperty("InformationalVersion", gitVersion.InformationalVersion)
    };

    DotNetCorePack(project, buildSettings);
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