using Cofe.Meta;
using Cofe.Serializations.Converters;
using Engine.Common.Blenders;
using Engine.Common.Services;
using Engine.Common.Weather;
using Engine.Impl.Services;
using System;
using System.Linq;

namespace Engine.Source.Services.Consoles.Binds
{
  [Initialisable]
  public static class BindWeatherConsoleCommands
  {
    [Cofe.Meta.Initialise]
    private static void Initialise()
    {
      ConsoleTargetService.AddTarget(typeof (WeatherController).Name, (Func<string, object>) (value => (object) ServiceLocator.GetService<IWeatherController>()));
      ConsoleTargetService.AddTarget("-weather", (Func<string, object>) (value => (object) ServiceLocator.GetService<ITemplateService>().GetTemplates<IWeatherSnapshot>().FirstOrDefault<IWeatherSnapshot>((Func<IWeatherSnapshot, bool>) (o => o.Name == value))));
      ConsoleTargetService.AddTarget("-weather_layer", (Func<string, object>) (value => (object) ServiceLocator.GetService<WeatherController>().Blender.Items.ElementAtOrDefault<ILayerBlenderItem<IWeatherSnapshot>>(DefaultConverter.ParseInt(value))));
      SetConsoleCommand.AddBind<IWeatherLayerBlenderItem, float>("weather_opacity", true, (Action<IWeatherLayerBlenderItem, float>) ((target, value) => target.SetOpacity(value)));
      GetConsoleCommand.AddBind<IWeatherLayerBlenderItem, float>("weather_opacity", true, (Func<IWeatherLayerBlenderItem, float>) (target => target.Opacity));
      SetConsoleCommand.AddBind<WeatherController, bool>("weather_enable", true, (Action<WeatherController, bool>) ((target, value) => target.IsEnabled = value));
      GetConsoleCommand.AddBind<WeatherController, bool>("weather_enable", true, (Func<WeatherController, bool>) (target => target.IsEnabled));
      EnumConsoleCommand.AddBind("-weather", (Func<string>) (() =>
      {
        string str = "\nWeather :\n";
        foreach (IWeatherSnapshot template in ServiceLocator.GetService<ITemplateService>().GetTemplates<IWeatherSnapshot>())
          str = str + template.Name + "\n";
        return str;
      }));
      EnumConsoleCommand.AddBind("-weather_layer", (Func<string>) (() => "\nWeather layer count : " + (object) ServiceLocator.GetService<WeatherController>().Blender.Items.Count<ILayerBlenderItem<IWeatherSnapshot>>()));
    }

    [ConsoleCommand("change_weather")]
    private static string WeatherCommand(string command, ConsoleParameter[] parameters)
    {
      if (parameters.Length == 0 || parameters.Length == 1 && parameters[0].Value == "?")
        return command + " [-weather_layer:index] -weather:snapshot interval";
      IWeatherLayerBlenderItem layerBlenderItem;
      IWeatherSnapshot target;
      float num;
      if (parameters.Length == 2)
      {
        layerBlenderItem = ServiceLocator.GetService<WeatherController>().Blender.Items.FirstOrDefault<ILayerBlenderItem<IWeatherSnapshot>>() as IWeatherLayerBlenderItem;
        target = ConsoleTargetService.GetTarget(typeof (IWeatherSnapshot), parameters[0]) as IWeatherSnapshot;
        num = DefaultConverter.ParseFloat(parameters[1].Value);
      }
      else
      {
        if (parameters.Length != 3)
          return "Error parameter count";
        layerBlenderItem = ConsoleTargetService.GetTarget(typeof (IWeatherLayerBlenderItem), parameters[0]) as IWeatherLayerBlenderItem;
        target = ConsoleTargetService.GetTarget(typeof (IWeatherSnapshot), parameters[1]) as IWeatherSnapshot;
        num = DefaultConverter.ParseFloat(parameters[2].Value);
      }
      if (layerBlenderItem == null)
        return "Layer not found";
      if (target == null)
        return "Snapshot not found";
      ITimeService service = ServiceLocator.GetService<ITimeService>();
      layerBlenderItem.Blender.BlendTo(target, TimeSpan.FromSeconds((double) num * (double) service.GameTimeFactor));
      return "Set weather snaphot : " + target.Name + " , to layer : " + layerBlenderItem.Name + " , interval : " + (object) num;
    }
  }
}
