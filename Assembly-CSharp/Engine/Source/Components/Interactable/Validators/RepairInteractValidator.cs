// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.Interactable.Validators.RepairInteractValidator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Meta;
using Engine.Common.Components;
using Engine.Common.Components.Interactable;
using Engine.Common.Components.Parameters;

#nullable disable
namespace Engine.Source.Components.Interactable.Validators
{
  [Initialisable]
  public class RepairInteractValidator
  {
    [InteractValidator(InteractType.Repair)]
    public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item)
    {
      ParametersComponent component = interactable.GetComponent<ParametersComponent>();
      if (component == null)
        return new ValidateResult(false, "ParametersComponent not found");
      IParameter<float> byName = component.GetByName<float>(ParameterNameEnum.Durability);
      if (byName == null || (double) byName.Value >= (double) byName.MaxValue)
        return new ValidateResult(false, "Durability >= max");
      return interactable.GetComponent<RepairableComponent>() == null ? new ValidateResult(false, "RepairableComponent not found") : new ValidateResult(true);
    }
  }
}
