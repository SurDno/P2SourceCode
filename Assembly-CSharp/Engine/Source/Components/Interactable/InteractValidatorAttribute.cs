// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.Interactable.InteractValidatorAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Loggers;
using Cofe.Meta;
using Engine.Common.Components;
using Engine.Common.Components.Interactable;
using System;
using System.Reflection;

#nullable disable
namespace Engine.Source.Components.Interactable
{
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
  public class InteractValidatorAttribute : InitialiseAttribute
  {
    private InteractType type;

    public InteractValidatorAttribute(InteractType type) => this.type = type;

    public override void ComputeMember(Container container, MemberInfo member)
    {
      MethodInfo methodInfo = (MethodInfo) member;
      if (!methodInfo.IsStatic || methodInfo.ReturnType != typeof (ValidateResult) || methodInfo.GetParameters().Length != 2 || methodInfo.GetParameters()[0].ParameterType != typeof (IInteractableComponent) || methodInfo.GetParameters()[1].ParameterType != typeof (InteractItem))
        Logger.AddError("Method error : " + (object) methodInfo);
      else
        container.GetHandler(InitialiseAttribute.Id).AddHandle((ComputeHandle) ((target, data) => InteractValidationService.AddValidator(this.type, (Func<IInteractableComponent, InteractItem, ValidateResult>) ((interactable, item) => (ValidateResult) ((MethodBase) member).Invoke((object) null, new object[2]
        {
          (object) interactable,
          (object) item
        })))));
    }
  }
}
