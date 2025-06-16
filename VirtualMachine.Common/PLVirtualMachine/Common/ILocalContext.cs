using System.Collections.Generic;

namespace PLVirtualMachine.Common
{
  public interface ILocalContext : 
    IContainer,
    IObject,
    IEditorBaseTemplate,
    INamedElement,
    INamed,
    IStaticUpdateable
  {
    List<IVariable> GetLocalContextVariables(
      EContextVariableCategory contextVarCategory,
      IContextElement currentElement,
      int counter = 0);

    IVariable GetLocalContextVariable(string variableUniName, IContextElement currentElement = null);
  }
}
