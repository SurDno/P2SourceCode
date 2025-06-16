using System;

namespace Cinemachine;

[DocumentationSorting(0.0f, Level.Undoc)]
public sealed class DocumentationSortingAttribute : Attribute {
	public float SortOrder { get; private set; }

	public Level Category { get; private set; }

	public DocumentationSortingAttribute(
		float sortOrder,
		Level category) {
		SortOrder = sortOrder;
		Category = category;
	}

	public enum Level {
		Undoc,
		API,
		UserRef
	}
}