using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
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
using Debug = UnityEngine.Debug;

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
  [RuntimeService(typeof (VirtualMachineController))]
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

    public string ProjectName => projectName;

    public int DataVersion => dataVersion;

    public bool OriginalExperienceSession { get; set; } = true;

    public bool IsInitialized => virtualMachine != null && virtualMachine.IsInitialized;

    public bool IsLoaded => virtualMachine != null && virtualMachine.IsLoaded;

    [Inspected]
    public IVirtualMachine VirtualMachine => virtualMachine;

    public void Initialise()
    {
      IEnumerator enumerator = AsyncInitialize();
      while (enumerator.MoveNext())
      {
        object current = enumerator.Current;
      }
    }

    public int AsyncCount => 0;

    public IEnumerator AsyncInitialize()
    {
      virtualMachine = WorldUtility.CreateVirtualMachine();
      if (virtualMachine == null)
      {
        Debug.LogError("Virtual Machine implement not found");
      }
      else
      {
        yield return InitializeVirtualMachine();
        if (!virtualMachine.IsInitialized)
          Debug.Log(ObjectInfoUtility.GetStream().Append(TypeUtility.GetTypeName(GetType())).Append(" : ").Append(MethodBase.GetCurrentMethod().Name).Append(" , is not inited"));
        timeService = ServiceLocator.GetService<TimeService>();
        updater = InstanceByRequest<UpdateService>.Instance.VirtualMachineUpdater;
        updater.AddUpdatable(this);
        InstanceByRequest<DifficultySettings>.Instance.OnApply += DifficultySettings_OnApply;
      }
    }

    public void Terminate()
    {
      InstanceByRequest<DifficultySettings>.Instance.OnApply -= DifficultySettings_OnApply;
      updater.RemoveUpdatable(this);
      if (virtualMachine == null || !virtualMachine.IsInitialized)
        return;
      if (virtualMachine.IsLoaded)
        virtualMachine.Unload();
      if (virtualMachine.IsDataLoaded)
        virtualMachine.UnloadData();
      virtualMachine.Terminate();
      virtualMachine = null;
    }

    private IEnumerator InitializeVirtualMachine()
    {
      bool debug = ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.VirtualMachineDebug;
      yield return virtualMachine.Initialize(debug, ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.VMEventsQueuePerFrameTimeMax);
    }

    public void ComputeUpdate()
    {
      if (!InstanceByRequest<EngineApplication>.Instance.ViewEnabled || virtualMachine == null || !virtualMachine.IsInitialized || !virtualMachine.IsLoaded)
        return;
      virtualMachine.Update(timeService.Delta);
    }

    public IEnumerator Load(IErrorLoadingHandler errorHandler)
    {
      Debug.Log(ObjectInfoUtility.GetStream().Append("IVirtualMachine").Append(" load\n").GetStackTrace());
      IVirtualMachine vm = VirtualMachine;
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
        yield return ComputeData(errorHandler);
        if (!errorHandler.HasErrorLoading)
        {
          OriginalExperienceSession = true;
          DifficultySettings_OnApply();
          Stopwatch sw = new Stopwatch();
          sw.Restart();
          yield return vm.Load();
          sw.Stop();
          Debug.Log(ObjectInfoUtility.GetStream().Append("Load, elapsed : ").Append(sw.Elapsed));
        }
      }
    }

    public IEnumerator Load(XmlElement element, string context, IErrorLoadingHandler errorHandler)
    {
      Debug.Log(ObjectInfoUtility.GetStream().Append("IVirtualMachine").Append(" load, context : ").Append(context).Append("\n").GetStackTrace());
      IVirtualMachine vm = VirtualMachine;
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
            yield return ComputeData(errorHandler);
            if (!errorHandler.HasErrorLoading)
            {
              string storedDataVersionText = element[nameof (VirtualMachineController)]?["DataVersion"]?.InnerText;
              if (storedDataVersionText.IsNullOrEmpty())
              {
                Debug.LogError("DataVersion node not found in file : " + context);
                storedDataVersionText = "0";
              }
              int saveVersion = DefaultConverter.ParseInt(storedDataVersionText);
              if (dataVersion < saveVersion)
              {
                errorHandler.LogError("Error save version, current data version : " + dataVersion + " , save version : " + saveVersion + " , context : " + context);
              }
              else
              {
                string originalExperienceSessionText = element[nameof (VirtualMachineController)]?["OriginalExperienceSession"]?.InnerText;
                OriginalExperienceSession = originalExperienceSessionText == null || DefaultConverter.ParseBool(originalExperienceSessionText);
                DifficultySettings_OnApply();
                Stopwatch sw = new Stopwatch();
                sw.Restart();
                yield return vm.Load(vmNode);
                sw.Stop();
                Debug.Log(ObjectInfoUtility.GetStream().Append("Load, elapsed : ").Append(sw.Elapsed));
              }
            }
          }
        }
      }
    }

    public void Unload()
    {
      Debug.Log(ObjectInfoUtility.GetStream().Append("IVirtualMachine").Append(" unload\n").GetStackTrace());
      IVirtualMachine virtualMachine = VirtualMachine;
      if (virtualMachine == null)
        Debug.LogError("IVirtualMachine not found");
      else if (!virtualMachine.IsInitialized)
        Debug.LogError("IVirtualMachine is not inited");
      else if (!virtualMachine.IsLoaded)
        Debug.LogError("IVirtualMachine not loaded");
      else
        virtualMachine.Unload();
    }

    public void Save(IDataWriter writer, string context)
    {
      IVirtualMachine virtualMachine = VirtualMachine;
      if (virtualMachine == null)
        Debug.LogError("IVirtualMachine not found");
      else if (!virtualMachine.IsInitialized)
        Debug.LogError("IVirtualMachine is not inited");
      else if (!virtualMachine.IsLoaded)
      {
        Debug.LogError("IVirtualMachine is not loaded");
      }
      else
      {
        writer.Begin(nameof (VirtualMachineController), null, true);
        writer.WriteSimple("Data", projectName);
        writer.WriteSimple("DataVersion", DefaultConverter.ToString(dataVersion));
        writer.WriteSimple("OriginalExperienceSession", DefaultConverter.ToString(OriginalExperienceSession));
        writer.End(nameof (VirtualMachineController), true);
        try
        {
          writer.Begin("VirtualMachine", null, true);
          virtualMachine.Save(writer);
          writer.End("VirtualMachine", true);
        }
        catch (Exception ex)
        {
          Debug.LogError("Error save vm to file : " + context + " , error : " + ex);
        }
      }
    }

    private IEnumerator ComputeData(IErrorLoadingHandler errorHandler)
    {
      string requiredProjectName = InstanceByRequest<GameDataService>.Instance.GetCurrentGameData().Name;
      if (projectName != requiredProjectName)
      {
        if (virtualMachine.IsDataLoaded)
        {
          virtualMachine.UnloadData();
          OptimizationUtility.ForceCollect();
          yield return null;
        }
        yield return LoadData(requiredProjectName, errorHandler);
        OptimizationUtility.ForceCollect();
        yield return null;
        InstanceByRequest<LabelService>.Instance.Invalidate();
      }
    }

    private static void SetFirstScene(string projectName)
    {
      StaticModelComponent component = ServiceLocator.GetService<ITemplateService>().GetTemplate<IEntity>(Ids.PathologicId).GetComponent<StaticModelComponent>();
      Guid id = (BuildSettingsUtility.GetAllGameData().FirstOrDefault(o => o.Name == projectName) ?? throw new Exception("Project not found : " + projectName)).Scene.Id;
      component.Connection = new Typed<IScene>(id);
      HierarchyService service = ServiceLocator.GetService<HierarchyService>();
      service.CreateMainContainer();
      ((IEntityHierarchy) ServiceLocator.GetService<ISimulation>().Hierarchy).HierarchyItem = new HierarchyItem(service.MainContainer);
    }

    public IEnumerator LoadData(string projectName, IErrorLoadingHandler errorHandler)
    {
      dataVersion = 0;
      int dataCapacity = 0;
      SetFirstScene(projectName);
      this.projectName = projectName;
      string dataFileName = PlatformUtility.GetPath("Data/VirtualMachine/{ProjectName}".Replace("{ProjectName}", projectName));
      string versionFileName = dataFileName + "/Version.xml";
      if (File.Exists(versionFileName))
      {
        string data = File.ReadAllText(versionFileName);
        Debug.Log(ObjectInfoUtility.GetStream().Append("Data name : ").Append(projectName).Append(" , version : ").Append("\r\n").Append(data));
        try
        {
          XmlDocument doc = new XmlDocument();
          doc.LoadXml(data);
          string text = doc["Root"]["ProjectVersion"].InnerText;
          dataVersion = DefaultConverter.ParseInt(text);
          int formatVersion = 0;
          text = doc["Root"]["XmlDataFormatVersion"].InnerText;
          formatVersion = DefaultConverter.ParseInt(text);
          text = doc["Root"]["DataCapacity"].InnerText;
          dataCapacity = DefaultConverter.ParseInt(text);
          if (formatVersion != 14)
          {
            errorHandler.LogError("Wrong format data version file : " + versionFileName + " , data version : " + formatVersion + " , parser version : " + 14);
            yield break;
          }

          doc = null;
          text = null;
        }
        catch (Exception ex)
        {
          errorHandler.LogError("Error load data version file : " + versionFileName + " , ex : " + ex);
          yield break;
        }
        data = null;
        Stopwatch sw = new Stopwatch();
        sw.Restart();
        int threadCount = ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.ThreadCount;
        yield return VirtualMachine.LoadData(dataFileName, threadCount, dataCapacity);
        sw.Stop();
        Debug.Log(ObjectInfoUtility.GetStream().Append("Load data, elapsed : ").Append(sw.Elapsed));
      }
      else
        errorHandler.LogError("Data version not found : " + versionFileName);
    }

    private void DifficultySettings_OnApply()
    {
      if (InstanceByRequest<DifficultySettings>.Instance.OriginalExperience.Value)
        return;
      OriginalExperienceSession = false;
    }
  }
}
