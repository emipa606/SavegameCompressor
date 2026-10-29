# GitHub Copilot Instructions for Savegame Compressor (Continued)

## Mod Overview and Purpose

Savegame Compressor (Continued) is a RimWorld mod that automatically compresses savegame files to make them smaller and faster to load. It disables the pretty printing of XML files and compresses them using the gzip format.

## Key Features and Systems

- **Automatic Compression**: Saves games are automatically compressed upon saving, reducing file size and improving load times.
- **Settings**: The mod provides a settings menu where users can enable or disable compression.
- **Harmony Patches**: The mod uses Harmony to patch various parts of the RimWorld codebase to ensure compatibility and proper functioning of the compression feature.

## Coding Patterns and Conventions

- **Harmony Patches**: All Harmony patches are located in the `HarmonyPatches` folder and follow a consistent naming convention (`<Class>_<Method>.cs`).
- **Settings Management**: The mod uses a custom settings class (`CompressorMod.cs`) to manage user settings and display the settings window.
- **File Handling**: The mod includes methods to handle file compression and decompression using gzip.

## XML Integration

The mod uses XML files for metadata and configuration. The `About.xml` file contains information about the mod, including its dependencies.

### Example XML (About.xml)

xml
<About>
  <Title>Savegame Compressor (Continued)</Title>
  <Author>Revolus</Author>
  <PackageId>Mlie.SavegameCompressor</PackageId>
  <Description>Automatically compress savegame files to reduce size and improve load times.</Description>
  <Dependencies>
    <Dependency>brrainz.harmony</Dependency>
  </Dependencies>
</About>


## Harmony Patching

The mod uses Harmony to patch various methods in the RimWorld codebase. Here are some examples of the patches used:

### Example Harmony Patch (ScribeLoader_InitLoading.cs)

csharp
using HarmonyLib;
using Verse;

namespace Revolus.Compressor.HarmonyPatches
{
    [HarmonyPatch(typeof(ScribeLoader), "InitLoading")]
    public static class ScribeLoader_InitLoading
    {
        public static bool Prefix(ScribeMode mode)
        {
            // Custom patch logic here
            return true;
        }
    }
}


### Example Harmony Patch (DataExposeUtility_AddLineBreaksToLongString.cs)

csharp
using HarmonyLib;
using Verse;

namespace Revolus.Compressor.HarmonyPatches
{
    [HarmonyPatch(typeof(DataExposeUtility), "AddLineBreaksToLongString")]
    public static class DataExposeUtility_AddLineBreaksToLongString
    {
        public static bool Prefix(ref string str)
        {
            // Custom patch logic here
            return true;
        }
    }
}


## Suggestions for Copilot

1. **Naming Conventions**: Use consistent naming conventions for Harmony patches and other classes and methods.
2. **Documentation**: Add XML comments to methods and classes to improve code readability and maintainability.
3. **Error Handling**: Ensure that all file operations and Harmony patches include proper error handling.
4. **Testing**: Write unit tests for key functionality, such as file compression and decompression.
5. **Dependency Management**: Ensure that all dependencies are correctly specified in the `About.xml` file and that the mod checks for them at runtime.

By following these guidelines and using Copilot effectively, you can contribute to the development of Savegame Compressor (Continued) and help improve the mod for all RimWorld players.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

