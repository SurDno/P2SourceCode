using System.Collections.Generic;
using System.Linq;
using PLVirtualMachine.Common.EngineAPI;

namespace PLVirtualMachine.Common
{
  public abstract class ContextVariable : IVariable, INamed, IContext
  {
    private VMType type;
    private string name;

    public void Initialize(string name, VMType type)
    {
      this.name = name;
      this.type = type;
    }

    public virtual EContextVariableCategory Category => EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_LOCAL_VAR;

    public string Name => name;

    public VMType Type => type;

    public IEnumerable<string> GetComponentNames()
    {
      if (Type.IsFunctionalSpecial)
      {
        foreach (string functionalPart in Type.GetFunctionalParts())
          yield return functionalPart;
      }
    }

    public virtual IEnumerable<IVariable> GetContextVariables(
      EContextVariableCategory contextVarCategory)
    {
      return TypedBlueprint != null ? TypedBlueprint.GetContextVariables(contextVarCategory) : GetFunctionalContextVariables(contextVarCategory);
    }

    public virtual IVariable GetContextVariable(string variableName)
    {
      foreach (IVariable functionalContextVariable in GetFunctionalContextVariables(EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_PARAM))
      {
        if (functionalContextVariable.Name == variableName)
          return functionalContextVariable;
      }
      foreach (IVariable functionalContextVariable in GetFunctionalContextVariables(EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_APIFUNCTION))
      {
        if (functionalContextVariable.Name == variableName)
          return functionalContextVariable;
      }
      return TypedBlueprint != null ? TypedBlueprint.GetContextVariable(variableName) : null;
    }

    public IBlueprint TypedBlueprint => Type.IsComplexSpecial ? Type.SpecialTypeBlueprint : null;

    public bool IsFunctionalSupport(string componentName)
    {
      return GetComponentNames().Contains(componentName);
    }

    public bool IsFunctionalSupport(IEnumerable<string> functionalsList)
    {
      IEnumerable<string> componentNames = GetComponentNames();
      foreach (string functionals in functionalsList)
      {
        if (!componentNames.Contains(functionals))
          return false;
      }
      return true;
    }

    public bool IsEqual(IVariable other)
    {
      return other is ContextVariable contextVariable && name == contextVariable.name;
    }

    public virtual void Clear()
    {
      if (type == null)
        return;
      type = null;
    }

    private IEnumerable<IVariable> GetFunctionalContextVariables(
      EContextVariableCategory contextVarCategory)
    {
      if ((contextVarCategory == EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_APIFUNCTION || contextVarCategory == EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_PARAM) && Type.IsFunctionalSpecial)
      {
        foreach (string functionalPart in Type.GetFunctionalParts())
        {
          foreach (IVariable functionalContextVariable in EngineAPIManager.GetAbstractVariablesByFunctionalName(functionalPart, contextVarCategory))
            yield return functionalContextVariable;
        }
      }
    }
  }
}
