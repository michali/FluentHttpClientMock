var configuration = Argument("Configuration", "Release");
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
        DotNetCoreBuild(solution,
           new DotNetCoreBuildSettings()
                {
                    Configuration = configuration
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
      }
    };

    MSBuild($"./{projectName}/{projectName}.csproj", new MSBuildSettings().SetConfiguration("Release"));
    NuGetPack($"./{projectName}/{projectName}.csproj", nuGetPackSettings);
  });

RunTarget("Package");