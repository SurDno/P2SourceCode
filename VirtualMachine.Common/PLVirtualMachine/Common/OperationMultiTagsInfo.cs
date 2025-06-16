// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.OperationMultiTagsInfo
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Cofe.Loggers;
using System.Collections.Generic;

#nullable disable
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

    public ETagOpType TagOpType => this.tagOperationType;

    public bool CheckTags(List<string> existTags)
    {
      for (int index = 0; index < this.tagsList.Count; ++index)
      {
        if (existTags.Contains(this.tagsList[index]))
        {
          if (this.TagOpType == ETagOpType.TAG_OP_TYPE_OR)
            return true;
        }
        else if (this.TagOpType == ETagOpType.TAG_OP_TYPE_AND)
          return false;
      }
      return this.TagOpType == ETagOpType.TAG_OP_TYPE_AND;
    }

    protected override void ReadTag(string sTagData)
    {
      if (sTagData != "&OP&AND&")
        this.tagsList.Add(sTagData);
      else
        this.tagOperationType = ETagOpType.TAG_OP_TYPE_AND;
    }
  }
}
