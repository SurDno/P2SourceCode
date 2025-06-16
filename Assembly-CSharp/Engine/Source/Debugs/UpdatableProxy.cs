// Decompiled with JetBrains decompiler
// Type: Engine.Source.Debugs.UpdatableProxy
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using System;

#nullable disable
namespace Engine.Source.Debugs
{
  public class UpdatableProxy : IUpdatable
  {
    private Action action;

    public UpdatableProxy(Action action) => this.action = action;

    public void ComputeUpdate()
    {
      Action action = this.action;
      if (action == null)
        return;
      action();
    }
  }
}
