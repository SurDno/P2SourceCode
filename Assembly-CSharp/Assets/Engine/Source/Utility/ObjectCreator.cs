// Decompiled with JetBrains decompiler
// Type: Assets.Engine.Source.Utility.ObjectCreator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Assets.Engine.Source.Utility
{
  public static class ObjectCreator
  {
    public static T InstantiateFromResources<T>(string path, Transform parent = null) where T : Object
    {
      return Object.Instantiate((Object) Resources.Load<T>(path), parent) as T;
    }
  }
}
