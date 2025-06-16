namespace Engine.Common.Services;

public interface IBackerUnlocksService {
	bool ItemUnlocked { get; }

	bool QuestUnlocked { get; }

	bool PolyhedralRoomUnlocked { get; }
}