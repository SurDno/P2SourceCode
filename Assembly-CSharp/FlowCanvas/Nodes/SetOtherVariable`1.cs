using System;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes
{
  [Name("Set Other Of Type<T>")]
  [Category("Variables/Blackboard")]
  [Description("Use this to set a variable value of blackboards other than the one this flowscript is using")]
  [AppendListTypes]
  public class SetOtherVariable<T> : FlowNode
  {
    public OperationMethod operation = OperationMethod.Set;
    private ValueInput<string> varName;

    public override string name => string.Format("${0}{1}Value", varName.value, OperationTools.GetOperationString(operation));

    protected override void RegisterPorts()
    {
      ValueInput<Blackboard> bb = AddValueInput<Blackboard>("Blackboard");
      varName = AddValueInput<string>("Variable");
      ValueInput<T> v = AddValueInput<T>("Value");
      FlowOutput o = AddFlowOutput("Out");
      AddValueOutput("Value", () => bb.value.GetValue<T>(varName.value));
      AddFlowInput("In", () =>
      {
        DoSet(bb.value, varName.value, v.value);
        o.Call();
      });
    }

    private void DoSet(Blackboard bb, string name, T value)
    {
      Variable<T> variable = bb.GetVariable<T>(name);
      if (operation != 0)
      {
        if (typeof(T) == typeof(float))
          variable.value = (T)(object)OperationTools.Operate((float)(object)variable.value, (float)(object)value, operation);
        else if (typeof(T) == typeof(int))
          variable.value = (T)(object)OperationTools.Operate((int)(object)variable.value, (int)(object)value, operation);
        else if (typeof(T) == typeof(Vector3))
          variable.value = (T)(object)OperationTools.Operate((Vector3)(object)variable.value, (Vector3)(object)value, operation);
        else
          variable.value = value;
      }
      else
        variable.value = value;
    }
  }
}
