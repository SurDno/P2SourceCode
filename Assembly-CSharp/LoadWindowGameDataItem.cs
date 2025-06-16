// Decompiled with JetBrains decompiler
// Type: LoadWindowGameDataItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Connections;
using System;

#nullable disable
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
