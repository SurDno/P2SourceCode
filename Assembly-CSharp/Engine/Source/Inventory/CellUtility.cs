// Decompiled with JetBrains decompiler
// Type: Engine.Source.Inventory.CellUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;

#nullable disable
namespace Engine.Source.Inventory
{
  public static class CellUtility
  {
    public static IntCell To(this Cell cell)
    {
      return new IntCell()
      {
        Column = cell.Column,
        Row = cell.Row
      };
    }

    public static Cell To(this IntCell vector)
    {
      Cell cell = ProxyFactory.Create<Cell>();
      cell.Column = vector.Column;
      cell.Row = vector.Row;
      return cell;
    }
  }
}
