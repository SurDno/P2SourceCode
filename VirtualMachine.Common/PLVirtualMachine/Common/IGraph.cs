using System.Collections.Generic;

namespace PLVirtualMachine.Common;

public interface IGraph :
	IContainer,
	IObject,
	IEditorBaseTemplate,
	INamedElement,
	INamed,
	IStaticUpdateable {
	List<ILink> Links { get; }

	List<ILink> GetLinksByDestState(IGraphObject state);

	List<ILink> GetLinksBySourceState(IGraphObject state);
}