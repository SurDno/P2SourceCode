// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Objects.VMBlueprint
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Loggers;
using Engine.Common.Commons;
using Engine.Common.Comparers;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Data;
using System.Collections.Generic;
using System.Xml;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;
using VirtualMachine.Data;

#nullable disable
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
    [FieldData("Static", DataFieldType.None)]
    protected bool isStatic;
    [FieldData("InheritanceInfo", DataFieldType.Reference)]
    protected List<IBlueprint> baseBlueprints;
    private Dictionary<string, IFunctionalComponent> allFunctionalComponentsDict;
    private bool isInheritanceMapLoaded;
    private Dictionary<ulong, ulong> inheritancyMappedEventGuids;
    private Dictionary<string, IEvent> functionalMappedEventsDict;

    public virtual void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
    {
      while (xml.Read())
      {
        if (xml.NodeType == XmlNodeType.Element)
        {
          switch (xml.Name)
          {
            case "ChildObjects":
              this.gameObjects = EditorDataReadUtility.ReadReferenceList<IContainer>(xml, creator, this.gameObjects);
              continue;
            case "CustomParams":
              this.customParamsDict = EditorDataReadUtility.ReadStringReferenceDictionary<IParam>(xml, creator, this.customParamsDict);
              continue;
            case "EventGraph":
              this.stateGraph = EditorDataReadUtility.ReadReference<IFiniteStateMachine>(xml, creator);
              continue;
            case "Events":
              this.customEventsList = EditorDataReadUtility.ReadReferenceList<IEvent>(xml, creator, this.customEventsList);
              continue;
            case "FunctionalComponents":
              this.functionalComponents = EditorDataReadUtility.ReadReferenceList<IFunctionalComponent>(xml, creator, this.functionalComponents);
              continue;
            case "GameTimeContext":
              this.gameTimeContext = EditorDataReadUtility.ReadReference<IGameMode>(xml, creator);
              continue;
            case "InheritanceInfo":
              this.baseBlueprints = EditorDataReadUtility.ReadReferenceList<IBlueprint>(xml, creator, this.baseBlueprints);
              continue;
            case "Name":
              this.name = EditorDataReadUtility.ReadValue(xml, this.name);
              continue;
            case "Parent":
              this.parent = EditorDataReadUtility.ReadReference<IContainer>(xml, creator);
              continue;
            case "StandartParams":
              this.standartParamsDict = EditorDataReadUtility.ReadStringReferenceDictionary<IParam>(xml, creator, this.standartParamsDict);
              continue;
            case "Static":
              this.isStatic = EditorDataReadUtility.ReadValue(xml, this.isStatic);
              continue;
            default:
              if (XMLDataLoader.Logs.Add(typeContext + " : " + xml.Name))
                Logger.AddError(typeContext + " : " + xml.Name);
              XmlReaderUtility.SkipNode(xml);
              continue;
          }
        }
        else if (xml.NodeType == XmlNodeType.EndElement)
          break;
      }
    }

    public VMBlueprint(ulong guid)
      : base(guid)
    {
    }

    public List<IBlueprint> BaseBlueprints => this.baseBlueprints;

    public override EObjectCategory GetCategory() => EObjectCategory.OBJECT_CATEGORY_CLASS;

    public override bool Static => this.isStatic;

    public override IBlueprint Blueprint => (IBlueprint) this;

    public override Dictionary<string, IFunctionalComponent> FunctionalComponents
    {
      get
      {
        if (this.allFunctionalComponentsDict == null)
          this.allFunctionalComponentsDict = this.GetAllFunctionalComponents();
        return this.allFunctionalComponentsDict;
      }
    }

    public override List<IEvent> CustomEvents
    {
      get
      {
        List<IEvent> mergedEventsTo = new List<IEvent>();
        if (this.baseBlueprints != null)
        {
          for (int index = 0; index < this.baseBlueprints.Count; ++index)
            VMBlueprintUtility.MergeCustomEvents(mergedEventsTo, ((VMLogicObject) this.baseBlueprints[index]).CustomEvents);
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
          if (this.baseBlueprints != null)
          {
            for (int index = 0; index < this.baseBlueprints.Count; ++index)
              VMBlueprintUtility.MergeContextParams(mergedParamsTo, this.baseBlueprints[index].GetContextVariables(contextVarCategory));
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
      if (bWithSelf && iBlueprintGuid != 0UL && (long) this.BaseGuid == (long) iBlueprintGuid)
        return true;
      if (this.baseBlueprints != null)
      {
        for (int index = 0; index < this.baseBlueprints.Count; ++index)
        {
          if ((long) this.baseBlueprints[index].BaseGuid == (long) iBlueprintGuid || this.baseBlueprints[index].IsDerivedFrom(iBlueprintGuid))
            return true;
        }
      }
      return false;
    }

    public override IEnumerable<IStateRef> GetObjectStates()
    {
      foreach (IStateRef objectState in base.GetObjectStates())
        yield return objectState;
      if (this.baseBlueprints != null)
      {
        for (int i = 0; i < this.baseBlueprints.Count; ++i)
        {
          foreach (IStateRef objectState in this.baseBlueprints[i].GetObjectStates())
            yield return objectState;
        }
      }
    }

    public virtual ulong GetInheritanceMappedEventGuid(ulong stEventGuid)
    {
      if (this.VirtualObjectTemplate != null)
        return ((VMBlueprint) this.VirtualObjectTemplate).GetInheritanceMappedEventGuid(stEventGuid);
      if (!this.isInheritanceMapLoaded)
        this.MakeInheritanceMapping();
      return this.inheritancyMappedEventGuids != null && this.inheritancyMappedEventGuids.ContainsKey(stEventGuid) ? this.inheritancyMappedEventGuids[stEventGuid] : stEventGuid;
    }

    public Dictionary<string, IEvent> FunctionalMappedEvents
    {
      get
      {
        if (this.functionalMappedEventsDict == null)
          this.LoadFunctionalMappedEvents();
        else if (this.functionalMappedEventsDict.Count == 0)
          this.LoadFunctionalMappedEvents();
        return this.functionalMappedEventsDict;
      }
    }

    public override void OnAfterLoad()
    {
      base.OnAfterLoad();
      this.MakeInheritanceMapping();
    }

    public virtual IContainer VirtualObjectTemplate => (IContainer) null;

    protected override void UpdateContextVariablesTotalCache()
    {
      base.UpdateContextVariablesTotalCache();
      foreach (IVariable contextVariable in this.GetContextVariables(EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_PARAM))
        this.LoadContextVaraiblesTotalCache(contextVariable);
    }

    protected override bool CanReloadVariablesTotalCache()
    {
      return this.baseBlueprints != null && this.baseBlueprints.Count > 0 || base.CanReloadVariablesTotalCache();
    }

    private Dictionary<string, IFunctionalComponent> GetAllFunctionalComponents()
    {
      Dictionary<string, IFunctionalComponent> mergeTo = new Dictionary<string, IFunctionalComponent>();
      if (this.baseBlueprints != null)
      {
        for (int index = 0; index < this.baseBlueprints.Count; ++index)
        {
          Dictionary<string, IFunctionalComponent> functionalComponents = ((VMBlueprint) this.baseBlueprints[index]).GetAllFunctionalComponents();
          VMBlueprintUtility.MergeInheritedComponents(mergeTo, functionalComponents);
        }
      }
      VMBlueprintUtility.MergeComponents(mergeTo, this.functionalComponents);
      return mergeTo;
    }

    protected virtual void MakeInheritanceMapping()
    {
      this.MakeInheritanceMapping(this.GetClassHierarchy());
    }

    protected void MakeInheritanceMapping(IEnumerable<IBlueprint> classHierarchyList)
    {
      if (this.events != null)
      {
        for (int index = 0; index < this.events.Count; ++index)
        {
          string key = this.events[index].Parent.Name + "." + this.events[index].Name;
          foreach (VMBlueprint classHierarchy in classHierarchyList)
          {
            Dictionary<string, IEvent> functionalMappedEvents = classHierarchy.FunctionalMappedEvents;
            if (functionalMappedEvents != null && functionalMappedEvents.ContainsKey(key))
            {
              IEvent @event = functionalMappedEvents[key];
              if (this.inheritancyMappedEventGuids == null)
                this.inheritancyMappedEventGuids = new Dictionary<ulong, ulong>((IEqualityComparer<ulong>) UlongComparer.Instance);
              if (!this.inheritancyMappedEventGuids.ContainsKey(@event.BaseGuid))
                this.inheritancyMappedEventGuids.Add(@event.BaseGuid, this.events[index].BaseGuid);
              else
                Logger.AddError(string.Format("Inheritance mapped event with id={0} is dublicated in {1}", (object) @event.BaseGuid, (object) this.Name));
            }
          }
        }
      }
      this.isInheritanceMapLoaded = true;
    }

    protected IEnumerable<IBlueprint> GetClassHierarchy()
    {
      if (this.baseBlueprints != null)
      {
        for (int i = 0; i < this.baseBlueprints.Count; ++i)
        {
          yield return this.baseBlueprints[i];
          foreach (IBlueprint blueprint in ((VMBlueprint) this.baseBlueprints[i]).GetClassHierarchy())
            yield return blueprint;
        }
      }
    }

    private void LoadFunctionalMappedEvents()
    {
      if (this.functionalMappedEventsDict != null)
        this.functionalMappedEventsDict.Clear();
      for (int index1 = 0; index1 < this.functionalComponents.Count; ++index1)
      {
        List<IEvent> engineEvents = this.functionalComponents[index1].EngineEvents;
        for (int index2 = 0; index2 < engineEvents.Count; ++index2)
        {
          IEvent @event = engineEvents[index2];
          string key = this.functionalComponents[index1].Name + "." + @event.Name;
          if (this.functionalMappedEventsDict == null)
            this.functionalMappedEventsDict = new Dictionary<string, IEvent>();
          if (!this.functionalMappedEventsDict.ContainsKey(key))
            this.functionalMappedEventsDict.Add(key, @event);
          else
            Logger.AddError(string.Format("Event with name {0} is dublicated in {1}", (object) key, (object) this.Name));
        }
      }
    }
  }
}
