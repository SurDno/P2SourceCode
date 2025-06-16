using Cofe.Serializations.Data;
using Cofe.Serializations.Data.Xml;
using Cofe.Utility;
using Engine.Common;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Saves;
using Engine.Source.Services;
using Engine.Source.Services.Saves;
using Inspectors;
using Scripts.Data;
using System;
using System.Collections;
using System.Xml;
using UnityEngine;

namespace Engine.Impl.Services
{
  [RuntimeService(new System.Type[] {typeof (TimeService), typeof (ITimeService)})]
  [GenerateProxy(TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class TimeService : ITimeService, IUpdatable, IInitialisable, ISavesController
  {
    public const float defaultTimeFactor = 12f;
    private float solarTimeFactor = 12f;
    private float gameTimeFactor = 12f;
    private TimeSpan solarTime;
    private TimeSpan gameTime;
    private TimeSpan absoluteGameTime;
    private TimeSpan realTime;
    [Inspected]
    private TimeSpan demoTime = new TimeSpan(2, 1, 0, 0);

    [Inspected]
    public TimeSpan SolarTime
    {
      get => this.solarTime;
      set => this.solarTime = value;
    }

    [Inspected]
    public float SolarTimeFactor
    {
      get => this.solarTimeFactor;
      set => this.solarTimeFactor = value;
    }

    [Inspected]
    public TimeSpan GameTime
    {
      get => this.gameTime;
      set => this.gameTime = value;
    }

    [Inspected]
    public float GameTimeFactor
    {
      get => this.gameTimeFactor;
      set => this.gameTimeFactor = value;
    }

    public event Action<TimeSpan> GameTimeChangedEvent;

    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected]
    public TimeSpan AbsoluteGameTime
    {
      get => this.absoluteGameTime;
      protected set => this.absoluteGameTime = value;
    }

    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected]
    public TimeSpan RealTime
    {
      get => this.realTime;
      protected set => this.realTime = value;
    }

    [Inspected]
    public float DefaultTimeFactor => 12f;

    public void SetGameTime(TimeSpan time)
    {
      Action<TimeSpan> timeChangedEvent = this.GameTimeChangedEvent;
      if (timeChangedEvent == null)
        return;
      timeChangedEvent(time);
    }

    public TimeSpan Delta { get; private set; }

    public void Initialise()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable((IUpdatable) this);
      this.SolarTime = ScriptableObjectInstance<BuildSettings>.Instance.MainMenuSolarTime.Value;
    }

    public void Terminate()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable((IUpdatable) this);
    }

    public void ComputeUpdate()
    {
      if (InstanceByRequest<EngineApplication>.Instance.IsPaused || !InstanceByRequest<EngineApplication>.Instance.ViewEnabled)
      {
        this.Delta = TimeSpan.Zero;
      }
      else
      {
        this.Delta = TimeSpan.FromSeconds((double) Time.deltaTime * (double) this.GameTimeFactor);
        this.absoluteGameTime += this.Delta;
        this.solarTime += TimeSpan.FromSeconds((double) Time.deltaTime * (double) this.SolarTimeFactor);
        this.realTime += TimeSpan.FromSeconds((double) Time.deltaTime);
        this.ComputeDemo();
      }
    }

    public IEnumerator Load(IErrorLoadingHandler errorHandler)
    {
      GameDataInfo data = InstanceByRequest<GameDataService>.Instance.GetCurrentGameData();
      this.SolarTime = data.SolarTime.Value;
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

    public void Unload() => this.Delta = TimeSpan.Zero;

    public void Save(IDataWriter writer, string context)
    {
      DefaultStateSaveUtility.SaveSerialize<TimeService>(writer, TypeUtility.GetTypeName(this.GetType()), this);
    }

    private void ComputeDemo()
    {
      if (!ScriptableObjectInstance<BuildData>.Instance.Demo || !(this.GameTime > this.demoTime))
        return;
      ServiceLocator.GetService<GameLauncher>().ExitToMainMenu();
    }
  }
}
