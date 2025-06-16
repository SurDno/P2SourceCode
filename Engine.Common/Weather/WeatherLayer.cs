using Engine.Common.Binders;
using System.ComponentModel;

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
