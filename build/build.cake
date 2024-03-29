#tool "dotnet:?package=coverlet.console"
#addin "nuget:?package=Cake.Coverlet"

using System.Linq;
using System.Globalization;
using System.Text.RegularExpressions;

// Target - The task you want to start. Runs the Default task if not specified.
var target = Argument("Target", "Default");

// Configuration - The build configuration (Debug/Release) to use.
// 1. If command line parameter parameter passed, use that.
// 2. Otherwise if an Environment variable exists, use that.
var configuration = 
    HasArgument("Configuration") 
        ? Argument<string>("Configuration") 
        : EnvironmentVariable("Configuration") ?? "Release";

// The build number to use in the version number of the built NuGet packages.
// There are multiple ways this value can be passed, this is a common pattern.
var buildNumber =
    HasArgument("BuildNumber") ? Argument<int>("BuildNumber") :
    EnvironmentVariable("BuildNumber") != null ? int.Parse(EnvironmentVariable("BuildNumber")) : 
    AppVeyor.IsRunningOnAppVeyor ? AppVeyor.Environment.Build.Number :
    TravisCI.IsRunningOnTravisCI ? TravisCI.Environment.Build.BuildNumber :
    GitHubActions.IsRunningOnGitHubActions ? int.Parse(EnvironmentVariable("GITHUB_RUN_NUMBER")) :
    0;

// Branch or tag name used in version suffix and packages info
var refName = 
    AppVeyor.IsRunningOnAppVeyor ? (AppVeyor.Environment.Repository.Tag.IsTag ? AppVeyor.Environment.Repository.Tag.Name : AppVeyor.Environment.Repository.Branch) :
    TravisCI.IsRunningOnTravisCI ? TravisCI.Environment.Build.Branch : 
    GitHubActions.IsRunningOnGitHubActions ? EnvironmentVariable("GITHUB_REF_NAME") :
    (string)null;
// The type of ref that triggered build. Valid values are branch or tag
var refType = 
    AppVeyor.IsRunningOnAppVeyor ? (AppVeyor.Environment.Repository.Tag.IsTag ? "tag" : "branch") :
    TravisCI.IsRunningOnTravisCI ? (string.IsNullOrWhiteSpace(TravisCI.Environment.Build.Tag) == false ? "tag" : "branch") : 
    GitHubActions.IsRunningOnGitHubActions ? EnvironmentVariable("GITHUB_REF_TYPE") :
    (string)null;
// Commit Id for packages info
var commitId =
    AppVeyor.IsRunningOnAppVeyor ? AppVeyor.Environment.Repository.Commit.Id :
    TravisCI.IsRunningOnTravisCI ? TravisCI.Environment.Repository.Commit : 
    GitHubActions.IsRunningOnGitHubActions ? EnvironmentVariable("GITHUB_SHA") :
    (string)null;

// Text suffix of the package version
string versionSuffix = null;
switch (refType)
{
    case "tag":
        var match = Regex.Match(refName, "v\\d+\\.\\d+\\.\\d+\\-?(.*)", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        if(match.Success == false)
            versionSuffix = refName;
        else if(string.IsNullOrWhiteSpace(match.Groups[1].Value) == false)
            versionSuffix = match.Groups[1].Value;
        break;
    case "branch":
        versionSuffix = $"{refName.Replace("/", "-", true, System.Globalization.CultureInfo.InvariantCulture)}-build{buildNumber:00000}";
        break;
}

// A directory path to an Artifacts directory.
var artifactsDirectory = MakeAbsolute(Directory("./artifacts"));

// Directories with tests template
var testsPathTemplate = "../test/**/*.Tests.csproj";
 
// Deletes the contents of the Artifacts folder if it should contain anything from a previous build.
Task("Clean")
    .Does(() =>
    {
        CleanDirectory(artifactsDirectory);
    });
 
// Find all csproj projects and build them using the build configuration specified as an argument.
 Task("Build")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        var settings =  new DotNetBuildSettings
                {
                    Configuration = configuration,
                    VersionSuffix = versionSuffix,
                    ArgumentCustomization = args => args.Append("--configfile ./NuGet.config")
                };

        if(buildNumber != 0)
            settings.MSBuildSettings = new DotNetMSBuildSettings {Properties = { {"Build", new[] {buildNumber.ToString()}}}};

        DotNetBuild("..", settings);
    });

// Look under a 'Tests' folder and run dotnet test against all of those projects.
// Then drop the XML test results file in the Artifacts folder at the root.
Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
    {
        var projects = GetFiles(testsPathTemplate);
        var settings = new DotNetCoreTestSettings
        {
            Configuration = configuration,
            NoRestore = true,
            NoBuild = true,
        };

        foreach(var project in projects)
            DotNetCoreTest(project.FullPath, settings);
    });

// Look under a 'test' folder and calculate tests against all of those projects.
// Then drop the XML test results file in the artifacts folder at the root.
Task("CalculateCoverage")
    .IsDependentOn("Build")
    .Does(() =>
    {
        var projects = GetFiles(testsPathTemplate).ToArray();
        var temporaryCoverageFile = artifactsDirectory.CombineWithFilePath("coverage.json");

        var coverletsettings = new CoverletSettings 
        {
            CollectCoverage = true,
            CoverletOutputDirectory = artifactsDirectory,
            CoverletOutputName = temporaryCoverageFile.GetFilenameWithoutExtension().ToString(),
            MergeWithFile = temporaryCoverageFile,
            Include = new List<string> {"[*]*"},
            Exclude = new List<string> 
            {
                "[xunit*]*",
                "[*.Tests]*"
            }
        };

        for(var i = 0; i < projects.Length; i++)
        {
            var project = projects[i];
            var projectName = project.GetFilenameWithoutExtension();
            var projectAbsolutePath = project.GetDirectory();
            var projectDll = GetFiles($"{projectAbsolutePath}/bin/Debug/*/*{projectName}.dll").First();

            if(i == projects.Length - 1)
                coverletsettings.CoverletOutputFormat = CoverletOutputFormat.opencover;

            Coverlet(projectDll, project, coverletsettings);
        }
    });

// Run dotnet pack to produce NuGet packages from our projects. Versions the package
// using the build number argument on the script which is used as the revision number 
// (Last number in 1.0.0.0). The packages are dropped in the Artifacts directory.
Task("Pack")
    .IsDependentOn("Test")
    .Does(() =>
    {
        var settings = new DotNetPackSettings
                {
                    Configuration = configuration,
                    NoRestore = true,
                    NoBuild = true,
                    OutputDirectory = artifactsDirectory,
                    IncludeSymbols = true,
                    VersionSuffix = versionSuffix,
                    MSBuildSettings = new DotNetMSBuildSettings {Properties = { {"SymbolPackageFormat", new[] {"snupkg"} } }}
                };
				
        if(string.IsNullOrWhiteSpace(versionSuffix) == false)
        {
            settings.MSBuildSettings.Properties["RepositoryRefName"] = new[] {refName};
            settings.MSBuildSettings.Properties["RepositoryCommit"] = new[] {commitId};
        }

        DotNetPack("..", settings);
    });
 
// The default task to run if none is explicitly specified. In this case, we want
// to run everything starting from Clean, all the way up to Test.
Task("Default")
    .IsDependentOn("Test");
 
// Executes the task specified in the target argument.
RunTarget(target);