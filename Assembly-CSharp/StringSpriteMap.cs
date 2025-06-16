[CreateAssetMenu(menuName = "Data/String Sprite Map")]
public class StringSpriteMap : ScriptableObject
{
  [SerializeField]
  private StringSpritePair[] map;

  public Sprite GetValue(string key)
  {
    if (!string.IsNullOrEmpty(key))
    {
      for (int index = 0; index < map.Length; ++index)
      {
        if (key == map[index].Key)
          return map[index].Value;
      }
      for (int index = 0; index < map.Length; ++index)
      {
        if (key.Replace(" ", "").Contains(map[index].Key.Replace(" ", "")))
          return map[index].Value;
      }
    }
    return (Sprite) null;
  }
}
