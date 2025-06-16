// Decompiled with JetBrains decompiler
// Type: Engine.Source.Effects.OpenRandomSymptomEffect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Common.Components.Storable;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Effects;
using Engine.Source.Components;
using Engine.Source.Components.Utilities;
using Engine.Source.Connections;
using Engine.Source.Inventory;
using Inspectors;
using System;
using System.Collections.Generic;

#nullable disable
namespace Engine.Source.Effects
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class OpenRandomSymptomEffect : IEffect
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterEffectQueueEnum queue = ParameterEffectQueueEnum.None;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected int count = 0;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected List<Typed<IEntity>> targetSymptoms = new List<Typed<IEntity>>();
    private List<IStorableComponent> symptomsToOpen;

    public string Name => this.GetType().Name;

    [Inspected]
    public AbilityItem AbilityItem { get; set; }

    public IEntity Target { get; set; }

    public ParameterEffectQueueEnum Queue => this.queue;

    public bool Prepare(float currentRealTime, float currentGameTime)
    {
      this.symptomsToOpen = new List<IStorableComponent>();
      List<IStorableComponent> all = new List<IStorableComponent>(this.Target?.GetComponent<StorageComponent>().Items)?.FindAll((Predicate<IStorableComponent>) (x => x.Container.GetGroup() == InventoryGroup.Symptom));
      List<IStorableComponent> storableComponentList = new List<IStorableComponent>();
      foreach (IStorableComponent component in all)
      {
        bool flag = this.targetSymptoms == null || this.targetSymptoms.Count == 0;
        if (!flag)
        {
          foreach (Typed<IEntity> targetSymptom in this.targetSymptoms)
          {
            if (targetSymptom.Value != null && StorageUtility.GetItemId(targetSymptom.Value) == StorageUtility.GetItemId(component.Owner))
            {
              flag = true;
              break;
            }
          }
        }
        if (flag)
        {
          IParameter<bool> byName = (component != null ? component.GetComponent<ParametersComponent>() : (ParametersComponent) null)?.GetByName<bool>(ParameterNameEnum.IsOpen);
          if (byName != null && !byName.Value)
            storableComponentList.Add(component);
        }
      }
      for (int index = 0; index < this.count && storableComponentList.Count != 0; ++index)
      {
        IStorableComponent storableComponent = storableComponentList[UnityEngine.Random.Range(0, storableComponentList.Count)];
        this.symptomsToOpen.Add(storableComponent);
        storableComponentList.Remove(storableComponent);
      }
      return true;
    }

    public bool Compute(float currentRealTime, float currentGameTime)
    {
      if (this.symptomsToOpen != null)
      {
        foreach (IStorableComponent component in this.symptomsToOpen)
        {
          IParameter<bool> byName = (component != null ? component.GetComponent<ParametersComponent>() : (ParametersComponent) null)?.GetByName<bool>(ParameterNameEnum.IsOpen);
          if (byName != null)
            byName.Value = true;
        }
      }
      return false;
    }

    public void Cleanup()
    {
    }
  }
}
