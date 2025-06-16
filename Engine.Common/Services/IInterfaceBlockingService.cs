namespace Engine.Common.Services;

public interface IInterfaceBlockingService {
	bool BlockMapInterface { get; set; }

	bool BlockMindMapInterface { get; set; }

	bool BlockInventoryInterface { get; set; }

	bool BlockStatsInterface { get; set; }

	bool BlockBoundsInterface { get; set; }
}