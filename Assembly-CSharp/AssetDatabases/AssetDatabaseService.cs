using Cofe.Utility;
using UnityEngine;

namespace AssetDatabases
{
  public static class AssetDatabaseService
  {
    private static IAssetDatabase instance;

    public static IAssetDatabase Instance
    {
      get
      {
        if (AssetDatabaseService.instance == null)
        {
          AssetDatabaseService.instance = (IAssetDatabase) new AssetDatabaseBuild();
          Debug.Log((object) ObjectInfoUtility.GetStream().Append("IAssetDatabase").Append(" : ").Append(TypeUtility.GetTypeName(AssetDatabaseService.instance.GetType())));
        }
        return AssetDatabaseService.instance;
      }
    }
  }
}
