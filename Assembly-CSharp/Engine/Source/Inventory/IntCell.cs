using Inspectors;

namespace Engine.Source.Inventory;

public struct IntCell {
	[Inspected(Header = true)] public int Column;
	[Inspected(Header = true)] public int Row;
}