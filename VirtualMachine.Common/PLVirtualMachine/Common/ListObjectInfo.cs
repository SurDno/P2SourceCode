// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.ListObjectInfo
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Cofe.Loggers;
using Cofe.Serializations.Data;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.Serialization;
using System;
using System.Xml;

#nullable disable
namespace PLVirtualMachine.Common
{
  public class ListObjectInfo : ISerializeStateSave, IDynamicLoadSerializable, IVMStringSerializable
  {
    public string ObjectInfoStr = "";
    public object ObjectValue;
    public VMType ObjectType;

    public ListObjectInfo()
    {
      this.ObjectInfoStr = "";
      this.ObjectValue = (object) null;
      this.ObjectType = (VMType) null;
    }

    public ListObjectInfo(ListObjectInfo listObjInfo)
    {
      this.ObjectInfoStr = listObjInfo.ObjectInfoStr;
      this.ObjectValue = listObjInfo.ObjectValue;
      this.ObjectType = listObjInfo.ObjectType;
    }

    public ListObjectInfo(object obj)
    {
      if (obj is CommonVariable)
      {
        this.ObjectInfoStr = ((CommonVariable) obj).Write();
        this.ObjectValue = (object) null;
        this.ObjectType = (VMType) null;
      }
      else
      {
        this.ObjectValue = obj;
        if (typeof (IRef).IsAssignableFrom(this.ObjectValue.GetType()))
          this.ObjectType = ((IVariable) obj).Type;
        else if (typeof (IVariable).IsAssignableFrom(this.ObjectValue.GetType()))
          this.ObjectType = ((IVariable) this.ObjectValue).Type;
        else
          this.ObjectType = new VMType(this.ObjectValue.GetType());
      }
    }

    public ListObjectInfo(object obj, VMType type)
    {
      this.ObjectInfoStr = "";
      this.ObjectValue = obj;
      this.ObjectType = type;
    }

    public void Read(string data)
    {
      if (data.StartsWith("value_"))
      {
        string[] strArray = data.Split('_');
        if (strArray.Length != 4)
          return;
        if (strArray[3] != "")
          this.ObjectType = VMTypePool.GetType(strArray[3]);
        if ((Type) null != this.ObjectType.BaseType)
        {
          this.ObjectValue = PLVirtualMachine.Common.Data.StringSerializer.ReadValue(strArray[1], this.ObjectType.BaseType);
        }
        else
        {
          this.ObjectType = new VMType(typeof (object));
          this.ObjectValue = (object) null;
        }
      }
      else
        this.ObjectInfoStr = data;
    }

    public string Write()
    {
      Logger.AddError("Not allowed serialization data struct in virtual machine!");
      return string.Empty;
    }

    public void StateSave(IDataWriter writer)
    {
      SaveManagerUtility.Save(writer, "ObjectInfoStr", this.ObjectInfoStr);
      SaveManagerUtility.SaveSerializable(writer, "ObjectType", (IVMStringSerializable) this.ObjectType);
      SaveManagerUtility.SaveCommon(writer, "ObjectValue", this.ObjectValue);
    }

    public void LoadFromXML(XmlElement xmlNode)
    {
      XmlNode xmlNode1 = (XmlNode) null;
      foreach (XmlElement childNode in xmlNode.ChildNodes)
      {
        if (childNode.Name == "ObjectInfoStr")
          this.ObjectInfoStr = childNode.InnerText;
        else if (childNode.Name == "ObjectType")
          this.ObjectType = VMTypePool.GetType(childNode.InnerText);
        else if (childNode.Name == "ObjectValue")
          xmlNode1 = (XmlNode) childNode;
      }
      if (xmlNode1 == null)
        return;
      if ((Type) null != this.ObjectType.BaseType)
      {
        this.ObjectValue = PLVirtualMachine.Common.Data.StringSerializer.ReadValue(xmlNode1.InnerText, this.ObjectType.BaseType);
      }
      else
      {
        this.ObjectType = new VMType(typeof (object));
        this.ObjectValue = (object) null;
      }
    }

    public bool IsLoaded => this.ObjectValue != null || this.ObjectType != null;
  }
}
