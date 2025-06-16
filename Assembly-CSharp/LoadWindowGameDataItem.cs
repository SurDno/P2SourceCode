using Engine.Source.Connections;
using System;

[Serializable]
public struct LoadWindowGameDataItem
{
  public string GameDataName;
  public IInventoryPlaceholderSerializable StartStorable;
  public string StartTooltip;
  public LoadWindowStorableData LoadStorables;
  public LoadWindowStringData LoadTooltips;

  public bool IsNull => this.GameDataName == null;

  public static LoadWindowGameDataItem Null => new LoadWindowGameDataItem();
}
