using Cofe.Serializations.Data;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.Serialization;
using PLVirtualMachine.Data.SaveLoad;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace PLVirtualMachine.Dynamic
{
  public class StateStack : ISerializeStateSave, IDynamicLoadSerializable
  {
    private System.Collections.Generic.Stack<StatePoint> stateStack = new System.Collections.Generic.Stack<StatePoint>();
    private EStateStackType stackType;

    public StateStack(EStateStackType stackType = EStateStackType.STATESTACK_TYPE_MAIN)
    {
      this.stackType = stackType;
    }

    public StateStack(StateStack memStack)
    {
      if (memStack.Stack == null)
        return;
      this.stateStack = new System.Collections.Generic.Stack<StatePoint>(memStack.Stack.Reverse<StatePoint>());
    }

    public void StateSave(IDataWriter writer)
    {
      SaveManagerUtility.SaveDynamicSerializableList<StatePoint>(writer, "StatePoints", (IEnumerable<StatePoint>) this.stateStack);
    }

    public void LoadFromXML(XmlElement objNode)
    {
      for (int i = 0; i < objNode.ChildNodes.Count; ++i)
      {
        if (objNode.ChildNodes[i].Name == "StatePoints")
        {
          XmlElement childNode = (XmlElement) objNode.ChildNodes[i];
          List<StatePoint> statePointList = new List<StatePoint>();
          List<StatePoint> list = statePointList;
          VMSaveLoadManager.LoadDynamiSerializableList<StatePoint>(childNode, list);
          this.stateStack.Clear();
          for (int index = statePointList.Count - 1; index >= 0; --index)
            this.stateStack.Push(statePointList[index]);
        }
      }
    }

    public void Push(StatePoint newStatePoint) => this.stateStack.Push(newStatePoint);

    public StatePoint Peek()
    {
      return this.stateStack.Count == 0 ? (StatePoint) null : this.stateStack.Peek();
    }

    public StatePoint Pop()
    {
      return this.stateStack.Count == 0 ? (StatePoint) null : this.stateStack.Pop();
    }

    public int Count => this.stateStack.Count;

    public System.Collections.Generic.Stack<StatePoint> Stack => this.stateStack;

    public EStateStackType StackType => this.stackType;

    public void Clear() => this.stateStack.Clear();

    public void Init() => this.Clear();

    public bool Modified => true;

    public void OnModify()
    {
    }
  }
}
