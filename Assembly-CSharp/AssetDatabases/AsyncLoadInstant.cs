namespace AssetDatabases
{
  public class AsyncLoadInstant : IAsyncLoad
  {
    public AsyncLoadInstant(object asset) => this.Asset = asset;

    public object Asset { get; private set; }

    public bool IsDone => true;
  }
}
