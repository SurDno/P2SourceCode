using Engine.Common;

public interface IStorableTooltipComponent {
	StorableTooltipInfo GetInfo(IEntity owner);

	bool IsEnabled { get; }
}