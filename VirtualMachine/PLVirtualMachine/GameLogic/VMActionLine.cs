using Cofe.Loggers;
using Engine.Common.Commons;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Data;
using System.Collections.Generic;
using System.Xml;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;
using VirtualMachine.Data;
using VirtualMachine.Data.Customs;

namespace PLVirtualMachine.GameLogic
{
  [TypeData(EDataType.TActionLine)]
  [DataFactory("ActionLine")]
  public class VMActionLine : 
    IStub,
    IEditorDataReader,
    IActionLine,
    IGameAction,
    IOrderedChild,
    IContextElement,
    IObject,
    IEditorBaseTemplate,
    INamed,
    IBaseAction,
    IStaticUpdateable,
    IActionLoop
  {
    private ulong guid;
    [FieldData("Name", DataFieldType.None)]
    private string name = "";
    [FieldData("Actions", DataFieldType.Reference)]
    private List<IGameAction> actionsList = new List<IGameAction>();
    [FieldData("LocalContext", DataFieldType.Reference)]
    private ILocalContext localContext;
    [FieldData("ActionLineType", DataFieldType.None)]
    private EActionLineType actionLineType;
    [FieldData("OrderIndex", DataFieldType.None)]
    private int orderIndex;
    [FieldData("ActionLoopInfo", DataFieldType.None)]
    private ActionLoopInfoData actionLoopInfo;
    private CommonVariable loopListParam;
    private object startLoopIndexParam = (object) 0;
    private object endLoopIndexParam = (object) 1;
    private IVariable loopListParamInstance;
    private List<IVariable> loopLocalVariables = new List<IVariable>();
    private bool loopRandomIndexing;
    private bool updated;

    public virtual void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
    {
      while (xml.Read())
      {
        if (xml.NodeType == XmlNodeType.Element)
        {
          switch (xml.Name)
          {
            case "Name":
              this.name = EditorDataReadUtility.ReadValue(xml, this.name);
              continue;
            case "Actions":
              this.actionsList = EditorDataReadUtility.ReadReferenceList<IGameAction>(xml, creator, this.actionsList);
              continue;
            case "LocalContext":
              this.localContext = EditorDataReadUtility.ReadReference<ILocalContext>(xml, creator);
              continue;
            case "ActionLineType":
              this.actionLineType = EditorDataReadUtility.ReadEnum<EActionLineType>(xml);
              continue;
            case "OrderIndex":
              this.orderIndex = EditorDataReadUtility.ReadValue(xml, this.orderIndex);
              continue;
            case "ActionLoopInfo":
              this.actionLoopInfo = EditorDataReadUtility.ReadEditorDataSerializable<ActionLoopInfoData>(xml, creator, typeContext);
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

    public VMActionLine(ulong guid) => this.guid = guid;

    public ulong BaseGuid => this.guid;

    public string Name => this.name;

    public int Order => this.orderIndex;

    public ILocalContext LocalContext => this.localContext;

    public bool IsValid
    {
      get
      {
        for (int index = 0; index < this.actionsList.Count; ++index)
        {
          if (!this.actionsList[index].IsValid)
            return false;
        }
        return true;
      }
    }

    public EActionLineType ActionLineType => this.actionLineType;

    public List<IGameAction> Actions => this.actionsList;

    public List<IVariable> LocalContextVariables => this.loopLocalVariables;

    public IVariable GetLocalContextVariable(string name)
    {
      foreach (IVariable loopLocalVariable in this.loopLocalVariables)
      {
        if (loopLocalVariable.Name == name)
          return loopLocalVariable;
      }
      return (IVariable) null;
    }

    public CommonVariable LoopListParam => this.loopListParam;

    public object StartIndexParam => this.startLoopIndexParam;

    public object EndIndexParam => this.endLoopIndexParam;

    public IVariable LoopListParamInstance
    {
      get
      {
        if (this.loopListParam == null)
          return (IVariable) null;
        if (this.loopListParamInstance == null)
        {
          IContext ownerContext = (IContext) IStaticDataContainer.StaticDataContainer.GameRoot;
          if (this.localContext != null && this.localContext.Owner != null && typeof (IContext).IsAssignableFrom(this.localContext.Owner.GetType()))
            ownerContext = (IContext) this.localContext.Owner;
          this.loopListParam.Bind(ownerContext, localContext: this.localContext);
          if (this.loopListParam.Variable != null && typeof (IVariable).IsAssignableFrom(this.loopListParam.Variable.GetType()))
            this.loopListParamInstance = (IVariable) this.loopListParam.Variable;
        }
        return this.loopListParamInstance;
      }
    }

    public bool LoopRandomIndexing => this.loopRandomIndexing;

    public bool IsEqual(IObject other)
    {
      return other != null && typeof (VMActionLine) == other.GetType() && (long) this.BaseGuid == (long) ((VMActionLine) other).BaseGuid;
    }

    public void Update()
    {
      if (!VMBaseObjectUtility.CheckOrders<IGameAction>(this.actionsList))
        Logger.AddError(string.Format("Action line id={0} has invalid actions ordering", (object) this.BaseGuid));
      if (this.ActionLineType == EActionLineType.ACTION_LINE_TYPE_LOOP)
      {
        this.startLoopIndexParam = (object) 0;
        this.loopListParam = (CommonVariable) null;
        if (this.actionLoopInfo != null)
        {
          string start = this.actionLoopInfo.Start;
          string end = this.actionLoopInfo.End;
          if ("none" != this.name.ToLower())
          {
            this.loopListParam = new CommonVariable();
            this.loopListParam.Read(this.actionLoopInfo.Name);
          }
          string str = "const_";
          if (start.StartsWith(str))
          {
            this.startLoopIndexParam = (object) StringUtility.ToInt32(start.Substring(str.Length));
          }
          else
          {
            this.startLoopIndexParam = (object) new CommonVariable();
            ((CommonVariable) this.startLoopIndexParam).Read(start);
          }
          if (end.StartsWith(str))
          {
            this.endLoopIndexParam = (object) StringUtility.ToInt32(end.Substring(str.Length));
          }
          else
          {
            this.endLoopIndexParam = (object) new CommonVariable();
            ((CommonVariable) this.endLoopIndexParam).Read(end);
          }
          this.loopRandomIndexing = this.actionLoopInfo.Random;
          this.actionLoopInfo = (ActionLoopInfoData) null;
        }
        else
          Logger.AddError("Invalid action line info for action loop");
        this.MakeLoopLocalVariables();
      }
      for (int index = 0; index < this.actionsList.Count; ++index)
        this.actionsList[index].Update();
      this.updated = true;
    }

    public bool IsUpdated => this.updated;

    public void Clear()
    {
      if (this.actionsList != null)
      {
        foreach (IGameAction actions in this.actionsList)
        {
          if (typeof (VMGameAction) == actions.GetType())
            ((VMGameAction) actions).Clear();
        }
        this.actionsList.Clear();
        this.actionsList = (List<IGameAction>) null;
      }
      this.actionLoopInfo = (ActionLoopInfoData) null;
      this.localContext = (ILocalContext) null;
      if (this.loopListParam != null)
      {
        this.loopListParam.Clear();
        this.loopListParam = (CommonVariable) null;
      }
      this.startLoopIndexParam = (object) null;
      this.endLoopIndexParam = (object) null;
      this.loopListParamInstance = (IVariable) null;
    }

    private void MakeLoopLocalVariables()
    {
      this.loopLocalVariables.Clear();
      IVariable listParamInstance = this.LoopListParamInstance;
      if (listParamInstance != null)
      {
        string name = "local_" + (object) this.BaseGuid + "_Loop_List_" + listParamInstance.Name + "_Element";
        VMType listElementType = listParamInstance.Type.GetListElementType();
        LocalVariable localVariable = new LocalVariable();
        localVariable.Initialize(name, listElementType);
        this.loopLocalVariables.Add((IVariable) localVariable);
      }
      string name1 = "local_" + (object) this.BaseGuid + "_Loop_Index";
      LocalVariable localVariable1 = new LocalVariable();
      localVariable1.Initialize(name1, new VMType(typeof (int)));
      this.loopLocalVariables.Add((IVariable) localVariable1);
    }

    public string GuidStr => this.guid.ToString();
  }
}
