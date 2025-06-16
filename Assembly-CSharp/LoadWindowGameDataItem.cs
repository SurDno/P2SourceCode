using System;
using Engine.Source.Connections;

[Serializable]
public struct LoadWindowGameDataItem
{
  public string GameDataName;
  public IInventoryPlaceholderSerializable StartStorable;
  public string StartTooltip;
  public LoadWindowStorableData LoadStorables;
  public LoadWindowStringData LoadTooltips;

  public bool IsNull => GameDataName == null;

  public static LoadWindowGameDataItem Null => new LoadWindowGameDataItem();
}
