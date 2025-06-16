[CreateAssetMenu(menuName = "Data/Load Window Game Data")]
public class LoadWindowGameData : ScriptableObject
{
  [SerializeField]
  private LoadWindowGameDataItem[] items;

  public LoadWindowGameDataItem GetItem(string gameDataName)
  {
    for (int index = 0; index < items.Length; ++index)
    {
      LoadWindowGameDataItem windowGameDataItem = items[index];
      if (items[index].GameDataName == gameDataName)
        return windowGameDataItem;
    }
    return LoadWindowGameDataItem.Null;
  }
}
