using PLVirtualMachine.Common.EngineAPI;
using System.Collections.Generic;

namespace PLVirtualMachine.Common
{
  public interface ICommonVariable
  {
    void Initialise(string contextData, string variableData);

    void Initialise(object value);

    EContextVariableCategory Category { get; }

    IContext Context { get; }

    object Variable { get; }

    IEnumerable<IVariable> GetContextVariables(EContextVariableCategory contextVarCategory);

    string Write();

    void Read(string data);

    bool IsSelf { get; }

    bool IsNull { get; }

    bool IsBinded { get; }

    ECommonVariableType CommonVariableType { get; }

    void Bind(
      IContext ownerContext,
      VMType needType = null,
      ILocalContext localContext = null,
      IContextElement contextElement = null);

    IVariable GetContextVariable(string variableName);

    IContext VariableContext { get; }
  }
}
