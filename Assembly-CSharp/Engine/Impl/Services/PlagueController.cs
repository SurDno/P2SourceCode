using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Services;
using System;
using UnityEngine;

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
