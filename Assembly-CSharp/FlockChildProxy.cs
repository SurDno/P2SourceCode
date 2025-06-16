// Decompiled with JetBrains decompiler
// Type: FlockChildProxy
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Source.Commons;
using System;

#nullable disable
public class FlockChildProxy : IUpdatable, IDisposable
{
  private FlockChild child;

  public FlockChildProxy(FlockChild child)
  {
    this.child = child;
    InstanceByRequest<UpdateService>.Instance.FlockCastUpdater.AddUpdatable((IUpdatable) this);
  }

  public void ComputeUpdate() => this.child.ProxyUpdate();

  public void Dispose()
  {
    InstanceByRequest<UpdateService>.Instance.FlockCastUpdater.RemoveUpdatable((IUpdatable) this);
  }
}
