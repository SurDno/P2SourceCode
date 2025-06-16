using System.ComponentModel;
using Engine.Common.Binders;

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
