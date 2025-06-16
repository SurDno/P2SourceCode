// Decompiled with JetBrains decompiler
// Type: StringSpriteMap
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[CreateAssetMenu(menuName = "Data/String Sprite Map")]
public class StringSpriteMap : ScriptableObject
{
  [SerializeField]
  private StringSpritePair[] map;

  public Sprite GetValue(string key)
  {
    if (!string.IsNullOrEmpty(key))
    {
      for (int index = 0; index < this.map.Length; ++index)
      {
        if (key == this.map[index].Key)
          return this.map[index].Value;
      }
      for (int index = 0; index < this.map.Length; ++index)
      {
        if (key.Replace(" ", "").Contains(this.map[index].Key.Replace(" ", "")))
          return this.map[index].Value;
      }
    }
    return (Sprite) null;
  }
}
