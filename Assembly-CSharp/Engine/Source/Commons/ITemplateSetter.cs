using Engine.Common;

namespace Engine.Source.Commons;

public interface ITemplateSetter {
	IObject Template { set; }

	bool IsTemplate { set; }
}