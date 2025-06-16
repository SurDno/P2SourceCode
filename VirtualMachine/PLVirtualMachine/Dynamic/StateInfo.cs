using Cofe.Serializations.Data;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.Serialization;
using PLVirtualMachine.Data.SaveLoad;
using System.Xml;

namespace PLVirtualMachine.Dynamic
{
  public class StateInfo : ISerializeStateSave, IDynamicLoadSerializable
  {
    public IState state;
    public StateStack stateStack;

    public StateInfo()
    {
    }

    public StateInfo(IState state)
    {
      this.state = state;
      this.stateStack = (StateStack) null;
    }

    public StateInfo(StateStack stateStack)
    {
      this.state = (IState) null;
      this.stateStack = stateStack;
    }

    public void StateSave(IDataWriter writer)
    {
      SaveManagerUtility.Save(writer, "State", this.State != null ? this.State.BaseGuid : 0UL);
      SaveManagerUtility.Save(writer, "StateName", this.State != null ? this.State.Name : "");
      SaveManagerUtility.SaveDynamicSerializable(writer, "StateStack", (ISerializeStateSave) this.stateStack);
    }

    public void LoadFromXML(XmlElement xmlNode)
    {
      for (int i = 0; i < xmlNode.ChildNodes.Count; ++i)
      {
        XmlElement childNode = (XmlElement) xmlNode.ChildNodes[i];
        if (childNode.Name == "State")
        {
          ulong id = VMSaveLoadManager.ReadUlong((XmlNode) childNode);
          if (id != 0UL)
          {
            IObject objectByGuid = IStaticDataContainer.StaticDataContainer.GetObjectByGuid(id);
            if (objectByGuid != null)
              this.state = (IState) objectByGuid;
          }
        }
        else if (childNode.Name == "StateStack" && childNode.ChildNodes.Count > 0)
        {
          this.stateStack = new StateStack();
          this.stateStack.LoadFromXML(childNode);
        }
      }
    }

    public IState State => this.state;

    public StateStack Stack => this.stateStack;
  }
}
