public interface ISettingEntity : ISelectable {
	bool Interactable { get; set; }

	void OnSelect();

	void OnDeSelect();

	void IncrementValue();

	void DecrementValue();

	bool IsActive();
}