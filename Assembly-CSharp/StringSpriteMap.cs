using UnityEngine;

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
