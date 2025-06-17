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

namespace PLVirtualMachine.Objects
{
  [TypeData(EDataType.TGameMode)]
  [DataFactory("GameMode")]
  public class VMGameMode(ulong guid) :
    VMBaseObject(guid),
    IStub,
    IEditorDataReader,
    IGameMode,
    IObject,
    IEditorBaseTemplate {
    [FieldData("IsMain")]
    private bool isMain;
    [FieldData("StartGameTime")]
    private GameTime startGameTime;
    [FieldData("StartSolarTime")]
    private GameTime startSolarTime;
    [FieldData("GameTimeSpeed")]
    private float gameTimeSpeed;
    [FieldData("SolarTimeSpeed")]
    private float solarTimeSpeed;
    [FieldData("PlayerRef")]
    private CommonVariable playerRef;

    public virtual void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
    {
      while (xml.Read()) {
        if (xml.NodeType == XmlNodeType.Element)
        {
          switch (xml.Name)
          {
            case "GameTimeSpeed":
              gameTimeSpeed = EditorDataReadUtility.ReadValue(xml, gameTimeSpeed);
              continue;
            case "IsMain":
              isMain = EditorDataReadUtility.ReadValue(xml, isMain);
              continue;
            case "Name":
              name = EditorDataReadUtility.ReadValue(xml, name);
              continue;
            case "Parent":
              parent = EditorDataReadUtility.ReadReference<IContainer>(xml, creator);
              continue;
            case "PlayerRef":
              playerRef = EditorDataReadUtility.ReadSerializable<CommonVariable>(xml);
              continue;
            case "SolarTimeSpeed":
              solarTimeSpeed = EditorDataReadUtility.ReadValue(xml, solarTimeSpeed);
              continue;
            case "StartGameTime":
              startGameTime = EditorDataReadUtility.ReadSerializable<GameTime>(xml);
              continue;
            case "StartSolarTime":
              startSolarTime = EditorDataReadUtility.ReadSerializable<GameTime>(xml);
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

    public override EObjectCategory GetCategory() => EObjectCategory.OBJECT_CATEGORY_GAME_MODE;

    public bool IsMain => isMain;

    public GameTime StartGameTime => startGameTime;

    public float GameTimeSpeed => gameTimeSpeed;

    public GameTime StartSolarTime => startSolarTime;

    public float SolarTimeSpeed => solarTimeSpeed;

    public CommonVariable PlayCharacterVariable => playerRef;
  }
}
