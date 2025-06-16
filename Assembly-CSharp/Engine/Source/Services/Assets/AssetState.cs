// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.Assets.AssetState
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Services.Engine.Assets;
using Inspectors;
using System;
using System.Collections;

#nullable disable
namespace Engine.Source.Services.Assets
{
  public class AssetState
  {
    [Inspected]
    public IAsset Asset;
    public Func<bool, IEnumerator> OnLoad;
    public Action OnDispose;
    public IEnumerator Processor;
    public bool NeedDispose;
    public string DisposeReason;
  }
}
