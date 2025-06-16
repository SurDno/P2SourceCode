using AssetDatabases;
using UnityEngine;

public class AssetDatabaseData : MonoBehaviour
{
  private void Awake()
  {
    AssetDatabaseService.Instance.RegisterAssets();
    Destroy(gameObject);
  }
}
