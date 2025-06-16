using System.Collections.Generic;

namespace PLVirtualMachine.Common.EngineAPI;

public class APIEventInfo {
	public string EventName;
	public bool AtOnce;
	public List<APIParamInfo> MessageParams = new();

	public APIEventInfo(string eventName) {
		EventName = eventName;
	}
}