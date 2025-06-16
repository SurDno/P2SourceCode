using Cofe.Loggers;
using Engine.Common.Commons;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Data;
using System.Xml;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;
using VirtualMachine.Data;

namespace PLVirtualMachine.Objects
{
  [TypeData(EDataType.TQuest)]
  [DataFactory("Quest")]
  public class VMQuest : VMBlueprint, IStub, IEditorDataReader
  {
    [FieldData("StartEvent", DataFieldType.Reference)]
    private new IEvent startEvent;

    public override void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
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
            case "StartEvent":
              this.startEvent = EditorDataReadUtility.ReadReference<IEvent>(xml, creator);
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

    public VMQuest(ulong guid)
      : base(guid)
    {
    }

    public override EObjectCategory GetCategory() => EObjectCategory.OBJECT_CATEGORY_QUEST;

    public IEvent StartEvent => this.startEvent;

    public override bool Static => true;
  }
}
