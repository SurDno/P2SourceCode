using Cofe.Loggers;
using Engine.Common.Commons;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Data;
using System.Xml;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;

namespace VirtualMachine.Data.Customs
{
  [DataFactory("NameTypeData")]
  public class NameTypeData : IStub, IEditorDataReader
  {
    [FieldData("Name", DataFieldType.None)]
    protected string name = "";
    [FieldData("Type", DataFieldType.None)]
    protected VMType type;

    public string Name => this.name;

    public VMType Type => this.type;

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
            case "Type":
              this.type = EditorDataReadUtility.ReadTypeSerializable(xml);
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
  }
}
