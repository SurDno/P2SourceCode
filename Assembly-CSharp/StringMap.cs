using UnityEngine;

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
