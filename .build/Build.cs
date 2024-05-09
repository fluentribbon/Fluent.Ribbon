using GlobExpressions;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using System;
using System.IO;
using System.Linq;
using System.Text;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

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

        Serilog.Log.Information("IsLocalBuild           : {0}", IsLocalBuild.ToString());

        Serilog.Log.Information("Informational   Version: {0}", InformationalVersion);
        Serilog.Log.Information("SemVer          Version: {0}", SemVer);
        Serilog.Log.Information("AssemblySemVer  Version: {0}", AssemblySemVer);
        Serilog.Log.Information("MajorMinorPatch Version: {0}", MajorMinorPatch);
        Serilog.Log.Information("NuGet           Version: {0}", NuGetVersion);
    }
    
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;
    
    [Solution(GenerateProjects = true)] readonly Solution Solution = null!;
    
    [GitVersion(Framework = "net6.0", NoFetch = true)] readonly GitVersion? GitVersion;
    
    string AssemblySemVer => GitVersion?.AssemblySemVer ?? "1.0.0";
    string SemVer => GitVersion?.SemVer ?? "1.0.0";
    string InformationalVersion => GitVersion?.InformationalVersion ?? "1.0.0";
    string NuGetVersion => GitVersion?.NuGetVersion ?? "1.0.0";
    string MajorMinorPatch => GitVersion?.MajorMinorPatch ?? "1.0.0";
    string AssemblySemFileVer => GitVersion?.AssemblySemFileVer ?? "1.0.0";

    // Define directories.
    AbsolutePath FluentRibbonDirectory => RootDirectory / "Fluent.Ribbon";

    AbsolutePath BuildBinDirectory => RootDirectory / "bin";

    AbsolutePath ReferenceDataDir => RootDirectory / "ReferenceData";

    [Parameter]
    readonly AbsolutePath ArtifactsDirectory = RootDirectory / "artifacts";

    AbsolutePath TestResultsDir => RootDirectory / "TestResults";

    Target CleanOutput => _ => _
        .Executes(() =>
        {
            ArtifactsDirectory.CreateOrCleanDirectory();
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetToolRestore();
            
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target XamlStyler => _ => _
        .DependsOn(Restore)
        .Executes(() =>
    {
        DotNet($"xstyler --recursive --directory \"{RootDirectory}\"");
    });

    Target Compile => _ => _
        .DependsOn(Restore)
        .DependsOn(XamlStyler)
        .Executes(() =>
    {
        DotNetBuild(s => s
            .SetProjectFile(Solution)
            .SetConfiguration(Configuration)
            .SetAssemblyVersion(AssemblySemVer)
            .SetFileVersion(AssemblySemVer)
            .SetInformationalVersion(InformationalVersion)

            .SetVerbosity(DotNetVerbosity.minimal));
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
            .SetVersion(NuGetVersion)
            .SetAssemblyVersion(AssemblySemVer)
            .SetFileVersion(AssemblySemFileVer)
            .SetInformationalVersion(InformationalVersion));
        
        (BuildBinDirectory / "Fluent.Ribbon" / Configuration).CompressTo(ArtifactsDirectory / $"Fluent.Ribbon-v{NuGetVersion}.zip");
        (BuildBinDirectory / "Fluent.Ribbon.Showcase" / Configuration).CompressTo(ArtifactsDirectory / $"Fluent.Ribbon.Showcase-v{NuGetVersion}.zip");
    });

    Target Test => _ => _
        .After(Compile)
        .Executes(() =>
    {
        TestResultsDir.CreateOrCleanDirectory();

        DotNetTest(_ => _
            .SetConfiguration(Configuration)
            .SetProjectFile(Solution.Fluent_Ribbon_Tests)
            .EnableNoBuild()
            .EnableNoRestore()
            .AddLoggers("trx")
            .SetResultsDirectory(TestResultsDir)
            .SetVerbosity(DotNetVerbosity.normal));
    });

    Target ResourceKeys => _ => _
        .After(Compile)
        .Executes(() =>
        {
            var resourceKeys = new ResourceKeys(FluentRibbonDirectory / "Themes" / "Styles.xaml", FluentRibbonDirectory / "Themes" / "Themes" / "Theme.Template.xaml");

            Serilog.Log.Information($"Peeked keys  : {resourceKeys.PeekedKeys.Count}");

            Serilog.Log.Information($"Filtered keys: {resourceKeys.ElementsWithNonTypeKeys.Count}");

            if (resourceKeys.CheckKeys() is false)
            {
                Assert.Fail("Wrong resource keys found.");
            }

            var vNextResourceKeys = resourceKeys.ElementsWithNonTypeKeys
                .Select(x => x.Key)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            Serilog.Log.Information($"Distinct keys: {vNextResourceKeys.Count}");

            File.WriteAllLines(ReferenceDataDir / "vNextResourceKeys.txt", vNextResourceKeys, Encoding.UTF8);

            var vCurrentResourceKeys = File.ReadAllLines(ReferenceDataDir / "vCurrentResourceKeys.txt", Encoding.UTF8);

            var removedKeys = vCurrentResourceKeys.Except(vNextResourceKeys);
            var addedKeys = vNextResourceKeys.Except(vCurrentResourceKeys);

            Serilog.Log.Information("|Old|New|");
            Serilog.Log.Information("|---|---|");

            foreach (var removedKey in removedKeys)
            {
                Serilog.Log.Information($"|{removedKey}|---|");
            }
    
            foreach (var addedKey in addedKeys)
            {
                Serilog.Log.Information($"|---|{addedKey}|");
            }
        });

    // ReSharper disable once UnusedMember.Local
    // ReSharper disable once InconsistentNaming
    Target CI => _ => _
        .DependsOn(Compile)
        .DependsOn(Test)
        .DependsOn(Pack);
}
