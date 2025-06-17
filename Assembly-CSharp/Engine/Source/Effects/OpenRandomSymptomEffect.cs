using System.Collections.Generic;
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
using UnityEngine;

namespace Engine.Source.Effects
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class OpenRandomSymptomEffect : IEffect
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterEffectQueueEnum queue = ParameterEffectQueueEnum.None;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected int count = 0;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected List<Typed<IEntity>> targetSymptoms = [];
    private List<IStorableComponent> symptomsToOpen;

    public string Name => GetType().Name;

    [Inspected]
    public AbilityItem AbilityItem { get; set; }

    public IEntity Target { get; set; }

    public ParameterEffectQueueEnum Queue => queue;

    public bool Prepare(float currentRealTime, float currentGameTime)
    {
      symptomsToOpen = [];
      List<IStorableComponent> all = new List<IStorableComponent>(Target?.GetComponent<StorageComponent>().Items)?.FindAll(x => x.Container.GetGroup() == InventoryGroup.Symptom);
      List<IStorableComponent> storableComponentList = [];
      foreach (IStorableComponent component in all)
      {
        bool flag = targetSymptoms == null || targetSymptoms.Count == 0;
        if (!flag)
        {
          foreach (Typed<IEntity> targetSymptom in targetSymptoms)
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
          IParameter<bool> byName = (component != null ? component.GetComponent<ParametersComponent>() : null)?.GetByName<bool>(ParameterNameEnum.IsOpen);
          if (byName != null && !byName.Value)
            storableComponentList.Add(component);
        }
      }
      for (int index = 0; index < count && storableComponentList.Count != 0; ++index)
      {
        IStorableComponent storableComponent = storableComponentList[Random.Range(0, storableComponentList.Count)];
        symptomsToOpen.Add(storableComponent);
        storableComponentList.Remove(storableComponent);
      }
      return true;
    }

    public bool Compute(float currentRealTime, float currentGameTime)
    {
      if (symptomsToOpen != null)
      {
        foreach (IStorableComponent component in symptomsToOpen)
        {
          IParameter<bool> byName = (component != null ? component.GetComponent<ParametersComponent>() : null)?.GetByName<bool>(ParameterNameEnum.IsOpen);
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
