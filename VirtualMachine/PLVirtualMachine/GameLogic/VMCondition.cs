using System.Collections.Generic;
using System.Xml;
using Cofe.Loggers;
using Engine.Common.Commons;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Data;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;
using VirtualMachine.Data;

namespace PLVirtualMachine.GameLogic
{
  [TypeData(EDataType.TCondition)]
  [DataFactory("Condition")]
  public class VMCondition(ulong guid) : VMPartCondition(guid), IStub, IEditorDataReader {
    [FieldData("Predicates", DataFieldType.Reference)]
    private List<ICondition> predicatesList = [];
    [FieldData("Operation")]
    private EConditionOperation operation = EConditionOperation.COP_NONE;
    private bool isUpdated;

    public override void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
    {
      while (xml.Read()) {
        if (xml.NodeType == XmlNodeType.Element)
        {
          switch (xml.Name)
          {
            case "Predicates":
              predicatesList = EditorDataReadUtility.ReadReferenceList(xml, creator, predicatesList);
              continue;
            case "Operation":
              operation = EditorDataReadUtility.ReadEnum<EConditionOperation>(xml);
              continue;
            case "Name":
              name = EditorDataReadUtility.ReadValue(xml, name);
              continue;
            case "OrderIndex":
              orderIndex = EditorDataReadUtility.ReadValue(xml, orderIndex);
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

    public override bool IsPart() => false;

    public EConditionOperation Operation => operation;

    public List<ICondition> Predicates => predicatesList;

    public override List<GameTime> GetCheckRaisingTimePoints()
    {
      List<GameTime> raisingTimePoints1 = [];
      int num = predicatesList.Count;
      if (Operation == EConditionOperation.COP_ROOT)
        num = 1;
      for (int index = 0; index < num; ++index)
      {
        List<GameTime> raisingTimePoints2 = ((VMPartCondition) predicatesList[index]).GetCheckRaisingTimePoints();
        if (raisingTimePoints2.Count == 0 && (Operation == EConditionOperation.COP_OR || Operation == EConditionOperation.COP_XOR))
          raisingTimePoints1.Clear();
        else if (raisingTimePoints2.Count > 0)
          raisingTimePoints1.AddRange(raisingTimePoints2);
      }
      return raisingTimePoints1;
    }

    public override bool IsConstant()
    {
      int num = predicatesList.Count;
      if (Operation == EConditionOperation.COP_ROOT)
        num = 1;
      for (int index = 0; index < num; ++index)
      {
        if (!((VMPartCondition) predicatesList[index]).IsConstant())
          return false;
      }
      return true;
    }

    public override List<VMParameter> GetCheckRaisingParams()
    {
      List<VMParameter> checkRaisingParams1 = [];
      int num = predicatesList.Count;
      if (Operation == EConditionOperation.COP_ROOT)
        num = 1;
      for (int index = 0; index < num; ++index)
      {
        List<VMParameter> checkRaisingParams2 = ((VMPartCondition) predicatesList[index]).GetCheckRaisingParams();
        checkRaisingParams1.AddRange(checkRaisingParams2);
      }
      return checkRaisingParams1;
    }

    public override List<BaseFunction> GetCheckRaisingFunctions()
    {
      List<BaseFunction> raisingFunctions1 = [];
      int num = predicatesList.Count;
      if (Operation == EConditionOperation.COP_ROOT)
        num = 1;
      for (int index = 0; index < num; ++index)
      {
        List<BaseFunction> raisingFunctions2 = ((VMPartCondition) predicatesList[index]).GetCheckRaisingFunctions();
        raisingFunctions1.AddRange(raisingFunctions2);
      }
      return raisingFunctions1;
    }

    public override void Update()
    {
      for (int index = 0; index < predicatesList.Count; ++index)
        predicatesList[index].Update();
      isUpdated = true;
    }

    public override bool IsUpdated => isUpdated;

    public override void Clear()
    {
      base.Clear();
      if (predicatesList == null)
        return;
      foreach (VMPartCondition predicates in predicatesList)
        predicates.Clear();
      predicatesList.Clear();
      predicatesList = null;
    }
  }
}
