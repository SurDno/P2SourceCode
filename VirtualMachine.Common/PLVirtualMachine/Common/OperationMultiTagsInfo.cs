using System.Collections.Generic;
using Cofe.Loggers;

namespace PLVirtualMachine.Common
{
  public class OperationMultiTagsInfo : OperationTagsInfo
  {
    private ETagOpType tagOperationType;

    public override string Write()
    {
      Logger.AddError("Not allowed serialization data struct in virtual machine!");
      return string.Empty;
    }

    public ETagOpType TagOpType => tagOperationType;

    public bool CheckTags(List<string> existTags)
    {
      for (int index = 0; index < tagsList.Count; ++index)
      {
        if (existTags.Contains(tagsList[index]))
        {
          if (TagOpType == ETagOpType.TAG_OP_TYPE_OR)
            return true;
        }
        else if (TagOpType == ETagOpType.TAG_OP_TYPE_AND)
          return false;
      }
      return TagOpType == ETagOpType.TAG_OP_TYPE_AND;
    }

    protected override void ReadTag(string sTagData)
    {
      if (sTagData != "&OP&AND&")
        tagsList.Add(sTagData);
      else
        tagOperationType = ETagOpType.TAG_OP_TYPE_AND;
    }
  }
}
