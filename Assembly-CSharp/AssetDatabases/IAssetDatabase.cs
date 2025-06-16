// Decompiled with JetBrains decompiler
// Type: AssetDatabases.IAssetDatabase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace AssetDatabases
{
  public interface IAssetDatabase
  {
    void RegisterAssets();

    IEnumerable<string> GetAllAssetPaths();

    int GetAllAssetPathsCount();

    string GetPath(Guid id);

    Guid GetId(string path);

    T Load<T>(string path) where T : UnityEngine.Object;

    UnityEngine.Object[] LoadAll(string path);

    IAsyncLoad LoadAsync<T>(string path) where T : UnityEngine.Object;

    IAsyncLoad LoadSceneAsync(string path);

    void Unload(UnityEngine.Object obj);
  }
}
