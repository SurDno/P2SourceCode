using Engine.Source.Services;
using Inspectors;
using UnityEngine;

namespace Engine.Services
{
  [GameService(new System.Type[] {typeof (SelectionService)})]
  public class SelectionService
  {
    [Inspected]
    private object[] selections = new object[10];

    public int SelectionCount => this.selections.Length;

    public void SetSelection(int index, object selection)
    {
      if (index < 0 || index >= this.selections.Length)
        Debug.LogWarning((object) "Selection index out of range");
      else
        this.selections[index] = selection;
    }

    public object GetSelection(int index)
    {
      if (index >= 0 && index < this.selections.Length)
        return this.selections[index];
      Debug.LogWarning((object) "Selection index out of range");
      return (object) null;
    }
  }
}
