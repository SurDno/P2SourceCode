using Engine.Source.Behaviours;

namespace StateSetters
{
  [StateSetter("{5cdc9818-ebc3-48b6-9e66-821c9641d3bf}")]
  public class ValueItemController : IStateSetterItemController
  {
    public void Apply(StateSetterItem item, float value)
    {
      Object objectValue1 = item.ObjectValue1;
      if (objectValue1 == (Object) null || !(objectValue1 is IValueController valueController))
        return;
      valueController.Value = value;
    }
  }
}
