using System.Xml;
using Cofe.Serializations.Data;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.Serialization;
using PLVirtualMachine.Data.SaveLoad;

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

    public EventMessage() => Initialize("simple_mesage", new VMType(typeof (object)));

    public void Initialize(BaseMessage staticMessage, object value)
    {
      Initialize(staticMessage.Name, staticMessage.Type);
      messageValue = value;
    }

    public void Initialize(string name, VMType type, object value)
    {
      Initialize(name, type);
      messageValue = value;
    }

    public void Copy(EventMessage copyMessage)
    {
      Initialize(copyMessage.Name, copyMessage.Type);
      messageValue = copyMessage.Value;
    }

    public object Value
    {
      get => messageValue;
      set => messageValue = value;
    }

    public bool Implicit => false;

    public void StateSave(IDataWriter writer)
    {
      VMType vmType = Type ?? new VMType(typeof (object));
      SaveManagerUtility.Save(writer, "Name", Name);
      SaveManagerUtility.SaveSerializable(writer, "Type", vmType);
      SaveManagerUtility.SaveCommon(writer, "Value", messageValue);
    }

    public void LoadFromXML(XmlElement objNode)
    {
      XmlNode valueNode = null;
      string name = null;
      VMType type = null;
      messageValue = null;
      for (int i = 0; i < objNode.ChildNodes.Count; ++i)
      {
        if (objNode.ChildNodes[i].Name == "Name")
          name = objNode.ChildNodes[i].InnerText;
        else if (objNode.ChildNodes[i].Name == "Type")
          type = VMSaveLoadManager.ReadValue<VMType>(objNode.ChildNodes[i]);
        else if (objNode.ChildNodes[i].Name == "Value")
          valueNode = objNode.ChildNodes[i];
      }
      Initialize(name, type);
      if (valueNode == null)
        return;
      messageValue = VMSaveLoadManager.ReadValue(valueNode, Type.BaseType);
    }

    public IGameObjectContext OwnerContext => null;
  }
}
