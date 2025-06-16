// Decompiled with JetBrains decompiler
// Type: AssetDatabases.AsyncLoadInstant
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace AssetDatabases
{
  public class AsyncLoadInstant : IAsyncLoad
  {
    public AsyncLoadInstant(object asset) => this.Asset = asset;

    public object Asset { get; private set; }

    public bool IsDone => true;
  }
}
