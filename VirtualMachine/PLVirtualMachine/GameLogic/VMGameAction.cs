using Cofe.Loggers;
using Engine.Common.Commons;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Data;
using System.Collections.Generic;
using System.Xml;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;
using VirtualMachine.Data;

namespace PLVirtualMachine.GameLogic
{
  [TypeData(EDataType.TAction)]
  [DataFactory("Action")]
  public class VMGameAction : 
    IStub,
    IEditorDataReader,
    ISingleAction,
    IGameAction,
    IOrderedChild,
    IContextElement,
    IObject,
    IEditorBaseTemplate,
    INamed,
    IBaseAction,
    IStaticUpdateable,
    IAbstractAction,
    IFunctionalPoint
  {
    protected ulong guid;
    [FieldData("Name", DataFieldType.None)]
    private string name = "";
    [FieldData("ActionType", DataFieldType.None)]
    private EActionType actionType;
    [FieldData("MathOperationType", DataFieldType.None)]
    private EMathOperationType mathOperationType;
    [FieldData("TargetFuncName", DataFieldType.None)]
    private string targetFunctionName;
    [FieldData("LocalContext", DataFieldType.Reference)]
    private ILocalContext localContext;
    [FieldData("SourceConst", DataFieldType.Reference)]
    private VMParameter sourceConst;
    [FieldData("SourceExpression", DataFieldType.Reference)]
    private VMExpression sourceExpression;
    [FieldData("OrderIndex", DataFieldType.None)]
    private int orderIndex;
    [FieldData("TargetObject", DataFieldType.None)]
    private CommonVariable targetObject;
    [FieldData("TargetParam", DataFieldType.None)]
    private CommonVariable targetParam;
    [FieldData("SourceParams", DataFieldType.None)]
    private List<CommonVariable> sourceParams = new List<CommonVariable>();
    private AbstractActionInfo actionInfo;
    private bool isUpdated;

    public virtual void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
    {
      while (xml.Read())
      {
        if (xml.NodeType == XmlNodeType.Element)
        {
          switch (xml.Name)
          {
            case "ActionType":
              this.actionType = EditorDataReadUtility.ReadEnum<EActionType>(xml);
              continue;
            case "LocalContext":
              this.localContext = EditorDataReadUtility.ReadReference<ILocalContext>(xml, creator);
              continue;
            case "MathOperationType":
              this.mathOperationType = EditorDataReadUtility.ReadEnum<EMathOperationType>(xml);
              continue;
            case "Name":
              this.name = EditorDataReadUtility.ReadValue(xml, this.name);
              continue;
            case "OrderIndex":
              this.orderIndex = EditorDataReadUtility.ReadValue(xml, this.orderIndex);
              continue;
            case "SourceConst":
              this.sourceConst = EditorDataReadUtility.ReadReference<VMParameter>(xml, creator);
              continue;
            case "SourceExpression":
              this.sourceExpression = EditorDataReadUtility.ReadReference<VMExpression>(xml, creator);
              continue;
            case "SourceParams":
              this.sourceParams = EditorDataReadUtility.ReadSerializableList<CommonVariable>(xml, this.sourceParams);
              continue;
            case "TargetFuncName":
              this.targetFunctionName = EditorDataReadUtility.ReadValue(xml, this.targetFunctionName);
              continue;
            case "TargetObject":
              this.targetObject = EditorDataReadUtility.ReadSerializable<CommonVariable>(xml);
              continue;
            case "TargetParam":
              this.targetParam = EditorDataReadUtility.ReadSerializable<CommonVariable>(xml);
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

    public VMGameAction(ulong guid)
    {
      this.guid = guid;
      this.actionInfo = new AbstractActionInfo((IAbstractAction) this);
    }

    public ulong BaseGuid => this.guid;

    public string Name => this.name;

    public ILocalContext LocalContext => this.localContext;

    public int Order => this.orderIndex;

    public virtual List<IVariable> LocalContextVariables => new List<IVariable>();

    public virtual IVariable GetLocalContextVariable(string varUniName) => (IVariable) null;

    public bool IsValid => this.actionInfo.IsValid;

    public EActionType ActionType => this.actionType;

    public EMathOperationType MathOperationType => this.mathOperationType;

    public string TargetFunction
    {
      get => this.targetFunctionName == null ? string.Empty : this.targetFunctionName;
    }

    public string TargetEvent
    {
      get => this.targetFunctionName == null ? string.Empty : this.targetFunctionName;
    }

    public IParam SourceConstant => (IParam) this.sourceConst;

    public IExpression SourceExpression => (IExpression) this.sourceExpression;

    public void Update()
    {
      if (this.isUpdated && this.actionInfo.IsValid)
        return;
      this.actionInfo.Update();
      this.isUpdated = this.actionInfo.IsValid;
    }

    public bool IsUpdated => this.isUpdated;

    public BaseFunction TargetFunctionInstance => this.actionInfo.TargetFunctionInstance;

    public CommonVariable TargetObject => this.targetObject;

    public CommonVariable TargetParam => this.targetParam;

    public List<CommonVariable> SourceParams => this.sourceParams;

    public bool IsEqual(IObject other)
    {
      return other != null && typeof (VMGameAction) == other.GetType() && (long) this.BaseGuid == (long) ((VMGameAction) other).BaseGuid;
    }

    public string GuidStr => this.guid.ToString();

    public void Clear()
    {
      this.localContext = (ILocalContext) null;
      if (this.sourceConst != null)
      {
        this.sourceConst.Clear();
        this.sourceConst = (VMParameter) null;
      }
      if (this.sourceExpression != null)
      {
        this.sourceExpression.Clear();
        this.sourceExpression = (VMExpression) null;
      }
      if (this.targetObject != null)
      {
        this.targetObject.Clear();
        this.targetObject = (CommonVariable) null;
      }
      if (this.targetParam != null)
      {
        this.targetParam.Clear();
        this.targetParam = (CommonVariable) null;
      }
      if (this.sourceParams != null)
      {
        foreach (ContextVariable sourceParam in this.sourceParams)
          sourceParam.Clear();
        this.sourceParams.Clear();
        this.sourceParams = (List<CommonVariable>) null;
      }
      if (this.actionInfo == null)
        return;
      this.actionInfo.Clear();
      this.actionInfo = (AbstractActionInfo) null;
    }
  }
}
