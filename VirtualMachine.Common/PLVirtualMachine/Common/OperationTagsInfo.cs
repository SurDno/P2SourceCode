using Cofe.Loggers;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using System;
using System.Collections.Generic;

namespace PLVirtualMachine.Common
{
  public class OperationTagsInfo : IVMStringSerializable
  {
    protected List<string> tagsList = new List<string>();

    public void AddTag(string sTag) => this.tagsList.Add(sTag);

    public List<string> TagsList => this.tagsList;

    public bool CheckTag(string sTag) => this.tagsList.Count <= 0 || this.tagsList.Contains(sTag);

    protected virtual void ReadTag(string tagDataStr)
    {
      if (!(tagDataStr != "&OP&AND&"))
        return;
      this.tagsList.Add(tagDataStr);
    }

    public void Read(string data)
    {
      switch (data)
      {
        case null:
          Logger.AddError(string.Format("Attempt to read null operation tags info at {0}", (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
          break;
        case "":
          break;
        case "0":
          break;
        default:
          this.tagsList.Clear();
          string[] separator = new string[1]{ ";" };
          foreach (string tagDataStr in data.Split(separator, StringSplitOptions.RemoveEmptyEntries))
            this.ReadTag(tagDataStr);
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
