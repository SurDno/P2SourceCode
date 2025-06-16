using System.ComponentModel;
using Engine.Common.Binders;

namespace Engine.Common.Weather
{
  [EnumType("WeatherLayer")]
  public enum WeatherLayer
  {
    [Description("Base")] BaseLayer,
    [Description("PlannedEvents")] PlannedEventsLayer,
    [Description("District")] DistrictLayer,
    [Description("CutScene")] CutSceneLayer,
  }
}
