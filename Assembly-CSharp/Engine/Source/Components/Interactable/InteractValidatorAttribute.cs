using System;
using System.Reflection;
using Cofe.Loggers;
using Cofe.Meta;
using Engine.Common.Components;
using Engine.Common.Components.Interactable;

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
        Logger.AddError("Method error : " + methodInfo);
      else
        container.GetHandler(Id).AddHandle((target, data) => InteractValidationService.AddValidator(type, (interactable, item) => (ValidateResult) ((MethodBase) member).Invoke(null, new object[2]
        {
          interactable,
          item
        })));
    }
  }
}
