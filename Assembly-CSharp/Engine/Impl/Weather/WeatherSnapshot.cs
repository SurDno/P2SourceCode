using Cofe.Proxies;
using Engine.Common;
using Engine.Common.Blenders;
using Engine.Common.Generator;
using Engine.Common.Weather;
using Engine.Impl.Services.Factories;
using Engine.Impl.Weather.Element;
using Engine.Source.Commons;
using Inspectors;

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
    [DataWriteProxy]
    [CopyableProxy(MemberEnum.OnlyCopy)]
    [Inspected]
    protected Clouds clouds = ProxyFactory.Create<Clouds>();
    [DataReadProxy(MemberEnum.OnlyCopy)]
    [DataWriteProxy]
    [CopyableProxy(MemberEnum.OnlyCopy)]
    [Inspected]
    protected Day day = ProxyFactory.Create<Day>();
    [DataReadProxy(MemberEnum.OnlyCopy)]
    [DataWriteProxy]
    [CopyableProxy(MemberEnum.OnlyCopy)]
    [Inspected]
    protected Fog fog = ProxyFactory.Create<Fog>();
    [DataReadProxy(MemberEnum.OnlyCopy)]
    [DataWriteProxy]
    [CopyableProxy(MemberEnum.OnlyCopy)]
    [Inspected]
    protected Location location = ProxyFactory.Create<Location>();
    [DataReadProxy(MemberEnum.OnlyCopy)]
    [DataWriteProxy]
    [CopyableProxy(MemberEnum.OnlyCopy)]
    [Inspected]
    protected Moon moon = ProxyFactory.Create<Moon>();
    [DataReadProxy(MemberEnum.OnlyCopy)]
    [DataWriteProxy]
    [CopyableProxy(MemberEnum.OnlyCopy)]
    [Inspected]
    protected Night night = ProxyFactory.Create<Night>();
    [DataReadProxy(MemberEnum.OnlyCopy)]
    [DataWriteProxy]
    [CopyableProxy(MemberEnum.OnlyCopy)]
    [Inspected]
    protected Stars stars = ProxyFactory.Create<Stars>();
    [DataReadProxy(MemberEnum.OnlyCopy)]
    [DataWriteProxy]
    [CopyableProxy(MemberEnum.OnlyCopy)]
    [Inspected]
    protected Sun sun = ProxyFactory.Create<Sun>();
    [DataReadProxy(MemberEnum.OnlyCopy)]
    [DataWriteProxy]
    [CopyableProxy(MemberEnum.OnlyCopy)]
    [Inspected]
    protected ThunderStorm thunderStorm = ProxyFactory.Create<ThunderStorm>();
    [DataReadProxy(MemberEnum.OnlyCopy)]
    [DataWriteProxy]
    [CopyableProxy(MemberEnum.OnlyCopy)]
    [Inspected]
    protected Wind wind = ProxyFactory.Create<Wind>();
    [DataReadProxy(MemberEnum.OnlyCopy)]
    [DataWriteProxy]
    [CopyableProxy(MemberEnum.OnlyCopy)]
    [Inspected]
    protected Element.Rain rain = ProxyFactory.Create<Element.Rain>();
    [DataReadProxy(MemberEnum.OnlyCopy)]
    [DataWriteProxy()]
    [CopyableProxy(MemberEnum.OnlyCopy)]
    [Inspected]
    protected FallingLeaves fallingLeaves = ProxyFactory.Create<FallingLeaves>();

    public Clouds Clouds => clouds;

    public Day Day => day;

    public Fog Fog => fog;

    public Location Location => location;

    public Moon Moon => moon;

    public Night Night => night;

    public Stars Stars => stars;

    public Sun Sun => sun;

    public ThunderStorm ThunderStorm => thunderStorm;

    public Wind Wind => wind;

    public Element.Rain Rain => rain;

    public FallingLeaves FallingLeaves => fallingLeaves;

    public void Blend(IWeatherSnapshot aa, IWeatherSnapshot bb, IPureBlendOperation op)
    {
      WeatherSnapshot weatherSnapshot1 = (WeatherSnapshot) aa;
      WeatherSnapshot weatherSnapshot2 = (WeatherSnapshot) bb;
      Clouds.Blend(weatherSnapshot1.Clouds, weatherSnapshot2.Clouds, op);
      Day.Blend(weatherSnapshot1.Day, weatherSnapshot2.Day, op);
      Fog.Blend(weatherSnapshot1.Fog, weatherSnapshot2.Fog, op);
      Location.Blend(weatherSnapshot1.Location, weatherSnapshot2.Location, op);
      Moon.Blend(weatherSnapshot1.Moon, weatherSnapshot2.Moon, op);
      Night.Blend(weatherSnapshot1.Night, weatherSnapshot2.Night, op);
      Stars.Blend(weatherSnapshot1.Stars, weatherSnapshot2.Stars, op);
      Sun.Blend(weatherSnapshot1.Sun, weatherSnapshot2.Sun, op);
      ThunderStorm.Blend(weatherSnapshot1.ThunderStorm, weatherSnapshot2.ThunderStorm, op);
      Wind.Blend(weatherSnapshot1.Wind, weatherSnapshot2.Wind, op);
      Rain.Blend(weatherSnapshot1.Rain, weatherSnapshot2.Rain, op);
      FallingLeaves.Blend(weatherSnapshot1.FallingLeaves, weatherSnapshot2.FallingLeaves, op);
    }
  }
}
