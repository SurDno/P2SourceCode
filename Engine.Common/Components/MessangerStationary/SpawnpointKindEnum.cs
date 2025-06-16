// Decompiled with JetBrains decompiler
// Type: Engine.Common.Components.MessangerStationary.SpawnpointKindEnum
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using Engine.Common.Binders;
using System.ComponentModel;

#nullable disable
namespace Engine.Common.Components.MessangerStationary
{
  [EnumType("SpawnpointKindEnum")]
  public enum SpawnpointKindEnum
  {
    [Description("None")] None,
    [Description("Open")] Open,
    [Description("Hidden")] Hidden,
    [Description("Open2")] Open2,
    [Description("OpenPaired1")] OpenPaired1,
    [Description("OpenPaired2")] OpenPaired2,
    [Description("Observer")] Observer,
    [Description("TragediansWalk")] TragediansWalk,
  }
}
