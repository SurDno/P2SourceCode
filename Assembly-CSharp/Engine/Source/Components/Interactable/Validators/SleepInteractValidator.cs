// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.Interactable.Validators.SleepInteractValidator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Meta;
using Engine.Common.Components;
using Engine.Common.Components.Interactable;
using Engine.Common.Components.Parameters;
using Engine.Common.Services;

#nullable disable
namespace Engine.Source.Components.Interactable.Validators
{
  [Initialisable]
  public static class SleepInteractValidator
  {
    [InteractValidator(InteractType.Sleep)]
    public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item)
    {
      if (interactable.GetComponent<ParametersComponent>() == null)
        return new ValidateResult(true, "ParametersComponent not found");
      IParameter<bool> byName = ServiceLocator.GetService<ISimulation>().Player.GetComponent<ParametersComponent>().GetByName<bool>(ParameterNameEnum.Sleep);
      return byName != null && byName.Value ? new ValidateResult(false, "Sleep is true") : new ValidateResult(true);
    }
  }
}
