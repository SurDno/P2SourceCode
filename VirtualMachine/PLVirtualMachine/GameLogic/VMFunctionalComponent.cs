using Cofe.Loggers;
using Engine.Common.Commons;
using Engine.Common.Reflection;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using PLVirtualMachine.Data;
using System;
using System.Collections.Generic;
using System.Xml;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;
using VirtualMachine.Data;

namespace PLVirtualMachine.GameLogic
{
  [TypeData(EDataType.TFunctionalComponent)]
  [DataFactory("FunctionalComponent")]
  public class VMFunctionalComponent : 
    VMBaseObject,
    IStub,
    IEditorDataReader,
    IFunctionalComponent,
    IContainer,
    IObject,
    IEditorBaseTemplate,
    INamedElement,
    INamed,
    IStaticUpdateable
  {
    [FieldData("Events", DataFieldType.Reference)]
    private List<IEvent> eventsList = new List<IEvent>();
    [FieldData("Main", DataFieldType.None)]
    private bool isMain;
    [FieldData("LoadPriority", DataFieldType.None)]
    private long loadPriority = long.MaxValue;
    private string dependedComponentName;
    private List<BaseFunction> functions = new List<BaseFunction>();
    private Type componentType;
    private bool afterLoaded;

    public virtual void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
    {
      while (xml.Read())
      {
        if (xml.NodeType == XmlNodeType.Element)
        {
          switch (xml.Name)
          {
            case "Events":
              this.eventsList = EditorDataReadUtility.ReadReferenceList<IEvent>(xml, creator, this.eventsList);
              continue;
            case "Main":
              this.isMain = EditorDataReadUtility.ReadValue(xml, this.isMain);
              continue;
            case "LoadPriority":
              this.loadPriority = EditorDataReadUtility.ReadValue(xml, this.loadPriority);
              continue;
            case "Name":
              this.name = EditorDataReadUtility.ReadValue(xml, this.name);
              continue;
            case "Parent":
              this.parent = EditorDataReadUtility.ReadReference<IContainer>(xml, creator);
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

    public VMFunctionalComponent(ulong guid)
      : base(guid)
    {
    }

    public override EObjectCategory GetCategory() => EObjectCategory.OBJECT_CATEGORY_FUNC_COMPONENT;

    public string DependedComponentName => this.dependedComponentName;

    public Type ComponentType => this.componentType;

    public List<IEvent> EngineEvents => this.eventsList;

    public List<BaseFunction> EngineFunctions => this.functions;

    public bool Main => this.isMain;

    public bool Inited => this.componentType != (Type) null;

    public long LoadPriority => this.loadPriority;

    public void OnAfterLoad()
    {
      this.componentType = EngineAPIManager.GetComponentTypeByName(this.name);
      if ((Type) null == this.componentType)
        return;
      ComponentReplectionInfo componentInfo = InfoAttribute.GetComponentInfo(this.Name);
      if (componentInfo != null)
        this.dependedComponentName = componentInfo.DependedComponentName;
      this.LoadAPIFunctions();
      for (int index = 0; index < this.eventsList.Count; ++index)
        ((VMEvent) this.eventsList[index]).OnAfterLoad();
      this.afterLoaded = true;
    }

    public bool IsAfterLoaded => this.afterLoaded;

    public override void Clear()
    {
      base.Clear();
      if (this.eventsList != null)
      {
        foreach (IContainer events in this.eventsList)
          events.Clear();
        this.eventsList.Clear();
        this.eventsList = (List<IEvent>) null;
      }
      if (this.functions == null)
        return;
      foreach (BaseFunction function in this.functions)
        function.Clear();
      this.functions.Clear();
      this.functions = (List<BaseFunction>) null;
    }

    private void LoadAPIFunctions()
    {
      this.functions.Clear();
      ComponentInfo functionalComponentByName = EngineAPIManager.GetFunctionalComponentByName(this.Name);
      for (int index = 0; index < functionalComponentByName.Methods.Count; ++index)
      {
        BaseFunction baseFunction = new BaseFunction(functionalComponentByName.Methods[index].MethodName, (IFunctionalComponent) this);
        baseFunction.InitParams(functionalComponentByName.Methods[index].InputParams, functionalComponentByName.Methods[index].ReturnParam);
        this.functions.Add(baseFunction);
      }
    }
  }
}
