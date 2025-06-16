// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.CombinationItemParams
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
  public class CombinationItemParams : 
    ISerializeStateSave,
    IDynamicLoadSerializable,
    IVMStringSerializable
  {
    private int minDurablityProc = 100;
    private int maxDurablityProc = 100;

    public CombinationItemParams()
    {
    }

    public CombinationItemParams(CombinationItemParams combItemParams)
    {
      this.minDurablityProc = combItemParams.minDurablityProc;
      this.maxDurablityProc = combItemParams.maxDurablityProc;
    }

    public int MinDurablityProc => this.minDurablityProc;

    public int MaxDurablityProc => this.maxDurablityProc;

    public void Read(string data)
    {
      switch (data)
      {
        case null:
          Logger.AddError(string.Format("Attempt to read null combination item params data at {0}", (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
          break;
        case "":
          break;
        default:
          string[] separator = new string[1]
          {
            "&CI&PARAMS&"
          };
          string[] strArray = data.Split(separator, StringSplitOptions.RemoveEmptyEntries);
          if (strArray.Length != 0)
          {
            this.minDurablityProc = StringUtility.ToInt32(strArray[0]);
            if (strArray.Length <= 1)
              break;
            this.maxDurablityProc = StringUtility.ToInt32(strArray[1]);
            break;
          }
          Logger.AddError(string.Format("Invalid combination item params data: {0}", (object) data));
          break;
      }
    }

    public string Write()
    {
      Logger.AddError("Not allowed serialization data struct in virtual machine!");
      return string.Empty;
    }

    public void StateSave(IDataWriter writer)
    {
      SaveManagerUtility.Save(writer, "MinDurablityProc", this.minDurablityProc);
      SaveManagerUtility.Save(writer, "MaxDurablityProc", this.maxDurablityProc);
    }

    public void LoadFromXML(XmlElement xmlNode)
    {
      foreach (XmlElement childNode in xmlNode.ChildNodes)
      {
        if (childNode.Name == "MinDurablityProc")
          this.minDurablityProc = StringUtility.ToInt32(childNode.InnerText);
        else if (childNode.Name == "MaxDurablityProc")
          this.maxDurablityProc = StringUtility.ToInt32(childNode.InnerText);
      }
    }
  }
}
