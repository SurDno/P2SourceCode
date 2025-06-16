using System.Xml;
using Cofe.Loggers;
using Engine.Common.Commons;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Data;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;

namespace VirtualMachine.Data.Customs
{
  [DataFactory("ActionLoopInfoData")]
  public class ActionLoopInfoData : IStub, IEditorDataReader
  {
    [FieldData("Name")]
    public string Name = "";
    [FieldData("Start")]
    public string Start = "";
    [FieldData("End")]
    public string End = "";
    [FieldData("Random")]
    public bool Random;

    public virtual void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
    {
      while (xml.Read()) {
        if (xml.NodeType == XmlNodeType.Element)
        {
          switch (xml.Name)
          {
            case "Name":
              Name = EditorDataReadUtility.ReadValue(xml, Name);
              continue;
            case "Start":
              Start = EditorDataReadUtility.ReadValue(xml, Start);
              continue;
            case "End":
              End = EditorDataReadUtility.ReadValue(xml, End);
              continue;
            case "Random":
              Random = EditorDataReadUtility.ReadValue(xml, Random);
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
