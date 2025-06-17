using System.Xml;
using Cofe.Loggers;
using Engine.Common.Commons;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Data;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;
using VirtualMachine.Data;

namespace PLVirtualMachine.Objects
{
  [TypeData(EDataType.TQuest)]
  [DataFactory("Quest")]
  public class VMQuest(ulong guid) : VMBlueprint(guid), IStub, IEditorDataReader {
    [FieldData("StartEvent", DataFieldType.Reference)]
    private new IEvent startEvent;

    public override void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
    {
      while (xml.Read()) {
        if (xml.NodeType == XmlNodeType.Element)
        {
          switch (xml.Name)
          {
            case "ChildObjects":
              gameObjects = EditorDataReadUtility.ReadReferenceList(xml, creator, gameObjects);
              continue;
            case "CustomParams":
              customParamsDict = EditorDataReadUtility.ReadStringReferenceDictionary(xml, creator, customParamsDict);
              continue;
            case "EventGraph":
              stateGraph = EditorDataReadUtility.ReadReference<IFiniteStateMachine>(xml, creator);
              continue;
            case "Events":
              customEventsList = EditorDataReadUtility.ReadReferenceList(xml, creator, customEventsList);
              continue;
            case "FunctionalComponents":
              functionalComponents = EditorDataReadUtility.ReadReferenceList(xml, creator, functionalComponents);
              continue;
            case "GameTimeContext":
              gameTimeContext = EditorDataReadUtility.ReadReference<IGameMode>(xml, creator);
              continue;
            case "InheritanceInfo":
              baseBlueprints = EditorDataReadUtility.ReadReferenceList(xml, creator, baseBlueprints);
              continue;
            case "Name":
              name = EditorDataReadUtility.ReadValue(xml, name);
              continue;
            case "Parent":
              parent = EditorDataReadUtility.ReadReference<IContainer>(xml, creator);
              continue;
            case "StandartParams":
              standartParamsDict = EditorDataReadUtility.ReadStringReferenceDictionary(xml, creator, standartParamsDict);
              continue;
            case "StartEvent":
              startEvent = EditorDataReadUtility.ReadReference<IEvent>(xml, creator);
              continue;
            case "Static":
              isStatic = EditorDataReadUtility.ReadValue(xml, isStatic);
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

    public override EObjectCategory GetCategory() => EObjectCategory.OBJECT_CATEGORY_QUEST;

    public IEvent StartEvent => startEvent;

    public override bool Static => true;
  }
}
