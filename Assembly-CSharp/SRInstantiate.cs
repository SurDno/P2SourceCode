// Decompiled with JetBrains decompiler
// Type: SRInstantiate
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public static class SRInstantiate
{
  public static T Instantiate<T>(T prefab) where T : Component => Object.Instantiate<T>(prefab);

  public static GameObject Instantiate(GameObject prefab) => Object.Instantiate<GameObject>(prefab);

  public static T Instantiate<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component
  {
    return Object.Instantiate<T>(prefab, position, rotation);
  }
}
