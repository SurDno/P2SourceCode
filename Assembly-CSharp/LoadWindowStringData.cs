[CreateAssetMenu(menuName = "Data/Load Window String Data")]
public class LoadWindowStringData : ScriptableObject
{
  [SerializeField]
  private LoadWindowStringDataItem[] items;

  public LoadWindowStringDataItem[] Items => items;
}
