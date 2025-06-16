using Cofe.Loggers;
using Engine.Common.Commons;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Data;
using System.Xml;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;

namespace VirtualMachine.Data.Customs
{
  [DataFactory("ActionLoopInfoData")]
  public class ActionLoopInfoData : IStub, IEditorDataReader
  {
    [FieldData("Name", DataFieldType.None)]
    public string Name = "";
    [FieldData("Start", DataFieldType.None)]
    public string Start = "";
    [FieldData("End", DataFieldType.None)]
    public string End = "";
    [FieldData("Random", DataFieldType.None)]
    public bool Random;

    public virtual void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
    {
      while (xml.Read())
      {
        if (xml.NodeType == XmlNodeType.Element)
        {
          switch (xml.Name)
          {
            case "Name":
              this.Name = EditorDataReadUtility.ReadValue(xml, this.Name);
              continue;
            case "Start":
              this.Start = EditorDataReadUtility.ReadValue(xml, this.Start);
              continue;
            case "End":
              this.End = EditorDataReadUtility.ReadValue(xml, this.End);
              continue;
            case "Random":
              this.Random = EditorDataReadUtility.ReadValue(xml, this.Random);
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
