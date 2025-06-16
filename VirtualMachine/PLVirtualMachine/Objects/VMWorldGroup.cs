// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Objects.VMWorldGroup
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Loggers;
using Engine.Common.Commons;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Data;
using System.Xml;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;
using VirtualMachine.Data;

#nullable disable
namespace PLVirtualMachine.Objects
{
  [TypeData(EDataType.TWorldGroup)]
  [DataFactory("Scene")]
  public class VMWorldGroup : VMWorldObject, IStub, IEditorDataReader
  {
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
            case "EngineBaseTemplateID":
              this.engineBaseTemplateID = EditorDataReadUtility.ReadValue(xml, this.engineBaseTemplateID);
              continue;
            case "EngineTemplateID":
              this.engineTemplateID = EditorDataReadUtility.ReadValue(xml, this.engineTemplateID);
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
            case "Instantiated":
              this.isInstantiated = EditorDataReadUtility.ReadValue(xml, this.isInstantiated);
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
            case "Static":
              this.isStatic = EditorDataReadUtility.ReadValue(xml, this.isStatic);
              continue;
            case "WorldPositionGuid":
              this.worldPositionGuid = EditorDataReadUtility.ReadValue(xml, this.worldPositionGuid);
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

    public VMWorldGroup(ulong guid)
      : base(guid)
    {
    }

    public override EObjectCategory GetCategory() => EObjectCategory.OBJECT_CATEGORY_WORLD_GROUP;
  }
}
