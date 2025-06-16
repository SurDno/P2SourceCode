// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EMMNodeContentType
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using System.ComponentModel;

#nullable disable
namespace PLVirtualMachine.Common
{
  public enum EMMNodeContentType
  {
    [Description("Information")] NODE_CONTENT_TYPE_INFO,
    [Description("Failure")] NODE_CONTENT_TYPE_FAILURE,
    [Description("Success")] NODE_CONTENT_TYPE_SUCCESS,
    [Description("Knowledge")] NODE_CONTENT_TYPE_KNOWLEDGE,
    [Description("IsolatedFact")] NODE_CONTENT_TYPE_ISOLATEDFACT,
  }
}
