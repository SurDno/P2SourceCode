using System;
using System.Linq;
using Cofe.Meta;
using Cofe.Serializations.Converters;
using Engine.Common.Services;
using Engine.Common.Weather;
using Engine.Impl.Services;

namespace Engine.Source.Services.Consoles.Binds
{
  [Initialisable]
  public static class BindWeatherConsoleCommands
  {
    [Initialise]
    private static void Initialise()
    {
      ConsoleTargetService.AddTarget(typeof (WeatherController).Name, value => ServiceLocator.GetService<IWeatherController>());
      ConsoleTargetService.AddTarget("-weather", value => ServiceLocator.GetService<ITemplateService>().GetTemplates<IWeatherSnapshot>().FirstOrDefault(o => o.Name == value));
      ConsoleTargetService.AddTarget("-weather_layer", value => ServiceLocator.GetService<WeatherController>().Blender.Items.ElementAtOrDefault(DefaultConverter.ParseInt(value)));
      SetConsoleCommand.AddBind<IWeatherLayerBlenderItem, float>("weather_opacity", true, (target, value) => target.SetOpacity(value));
      GetConsoleCommand.AddBind<IWeatherLayerBlenderItem, float>("weather_opacity", true, target => target.Opacity);
      SetConsoleCommand.AddBind<WeatherController, bool>("weather_enable", true, (target, value) => target.IsEnabled = value);
      GetConsoleCommand.AddBind<WeatherController, bool>("weather_enable", true, target => target.IsEnabled);
      EnumConsoleCommand.AddBind("-weather", (Func<string>) (() =>
      {
        string str = "\nWeather :\n";
        foreach (IWeatherSnapshot template in ServiceLocator.GetService<ITemplateService>().GetTemplates<IWeatherSnapshot>())
          str = str + template.Name + "\n";
        return str;
      }));
      EnumConsoleCommand.AddBind("-weather_layer", (Func<string>) (() => "\nWeather layer count : " + ServiceLocator.GetService<WeatherController>().Blender.Items.Count()));
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
        layerBlenderItem = ServiceLocator.GetService<WeatherController>().Blender.Items.FirstOrDefault() as IWeatherLayerBlenderItem;
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
      layerBlenderItem.Blender.BlendTo(target, TimeSpan.FromSeconds(num * (double) service.GameTimeFactor));
      return "Set weather snaphot : " + target.Name + " , to layer : " + layerBlenderItem.Name + " , interval : " + num;
    }
  }
}
