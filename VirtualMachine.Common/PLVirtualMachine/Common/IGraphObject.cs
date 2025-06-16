using System.Collections.Generic;

namespace PLVirtualMachine.Common;

public interface IGraphObject :
	IContainer,
	IObject,
	IEditorBaseTemplate,
	INamedElement,
	INamed,
	IStaticUpdateable {
	List<ILink> OutputLinks { get; }
}