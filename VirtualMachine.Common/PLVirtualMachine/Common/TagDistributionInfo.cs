using Cofe.Loggers;
using PLVirtualMachine.Common.Data;
using System;
using System.Collections.Generic;

namespace PLVirtualMachine.Common
{
  public class TagDistributionInfo : ITagInfo, IVMStringSerializable
  {
    private List<ITagInfo> tagInfoList = new List<ITagInfo>();
    private bool isDistribInPercentage;
    private bool isComplex;
    private int percentageValue = 100;

    public TagDistributionInfo() => this.isDistribInPercentage = true;

    public bool Complex => this.isComplex;

    public List<ITagInfo> TagInfoList => this.tagInfoList;

    public bool DistribInPercentage => this.isDistribInPercentage;

    public int Percentage => this.percentageValue;

    public string Tag => "TagDistribution";

    public void Read(string data)
    {
      switch (data)
      {
        case null:
          Logger.AddError(string.Format("Attempt to read null tag info data"));
          break;
        case "":
          break;
        default:
          if (data.StartsWith("COMPLEX&DISTRIB"))
          {
            this.isComplex = true;
            this.isDistribInPercentage = true;
            string str = data.Substring("COMPLEX&DISTRIB".Length);
            string[] separator = new string[1]
            {
              "END&DISTRIB"
            };
            foreach (string data1 in str.Split(separator, StringSplitOptions.RemoveEmptyEntries))
            {
              TagDistributionInfo distributionInfo = new TagDistributionInfo();
              distributionInfo.Read(data1);
              this.tagInfoList.Add((ITagInfo) distributionInfo);
            }
            break;
          }
          this.isDistribInPercentage = true;
          string[] separator1 = new string[1]{ "END&TAG" };
          string[] strArray = data.Split(separator1, StringSplitOptions.RemoveEmptyEntries);
          for (int index = 0; index < strArray.Length; ++index)
          {
            try
            {
              if (strArray[index].StartsWith("SUB&PERCENT"))
                this.percentageValue = StringUtility.ToInt32(strArray[index].Substring("SUB&PERCENT".Length));
              else if (strArray[index] == "IN&ABS")
              {
                this.isDistribInPercentage = false;
              }
              else
              {
                TagInfo tagInfo = new TagInfo();
                tagInfo.Read(strArray[index]);
                this.tagInfoList.Add((ITagInfo) tagInfo);
              }
            }
            catch (Exception ex)
            {
              Logger.AddError(string.Format("{0} isn't valid tag info data, error: {1}", (object) strArray[index], (object) ex));
            }
          }
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
