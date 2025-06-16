[CreateAssetMenu(menuName = "Data/String Map")]
public class StringMap : ScriptableObject
{
  [SerializeField]
  private StringPair[] map;

  public string GetValue(string key)
  {
    for (int index = 0; index < map.Length; ++index)
    {
      if (map[index].Key == key)
        return map[index].Value;
    }
    return null;
  }
}
