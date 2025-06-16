using System.Xml;
using Cofe.Loggers;
using Engine.Common.Commons;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Data;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;

namespace VirtualMachine.Data.Customs
{
  [DataFactory("NameTypeData")]
  public class NameTypeData : IStub, IEditorDataReader
  {
    [FieldData("Name")]
    protected string name = "";
    [FieldData("Type")]
    protected VMType type;

    public string Name => name;

    public VMType Type => type;

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
            case "Type":
              type = EditorDataReadUtility.ReadTypeSerializable(xml);
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
  }
}
