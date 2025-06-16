using System.Collections.Generic;
using System.Xml;
using Cofe.Loggers;
using Engine.Common.Commons;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Data;
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
    [FieldData("Name")]
    private string name = "";
    [FieldData("Actions", DataFieldType.Reference)]
    private List<IGameAction> actionsList = new List<IGameAction>();
    [FieldData("LocalContext", DataFieldType.Reference)]
    private ILocalContext localContext;
    [FieldData("ActionLineType")]
    private EActionLineType actionLineType;
    [FieldData("OrderIndex")]
    private int orderIndex;
    [FieldData("ActionLoopInfo")]
    private ActionLoopInfoData actionLoopInfo;
    private CommonVariable loopListParam;
    private object startLoopIndexParam = 0;
    private object endLoopIndexParam = 1;
    private IVariable loopListParamInstance;
    private List<IVariable> loopLocalVariables = new List<IVariable>();
    private bool loopRandomIndexing;
    private bool updated;

    public virtual void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
    {
      while (xml.Read()) {
        if (xml.NodeType == XmlNodeType.Element)
        {
          switch (xml.Name)
          {
            case "Name":
              name = EditorDataReadUtility.ReadValue(xml, name);
              continue;
            case "Actions":
              actionsList = EditorDataReadUtility.ReadReferenceList(xml, creator, actionsList);
              continue;
            case "LocalContext":
              localContext = EditorDataReadUtility.ReadReference<ILocalContext>(xml, creator);
              continue;
            case "ActionLineType":
              actionLineType = EditorDataReadUtility.ReadEnum<EActionLineType>(xml);
              continue;
            case "OrderIndex":
              orderIndex = EditorDataReadUtility.ReadValue(xml, orderIndex);
              continue;
            case "ActionLoopInfo":
              actionLoopInfo = EditorDataReadUtility.ReadEditorDataSerializable<ActionLoopInfoData>(xml, creator, typeContext);
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

    public VMActionLine(ulong guid) => this.guid = guid;

    public ulong BaseGuid => guid;

    public string Name => name;

    public int Order => orderIndex;

    public ILocalContext LocalContext => localContext;

    public bool IsValid
    {
      get
      {
        for (int index = 0; index < actionsList.Count; ++index)
        {
          if (!actionsList[index].IsValid)
            return false;
        }
        return true;
      }
    }

    public EActionLineType ActionLineType => actionLineType;

    public List<IGameAction> Actions => actionsList;

    public List<IVariable> LocalContextVariables => loopLocalVariables;

    public IVariable GetLocalContextVariable(string name)
    {
      foreach (IVariable loopLocalVariable in loopLocalVariables)
      {
        if (loopLocalVariable.Name == name)
          return loopLocalVariable;
      }
      return null;
    }

    public CommonVariable LoopListParam => loopListParam;

    public object StartIndexParam => startLoopIndexParam;

    public object EndIndexParam => endLoopIndexParam;

    public IVariable LoopListParamInstance
    {
      get
      {
        if (loopListParam == null)
          return null;
        if (loopListParamInstance == null)
        {
          IContext ownerContext = IStaticDataContainer.StaticDataContainer.GameRoot;
          if (localContext != null && localContext.Owner != null && typeof (IContext).IsAssignableFrom(localContext.Owner.GetType()))
            ownerContext = (IContext) localContext.Owner;
          loopListParam.Bind(ownerContext, localContext: localContext);
          if (loopListParam.Variable != null && typeof (IVariable).IsAssignableFrom(loopListParam.Variable.GetType()))
            loopListParamInstance = (IVariable) loopListParam.Variable;
        }
        return loopListParamInstance;
      }
    }

    public bool LoopRandomIndexing => loopRandomIndexing;

    public bool IsEqual(IObject other)
    {
      return other != null && typeof (VMActionLine) == other.GetType() && (long) BaseGuid == (long) ((VMActionLine) other).BaseGuid;
    }

    public void Update()
    {
      if (!VMBaseObjectUtility.CheckOrders(actionsList))
        Logger.AddError(string.Format("Action line id={0} has invalid actions ordering", BaseGuid));
      if (ActionLineType == EActionLineType.ACTION_LINE_TYPE_LOOP)
      {
        startLoopIndexParam = 0;
        loopListParam = null;
        if (actionLoopInfo != null)
        {
          string start = actionLoopInfo.Start;
          string end = actionLoopInfo.End;
          if ("none" != name.ToLower())
          {
            loopListParam = new CommonVariable();
            loopListParam.Read(actionLoopInfo.Name);
          }
          string str = "const_";
          if (start.StartsWith(str))
          {
            startLoopIndexParam = StringUtility.ToInt32(start.Substring(str.Length));
          }
          else
          {
            startLoopIndexParam = new CommonVariable();
            ((CommonVariable) startLoopIndexParam).Read(start);
          }
          if (end.StartsWith(str))
          {
            endLoopIndexParam = StringUtility.ToInt32(end.Substring(str.Length));
          }
          else
          {
            endLoopIndexParam = new CommonVariable();
            ((CommonVariable) endLoopIndexParam).Read(end);
          }
          loopRandomIndexing = actionLoopInfo.Random;
          actionLoopInfo = null;
        }
        else
          Logger.AddError("Invalid action line info for action loop");
        MakeLoopLocalVariables();
      }
      for (int index = 0; index < actionsList.Count; ++index)
        actionsList[index].Update();
      updated = true;
    }

    public bool IsUpdated => updated;

    public void Clear()
    {
      if (actionsList != null)
      {
        foreach (IGameAction actions in actionsList)
        {
          if (typeof (VMGameAction) == actions.GetType())
            ((VMGameAction) actions).Clear();
        }
        actionsList.Clear();
        actionsList = null;
      }
      actionLoopInfo = null;
      localContext = null;
      if (loopListParam != null)
      {
        loopListParam.Clear();
        loopListParam = null;
      }
      startLoopIndexParam = null;
      endLoopIndexParam = null;
      loopListParamInstance = null;
    }

    private void MakeLoopLocalVariables()
    {
      loopLocalVariables.Clear();
      IVariable listParamInstance = LoopListParamInstance;
      if (listParamInstance != null)
      {
        string name = "local_" + BaseGuid + "_Loop_List_" + listParamInstance.Name + "_Element";
        VMType listElementType = listParamInstance.Type.GetListElementType();
        LocalVariable localVariable = new LocalVariable();
        localVariable.Initialize(name, listElementType);
        loopLocalVariables.Add(localVariable);
      }
      string name1 = "local_" + BaseGuid + "_Loop_Index";
      LocalVariable localVariable1 = new LocalVariable();
      localVariable1.Initialize(name1, new VMType(typeof (int)));
      loopLocalVariables.Add(localVariable1);
    }

    public string GuidStr => guid.ToString();
  }
}
