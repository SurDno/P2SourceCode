using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Commons.Parameters;
using Engine.Source.Components.Repairing;
using Engine.Source.Connections;
using Inspectors;

namespace Engine.Source.Components
{
  [Factory(typeof (IRepairableComponent))]
  [Required(typeof (ParametersComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class RepairableComponent : EngineComponent, IRepairableComponent, IComponent
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected Typed<RepairableSettings> settings;
    [FromThis]
    private ParametersComponent parameters;

    [Inspected]
    public RepairableSettings Settings => settings.Value;

    [Inspected]
    public IParameterValue<float> Durability { get; } = new ParameterValue<float>();

    public RepairableLevel TargetLevel()
    {
      if (Settings == null)
        return null;
      List<RepairableLevel> levels = Settings.Levels;
      if (levels == null || levels.Count == 0)
        return null;
      IParameter<float> byName = parameters.GetByName<float>(ParameterNameEnum.Durability);
      if (byName == null)
        return null;
      float num1 = byName.Value;
      float num2 = float.MaxValue;
      RepairableLevel repairableLevel1 = BaseLevel();
      for (int index = 0; index < levels.Count; ++index)
      {
        RepairableLevel repairableLevel2 = levels[index];
        if (repairableLevel2 != null)
        {
          float maxDurability = repairableLevel2.MaxDurability;
          if (maxDurability > (double) num1 && maxDurability < (double) num2)
          {
            num2 = maxDurability;
            repairableLevel1 = repairableLevel2;
          }
        }
      }
      return repairableLevel1;
    }

    public RepairableLevel BaseLevel()
    {
      if (Settings == null)
        return null;
      List<RepairableLevel> levels = Settings.Levels;
      if (levels == null || levels.Count == 0)
        return null;
      IParameter<float> byName = parameters.GetByName<float>(ParameterNameEnum.Durability);
      if (byName == null)
        return null;
      float num1 = byName.Value;
      float num2 = float.MinValue;
      RepairableLevel repairableLevel1 = null;
      for (int index = 0; index < levels.Count; ++index)
      {
        RepairableLevel repairableLevel2 = levels[index];
        if (repairableLevel2 != null)
        {
          float maxDurability = repairableLevel2.MaxDurability;
          if (maxDurability <= (double) num1 && maxDurability > (double) num2)
          {
            num2 = maxDurability;
            repairableLevel1 = repairableLevel2;
          }
        }
      }
      return repairableLevel1;
    }

    public override void OnAdded()
    {
      base.OnAdded();
      Durability.Set(parameters.GetByName<float>(ParameterNameEnum.Durability));
    }

    public override void OnRemoved()
    {
      Durability.Set(null);
      base.OnRemoved();
    }
  }
}
