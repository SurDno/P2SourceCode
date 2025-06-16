// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.AbilitiesComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Commons.Abilities;
using Engine.Source.Connections;
using Inspectors;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Engine.Source.Components
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class AbilitiesComponent : EngineComponent, INeedSave
  {
    [DataReadProxy(MemberEnum.None, Name = "Abilities")]
    [DataWriteProxy(MemberEnum.None, Name = "Abilities")]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy(MemberEnum.None)]
    protected List<Typed<IAbility>> resourceAbilities = new List<Typed<IAbility>>();
    [Inspected]
    private List<IAbility> abilities = new List<IAbility>();

    public bool NeedSave
    {
      get
      {
        if (!(this.Owner.Template is IEntity template))
        {
          Debug.LogError((object) ("Template not found, owner : " + this.Owner.GetInfo()));
          return true;
        }
        if (template.GetComponent<AbilitiesComponent>() != null)
          return false;
        Debug.LogError((object) (this.GetType().Name + " not found, owner : " + this.Owner.GetInfo()));
        return true;
      }
    }

    public override void OnAdded()
    {
      base.OnAdded();
      foreach (Typed<IAbility> resourceAbility in this.resourceAbilities)
      {
        IAbility source = resourceAbility.Value;
        if (source != null)
        {
          IAbility ability = CloneableObjectUtility.Clone<IAbility>(source);
          this.abilities.Add(ability);
          ((Ability) ability).Initialise(this.Owner);
        }
      }
    }

    public override void OnRemoved()
    {
      foreach (IAbility ability in this.abilities)
      {
        ((Ability) ability).Shutdown();
        ((EngineObject) ability).Dispose();
      }
      this.abilities.Clear();
      base.OnRemoved();
    }
  }
}
