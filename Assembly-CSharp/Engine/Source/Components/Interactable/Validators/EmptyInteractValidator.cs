// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.Interactable.Validators.EmptyInteractValidator
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
  public static class EmptyInteractValidator
  {
    [InteractValidator(InteractType.Empty)]
    public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item)
    {
      ParametersComponent component = interactable.GetComponent<ParametersComponent>();
      if (component == null)
        return new ValidateResult(false, "ParametersComponent not found");
      IParameter<int> byName = component.GetByName<int>(ParameterNameEnum.Bullets);
      if (byName == null)
        return new ValidateResult(false, "Bullets not found");
      return byName.Value > 0 ? new ValidateResult(false, "Bullets not empty") : new ValidateResult(true);
    }
  }
}
