using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Cofe.Serializations.Data;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.Serialization;
using PLVirtualMachine.Data.SaveLoad;

namespace PLVirtualMachine.Dynamic
{
  public class StateStack : ISerializeStateSave, IDynamicLoadSerializable
  {
    private Stack<StatePoint> stateStack = new();
    private EStateStackType stackType;

    public StateStack(EStateStackType stackType = EStateStackType.STATESTACK_TYPE_MAIN)
    {
      this.stackType = stackType;
    }

    public StateStack(StateStack memStack)
    {
      if (memStack.Stack == null)
        return;
      stateStack = new Stack<StatePoint>(memStack.Stack.Reverse());
    }

    public void StateSave(IDataWriter writer)
    {
      SaveManagerUtility.SaveDynamicSerializableList(writer, "StatePoints", stateStack);
    }

    public void LoadFromXML(XmlElement objNode)
    {
      for (int i = 0; i < objNode.ChildNodes.Count; ++i)
      {
        if (objNode.ChildNodes[i].Name == "StatePoints")
        {
          XmlElement childNode = (XmlElement) objNode.ChildNodes[i];
          List<StatePoint> statePointList = [];
          List<StatePoint> list = statePointList;
          VMSaveLoadManager.LoadDynamiSerializableList(childNode, list);
          stateStack.Clear();
          for (int index = statePointList.Count - 1; index >= 0; --index)
            stateStack.Push(statePointList[index]);
        }
      }
    }

    public void Push(StatePoint newStatePoint) => stateStack.Push(newStatePoint);

    public StatePoint Peek()
    {
      return stateStack.Count == 0 ? null : stateStack.Peek();
    }

    public StatePoint Pop()
    {
      return stateStack.Count == 0 ? null : stateStack.Pop();
    }

    public int Count => stateStack.Count;

    public Stack<StatePoint> Stack => stateStack;

    public EStateStackType StackType => stackType;

    public void Clear() => stateStack.Clear();

    public void Init() => Clear();

    public bool Modified => true;

    public void OnModify()
    {
    }
  }
}
