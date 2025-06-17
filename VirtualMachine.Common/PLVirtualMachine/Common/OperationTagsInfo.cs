using System;
using System.Collections.Generic;
using Cofe.Loggers;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;

namespace PLVirtualMachine.Common
{
  public class OperationTagsInfo : IVMStringSerializable
  {
    protected List<string> tagsList = [];

    public void AddTag(string sTag) => tagsList.Add(sTag);

    public List<string> TagsList => tagsList;

    public bool CheckTag(string sTag) => tagsList.Count <= 0 || tagsList.Contains(sTag);

    protected virtual void ReadTag(string tagDataStr)
    {
      if (!(tagDataStr != "&OP&AND&"))
        return;
      tagsList.Add(tagDataStr);
    }

    public void Read(string data)
    {
      switch (data)
      {
        case null:
          Logger.AddError(string.Format("Attempt to read null operation tags info at {0}", EngineAPIManager.Instance.CurrentFSMStateInfo));
          break;
        case "":
          break;
        case "0":
          break;
        default:
          tagsList.Clear();
          string[] separator = [";"];
          foreach (string tagDataStr in data.Split(separator, StringSplitOptions.RemoveEmptyEntries))
            ReadTag(tagDataStr);
          break;
      }
    }

    public virtual string Write()
    {
      Logger.AddError("Not allowed serialization data struct in virtual machine!");
      return string.Empty;
    }
  }
}
