// Decompiled with JetBrains decompiler
// Type: Engine.Source.Weather.WeatherLayerBlenderItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Blenders;
using Engine.Common.Generator;
using Engine.Common.Weather;
using Engine.Impl.Services.Factories;
using Engine.Source.Blenders;

#nullable disable
namespace Engine.Source.Weather
{
  [Factory(typeof (IWeatherLayerBlenderItem))]
  [GenerateProxy(TypeEnum.Copyable | TypeEnum.EngineCloneable)]
  public class WeatherLayerBlenderItem : 
    LayerBlenderItem<IWeatherSnapshot>,
    IWeatherLayerBlenderItem,
    ILayerBlenderItem<IWeatherSnapshot>,
    IObject
  {
  }
}
