using System.Collections.Generic;

namespace PLVirtualMachine.Common;

public interface IParamListSource {
	List<CommonVariable> SourceParams { get; }
}