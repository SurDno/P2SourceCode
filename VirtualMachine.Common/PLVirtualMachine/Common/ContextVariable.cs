using PLVirtualMachine.Common.EngineAPI;
using System.Collections.Generic;
using System.Linq;

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

    public virtual EContextVariableCategory Category
    {
      get => EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_LOCAL_VAR;
    }

    public string Name => this.name;

    public VMType Type => this.type;

    public IEnumerable<string> GetComponentNames()
    {
      if (this.Type.IsFunctionalSpecial)
      {
        foreach (string functionalPart in this.Type.GetFunctionalParts())
          yield return functionalPart;
      }
    }

    public virtual IEnumerable<IVariable> GetContextVariables(
      EContextVariableCategory contextVarCategory)
    {
      return this.TypedBlueprint != null ? this.TypedBlueprint.GetContextVariables(contextVarCategory) : this.GetFunctionalContextVariables(contextVarCategory);
    }

    public virtual IVariable GetContextVariable(string variableName)
    {
      foreach (IVariable functionalContextVariable in this.GetFunctionalContextVariables(EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_PARAM))
      {
        if (functionalContextVariable.Name == variableName)
          return functionalContextVariable;
      }
      foreach (IVariable functionalContextVariable in this.GetFunctionalContextVariables(EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_APIFUNCTION))
      {
        if (functionalContextVariable.Name == variableName)
          return functionalContextVariable;
      }
      return this.TypedBlueprint != null ? this.TypedBlueprint.GetContextVariable(variableName) : (IVariable) null;
    }

    public IBlueprint TypedBlueprint
    {
      get => this.Type.IsComplexSpecial ? this.Type.SpecialTypeBlueprint : (IBlueprint) null;
    }

    public bool IsFunctionalSupport(string componentName)
    {
      return this.GetComponentNames().Contains<string>(componentName);
    }

    public bool IsFunctionalSupport(IEnumerable<string> functionalsList)
    {
      IEnumerable<string> componentNames = this.GetComponentNames();
      foreach (string functionals in functionalsList)
      {
        if (!componentNames.Contains<string>(functionals))
          return false;
      }
      return true;
    }

    public bool IsEqual(IVariable other)
    {
      return other is ContextVariable contextVariable && this.name == contextVariable.name;
    }

    public virtual void Clear()
    {
      if (this.type == null)
        return;
      this.type = (VMType) null;
    }

    private IEnumerable<IVariable> GetFunctionalContextVariables(
      EContextVariableCategory contextVarCategory)
    {
      if ((contextVarCategory == EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_APIFUNCTION || contextVarCategory == EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_PARAM) && this.Type.IsFunctionalSpecial)
      {
        foreach (string functionalPart in this.Type.GetFunctionalParts())
        {
          foreach (IVariable functionalContextVariable in EngineAPIManager.GetAbstractVariablesByFunctionalName(functionalPart, contextVarCategory))
            yield return functionalContextVariable;
        }
      }
    }
  }
}
