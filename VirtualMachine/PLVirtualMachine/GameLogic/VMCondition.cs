using Cofe.Loggers;
using Engine.Common.Commons;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Data;
using System.Collections.Generic;
using System.Xml;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;
using VirtualMachine.Data;

namespace PLVirtualMachine.GameLogic
{
  [TypeData(EDataType.TCondition)]
  [DataFactory("Condition")]
  public class VMCondition : VMPartCondition, IStub, IEditorDataReader
  {
    [FieldData("Predicates", DataFieldType.Reference)]
    private List<ICondition> predicatesList = new List<ICondition>();
    [FieldData("Operation", DataFieldType.None)]
    private EConditionOperation operation = EConditionOperation.COP_NONE;
    private bool isUpdated;

    public override void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
    {
      while (xml.Read())
      {
        if (xml.NodeType == XmlNodeType.Element)
        {
          switch (xml.Name)
          {
            case "Predicates":
              this.predicatesList = EditorDataReadUtility.ReadReferenceList<ICondition>(xml, creator, this.predicatesList);
              continue;
            case "Operation":
              this.operation = EditorDataReadUtility.ReadEnum<EConditionOperation>(xml);
              continue;
            case "Name":
              this.name = EditorDataReadUtility.ReadValue(xml, this.name);
              continue;
            case "OrderIndex":
              this.orderIndex = EditorDataReadUtility.ReadValue(xml, this.orderIndex);
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

    public VMCondition(ulong guid)
      : base(guid)
    {
    }

    public override bool IsPart() => false;

    public EConditionOperation Operation => this.operation;

    public List<ICondition> Predicates => this.predicatesList;

    public override List<GameTime> GetCheckRaisingTimePoints()
    {
      List<GameTime> raisingTimePoints1 = new List<GameTime>();
      int num = this.predicatesList.Count;
      if (this.Operation == EConditionOperation.COP_ROOT)
        num = 1;
      for (int index = 0; index < num; ++index)
      {
        List<GameTime> raisingTimePoints2 = ((VMPartCondition) this.predicatesList[index]).GetCheckRaisingTimePoints();
        if (raisingTimePoints2.Count == 0 && (this.Operation == EConditionOperation.COP_OR || this.Operation == EConditionOperation.COP_XOR))
          raisingTimePoints1.Clear();
        else if (raisingTimePoints2.Count > 0)
          raisingTimePoints1.AddRange((IEnumerable<GameTime>) raisingTimePoints2);
      }
      return raisingTimePoints1;
    }

    public override bool IsConstant()
    {
      int num = this.predicatesList.Count;
      if (this.Operation == EConditionOperation.COP_ROOT)
        num = 1;
      for (int index = 0; index < num; ++index)
      {
        if (!((VMPartCondition) this.predicatesList[index]).IsConstant())
          return false;
      }
      return true;
    }

    public override List<VMParameter> GetCheckRaisingParams()
    {
      List<VMParameter> checkRaisingParams1 = new List<VMParameter>();
      int num = this.predicatesList.Count;
      if (this.Operation == EConditionOperation.COP_ROOT)
        num = 1;
      for (int index = 0; index < num; ++index)
      {
        List<VMParameter> checkRaisingParams2 = ((VMPartCondition) this.predicatesList[index]).GetCheckRaisingParams();
        checkRaisingParams1.AddRange((IEnumerable<VMParameter>) checkRaisingParams2);
      }
      return checkRaisingParams1;
    }

    public override List<BaseFunction> GetCheckRaisingFunctions()
    {
      List<BaseFunction> raisingFunctions1 = new List<BaseFunction>();
      int num = this.predicatesList.Count;
      if (this.Operation == EConditionOperation.COP_ROOT)
        num = 1;
      for (int index = 0; index < num; ++index)
      {
        List<BaseFunction> raisingFunctions2 = ((VMPartCondition) this.predicatesList[index]).GetCheckRaisingFunctions();
        raisingFunctions1.AddRange((IEnumerable<BaseFunction>) raisingFunctions2);
      }
      return raisingFunctions1;
    }

    public override void Update()
    {
      for (int index = 0; index < this.predicatesList.Count; ++index)
        this.predicatesList[index].Update();
      this.isUpdated = true;
    }

    public override bool IsUpdated => this.isUpdated;

    public override void Clear()
    {
      base.Clear();
      if (this.predicatesList == null)
        return;
      foreach (VMPartCondition predicates in this.predicatesList)
        predicates.Clear();
      this.predicatesList.Clear();
      this.predicatesList = (List<ICondition>) null;
    }
  }
}
