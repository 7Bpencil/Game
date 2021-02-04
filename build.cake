var target = Argument("target", "Build");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory($"./bin/{configuration}");
	CleanDirectory($"./code_src/App/obj");
});

Task("CopyAssets")
    .IsDependentOn("Clean")
    .Does(() =>
{
    CreateDirectory($"./bin/{configuration}/Assets");
    CopyDirectory($"./assets/", $"./bin/{configuration}/Assets");
});

Task("CopyLibs")
    .IsDependentOn("Clean")
    .Does(() =>
{
    CreateDirectory($"./bin/{configuration}/libs");
    CopyDirectory($"./libs/", $"./bin/{configuration}/libs");
});

Task("Build")
    .IsDependentOn("CopyAssets")
	.IsDependentOn("CopyLibs")
    .Does(() =>
{
    NuGetRestore("./code_src/Game.sln");
    DotNetCoreBuild("./code_src/Game.sln", new DotNetCoreBuildSettings
    {
        Configuration = configuration,
    });
});

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);