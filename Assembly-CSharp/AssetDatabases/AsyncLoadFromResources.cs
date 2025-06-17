using UnityEngine;

namespace AssetDatabases
{
  public class AsyncLoadFromResources(ResourceRequest operation) : IAsyncLoad 
  {
    public object Asset => operation.asset;

    public bool IsDone => operation.isDone;
  }
}
