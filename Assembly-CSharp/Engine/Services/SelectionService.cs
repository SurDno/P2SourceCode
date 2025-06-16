using Engine.Source.Services;
using Inspectors;

namespace Engine.Services
{
  [GameService(typeof (SelectionService))]
  public class SelectionService
  {
    [Inspected]
    private object[] selections = new object[10];

    public int SelectionCount => selections.Length;

    public void SetSelection(int index, object selection)
    {
      if (index < 0 || index >= selections.Length)
        Debug.LogWarning((object) "Selection index out of range");
      else
        selections[index] = selection;
    }

    public object GetSelection(int index)
    {
      if (index >= 0 && index < selections.Length)
        return selections[index];
      Debug.LogWarning((object) "Selection index out of range");
      return null;
    }
  }
}
