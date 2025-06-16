using Cofe.Loggers;
using PLVirtualMachine.Common.Data;
using System;

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
