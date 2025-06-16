using UnityEngine;

namespace BehaviorDesigner.Runtime
{
  public interface IBehaviorTree
  {
    string GetOwnerName();

    int GetInstanceID();

    BehaviorSource BehaviorSource { get; }

    Object GetObject();

    SharedVariable GetVariable(string name);

    void SetVariable(string name, SharedVariable item);

    void SetVariableValue(string name, object value);
  }
}
