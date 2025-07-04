﻿using System;
using System.Collections.Generic;
using Cofe.Loggers;
using PLVirtualMachine.Common.Data;

namespace PLVirtualMachine.Common
{
  public class TagDistributionInfo : ITagInfo, IVMStringSerializable
  {
    private List<ITagInfo> tagInfoList = [];
    private bool isDistribInPercentage = true;
    private bool isComplex;
    private int percentageValue = 100;

    public bool Complex => isComplex;

    public List<ITagInfo> TagInfoList => tagInfoList;

    public bool DistribInPercentage => isDistribInPercentage;

    public int Percentage => percentageValue;

    public string Tag => "TagDistribution";

    public void Read(string data)
    {
      switch (data)
      {
        case null:
          Logger.AddError("Attempt to read null tag info data");
          break;
        case "":
          break;
        default:
          if (data.StartsWith("COMPLEX&DISTRIB"))
          {
            isComplex = true;
            isDistribInPercentage = true;
            string str = data.Substring("COMPLEX&DISTRIB".Length);
            string[] separator = [
              "END&DISTRIB"
            ];
            foreach (string data1 in str.Split(separator, StringSplitOptions.RemoveEmptyEntries))
            {
              TagDistributionInfo distributionInfo = new TagDistributionInfo();
              distributionInfo.Read(data1);
              tagInfoList.Add(distributionInfo);
            }
            break;
          }
          isDistribInPercentage = true;
          string[] separator1 = ["END&TAG"];
          string[] strArray = data.Split(separator1, StringSplitOptions.RemoveEmptyEntries);
          for (int index = 0; index < strArray.Length; ++index)
          {
            try
            {
              if (strArray[index].StartsWith("SUB&PERCENT"))
                percentageValue = StringUtility.ToInt32(strArray[index].Substring("SUB&PERCENT".Length));
              else if (strArray[index] == "IN&ABS")
              {
                isDistribInPercentage = false;
              }
              else
              {
                TagInfo tagInfo = new TagInfo();
                tagInfo.Read(strArray[index]);
                tagInfoList.Add(tagInfo);
              }
            }
            catch (Exception ex)
            {
              Logger.AddError(string.Format("{0} isn't valid tag info data, error: {1}", strArray[index], ex));
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
