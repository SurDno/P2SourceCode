using System;
using System.Collections.Generic;
using Cofe.Loggers;
using PLVirtualMachine.Common;
using PLVirtualMachine.GameLogic;

namespace PLVirtualMachine.Objects
{
  public class VMWorldHierarchyObject : 
    IWorldHierarchyObject,
    ILogicObject,
    IContext,
    INamed,
    IWorldObject,
    IEngineTemplated,
    IHierarchyObject,
    IEngineInstanced
  {
    private VMObjRef selfRef;
    private Guid engineInstanceGuid = Guid.Empty;
    private HierarchyGuid hierarchyGuid;
    private IWorldBlueprint template;
    private IWorldHierarchyObject parent;
    private List<IHierarchyObject> hierarchySimpleChildsList;
    private List<IWorldHierarchyObject> hierarchyChilds;

    public void Initialize(IWorldBlueprint templateObject)
    {
      if (!typeof (VMWorldObject).IsAssignableFrom(templateObject.GetType()))
      {
        Logger.AddError(string.Format("Template {0} for hierarchy object must be vm world object !", templateObject.Name));
      }
      else
      {
        template = templateObject;
        hierarchyGuid = new HierarchyGuid(template.BaseGuid);
        HierarchyManager.CreateChilds(this);
        HierarchyManager.CreateSimpleChilds(this);
      }
    }

    public ulong BaseGuid => template.BaseGuid;

    public string Name => template.Name;

    public bool Static => true;

    public IBlueprint Blueprint => template;

    public IBlueprint EditorTemplate => template;

    public Guid EngineGuid => engineInstanceGuid;

    public Guid EngineTemplateGuid => template.EngineTemplateGuid;

    public Guid EngineBaseTemplateGuid => template.EngineBaseTemplateGuid;

    public IVariable GetSelf()
    {
      if (selfRef == null)
      {
        selfRef = new VMObjRef();
        selfRef.Initialize(HierarchyGuid);
      }
      return selfRef;
    }

    public IEnumerable<string> GetComponentNames() => Blueprint.GetComponentNames();

    public IEnumerable<IVariable> GetContextVariables(EContextVariableCategory contextVarCategory)
    {
      return Blueprint.GetContextVariables(contextVarCategory);
    }

    public IVariable GetContextVariable(string variableName)
    {
      return Blueprint.GetContextVariable(variableName);
    }

    public bool IsFunctionalSupport(string componentName)
    {
      return Blueprint.IsFunctionalSupport(componentName);
    }

    public bool IsFunctionalSupport(IEnumerable<string> functionalsList)
    {
      return Blueprint.IsFunctionalSupport(functionalsList);
    }

    public HierarchyGuid WorldPositionGuid
    {
      get
      {
        return template.WorldPositionGuid.IsEmpty ? HierarchyGuid : template.WorldPositionGuid;
      }
    }

    public HierarchyGuid HierarchyGuid => hierarchyGuid;

    public IEnumerable<IHierarchyObject> HierarchyChilds
    {
      get
      {
        if (hierarchyChilds != null)
        {
          for (int i = 0; i < hierarchyChilds.Count; ++i)
            yield return hierarchyChilds[i];
        }
      }
    }

    public void AddHierarchyChild(IHierarchyObject child)
    {
      if (hierarchyChilds == null)
        hierarchyChilds = new List<IWorldHierarchyObject>();
      hierarchyChilds.Add((IWorldHierarchyObject) child);
    }

    public void AddHierarchySimpleChild(IHierarchyObject child)
    {
      if (hierarchySimpleChildsList == null)
        hierarchySimpleChildsList = new List<IHierarchyObject>();
      hierarchySimpleChildsList.Add(child);
    }

    public IEnumerable<IHierarchyObject> SimpleChilds
    {
      get
      {
        if (hierarchySimpleChildsList != null)
        {
          foreach (IHierarchyObject hierarchySimpleChilds in hierarchySimpleChildsList)
            yield return hierarchySimpleChilds;
        }
      }
    }

    public void ClearHierarchy()
    {
      if (hierarchyChilds != null)
      {
        foreach (IWorldHierarchyObject hierarchyChild in hierarchyChilds)
          hierarchyChild.ClearHierarchy();
        hierarchyChilds.Clear();
        hierarchyChilds = null;
      }
      if (hierarchySimpleChildsList != null)
      {
        foreach (HierarchySimpleChildInfo hierarchySimpleChilds in hierarchySimpleChildsList)
          hierarchySimpleChilds.ClearHierarchy();
        hierarchySimpleChildsList.Clear();
        hierarchySimpleChildsList = null;
      }
      selfRef = null;
      template = null;
      parent = null;
    }

    public virtual bool IsPhysic => true;

    public bool Instantiated => false;

    public bool IsEngineRoot => template.WorldPositionGuid.TemplateGuid == ulong.MaxValue;

    public bool DirectEngineCreated => ((VMLogicObject) template).DirectEngineCreated;

    public void InitInstanceGuid(Guid instanceGuid)
    {
      if (Guid.Empty != engineInstanceGuid)
        Logger.AddError("Engine instance guid already inited in world hierarchy object with hid = " + HierarchyGuid.Write());
      else
        engineInstanceGuid = instanceGuid;
    }

    public IWorldHierarchyObject Parent
    {
      get => parent;
      set
      {
        parent = value;
        hierarchyGuid = new HierarchyGuid(parent.HierarchyGuid, Blueprint.BaseGuid);
        if (hierarchyChilds != null)
        {
          foreach (IWorldHierarchyObject hierarchyChild in hierarchyChilds)
            hierarchyChild.Parent = value;
        }
        foreach (HierarchySimpleChildInfo simpleChild in SimpleChilds)
          simpleChild.SetParent(this);
      }
    }

    public bool IsPhantom => ((VMWorldObject) template).IsPhantom;
  }
}
