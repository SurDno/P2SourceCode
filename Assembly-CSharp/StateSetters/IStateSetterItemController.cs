namespace StateSetters;

public interface IStateSetterItemController {
	void Apply(StateSetterItem item, float value);
}