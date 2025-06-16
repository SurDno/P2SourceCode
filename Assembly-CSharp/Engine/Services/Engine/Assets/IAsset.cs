// Decompiled with JetBrains decompiler
// Type: Engine.Services.Engine.Assets.IAsset
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace Engine.Services.Engine.Assets
{
  public interface IAsset
  {
    bool IsError { get; }

    bool IsLoaded { get; }

    bool IsDisposed { get; }

    bool IsReadyToDispose { get; }

    bool IsValid { get; }

    string Path { get; }

    void Update();

    void Dispose(string reason);
  }
}
