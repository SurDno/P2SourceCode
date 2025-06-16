using UnityEngine;

[CreateAssetMenu(menuName = "Data/Load Window Storable Data")]
public class LoadWindowStorableData : ScriptableObject
{
  [SerializeField]
  private LoadWindowStorableDataItem[] items;

  public LoadWindowStorableDataItem[] Items => items;
}
