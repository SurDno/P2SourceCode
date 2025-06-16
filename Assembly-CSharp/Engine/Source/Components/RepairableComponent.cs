// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.RepairableComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

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
using System.Collections.Generic;

#nullable disable
namespace Engine.Source.Components
{
  [Factory(typeof (IRepairableComponent))]
  [Required(typeof (ParametersComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class RepairableComponent : EngineComponent, IRepairableComponent, IComponent
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected Typed<RepairableSettings> settings;
    [FromThis]
    private ParametersComponent parameters;

    [Inspected]
    public RepairableSettings Settings => this.settings.Value;

    [Inspected]
    public IParameterValue<float> Durability { get; } = (IParameterValue<float>) new ParameterValue<float>();

    public RepairableLevel TargetLevel()
    {
      if (this.Settings == null)
        return (RepairableLevel) null;
      List<RepairableLevel> levels = this.Settings.Levels;
      if (levels == null || levels.Count == 0)
        return (RepairableLevel) null;
      IParameter<float> byName = this.parameters.GetByName<float>(ParameterNameEnum.Durability);
      if (byName == null)
        return (RepairableLevel) null;
      float num1 = byName.Value;
      float num2 = float.MaxValue;
      RepairableLevel repairableLevel1 = this.BaseLevel();
      for (int index = 0; index < levels.Count; ++index)
      {
        RepairableLevel repairableLevel2 = levels[index];
        if (repairableLevel2 != null)
        {
          float maxDurability = repairableLevel2.MaxDurability;
          if ((double) maxDurability > (double) num1 && (double) maxDurability < (double) num2)
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
      if (this.Settings == null)
        return (RepairableLevel) null;
      List<RepairableLevel> levels = this.Settings.Levels;
      if (levels == null || levels.Count == 0)
        return (RepairableLevel) null;
      IParameter<float> byName = this.parameters.GetByName<float>(ParameterNameEnum.Durability);
      if (byName == null)
        return (RepairableLevel) null;
      float num1 = byName.Value;
      float num2 = float.MinValue;
      RepairableLevel repairableLevel1 = (RepairableLevel) null;
      for (int index = 0; index < levels.Count; ++index)
      {
        RepairableLevel repairableLevel2 = levels[index];
        if (repairableLevel2 != null)
        {
          float maxDurability = repairableLevel2.MaxDurability;
          if ((double) maxDurability <= (double) num1 && (double) maxDurability > (double) num2)
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
      this.Durability.Set<float>(this.parameters.GetByName<float>(ParameterNameEnum.Durability));
    }

    public override void OnRemoved()
    {
      this.Durability.Set<float>((IParameter<float>) null);
      base.OnRemoved();
    }
  }
}
