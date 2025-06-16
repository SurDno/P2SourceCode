using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using Inspectors;
using UnityEngine;

[Factory(typeof (IStorableTooltipComponent))]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class StorableTooltipItemDurability : IStorableTooltipComponent
{
  [DataReadProxy(MemberEnum.None)]
  [DataWriteProxy(MemberEnum.None)]
  [CopyableProxy(MemberEnum.None)]
  [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
  protected bool isEnabled = true;
  [DataReadProxy(MemberEnum.None)]
  [DataWriteProxy(MemberEnum.None)]
  [CopyableProxy(MemberEnum.None)]
  [Inspected]
  [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
  protected StorableTooltipNameEnum name;
  [DataReadProxy(MemberEnum.None)]
  [DataWriteProxy(MemberEnum.None)]
  [CopyableProxy(MemberEnum.None)]
  [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
  protected ParameterNameEnum parameter;
  [DataReadProxy(MemberEnum.None)]
  [DataWriteProxy(MemberEnum.None)]
  [CopyableProxy(MemberEnum.None)]
  [Inspected]
  [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
  protected bool isFood;

  [Inspected]
  public bool IsEnabled => this.isEnabled;

  public StorableTooltipInfo GetInfo(IEntity owner)
  {
    StorableTooltipInfo info = new StorableTooltipInfo();
    info.Name = this.name;
    if (owner != null)
    {
      ParametersComponent component = owner.GetComponent<ParametersComponent>();
      if (component != null)
      {
        IParameter<float> byName = component.GetByName<float>(this.parameter);
        this.CountParams(info, byName.Value);
      }
    }
    return info;
  }

  private void CountParams(StorableTooltipInfo info, float value)
  {
    if ((double) value > 0.89999997615814209)
    {
      info.Value = this.isFood ? "{StorableTooltip.FoodExcellent}" : "{StorableTooltip.Excellent}";
      info.Color = Color.blue;
    }
    else if ((double) value > 0.699999988079071)
    {
      info.Value = this.isFood ? "{StorableTooltip.FoodGood}" : "{StorableTooltip.Good}";
      info.Color = Color.green;
    }
    else if ((double) value > 0.30000001192092896)
    {
      info.Value = this.isFood ? "{StorableTooltip.FoodAverage}" : "{StorableTooltip.Average}";
      info.Color = Color.yellow;
    }
    else if ((double) value > 0.0)
    {
      info.Value = this.isFood ? "{StorableTooltip.FoodBad}" : "{StorableTooltip.Bad}";
      info.Color = Color.red;
    }
    else
    {
      info.Value = this.isFood ? "{StorableTooltip.FoodBroken}" : "{StorableTooltip.Broken}";
      info.Color = Color.red;
    }
  }
}
