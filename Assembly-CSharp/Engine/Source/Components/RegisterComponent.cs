using System;
using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Inspectors;

namespace Engine.Source.Components
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class RegisterComponent : EngineComponent
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected string tag = "";
    private static Dictionary<string, IEntity> entities = new();

    public static IEntity GetByTag(string tag)
    {
      entities.TryGetValue(tag, out IEntity byTag);
      return byTag;
    }

    public override void OnAdded()
    {
      base.OnAdded();
      if (entities.TryGetValue(tag, out IEntity entity))
        throw new Exception("Already add with tag : " + tag + " , exist : " + entity.GetInfo() + " , new : " + Owner.GetInfo());
      entities.Add(tag, Owner);
    }

    public override void OnRemoved()
    {
      entities.Remove(tag);
      base.OnRemoved();
    }
  }
}
