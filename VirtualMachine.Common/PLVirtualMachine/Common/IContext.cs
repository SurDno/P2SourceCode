using System.Collections.Generic;

namespace PLVirtualMachine.Common
{
  public interface IContext : INamed
  {
    IEnumerable<string> GetComponentNames();

    IEnumerable<IVariable> GetContextVariables(EContextVariableCategory contextVarCategory);

    IVariable GetContextVariable(string variableName);

    bool IsFunctionalSupport(string componentName);

    bool IsFunctionalSupport(IEnumerable<string> functionalsList);
  }
}
