// Decompiled with JetBrains decompiler
// Type: AssetDatabases.AssetDatabaseService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Utility;
using UnityEngine;

#nullable disable
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
