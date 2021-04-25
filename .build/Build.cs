using System;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.CI;
using static Nuke.Common.IO.CompressionTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.IO.FileSystemTasks;

class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Compile);
    
    protected override void OnBuildInitialized()
    {
        base.OnBuildInitialized();

        ProcessTasks.DefaultLogInvocation = true;
        ProcessTasks.DefaultLogOutput = true;

        if (GitVersion is null
            && IsLocalBuild == false)
        {
            throw new Exception("Could not initialize GitVersion.");
        }

        Console.WriteLine("IsLocalBuild           : {0}", IsLocalBuild.ToString());

        Console.WriteLine("Informational   Version: {0}", InformationalVersion);
        Console.WriteLine("SemVer          Version: {0}", SemVer);
        Console.WriteLine("AssemblySemVer  Version: {0}", AssemblySemVer);
        Console.WriteLine("MajorMinorPatch Version: {0}", MajorMinorPatch);
        Console.WriteLine("NuGet           Version: {0}", NuGetVersion);
    }
    
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;
    
    [Solution(GenerateProjects = true)] readonly Solution Solution = null!;
    
    [GitVersion(Framework = "netcoreapp3.1")] readonly GitVersion? GitVersion;
    
    string AssemblySemVer => GitVersion?.AssemblySemVer ?? "1.0.0";
    string SemVer => GitVersion?.SemVer ?? "1.0.0";
    string InformationalVersion => GitVersion?.InformationalVersion ?? "1.0.0";
    string NuGetVersion => GitVersion?.NuGetVersion ?? "1.0.0";
    string MajorMinorPatch => GitVersion?.MajorMinorPatch ?? "1.0.0";
    string AssemblySemFileVer => GitVersion?.AssemblySemFileVer ?? "1.0.0";
    
    // Define directories.
    AbsolutePath BuildBinDirectory => RootDirectory / "bin";
    
    AbsolutePath TestResultsDir => RootDirectory / "TestResults";
    
    [Parameter]
    readonly AbsolutePath ArtifactsDirectory = RootDirectory / "artifacts";

    Target CleanOutput => _ => _
        .Executes(() =>
        {
            EnsureCleanDirectory(ArtifactsDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
    {
        DotNetBuild(s => s
            .SetProjectFile(Solution)
            .SetConfiguration(Configuration)
            .SetAssemblyVersion(AssemblySemVer)
            .SetFileVersion(AssemblySemVer)
            .SetInformationalVersion(InformationalVersion)

            .SetVerbosity(DotNetVerbosity.Minimal));
    });

    Target Pack => _ => _
        .DependsOn(CleanOutput)
        .DependsOn(Compile)
        .Produces(ArtifactsDirectory / "*.nupkg", ArtifactsDirectory / "*.zip")
        .Executes(() =>
    {
        DotNetPack(_ => _
            .SetProject(Solution.Fluent_Ribbon)
            .SetConfiguration(Configuration)
            .SetProperty("PackageOutputPath", ArtifactsDirectory)
            .When(GitVersion is not null, x => x
                .SetProperty("RepositoryBranch", GitVersion?.BranchName)
                .SetProperty("RepositoryCommit", GitVersion?.Sha))
            .SetProperty("Version", NuGetVersion)
            .SetProperty("AssemblyVersion", AssemblySemVer)
            .SetProperty("FileVersion", AssemblySemFileVer)
            .SetProperty("InformationalVersion", InformationalVersion));
        
        Compress(BuildBinDirectory / "Fluent.Ribbon" / Configuration, ArtifactsDirectory / $"Fluent.Ribbon-v{NuGetVersion}.zip");
        Compress(BuildBinDirectory / "Fluent.Ribbon.Showcase" / Configuration, ArtifactsDirectory / $"Fluent.Ribbon.Showcase-v{NuGetVersion}.zip");
    });

    Target Test => _ => _
        .After(Compile)
        .Executes(() =>
    {
        EnsureCleanDirectory(TestResultsDir);

        DotNetTest(_ => _
            .SetConfiguration(Configuration)
            .SetProjectFile(Solution.Fluent_Ribbon_Tests)
            .SetNoBuild(true)
            .SetNoRestore(true)
            .SetLogger("trx")
            .SetResultsDirectory(TestResultsDir)
            .SetVerbosity(DotNetVerbosity.Normal));
    });

    Target CI => _ => _
        .DependsOn(Compile)
        .DependsOn(Test)
        .DependsOn(Pack);
}
