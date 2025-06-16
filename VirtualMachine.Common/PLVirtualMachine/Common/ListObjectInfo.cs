using System.Xml;
using Cofe.Loggers;
using Cofe.Serializations.Data;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.Serialization;

namespace PLVirtualMachine.Common
{
  public class ListObjectInfo : ISerializeStateSave, IDynamicLoadSerializable, IVMStringSerializable
  {
    public string ObjectInfoStr = "";
    public object ObjectValue;
    public VMType ObjectType;

    public ListObjectInfo()
    {
      ObjectInfoStr = "";
      ObjectValue = null;
      ObjectType = null;
    }

    public ListObjectInfo(ListObjectInfo listObjInfo)
    {
      ObjectInfoStr = listObjInfo.ObjectInfoStr;
      ObjectValue = listObjInfo.ObjectValue;
      ObjectType = listObjInfo.ObjectType;
    }

    public ListObjectInfo(object obj)
    {
      if (obj is CommonVariable)
      {
        ObjectInfoStr = ((CommonVariable) obj).Write();
        ObjectValue = null;
        ObjectType = null;
      }
      else
      {
        ObjectValue = obj;
        if (typeof (IRef).IsAssignableFrom(ObjectValue.GetType()))
          ObjectType = ((IVariable) obj).Type;
        else if (typeof (IVariable).IsAssignableFrom(ObjectValue.GetType()))
          ObjectType = ((IVariable) ObjectValue).Type;
        else
          ObjectType = new VMType(ObjectValue.GetType());
      }
    }

    public ListObjectInfo(object obj, VMType type)
    {
      ObjectInfoStr = "";
      ObjectValue = obj;
      ObjectType = type;
    }

    public void Read(string data)
    {
      if (data.StartsWith("value_"))
      {
        string[] strArray = data.Split('_');
        if (strArray.Length != 4)
          return;
        if (strArray[3] != "")
          ObjectType = VMTypePool.GetType(strArray[3]);
        if (null != ObjectType.BaseType)
        {
          ObjectValue = StringSerializer.ReadValue(strArray[1], ObjectType.BaseType);
        }
        else
        {
          ObjectType = new VMType(typeof (object));
          ObjectValue = null;
        }
      }
      else
        ObjectInfoStr = data;
    }

    public string Write()
    {
      Logger.AddError("Not allowed serialization data struct in virtual machine!");
      return string.Empty;
    }

    public void StateSave(IDataWriter writer)
    {
      SaveManagerUtility.Save(writer, "ObjectInfoStr", ObjectInfoStr);
      SaveManagerUtility.SaveSerializable(writer, "ObjectType", ObjectType);
      SaveManagerUtility.SaveCommon(writer, "ObjectValue", ObjectValue);
    }

    public void LoadFromXML(XmlElement xmlNode)
    {
      XmlNode xmlNode1 = null;
      foreach (XmlElement childNode in xmlNode.ChildNodes)
      {
        if (childNode.Name == "ObjectInfoStr")
          ObjectInfoStr = childNode.InnerText;
        else if (childNode.Name == "ObjectType")
          ObjectType = VMTypePool.GetType(childNode.InnerText);
        else if (childNode.Name == "ObjectValue")
          xmlNode1 = childNode;
      }
      if (xmlNode1 == null)
        return;
      if (null != ObjectType.BaseType)
      {
        ObjectValue = StringSerializer.ReadValue(xmlNode1.InnerText, ObjectType.BaseType);
      }
      else
      {
        ObjectType = new VMType(typeof (object));
        ObjectValue = null;
      }
    }

    public bool IsLoaded => ObjectValue != null || ObjectType != null;
  }
}
