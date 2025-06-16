// Decompiled with JetBrains decompiler
// Type: AssetDatabases.AsyncLoadFromResources
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
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
