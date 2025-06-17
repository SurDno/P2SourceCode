namespace AssetDatabases
{
  public class AsyncLoadInstant(object asset) : IAsyncLoad 
  {
    public object Asset { get; private set; } = asset;

    public bool IsDone => true;
  }
}
