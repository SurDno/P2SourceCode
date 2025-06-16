using System.ComponentModel;

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
