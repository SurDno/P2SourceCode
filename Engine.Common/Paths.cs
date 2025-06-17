using System.Collections.Generic;

namespace Engine.Common
{
  public static class Paths
  {
    public const string Text = ".txt";
    public const string Sharp = ".cs";
    public const string Bytes = ".bytes";
    public const string Meta = ".meta";
    public const string Scene = ".unity";
    public const string Prefab = ".prefab";
    public const string Fbx = ".fbx";
    public const string Mixer = ".mixer";
    public const string Asset = ".asset";
    public const string PhysicMaterial = ".physicmaterial";
    public const string Animation = ".anim";
    public const string Xml = ".xml";
    public const string Gz = ".gz";
    public const string Ai = "_AI.asset";
    public const string Blueprint = "_Blueprint.prefab";
    public const string LipSync = ".anno";
    public const string ItemSoundGroup = "_ItemSoundGroup.asset";
    public static readonly List<string> ComplexExts = [
      "_AI.asset",
      "_Blueprint.prefab",
      "_ItemSoundGroup.asset"
    ];
    public static readonly HashSet<string> AudioExts = [
      ".aif",
      ".wav",
      ".mp3",
      ".ogg",
      ".xm",
      ".mod",
      ".it",
      ".s3m"
    ];
    public static readonly HashSet<string> TextureExts = [
      ".bmp",
      ".tif",
      ".tga",
      ".jpg",
      ".psd",
      ".png"
    ];
    public static readonly HashSet<string> ReferenceExts = [
      ".unity",
      ".prefab",
      ".fbx",
      ".bytes",
      ".mixer",
      ".physicmaterial",
      ".anim",
      ".bmp",
      ".tif",
      ".tga",
      ".jpg",
      ".psd",
      ".png",
      ".shader"
    ];
    public const string Assets = "Assets";
    public const string Resources = "/Resources/";
    public const string CraftRecipes = "Assets/Data/CraftRecipes/Resources/";
    public const string Settings = "Assets/Data/Settings/Resources/";
    public const string GeneratedScenes = "Assets/Data/Generated/Scenes/Resources/";
    public const string GeneratedEntities = "Assets/Data/Generated/Entities/Resources/";
    public const string GameResources = "Assets/Game/Resources/";
    public const string GameData = "Assets/Game/Data/";
    public const string MainScene = "Main.unity";
    public const string LoaderScene = "Loader.unity";
    public const string ProjectNameTag = "{ProjectName}";
    public const string VmLocalizations = "Data/VirtualMachine/{ProjectName}/Localizations/";
    public const string VmDataPath = "Data/VirtualMachine/{ProjectName}";
    public const string Templates = "Data/Templates/";
    public const string Database = "Data/Database/";
    public const string DataPathTag = "{DataPath}";
    public const string Saves = "{DataPath}/Saves/";
    public const string Profiles = "{DataPath}/Saves/Profiles.xml";
    public const string PlayerSettings = "{DataPath}/Settings/";
    public const string PlayerFileSettings = "{DataPath}/Settings/PlayerSettings.xml";
    public const string DefaultLocalization = "Data/Settings/DefaultLocalization.txt";
    public const string SubTitlesPrefix = "SubTitles";
    public const string VmDataVersion = "Version.xml";
    public static readonly List<string> ContentFolders = [
      "Assets/Data/Generated/Scenes/Resources/",
      "Assets/Data/Generated/Entities/Resources/",
      "Assets/Game/Resources/",
      "Assets/Game/Data/"
    ];
  }
}
