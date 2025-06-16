using Engine.Common;

namespace Engine.Source.Inventory
{
  public interface IInventoryGridBase : IObject
  {
    int Rows { get; set; }

    int Columns { get; set; }
  }
}
