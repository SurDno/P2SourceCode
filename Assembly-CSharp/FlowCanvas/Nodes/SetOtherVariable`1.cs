using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using System;
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

    public override string name
    {
      get
      {
        return string.Format("${0}{1}Value", (object) this.varName.value, (object) OperationTools.GetOperationString(this.operation));
      }
    }

    protected override void RegisterPorts()
    {
      ValueInput<Blackboard> bb = this.AddValueInput<Blackboard>("Blackboard");
      this.varName = this.AddValueInput<string>("Variable");
      ValueInput<T> v = this.AddValueInput<T>("Value");
      FlowOutput o = this.AddFlowOutput("Out");
      this.AddValueOutput<T>("Value", (ValueHandler<T>) (() => bb.value.GetValue<T>(this.varName.value)));
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        this.DoSet(bb.value, this.varName.value, v.value);
        o.Call();
      }));
    }

    private void DoSet(Blackboard bb, string name, T value)
    {
      Variable<T> variable = bb.GetVariable<T>(name);
      if (this.operation != 0)
      {
        if (typeof (T) == typeof (float))
          variable.value = (T) (ValueType) OperationTools.Operate((float) (object) variable.value, (float) (object) value, this.operation);
        else if (typeof (T) == typeof (int))
          variable.value = (T) (ValueType) OperationTools.Operate((int) (object) variable.value, (int) (object) value, this.operation);
        else if (typeof (T) == typeof (Vector3))
          variable.value = (T) (ValueType) OperationTools.Operate((Vector3) (object) variable.value, (Vector3) (object) value, this.operation);
        else
          variable.value = value;
      }
      else
        variable.value = value;
    }
  }
}
