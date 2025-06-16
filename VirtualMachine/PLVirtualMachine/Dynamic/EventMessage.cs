using Cofe.Serializations.Data;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.Serialization;
using PLVirtualMachine.Data.SaveLoad;
using System.Xml;

namespace PLVirtualMachine.Dynamic
{
  public class EventMessage : 
    BaseMessage,
    IParam,
    IVariable,
    INamed,
    ISerializeStateSave,
    IDynamicLoadSerializable
  {
    private object messageValue;

    public EventMessage() => this.Initialize("simple_mesage", new VMType(typeof (object)));

    public void Initialize(BaseMessage staticMessage, object value)
    {
      this.Initialize(staticMessage.Name, staticMessage.Type);
      this.messageValue = value;
    }

    public void Initialize(string name, VMType type, object value)
    {
      this.Initialize(name, type);
      this.messageValue = value;
    }

    public void Copy(EventMessage copyMessage)
    {
      this.Initialize(copyMessage.Name, copyMessage.Type);
      this.messageValue = copyMessage.Value;
    }

    public object Value
    {
      get => this.messageValue;
      set => this.messageValue = value;
    }

    public bool Implicit => false;

    public void StateSave(IDataWriter writer)
    {
      VMType vmType = this.Type ?? new VMType(typeof (object));
      SaveManagerUtility.Save(writer, "Name", this.Name);
      SaveManagerUtility.SaveSerializable(writer, "Type", (IVMStringSerializable) vmType);
      SaveManagerUtility.SaveCommon(writer, "Value", this.messageValue);
    }

    public void LoadFromXML(XmlElement objNode)
    {
      XmlNode valueNode = (XmlNode) null;
      string name = (string) null;
      VMType type = (VMType) null;
      this.messageValue = (object) null;
      for (int i = 0; i < objNode.ChildNodes.Count; ++i)
      {
        if (objNode.ChildNodes[i].Name == "Name")
          name = objNode.ChildNodes[i].InnerText;
        else if (objNode.ChildNodes[i].Name == "Type")
          type = VMSaveLoadManager.ReadValue<VMType>(objNode.ChildNodes[i]);
        else if (objNode.ChildNodes[i].Name == "Value")
          valueNode = objNode.ChildNodes[i];
      }
      this.Initialize(name, type);
      if (valueNode == null)
        return;
      this.messageValue = VMSaveLoadManager.ReadValue(valueNode, this.Type.BaseType);
    }

    public IGameObjectContext OwnerContext => (IGameObjectContext) null;
  }
}
