// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Objects.VMGameMode
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Loggers;
using Engine.Common.Commons;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Data;
using System.Xml;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;
using VirtualMachine.Data;

#nullable disable
namespace PLVirtualMachine.Objects
{
  [TypeData(EDataType.TGameMode)]
  [DataFactory("GameMode")]
  public class VMGameMode : 
    VMBaseObject,
    IStub,
    IEditorDataReader,
    IGameMode,
    IObject,
    IEditorBaseTemplate
  {
    [FieldData("IsMain", DataFieldType.None)]
    private bool isMain;
    [FieldData("StartGameTime", DataFieldType.None)]
    private GameTime startGameTime;
    [FieldData("StartSolarTime", DataFieldType.None)]
    private GameTime startSolarTime;
    [FieldData("GameTimeSpeed", DataFieldType.None)]
    private float gameTimeSpeed;
    [FieldData("SolarTimeSpeed", DataFieldType.None)]
    private float solarTimeSpeed;
    [FieldData("PlayerRef", DataFieldType.None)]
    private CommonVariable playerRef;

    public virtual void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
    {
      while (xml.Read())
      {
        if (xml.NodeType == XmlNodeType.Element)
        {
          switch (xml.Name)
          {
            case "GameTimeSpeed":
              this.gameTimeSpeed = EditorDataReadUtility.ReadValue(xml, this.gameTimeSpeed);
              continue;
            case "IsMain":
              this.isMain = EditorDataReadUtility.ReadValue(xml, this.isMain);
              continue;
            case "Name":
              this.name = EditorDataReadUtility.ReadValue(xml, this.name);
              continue;
            case "Parent":
              this.parent = EditorDataReadUtility.ReadReference<IContainer>(xml, creator);
              continue;
            case "PlayerRef":
              this.playerRef = EditorDataReadUtility.ReadSerializable<CommonVariable>(xml);
              continue;
            case "SolarTimeSpeed":
              this.solarTimeSpeed = EditorDataReadUtility.ReadValue(xml, this.solarTimeSpeed);
              continue;
            case "StartGameTime":
              this.startGameTime = EditorDataReadUtility.ReadSerializable<GameTime>(xml);
              continue;
            case "StartSolarTime":
              this.startSolarTime = EditorDataReadUtility.ReadSerializable<GameTime>(xml);
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

    public VMGameMode(ulong guid)
      : base(guid)
    {
    }

    public override EObjectCategory GetCategory() => EObjectCategory.OBJECT_CATEGORY_GAME_MODE;

    public bool IsMain => this.isMain;

    public GameTime StartGameTime => this.startGameTime;

    public float GameTimeSpeed => this.gameTimeSpeed;

    public GameTime StartSolarTime => this.startSolarTime;

    public float SolarTimeSpeed => this.solarTimeSpeed;

    public CommonVariable PlayCharacterVariable => this.playerRef;
  }
}
