using Cofe.Proxies;
using Cofe.Serializations.Data;
using Cofe.Serializations.Data.Xml;
using Cofe.Utility;
using Engine.Common;
using Engine.Common.Blenders;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Common.Weather;
using Engine.Impl.Weather;
using Engine.Proxy.Weather;
using Engine.Services.Engine;
using Engine.Source.Blenders;
using Engine.Source.Commons;
using Engine.Source.Saves;
using Engine.Source.Services;
using Engine.Source.Services.Saves;
using Engine.Source.Services.Utilities;
using Inspectors;
using Scripts.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Engine.Impl.Services
{
  [Depend(typeof (IFactory))]
  [Depend(typeof (ITimeService))]
  [Depend(typeof (EnvironmentService))]
  [Depend(typeof (FogController))]
  [RuntimeService(new Type[] {typeof (WeatherController), typeof (IWeatherController)})]
  [GenerateProxy(TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class WeatherController : IWeatherController, IUpdatable, IInitialisable, ISavesController
  {
    private bool isEnabled = true;
    private bool invalidate;
    private Dictionary<WeatherLayer, IWeatherLayerBlenderItem> layers = new Dictionary<WeatherLayer, IWeatherLayerBlenderItem>();
    [Inspected]
    private IWeatherLayerBlender blender;
    private IUpdater updater;

    [Inspected(Mutable = true)]
    public bool IsEnabled
    {
      get => this.isEnabled;
      set
      {
        this.isEnabled = value;
        this.invalidate = true;
      }
    }

    public IWeatherLayerBlender Blender => this.blender;

    public IWeatherLayerBlenderItem GetItem(WeatherLayer layer)
    {
      IWeatherLayerBlenderItem layerBlenderItem;
      this.layers.TryGetValue(layer, out layerBlenderItem);
      return layerBlenderItem;
    }

    public void Initialise()
    {
      this.blender = ServiceLocator.GetService<IFactory>().Create<IWeatherLayerBlender>();
      this.blender.OnChanged += new Action<ILayerBlender<IWeatherSnapshot>>(this.BlenderOnChanged);
      this.updater = InstanceByRequest<UpdateService>.Instance.WeatherUpdater;
      this.updater.AddUpdatable((IUpdatable) this);
      this.AddWeatherLayer(WeatherLayer.BaseLayer, 1f);
      this.AddWeatherLayer(WeatherLayer.PlannedEventsLayer, 0.0f);
      this.AddWeatherLayer(WeatherLayer.DistrictLayer, 0.0f);
      this.AddWeatherLayer(WeatherLayer.CutSceneLayer, 0.0f);
      ServiceLocator.GetService<EnvironmentService>().OnInvalidate += new Action(this.EnvironmentService_OnInvalidate);
    }

    public void Terminate()
    {
      ServiceLocator.GetService<EnvironmentService>().OnInvalidate -= new Action(this.EnvironmentService_OnInvalidate);
      this.updater.RemoveUpdatable((IUpdatable) this);
      this.blender.OnChanged -= new Action<ILayerBlender<IWeatherSnapshot>>(this.BlenderOnChanged);
      ((IDisposable) this.blender).Dispose();
      this.blender = (IWeatherLayerBlender) null;
      this.layers.Clear();
    }

    private void AddWeatherLayer(WeatherLayer weatherLayer, float weight)
    {
      IWeatherSmoothBlender weatherSmoothBlender = ServiceLocator.GetService<IFactory>().Create<IWeatherSmoothBlender>();
      IWeatherLayerBlenderItem layerBlenderItem = ServiceLocator.GetService<IFactory>().Create<IWeatherLayerBlenderItem>();
      layerBlenderItem.Blender = (ISmoothBlender<IWeatherSnapshot>) weatherSmoothBlender;
      layerBlenderItem.SetOpacity(weight);
      this.blender.AddItem((ILayerBlenderItem<IWeatherSnapshot>) layerBlenderItem);
      this.layers.Add(weatherLayer, layerBlenderItem);
    }

    public void ComputeUpdate()
    {
      if (!InstanceByRequest<EngineApplication>.Instance.ViewEnabled || !this.isEnabled || !this.invalidate)
        return;
      this.invalidate = false;
      WeatherSnapshotUtility.CopyFrom((WeatherSnapshot) this.blender.Current);
    }

    private void BlenderOnChanged(ILayerBlender<IWeatherSnapshot> layer) => this.invalidate = true;

    private void EnvironmentService_OnInvalidate() => this.invalidate = true;

    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected]
    protected WeatherInfo WeatherInfo
    {
      get
      {
        WeatherInfo weatherInfo = ProxyFactory.Create<WeatherInfo>();
        foreach (KeyValuePair<WeatherLayer, IWeatherLayerBlenderItem> layer in this.layers)
        {
          WeatherLayerInfo weatherLayerInfo = ProxyFactory.Create<WeatherLayerInfo>();
          weatherLayerInfo.Layer = layer.Key;
          weatherLayerInfo.Opacity = layer.Value.TargetOpacity;
          weatherLayerInfo.SnapshotTemplateId = ((SmoothBlender<IWeatherSnapshot>) layer.Value.Blender).SnapshotTemplateId;
          weatherInfo.Layers.Add(weatherLayerInfo);
        }
        return weatherInfo;
      }
      set
      {
        if (value == null)
          return;
        ITemplateService service = ServiceLocator.GetService<ITemplateService>();
        foreach (KeyValuePair<WeatherLayer, IWeatherLayerBlenderItem> layer1 in this.layers)
        {
          KeyValuePair<WeatherLayer, IWeatherLayerBlenderItem> layer = layer1;
          WeatherLayerInfo weatherLayerInfo = value.Layers.FirstOrDefault<WeatherLayerInfo>((Func<WeatherLayerInfo, bool>) (o => o.Layer == layer.Key));
          if (weatherLayerInfo != null)
          {
            layer.Value.SetOpacity(weatherLayerInfo.Opacity);
            IWeatherSnapshot template = service.GetTemplate<IWeatherSnapshot>(weatherLayerInfo.SnapshotTemplateId);
            if (template != null)
              layer.Value.Blender.BlendTo(template, TimeSpan.Zero);
          }
        }
      }
    }

    public IEnumerator Load(IErrorLoadingHandler errorHandler)
    {
      GameDataInfo data = InstanceByRequest<GameDataService>.Instance.GetCurrentGameData();
      WeatherUtility.SetDefaultWeather((IWeatherController) this, data.WeatherSnapshot.Value);
      yield break;
    }

    public IEnumerator Load(XmlElement element, string context, IErrorLoadingHandler errorHandler)
    {
      XmlElement node = element[TypeUtility.GetTypeName(this.GetType())];
      if (node == null)
      {
        errorHandler.LogError(TypeUtility.GetTypeName(this.GetType()) + " node not found , context : " + context);
      }
      else
      {
        XmlNodeDataReader reader = new XmlNodeDataReader((XmlNode) node, context);
        ((ISerializeStateLoad) this).StateLoad((IDataReader) reader, this.GetType());
        yield break;
      }
    }

    public void Unload()
    {
    }

    public void Save(IDataWriter writer, string context)
    {
      DefaultStateSaveUtility.SaveSerialize<WeatherController>(writer, TypeUtility.GetTypeName(this.GetType()), this);
    }
  }
}
