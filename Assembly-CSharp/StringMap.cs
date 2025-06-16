// Decompiled with JetBrains decompiler
// Type: StringMap
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[CreateAssetMenu(menuName = "Data/String Map")]
public class StringMap : ScriptableObject
{
  [SerializeField]
  private StringPair[] map;

  public string GetValue(string key)
  {
    for (int index = 0; index < this.map.Length; ++index)
    {
      if (this.map[index].Key == key)
        return this.map[index].Value;
    }
    return (string) null;
  }
}
