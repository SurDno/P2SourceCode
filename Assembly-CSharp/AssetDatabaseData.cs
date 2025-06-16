using AssetDatabases;
using UnityEngine;

public class AssetDatabaseData : MonoBehaviour
{
  private void Awake()
  {
    AssetDatabaseService.Instance.RegisterAssets();
    Object.Destroy((Object) this.gameObject);
  }
}
