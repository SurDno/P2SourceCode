// Decompiled with JetBrains decompiler
// Type: LoadWindowGameData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[CreateAssetMenu(menuName = "Data/Load Window Game Data")]
public class LoadWindowGameData : ScriptableObject
{
  [SerializeField]
  private LoadWindowGameDataItem[] items;

  public LoadWindowGameDataItem GetItem(string gameDataName)
  {
    for (int index = 0; index < this.items.Length; ++index)
    {
      LoadWindowGameDataItem windowGameDataItem = this.items[index];
      if (this.items[index].GameDataName == gameDataName)
        return windowGameDataItem;
    }
    return LoadWindowGameDataItem.Null;
  }
}
