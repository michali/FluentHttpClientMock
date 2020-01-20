var configuration = Argument("Configuration", "Release");
var version = Argument("version", "1.0.0.0-beta");
var projectName = "FluentHttpClientMock";
var solution = "./FluentHttpClientMock.sln";

// Run dotnet restore to restore all package references.
Task("Restore")
    .Does(() =>
    {
        DotNetCoreRestore();
    });

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        var msBuildSettings = new DotNetCoreMSBuildSettings();
        msBuildSettings.SetVersion(version);

        DotNetCoreBuild(solution,
           new DotNetCoreBuildSettings()
                {
                    Configuration = configuration,
                    MSBuildSettings = msBuildSettings
                });
    });

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
    {
        var projects = GetFiles($"./{projectName}.Tests/**/*.csproj");
        foreach(var project in projects)
        {
            DotNetCoreTest(
                project.FullPath,
                new DotNetCoreTestSettings()
                {
                    Configuration = configuration,
                    NoBuild = true
                });
        }
    });

Task("Package")
  .IsDependentOn("Test")
  .Does(() => 
  {
    var nuGetPackSettings = new NuGetPackSettings
    {
      OutputDirectory = "artifacts",
      IncludeReferencedProjects = true,
      Properties = new Dictionary<string, string>
      {
        { "Configuration", "Release" }
      },
      Version = version
    };

    MSBuild($"./{projectName}/{projectName}.csproj", new MSBuildSettings().SetConfiguration("Release"));
    NuGetPack($"./{projectName}/{projectName}.csproj", nuGetPackSettings);
  });

RunTarget("Package");
