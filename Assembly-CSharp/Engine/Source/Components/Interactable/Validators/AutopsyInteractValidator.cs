// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.Interactable.Validators.AutopsyInteractValidator
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
  public class AutopsyInteractValidator
  {
    [InteractValidator(InteractType.Autopsy)]
    public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item)
    {
      ParametersComponent component = interactable.GetComponent<ParametersComponent>();
      if (component == null)
        return new ValidateResult(true, "ParametersComponent not found");
      IParameter<bool> byName1 = component.GetByName<bool>(ParameterNameEnum.Dead);
      if (byName1 != null && !byName1.Value)
        return new ValidateResult(false, "Dead is false");
      IParameter<bool> byName2 = component.GetByName<bool>(ParameterNameEnum.CanAutopsy);
      return byName2 != null && !byName2.Value ? new ValidateResult(false, "CanAutopsy is false") : new ValidateResult(true);
    }
  }
}
