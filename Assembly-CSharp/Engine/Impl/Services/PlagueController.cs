// Decompiled with JetBrains decompiler
// Type: Engine.Impl.Services.PlagueController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Services;
using System;
using UnityEngine;

#nullable disable
namespace Engine.Impl.Services
{
  [GameService(new System.Type[] {typeof (IPlagueController)})]
  public class PlagueController : IPlagueController
  {
    public float GetLevel(IEntity entity)
    {
      if (entity == null)
        throw new Exception("Entity is null!");
      IEntityView entityView = !entity.IsDisposed ? (IEntityView) entity : throw new Exception("Entity is Disposed!");
      return PlagueZone.GetLevel(new Vector2(entityView.Position.x, entityView.Position.z));
    }
  }
}
