// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.ICommonVariable
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using PLVirtualMachine.Common.EngineAPI;
using System.Collections.Generic;

#nullable disable
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
