// Decompiled with JetBrains decompiler
// Type: Engine.Impl.Services.VirtualMachineController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Serializations.Converters;
using Cofe.Serializations.Data;
using Cofe.Utility;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Services;
using Engine.Impl.Services.HierarchyServices;
using Engine.Impl.UI.Menu.Main;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Connections;
using Engine.Source.Difficulties;
using Engine.Source.Saves;
using Engine.Source.Services;
using Engine.Source.Services.Saves;
using Engine.Source.Settings.External;
using Inspectors;
using Scripts.Data;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

#nullable disable
namespace Engine.Impl.Services
{
  [Depend(typeof (HierarchyService))]
  [Depend(typeof (ITimeService))]
  [Depend(typeof (ILogicEventService))]
  [Depend(typeof (ITemplateService))]
  [Depend(typeof (ISimulation))]
  [Depend(typeof (IWeatherController))]
  [Depend(typeof (IProfilesService))]
  [SaveDepend(typeof (ITimeService))]
  [SaveDepend(typeof (ITemplateService))]
  [SaveDepend(typeof (IProfilesService))]
  [RuntimeService(new System.Type[] {typeof (VirtualMachineController)})]
  public class VirtualMachineController : 
    IUpdatable,
    IInitialisable,
    IAsyncInitializable,
    ISavesController
  {
    private const int XmlDataFormatVersion = 14;
    private const string vmNodeName = "VirtualMachine";
    private const string nodeName = "VirtualMachineController";
    private const string dataNodeName = "Data";
    private const string dataVersionNodeName = "DataVersion";
    private const string projectVersionNodeName = "ProjectVersion";
    private const string xmlDataFormatVersionName = "XmlDataFormatVersion";
    private const string xmlDataCapacityName = "DataCapacity";
    private const string dataOriginalExperienceSessionNodeName = "OriginalExperienceSession";
    private IVirtualMachine virtualMachine;
    private TimeService timeService;
    private IUpdater updater;
    private string projectName = "";
    private int dataVersion;

    public string ProjectName => this.projectName;

    public int DataVersion => this.dataVersion;

    public bool OriginalExperienceSession { get; set; } = true;

    public bool IsInitialized => this.virtualMachine != null && this.virtualMachine.IsInitialized;

    public bool IsLoaded => this.virtualMachine != null && this.virtualMachine.IsLoaded;

    [Inspected]
    public IVirtualMachine VirtualMachine => this.virtualMachine;

    public void Initialise()
    {
      IEnumerator enumerator = this.AsyncInitialize();
      while (enumerator.MoveNext())
      {
        object current = enumerator.Current;
      }
    }

    public int AsyncCount => 0;

    public IEnumerator AsyncInitialize()
    {
      this.virtualMachine = WorldUtility.CreateVirtualMachine();
      if (this.virtualMachine == null)
      {
        UnityEngine.Debug.LogError((object) "Virtual Machine implement not found");
      }
      else
      {
        yield return (object) this.InitializeVirtualMachine();
        if (!this.virtualMachine.IsInitialized)
          UnityEngine.Debug.Log((object) ObjectInfoUtility.GetStream().Append(TypeUtility.GetTypeName(this.GetType())).Append(" : ").Append(MethodBase.GetCurrentMethod().Name).Append(" , is not inited"));
        this.timeService = ServiceLocator.GetService<TimeService>();
        this.updater = InstanceByRequest<UpdateService>.Instance.VirtualMachineUpdater;
        this.updater.AddUpdatable((IUpdatable) this);
        InstanceByRequest<DifficultySettings>.Instance.OnApply += new Action(this.DifficultySettings_OnApply);
      }
    }

    public void Terminate()
    {
      InstanceByRequest<DifficultySettings>.Instance.OnApply -= new Action(this.DifficultySettings_OnApply);
      this.updater.RemoveUpdatable((IUpdatable) this);
      if (this.virtualMachine == null || !this.virtualMachine.IsInitialized)
        return;
      if (this.virtualMachine.IsLoaded)
        this.virtualMachine.Unload();
      if (this.virtualMachine.IsDataLoaded)
        this.virtualMachine.UnloadData();
      this.virtualMachine.Terminate();
      this.virtualMachine = (IVirtualMachine) null;
    }

    private IEnumerator InitializeVirtualMachine()
    {
      bool debug = ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.VirtualMachineDebug;
      yield return (object) this.virtualMachine.Initialize(debug, ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.VMEventsQueuePerFrameTimeMax);
    }

    public void ComputeUpdate()
    {
      if (!InstanceByRequest<EngineApplication>.Instance.ViewEnabled || this.virtualMachine == null || !this.virtualMachine.IsInitialized || !this.virtualMachine.IsLoaded)
        return;
      this.virtualMachine.Update(this.timeService.Delta);
    }

    public IEnumerator Load(IErrorLoadingHandler errorHandler)
    {
      UnityEngine.Debug.Log((object) ObjectInfoUtility.GetStream().Append("IVirtualMachine").Append(" load\n").GetStackTrace());
      IVirtualMachine vm = this.VirtualMachine;
      if (vm == null)
        errorHandler.LogError("IVirtualMachine not found");
      else if (!vm.IsInitialized)
        errorHandler.LogError("IVirtualMachine is not inited");
      else if (vm.IsLoaded)
      {
        errorHandler.LogError("IVirtualMachine already loaded");
      }
      else
      {
        yield return (object) this.ComputeData(errorHandler);
        if (!errorHandler.HasErrorLoading)
        {
          this.OriginalExperienceSession = true;
          this.DifficultySettings_OnApply();
          Stopwatch sw = new Stopwatch();
          sw.Restart();
          yield return (object) vm.Load();
          sw.Stop();
          UnityEngine.Debug.Log((object) ObjectInfoUtility.GetStream().Append("Load, elapsed : ").Append((object) sw.Elapsed));
        }
      }
    }

    public IEnumerator Load(XmlElement element, string context, IErrorLoadingHandler errorHandler)
    {
      UnityEngine.Debug.Log((object) ObjectInfoUtility.GetStream().Append("IVirtualMachine").Append(" load, context : ").Append(context).Append("\n").GetStackTrace());
      IVirtualMachine vm = this.VirtualMachine;
      if (vm == null)
        errorHandler.LogError("IVirtualMachine not found");
      else if (!vm.IsInitialized)
        errorHandler.LogError("IVirtualMachine is not inited");
      else if (vm.IsLoaded)
      {
        errorHandler.LogError("IVirtualMachine already loaded");
      }
      else
      {
        XmlElement vmNode = element["VirtualMachine"];
        if (vmNode == null)
        {
          errorHandler.LogError("VirtualMachine node not found in file : " + context);
        }
        else
        {
          string storedDataName = element[nameof (VirtualMachineController)]?["Data"]?.InnerText;
          if (storedDataName.IsNullOrEmpty())
          {
            errorHandler.LogError("Data node not found in file : " + context);
          }
          else
          {
            InstanceByRequest<GameDataService>.Instance.SetCurrentGameData(storedDataName);
            LoadWindow.Instance.Mode = LoadWindowMode.LoadSavedGame;
            yield return (object) this.ComputeData(errorHandler);
            if (!errorHandler.HasErrorLoading)
            {
              string storedDataVersionText = element[nameof (VirtualMachineController)]?["DataVersion"]?.InnerText;
              if (storedDataVersionText.IsNullOrEmpty())
              {
                UnityEngine.Debug.LogError((object) ("DataVersion node not found in file : " + context));
                storedDataVersionText = "0";
              }
              int saveVersion = DefaultConverter.ParseInt(storedDataVersionText);
              if (this.dataVersion < saveVersion)
              {
                errorHandler.LogError("Error save version, current data version : " + (object) this.dataVersion + " , save version : " + (object) saveVersion + " , context : " + context);
              }
              else
              {
                string originalExperienceSessionText = element[nameof (VirtualMachineController)]?["OriginalExperienceSession"]?.InnerText;
                this.OriginalExperienceSession = originalExperienceSessionText == null || DefaultConverter.ParseBool(originalExperienceSessionText);
                this.DifficultySettings_OnApply();
                Stopwatch sw = new Stopwatch();
                sw.Restart();
                yield return (object) vm.Load(vmNode);
                sw.Stop();
                UnityEngine.Debug.Log((object) ObjectInfoUtility.GetStream().Append("Load, elapsed : ").Append((object) sw.Elapsed));
              }
            }
          }
        }
      }
    }

    public void Unload()
    {
      UnityEngine.Debug.Log((object) ObjectInfoUtility.GetStream().Append("IVirtualMachine").Append(" unload\n").GetStackTrace());
      IVirtualMachine virtualMachine = this.VirtualMachine;
      if (virtualMachine == null)
        UnityEngine.Debug.LogError((object) "IVirtualMachine not found");
      else if (!virtualMachine.IsInitialized)
        UnityEngine.Debug.LogError((object) "IVirtualMachine is not inited");
      else if (!virtualMachine.IsLoaded)
        UnityEngine.Debug.LogError((object) "IVirtualMachine not loaded");
      else
        virtualMachine.Unload();
    }

    public void Save(IDataWriter writer, string context)
    {
      IVirtualMachine virtualMachine = this.VirtualMachine;
      if (virtualMachine == null)
        UnityEngine.Debug.LogError((object) "IVirtualMachine not found");
      else if (!virtualMachine.IsInitialized)
        UnityEngine.Debug.LogError((object) "IVirtualMachine is not inited");
      else if (!virtualMachine.IsLoaded)
      {
        UnityEngine.Debug.LogError((object) "IVirtualMachine is not loaded");
      }
      else
      {
        writer.Begin(nameof (VirtualMachineController), (System.Type) null, true);
        writer.WriteSimple("Data", this.projectName);
        writer.WriteSimple("DataVersion", DefaultConverter.ToString(this.dataVersion));
        writer.WriteSimple("OriginalExperienceSession", DefaultConverter.ToString(this.OriginalExperienceSession));
        writer.End(nameof (VirtualMachineController), true);
        try
        {
          writer.Begin("VirtualMachine", (System.Type) null, true);
          virtualMachine.Save(writer);
          writer.End("VirtualMachine", true);
        }
        catch (Exception ex)
        {
          UnityEngine.Debug.LogError((object) ("Error save vm to file : " + context + " , error : " + (object) ex));
        }
      }
    }

    private IEnumerator ComputeData(IErrorLoadingHandler errorHandler)
    {
      string requiredProjectName = InstanceByRequest<GameDataService>.Instance.GetCurrentGameData().Name;
      if (this.projectName != requiredProjectName)
      {
        if (this.virtualMachine.IsDataLoaded)
        {
          this.virtualMachine.UnloadData();
          OptimizationUtility.ForceCollect();
          yield return (object) null;
        }
        yield return (object) this.LoadData(requiredProjectName, errorHandler);
        OptimizationUtility.ForceCollect();
        yield return (object) null;
        InstanceByRequest<LabelService>.Instance.Invalidate();
      }
    }

    private static void SetFirstScene(string projectName)
    {
      StaticModelComponent component = ServiceLocator.GetService<ITemplateService>().GetTemplate<IEntity>(Ids.PathologicId).GetComponent<StaticModelComponent>();
      Guid id = (BuildSettingsUtility.GetAllGameData().FirstOrDefault<GameDataInfo>((Func<GameDataInfo, bool>) (o => o.Name == projectName)) ?? throw new Exception("Project not found : " + projectName)).Scene.Id;
      component.Connection = new Typed<IScene>(id);
      HierarchyService service = ServiceLocator.GetService<HierarchyService>();
      service.CreateMainContainer();
      ((IEntityHierarchy) ServiceLocator.GetService<ISimulation>().Hierarchy).HierarchyItem = new HierarchyItem(service.MainContainer);
    }

    public IEnumerator LoadData(string projectName, IErrorLoadingHandler errorHandler)
    {
      this.dataVersion = 0;
      int dataCapacity = 0;
      VirtualMachineController.SetFirstScene(projectName);
      this.projectName = projectName;
      string dataFileName = PlatformUtility.GetPath("Data/VirtualMachine/{ProjectName}".Replace("{ProjectName}", projectName));
      string versionFileName = dataFileName + "/Version.xml";
      if (File.Exists(versionFileName))
      {
        string data = File.ReadAllText(versionFileName);
        UnityEngine.Debug.Log((object) ObjectInfoUtility.GetStream().Append("Data name : ").Append(projectName).Append(" , version : ").Append("\r\n").Append(data));
        try
        {
          XmlDocument doc = new XmlDocument();
          doc.LoadXml(data);
          string text = doc["Root"]["ProjectVersion"].InnerText;
          this.dataVersion = DefaultConverter.ParseInt(text);
          int formatVersion = 0;
          text = doc["Root"]["XmlDataFormatVersion"].InnerText;
          formatVersion = DefaultConverter.ParseInt(text);
          text = doc["Root"]["DataCapacity"].InnerText;
          dataCapacity = DefaultConverter.ParseInt(text);
          if (formatVersion != 14)
          {
            errorHandler.LogError("Wrong format data version file : " + versionFileName + " , data version : " + (object) formatVersion + " , parser version : " + (object) 14);
            yield break;
          }
          else
          {
            doc = (XmlDocument) null;
            text = (string) null;
          }
        }
        catch (Exception ex)
        {
          errorHandler.LogError("Error load data version file : " + versionFileName + " , ex : " + (object) ex);
          yield break;
        }
        data = (string) null;
        Stopwatch sw = new Stopwatch();
        sw.Restart();
        int threadCount = ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.ThreadCount;
        yield return (object) this.VirtualMachine.LoadData(dataFileName, threadCount, dataCapacity);
        sw.Stop();
        UnityEngine.Debug.Log((object) ObjectInfoUtility.GetStream().Append("Load data, elapsed : ").Append((object) sw.Elapsed));
      }
      else
        errorHandler.LogError("Data version not found : " + versionFileName);
    }

    private void DifficultySettings_OnApply()
    {
      if (InstanceByRequest<DifficultySettings>.Instance.OriginalExperience.Value)
        return;
      this.OriginalExperienceSession = false;
    }
  }
}
