// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Objects.VMWorldHierarchyObject
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Loggers;
using PLVirtualMachine.Common;
using PLVirtualMachine.GameLogic;
using System;
using System.Collections.Generic;

#nullable disable
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
        Logger.AddError(string.Format("Template {0} for hierarchy object must be vm world object !", (object) templateObject.Name));
      }
      else
      {
        this.template = templateObject;
        this.hierarchyGuid = new HierarchyGuid(this.template.BaseGuid);
        HierarchyManager.CreateChilds(this);
        HierarchyManager.CreateSimpleChilds(this);
      }
    }

    public ulong BaseGuid => this.template.BaseGuid;

    public string Name => this.template.Name;

    public bool Static => true;

    public IBlueprint Blueprint => (IBlueprint) this.template;

    public IBlueprint EditorTemplate => (IBlueprint) this.template;

    public Guid EngineGuid => this.engineInstanceGuid;

    public Guid EngineTemplateGuid => this.template.EngineTemplateGuid;

    public Guid EngineBaseTemplateGuid => this.template.EngineBaseTemplateGuid;

    public IVariable GetSelf()
    {
      if (this.selfRef == null)
      {
        this.selfRef = new VMObjRef();
        this.selfRef.Initialize(this.HierarchyGuid);
      }
      return (IVariable) this.selfRef;
    }

    public IEnumerable<string> GetComponentNames() => this.Blueprint.GetComponentNames();

    public IEnumerable<IVariable> GetContextVariables(EContextVariableCategory contextVarCategory)
    {
      return this.Blueprint.GetContextVariables(contextVarCategory);
    }

    public IVariable GetContextVariable(string variableName)
    {
      return this.Blueprint.GetContextVariable(variableName);
    }

    public bool IsFunctionalSupport(string componentName)
    {
      return this.Blueprint.IsFunctionalSupport(componentName);
    }

    public bool IsFunctionalSupport(IEnumerable<string> functionalsList)
    {
      return this.Blueprint.IsFunctionalSupport(functionalsList);
    }

    public HierarchyGuid WorldPositionGuid
    {
      get
      {
        return this.template.WorldPositionGuid.IsEmpty ? this.HierarchyGuid : this.template.WorldPositionGuid;
      }
    }

    public HierarchyGuid HierarchyGuid => this.hierarchyGuid;

    public IEnumerable<IHierarchyObject> HierarchyChilds
    {
      get
      {
        if (this.hierarchyChilds != null)
        {
          for (int i = 0; i < this.hierarchyChilds.Count; ++i)
            yield return (IHierarchyObject) this.hierarchyChilds[i];
        }
      }
    }

    public void AddHierarchyChild(IHierarchyObject child)
    {
      if (this.hierarchyChilds == null)
        this.hierarchyChilds = new List<IWorldHierarchyObject>();
      this.hierarchyChilds.Add((IWorldHierarchyObject) child);
    }

    public void AddHierarchySimpleChild(IHierarchyObject child)
    {
      if (this.hierarchySimpleChildsList == null)
        this.hierarchySimpleChildsList = new List<IHierarchyObject>();
      this.hierarchySimpleChildsList.Add(child);
    }

    public IEnumerable<IHierarchyObject> SimpleChilds
    {
      get
      {
        if (this.hierarchySimpleChildsList != null)
        {
          foreach (IHierarchyObject hierarchySimpleChilds in this.hierarchySimpleChildsList)
            yield return hierarchySimpleChilds;
        }
      }
    }

    public void ClearHierarchy()
    {
      if (this.hierarchyChilds != null)
      {
        foreach (IWorldHierarchyObject hierarchyChild in this.hierarchyChilds)
          hierarchyChild.ClearHierarchy();
        this.hierarchyChilds.Clear();
        this.hierarchyChilds = (List<IWorldHierarchyObject>) null;
      }
      if (this.hierarchySimpleChildsList != null)
      {
        foreach (HierarchySimpleChildInfo hierarchySimpleChilds in this.hierarchySimpleChildsList)
          hierarchySimpleChilds.ClearHierarchy();
        this.hierarchySimpleChildsList.Clear();
        this.hierarchySimpleChildsList = (List<IHierarchyObject>) null;
      }
      this.selfRef = (VMObjRef) null;
      this.template = (IWorldBlueprint) null;
      this.parent = (IWorldHierarchyObject) null;
    }

    public virtual bool IsPhysic => true;

    public bool Instantiated => false;

    public bool IsEngineRoot => this.template.WorldPositionGuid.TemplateGuid == ulong.MaxValue;

    public bool DirectEngineCreated => ((VMLogicObject) this.template).DirectEngineCreated;

    public void InitInstanceGuid(Guid instanceGuid)
    {
      if (Guid.Empty != this.engineInstanceGuid)
        Logger.AddError("Engine instance guid already inited in world hierarchy object with hid = " + this.HierarchyGuid.Write());
      else
        this.engineInstanceGuid = instanceGuid;
    }

    public IWorldHierarchyObject Parent
    {
      get => this.parent;
      set
      {
        this.parent = value;
        this.hierarchyGuid = new HierarchyGuid(this.parent.HierarchyGuid, this.Blueprint.BaseGuid);
        if (this.hierarchyChilds != null)
        {
          foreach (IWorldHierarchyObject hierarchyChild in this.hierarchyChilds)
            hierarchyChild.Parent = value;
        }
        foreach (HierarchySimpleChildInfo simpleChild in this.SimpleChilds)
          simpleChild.SetParent((IWorldHierarchyObject) this);
      }
    }

    public bool IsPhantom => ((VMWorldObject) this.template).IsPhantom;
  }
}
