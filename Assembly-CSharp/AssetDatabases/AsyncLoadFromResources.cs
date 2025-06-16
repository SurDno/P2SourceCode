using UnityEngine;

namespace AssetDatabases
{
  public class AsyncLoadFromResources : IAsyncLoad
  {
    private ResourceRequest operation;

    public AsyncLoadFromResources(ResourceRequest operation) => this.operation = operation;

    public object Asset => (object) this.operation.asset;

    public bool IsDone => this.operation.isDone;
  }
}
