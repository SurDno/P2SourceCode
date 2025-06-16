// Decompiled with JetBrains decompiler
// Type: Engine.Source.Comparers.SceneComparer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine.SceneManagement;

#nullable disable
namespace Engine.Source.Comparers
{
  public class SceneComparer : IEqualityComparer<Scene>
  {
    public static readonly SceneComparer Instance = new SceneComparer();

    public bool Equals(Scene x, Scene y) => x == y;

    public int GetHashCode(Scene obj) => obj.GetHashCode();
  }
}
