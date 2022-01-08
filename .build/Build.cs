using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
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
    
    [GitVersion(Framework = "netcoreapp3.1", NoFetch = true)] readonly GitVersion? GitVersion;
    
    string AssemblySemVer => GitVersion?.AssemblySemVer ?? "1.0.0";
    string SemVer => GitVersion?.SemVer ?? "1.0.0";
    string InformationalVersion => GitVersion?.InformationalVersion ?? "1.0.0";
    string NuGetVersion => GitVersion?.NuGetVersion ?? "1.0.0";
    string MajorMinorPatch => GitVersion?.MajorMinorPatch ?? "1.0.0";
    string AssemblySemFileVer => GitVersion?.AssemblySemFileVer ?? "1.0.0";
    
    // Define directories.
    AbsolutePath BuildBinDirectory => RootDirectory / "bin";

    AbsolutePath ReferenceDataDir => RootDirectory / "ReferenceData";

    [Parameter]
    readonly AbsolutePath ArtifactsDirectory = RootDirectory / "artifacts";

    AbsolutePath TestResultsDir => RootDirectory / "TestResults";

    Target CleanOutput => _ => _
        .Executes(() =>
        {
            EnsureCleanDirectory(ArtifactsDirectory);
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
        DotNet($"xstyler -r -d \"{RootDirectory}\"");
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
            .SetVersion(NuGetVersion)
            .SetAssemblyVersion(AssemblySemVer)
            .SetFileVersion(AssemblySemFileVer)
            .SetInformationalVersion(InformationalVersion));
        
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
            .AddLoggers("trx")
            .SetResultsDirectory(TestResultsDir)
            .SetVerbosity(DotNetVerbosity.Normal));
    });

    Target ResourceKeys => _ => _
        .After(Compile)
        .Executes(() =>
    {
        var xmlPeekElements = XmlTasks.XmlPeekElements(RootDirectory / @"Fluent.Ribbon" / "Themes" / "Styles.xaml", "//*[@x:Key]", ("x", "http://schemas.microsoft.com/winfx/2006/xaml"))
            .Concat(XmlTasks.XmlPeekElements(RootDirectory / @"Fluent.Ribbon" / "Themes" / "Themes" / "Theme.Template.xaml", "//*[@x:Key]", ("x", "http://schemas.microsoft.com/winfx/2006/xaml")))
            .ToList();
        Console.WriteLine($"Peeked: {xmlPeekElements.Count}");

        var xKey = XName.Get("Key", "http://schemas.microsoft.com/winfx/2006/xaml");

        var vNextResourceKeys = xmlPeekElements
            .Where(x => x.HasAttributes && x.Attribute(xKey) is not null)
            .Select(x => x.Attribute(xKey)!.Value)
            // Exclude type-keyed styles like x:Key="{x:Type Button}" etc.
            .Where(x => x.StartsWith("{") == false)
            .Distinct()
            .OrderBy(x => x)
            .ToList();
    
        Console.WriteLine($"Distinct keys: {vNextResourceKeys.Count}");
    
        File.WriteAllLines(ReferenceDataDir / "vNextResourceKeys.txt", vNextResourceKeys, Encoding.UTF8);
    
        var vCurrentResourceKeys = File.ReadAllLines(ReferenceDataDir / "vCurrentResourceKeys.txt", Encoding.UTF8);

        var removedKeys = vCurrentResourceKeys.Except(vNextResourceKeys);
        var addedKeys = vNextResourceKeys.Except(vCurrentResourceKeys);
    
        Console.WriteLine("|Old|New|");
        Console.WriteLine("|---|---|");

        foreach (var removedKey in removedKeys)
        {
            Console.WriteLine($"|{removedKey}|---|");
        }
    
        foreach (var addedKey in addedKeys)
        {
            Console.WriteLine($"|---|{addedKey}|");
        }
    });

    Target CI => _ => _
        .DependsOn(Compile)
        .DependsOn(Test)
        .DependsOn(Pack);
}
