using System.Collections.Generic;
using System.Xml;
using Cofe.Loggers;
using Engine.Common.Commons;
using Engine.Common.Comparers;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Data;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;
using VirtualMachine.Data;

namespace PLVirtualMachine.Objects
{
  [TypeData(EDataType.TBlueprint)]
  [DataFactory("Blueprint")]
  public class VMBlueprint : 
    VMLogicObject,
    IStub,
    IEditorDataReader,
    IBlueprint,
    IGameObjectContext,
    IContainer,
    IObject,
    IEditorBaseTemplate,
    INamedElement,
    INamed,
    IStaticUpdateable,
    IContext,
    ILogicObject
  {
    [FieldData("Static")]
    protected bool isStatic;
    [FieldData("InheritanceInfo", DataFieldType.Reference)]
    protected List<IBlueprint> baseBlueprints;
    private Dictionary<string, IFunctionalComponent> allFunctionalComponentsDict;
    private bool isInheritanceMapLoaded;
    private Dictionary<ulong, ulong> inheritancyMappedEventGuids;
    private Dictionary<string, IEvent> functionalMappedEventsDict;

    public virtual void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
    {
      while (xml.Read()) {
        if (xml.NodeType == XmlNodeType.Element)
        {
          switch (xml.Name)
          {
            case "ChildObjects":
              gameObjects = EditorDataReadUtility.ReadReferenceList(xml, creator, gameObjects);
              continue;
            case "CustomParams":
              customParamsDict = EditorDataReadUtility.ReadStringReferenceDictionary(xml, creator, customParamsDict);
              continue;
            case "EventGraph":
              stateGraph = EditorDataReadUtility.ReadReference<IFiniteStateMachine>(xml, creator);
              continue;
            case "Events":
              customEventsList = EditorDataReadUtility.ReadReferenceList(xml, creator, customEventsList);
              continue;
            case "FunctionalComponents":
              functionalComponents = EditorDataReadUtility.ReadReferenceList(xml, creator, functionalComponents);
              continue;
            case "GameTimeContext":
              gameTimeContext = EditorDataReadUtility.ReadReference<IGameMode>(xml, creator);
              continue;
            case "InheritanceInfo":
              baseBlueprints = EditorDataReadUtility.ReadReferenceList(xml, creator, baseBlueprints);
              continue;
            case "Name":
              name = EditorDataReadUtility.ReadValue(xml, name);
              continue;
            case "Parent":
              parent = EditorDataReadUtility.ReadReference<IContainer>(xml, creator);
              continue;
            case "StandartParams":
              standartParamsDict = EditorDataReadUtility.ReadStringReferenceDictionary(xml, creator, standartParamsDict);
              continue;
            case "Static":
              isStatic = EditorDataReadUtility.ReadValue(xml, isStatic);
              continue;
            default:
              if (XMLDataLoader.Logs.Add(typeContext + " : " + xml.Name))
                Logger.AddError(typeContext + " : " + xml.Name);
              XmlReaderUtility.SkipNode(xml);
              continue;
          }
        }

        if (xml.NodeType == XmlNodeType.EndElement)
          break;
      }
    }

    public VMBlueprint(ulong guid)
      : base(guid)
    {
    }

    public List<IBlueprint> BaseBlueprints => baseBlueprints;

    public override EObjectCategory GetCategory() => EObjectCategory.OBJECT_CATEGORY_CLASS;

    public override bool Static => isStatic;

    public override IBlueprint Blueprint => this;

    public override Dictionary<string, IFunctionalComponent> FunctionalComponents
    {
      get
      {
        if (allFunctionalComponentsDict == null)
          allFunctionalComponentsDict = GetAllFunctionalComponents();
        return allFunctionalComponentsDict;
      }
    }

    public override List<IEvent> CustomEvents
    {
      get
      {
        List<IEvent> mergedEventsTo = new List<IEvent>();
        if (baseBlueprints != null)
        {
          for (int index = 0; index < baseBlueprints.Count; ++index)
            VMBlueprintUtility.MergeCustomEvents(mergedEventsTo, ((VMLogicObject) baseBlueprints[index]).CustomEvents);
        }
        VMBlueprintUtility.MergeCustomEvents(mergedEventsTo, base.CustomEvents);
        return mergedEventsTo;
      }
    }

    public override IEnumerable<IVariable> GetContextVariables(
      EContextVariableCategory contextVarCategory)
    {
      switch (contextVarCategory)
      {
        case EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_PARAM:
          List<IVariable> mergedParamsTo = new List<IVariable>();
          if (baseBlueprints != null)
          {
            for (int index = 0; index < baseBlueprints.Count; ++index)
              VMBlueprintUtility.MergeContextParams(mergedParamsTo, baseBlueprints[index].GetContextVariables(contextVarCategory));
          }
          VMBlueprintUtility.MergeContextParams(mergedParamsTo, base.GetContextVariables(contextVarCategory));
          foreach (IVariable contextVariable in mergedParamsTo)
            yield return contextVariable;
          break;
        case EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_STATUS:
          break;
        default:
          foreach (IVariable contextVariable in base.GetContextVariables(contextVarCategory))
            yield return contextVariable;
          break;
      }
    }

    public override bool IsDerivedFrom(ulong iBlueprintGuid, bool bWithSelf = false)
    {
      if (bWithSelf && iBlueprintGuid != 0UL && (long) BaseGuid == (long) iBlueprintGuid)
        return true;
      if (baseBlueprints != null)
      {
        for (int index = 0; index < baseBlueprints.Count; ++index)
        {
          if ((long) baseBlueprints[index].BaseGuid == (long) iBlueprintGuid || baseBlueprints[index].IsDerivedFrom(iBlueprintGuid))
            return true;
        }
      }
      return false;
    }

    public override IEnumerable<IStateRef> GetObjectStates()
    {
      foreach (IStateRef objectState in base.GetObjectStates())
        yield return objectState;
      if (baseBlueprints != null)
      {
        for (int i = 0; i < baseBlueprints.Count; ++i)
        {
          foreach (IStateRef objectState in baseBlueprints[i].GetObjectStates())
            yield return objectState;
        }
      }
    }

    public virtual ulong GetInheritanceMappedEventGuid(ulong stEventGuid)
    {
      if (VirtualObjectTemplate != null)
        return ((VMBlueprint) VirtualObjectTemplate).GetInheritanceMappedEventGuid(stEventGuid);
      if (!isInheritanceMapLoaded)
        MakeInheritanceMapping();
      return inheritancyMappedEventGuids != null && inheritancyMappedEventGuids.ContainsKey(stEventGuid) ? inheritancyMappedEventGuids[stEventGuid] : stEventGuid;
    }

    public Dictionary<string, IEvent> FunctionalMappedEvents
    {
      get
      {
        if (functionalMappedEventsDict == null)
          LoadFunctionalMappedEvents();
        else if (functionalMappedEventsDict.Count == 0)
          LoadFunctionalMappedEvents();
        return functionalMappedEventsDict;
      }
    }

    public override void OnAfterLoad()
    {
      base.OnAfterLoad();
      MakeInheritanceMapping();
    }

    public virtual IContainer VirtualObjectTemplate => null;

    protected override void UpdateContextVariablesTotalCache()
    {
      base.UpdateContextVariablesTotalCache();
      foreach (IVariable contextVariable in GetContextVariables(EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_PARAM))
        LoadContextVaraiblesTotalCache(contextVariable);
    }

    protected override bool CanReloadVariablesTotalCache()
    {
      return baseBlueprints != null && baseBlueprints.Count > 0 || base.CanReloadVariablesTotalCache();
    }

    private Dictionary<string, IFunctionalComponent> GetAllFunctionalComponents()
    {
      Dictionary<string, IFunctionalComponent> mergeTo = new Dictionary<string, IFunctionalComponent>();
      if (baseBlueprints != null)
      {
        for (int index = 0; index < baseBlueprints.Count; ++index)
        {
          Dictionary<string, IFunctionalComponent> functionalComponents = ((VMBlueprint) baseBlueprints[index]).GetAllFunctionalComponents();
          VMBlueprintUtility.MergeInheritedComponents(mergeTo, functionalComponents);
        }
      }
      VMBlueprintUtility.MergeComponents(mergeTo, this.functionalComponents);
      return mergeTo;
    }

    protected virtual void MakeInheritanceMapping()
    {
      MakeInheritanceMapping(GetClassHierarchy());
    }

    protected void MakeInheritanceMapping(IEnumerable<IBlueprint> classHierarchyList)
    {
      if (events != null)
      {
        for (int index = 0; index < events.Count; ++index)
        {
          string key = events[index].Parent.Name + "." + events[index].Name;
          foreach (VMBlueprint classHierarchy in classHierarchyList)
          {
            Dictionary<string, IEvent> functionalMappedEvents = classHierarchy.FunctionalMappedEvents;
            if (functionalMappedEvents != null && functionalMappedEvents.ContainsKey(key))
            {
              IEvent @event = functionalMappedEvents[key];
              if (inheritancyMappedEventGuids == null)
                inheritancyMappedEventGuids = new Dictionary<ulong, ulong>(UlongComparer.Instance);
              if (!inheritancyMappedEventGuids.ContainsKey(@event.BaseGuid))
                inheritancyMappedEventGuids.Add(@event.BaseGuid, events[index].BaseGuid);
              else
                Logger.AddError(string.Format("Inheritance mapped event with id={0} is dublicated in {1}", @event.BaseGuid, Name));
            }
          }
        }
      }
      isInheritanceMapLoaded = true;
    }

    protected IEnumerable<IBlueprint> GetClassHierarchy()
    {
      if (baseBlueprints != null)
      {
        for (int i = 0; i < baseBlueprints.Count; ++i)
        {
          yield return baseBlueprints[i];
          foreach (IBlueprint blueprint in ((VMBlueprint) baseBlueprints[i]).GetClassHierarchy())
            yield return blueprint;
        }
      }
    }

    private void LoadFunctionalMappedEvents()
    {
      if (functionalMappedEventsDict != null)
        functionalMappedEventsDict.Clear();
      for (int index1 = 0; index1 < functionalComponents.Count; ++index1)
      {
        List<IEvent> engineEvents = functionalComponents[index1].EngineEvents;
        for (int index2 = 0; index2 < engineEvents.Count; ++index2)
        {
          IEvent @event = engineEvents[index2];
          string key = functionalComponents[index1].Name + "." + @event.Name;
          if (functionalMappedEventsDict == null)
            functionalMappedEventsDict = new Dictionary<string, IEvent>();
          if (!functionalMappedEventsDict.ContainsKey(key))
            functionalMappedEventsDict.Add(key, @event);
          else
            Logger.AddError(string.Format("Event with name {0} is dublicated in {1}", key, Name));
        }
      }
    }
  }
}
