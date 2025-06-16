// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.TagInfo
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Cofe.Loggers;
using PLVirtualMachine.Common.Data;
using System;

#nullable disable
namespace PLVirtualMachine.Common
{
  public class TagInfo : ITagInfo, IVMStringSerializable
  {
    private string tagStr;
    private int percentageValue;

    public string Tag => this.tagStr;

    public int Percentage => this.percentageValue;

    public void Read(string data)
    {
      switch (data)
      {
        case null:
          Logger.AddError(string.Format("Attempt to read null tag info data"));
          break;
        case "":
          break;
        case "0":
          break;
        default:
          string[] separator = new string[1]{ "&=&" };
          string[] strArray = data.Split(separator, StringSplitOptions.RemoveEmptyEntries);
          if (strArray.Length < 2)
          {
            Logger.AddError(string.Format("Tags distribution info data read error: {0} isn't full tag info data", (object) data));
            break;
          }
          this.tagStr = strArray[0];
          this.percentageValue = StringUtility.ToInt32(strArray[1]);
          break;
      }
    }

    public string Write()
    {
      Logger.AddError("Not allowed serialization data struct in virtual machine!");
      return string.Empty;
    }
  }
}
