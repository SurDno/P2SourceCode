// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.RegisterComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Inspectors;
using System;
using System.Collections.Generic;

#nullable disable
namespace Engine.Source.Components
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class RegisterComponent : EngineComponent
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected string tag = "";
    private static Dictionary<string, IEntity> entities = new Dictionary<string, IEntity>();

    public static IEntity GetByTag(string tag)
    {
      IEntity byTag;
      RegisterComponent.entities.TryGetValue(tag, out byTag);
      return byTag;
    }

    public override void OnAdded()
    {
      base.OnAdded();
      IEntity entity = (IEntity) null;
      if (RegisterComponent.entities.TryGetValue(this.tag, out entity))
        throw new Exception("Already add with tag : " + this.tag + " , exist : " + entity.GetInfo() + " , new : " + this.Owner.GetInfo());
      RegisterComponent.entities.Add(this.tag, this.Owner);
    }

    public override void OnRemoved()
    {
      RegisterComponent.entities.Remove(this.tag);
      base.OnRemoved();
    }
  }
}
