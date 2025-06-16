using AssetDatabases;

public class AssetDatabaseData : MonoBehaviour
{
  private void Awake()
  {
    AssetDatabaseService.Instance.RegisterAssets();
    Object.Destroy((Object) this.gameObject);
  }
}
