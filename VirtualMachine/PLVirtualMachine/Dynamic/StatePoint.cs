// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Dynamic.StatePoint
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Loggers;
using Cofe.Serializations.Data;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.Serialization;
using PLVirtualMachine.Data.SaveLoad;
using System.Xml;

#nullable disable
namespace PLVirtualMachine.Dynamic
{
  public class StatePoint : ISerializeStateSave, IDynamicLoadSerializable
  {
    public IState currentState;
    public StateInfo prevState;

    public StatePoint()
    {
    }

    public StatePoint(IState current, IState prev)
    {
      this.currentState = current;
      this.prevState = new StateInfo(prev);
    }

    public StatePoint(IState current, StateStack prev)
    {
      this.currentState = current;
      this.prevState = new StateInfo(prev);
    }

    public StatePoint(IState current)
    {
      this.currentState = current;
      this.prevState = (StateInfo) null;
    }

    public void StateSave(IDataWriter writer)
    {
      SaveManagerUtility.Save(writer, "CurrentState", this.currentState != null ? this.currentState.BaseGuid : 0UL);
      SaveManagerUtility.Save(writer, "CurrentStateName", this.currentState != null ? this.currentState.Name : "");
      SaveManagerUtility.SaveDynamicSerializable(writer, "PrevStateInfo", (ISerializeStateSave) this.prevState);
    }

    public void LoadFromXML(XmlElement xmlNode)
    {
      for (int i = 0; i < xmlNode.ChildNodes.Count; ++i)
      {
        XmlElement childNode = (XmlElement) xmlNode.ChildNodes[i];
        if (childNode.Name == "CurrentState")
        {
          ulong id = VMSaveLoadManager.ReadUlong((XmlNode) childNode);
          if (id != 0UL)
          {
            IObject objectByGuid = IStaticDataContainer.StaticDataContainer.GetObjectByGuid(id);
            if (objectByGuid != null)
              this.currentState = (IState) objectByGuid;
            else
              Logger.AddError(string.Format("Saveload error: loading state id={0} not found", (object) id));
          }
        }
        else if (childNode.Name == "PrevStateInfo" && childNode.ChildNodes.Count > 0)
        {
          this.prevState = new StateInfo();
          VMSaveLoadManager.LoadDynamicSerializable(childNode, (IDynamicLoadSerializable) this.prevState);
        }
      }
    }

    public bool Modified => true;

    public void OnModify()
    {
    }
  }
}
