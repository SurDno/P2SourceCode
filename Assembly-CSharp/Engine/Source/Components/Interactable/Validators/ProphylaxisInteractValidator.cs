// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.Interactable.Validators.ProphylaxisInteractValidator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Meta;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Components.Interactable;
using Engine.Common.Components.Parameters;

#nullable disable
namespace Engine.Source.Components.Interactable.Validators
{
  [Initialisable]
  internal class ProphylaxisInteractValidator
  {
    [InteractValidator(InteractType.Prophylaxis)]
    public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item)
    {
      ParametersComponent component = interactable.GetComponent<ParametersComponent>();
      if (component == null)
        return new ValidateResult(false, "ParametersComponent not found");
      IParameter<bool> byName1 = component.GetByName<bool>(ParameterNameEnum.Dead);
      if (byName1 != null && byName1.Value)
        return new ValidateResult(false, "Dead is true");
      IParameter<bool> byName2 = component.GetByName<bool>(ParameterNameEnum.CanHeal);
      if (byName2 != null && !byName2.Value)
        return new ValidateResult(false, "CanHeal is false");
      IParameter<BoundHealthStateEnum> byName3 = component.GetByName<BoundHealthStateEnum>(ParameterNameEnum.BoundHealthState);
      if (byName3 == null)
        return new ValidateResult(false, "BoundHealthState not found");
      if (byName3.Value != BoundHealthStateEnum.Danger)
        return new ValidateResult(false, "BoundHealthState " + (object) byName3.Value);
      IParameter<bool> byName4 = component.GetByName<bool>(ParameterNameEnum.ImmuneBoostAttempted);
      return byName4 != null && byName4.Value ? new ValidateResult(false, "ImmuneBoostAttempted is true") : new ValidateResult(true);
    }
  }
}
