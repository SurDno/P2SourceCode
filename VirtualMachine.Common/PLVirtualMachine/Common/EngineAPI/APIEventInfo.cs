using System.Collections.Generic;

namespace PLVirtualMachine.Common.EngineAPI
{
  public class APIEventInfo(string eventName) {
    public string EventName = eventName;
    public bool AtOnce;
    public List<APIParamInfo> MessageParams = [];
  }
}
