// Decompiled with JetBrains decompiler
// Type: Engine.Impl.Weather.WeatherSnapshot
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Engine.Common;
using Engine.Common.Blenders;
using Engine.Common.Generator;
using Engine.Common.Weather;
using Engine.Impl.Services.Factories;
using Engine.Impl.Weather.Element;
using Engine.Source.Commons;
using Inspectors;

#nullable disable
namespace Engine.Impl.Weather
{
  [Factory(typeof (IWeatherSnapshot))]
  [GenerateProxy(TypeEnum.Copyable | TypeEnum.EngineCloneable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class WeatherSnapshot : 
    EngineObject,
    IWeatherSnapshot,
    IBlendable<IWeatherSnapshot>,
    IObject
  {
    [DataReadProxy(MemberEnum.OnlyCopy)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.OnlyCopy)]
    [Inspected]
    protected Clouds clouds = ProxyFactory.Create<Clouds>();
    [DataReadProxy(MemberEnum.OnlyCopy)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.OnlyCopy)]
    [Inspected]
    protected Day day = ProxyFactory.Create<Day>();
    [DataReadProxy(MemberEnum.OnlyCopy)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.OnlyCopy)]
    [Inspected]
    protected Fog fog = ProxyFactory.Create<Fog>();
    [DataReadProxy(MemberEnum.OnlyCopy)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.OnlyCopy)]
    [Inspected]
    protected Location location = ProxyFactory.Create<Location>();
    [DataReadProxy(MemberEnum.OnlyCopy)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.OnlyCopy)]
    [Inspected]
    protected Moon moon = ProxyFactory.Create<Moon>();
    [DataReadProxy(MemberEnum.OnlyCopy)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.OnlyCopy)]
    [Inspected]
    protected Night night = ProxyFactory.Create<Night>();
    [DataReadProxy(MemberEnum.OnlyCopy)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.OnlyCopy)]
    [Inspected]
    protected Stars stars = ProxyFactory.Create<Stars>();
    [DataReadProxy(MemberEnum.OnlyCopy)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.OnlyCopy)]
    [Inspected]
    protected Sun sun = ProxyFactory.Create<Sun>();
    [DataReadProxy(MemberEnum.OnlyCopy)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.OnlyCopy)]
    [Inspected]
    protected ThunderStorm thunderStorm = ProxyFactory.Create<ThunderStorm>();
    [DataReadProxy(MemberEnum.OnlyCopy)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.OnlyCopy)]
    [Inspected]
    protected Wind wind = ProxyFactory.Create<Wind>();
    [DataReadProxy(MemberEnum.OnlyCopy)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.OnlyCopy)]
    [Inspected]
    protected Rain rain = ProxyFactory.Create<Rain>();
    [DataReadProxy(MemberEnum.OnlyCopy)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.OnlyCopy)]
    [Inspected]
    protected FallingLeaves fallingLeaves = ProxyFactory.Create<FallingLeaves>();

    public Clouds Clouds => this.clouds;

    public Day Day => this.day;

    public Fog Fog => this.fog;

    public Location Location => this.location;

    public Moon Moon => this.moon;

    public Night Night => this.night;

    public Stars Stars => this.stars;

    public Sun Sun => this.sun;

    public ThunderStorm ThunderStorm => this.thunderStorm;

    public Wind Wind => this.wind;

    public Rain Rain => this.rain;

    public FallingLeaves FallingLeaves => this.fallingLeaves;

    public void Blend(IWeatherSnapshot aa, IWeatherSnapshot bb, IPureBlendOperation op)
    {
      WeatherSnapshot weatherSnapshot1 = (WeatherSnapshot) aa;
      WeatherSnapshot weatherSnapshot2 = (WeatherSnapshot) bb;
      this.Clouds.Blend(weatherSnapshot1.Clouds, weatherSnapshot2.Clouds, op);
      this.Day.Blend(weatherSnapshot1.Day, weatherSnapshot2.Day, op);
      this.Fog.Blend(weatherSnapshot1.Fog, weatherSnapshot2.Fog, op);
      this.Location.Blend(weatherSnapshot1.Location, weatherSnapshot2.Location, op);
      this.Moon.Blend(weatherSnapshot1.Moon, weatherSnapshot2.Moon, op);
      this.Night.Blend(weatherSnapshot1.Night, weatherSnapshot2.Night, op);
      this.Stars.Blend(weatherSnapshot1.Stars, weatherSnapshot2.Stars, op);
      this.Sun.Blend(weatherSnapshot1.Sun, weatherSnapshot2.Sun, op);
      this.ThunderStorm.Blend(weatherSnapshot1.ThunderStorm, weatherSnapshot2.ThunderStorm, op);
      this.Wind.Blend(weatherSnapshot1.Wind, weatherSnapshot2.Wind, op);
      this.Rain.Blend(weatherSnapshot1.Rain, weatherSnapshot2.Rain, op);
      this.FallingLeaves.Blend(weatherSnapshot1.FallingLeaves, weatherSnapshot2.FallingLeaves, op);
    }
  }
}
