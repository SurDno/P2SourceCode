using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using System;
using UnityEngine;

namespace FlowCanvas.Nodes
{
  [DoNotList]
  [Name("Set Of Type<T>")]
  [Category("Variables/Set Blackboard Variable")]
  [Description("Set a Blackboard variable value")]
  [AppendListTypes]
  public class SetVariable<T> : FlowNode
  {
    [BlackboardOnly]
    public BBParameter<T> targetVariable;
    [HideInInspector]
    public OperationMethod operation = OperationMethod.Set;
    [HideInInspector]
    public bool perSecond = false;

    public override string name
    {
      get
      {
        return string.Format("{0}{1}{2}", (object) this.targetVariable.ToString(), (object) OperationTools.GetOperationString(this.operation), (object) "Value");
      }
    }

    protected override void RegisterPorts()
    {
      FlowOutput o = this.AddFlowOutput("Out");
      ValueInput<T> v = this.AddValueInput<T>("Value");
      this.AddValueOutput<T>("Value", (ValueHandler<T>) (() => this.targetVariable.value));
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        this.DoSet(v.value);
        o.Call();
      }));
    }

    private void DoSet(T value)
    {
      if (this.operation != 0)
      {
        if (typeof (T) == typeof (float))
          this.targetVariable.value = (T) (ValueType) OperationTools.Operate((float) (object) this.targetVariable.value, (float) (object) value, this.operation);
        else if (typeof (T) == typeof (int))
          this.targetVariable.value = (T) (ValueType) OperationTools.Operate((int) (object) this.targetVariable.value, (int) (object) value, this.operation);
        else if (typeof (T) == typeof (Vector3))
          this.targetVariable.value = (T) (ValueType) OperationTools.Operate((Vector3) (object) this.targetVariable.value, (Vector3) (object) value, this.operation);
        else
          this.targetVariable.value = value;
      }
      else
        this.targetVariable.value = value;
    }

    public void SetTargetVariableName(string name) => this.targetVariable.name = name;
  }
}
