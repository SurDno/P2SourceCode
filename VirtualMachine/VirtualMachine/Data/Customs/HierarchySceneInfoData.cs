using System.Collections.Generic;
using System.Xml;
using Cofe.Loggers;
using Engine.Common.Commons;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Data;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;

namespace VirtualMachine.Data.Customs
{
  [DataFactory("HierarchySceneInfoData")]
  public class HierarchySceneInfoData : IStub, IEditorDataReader
  {
    [FieldData("Scenes")]
    protected List<ulong> scenes = new List<ulong>();
    [FieldData("Childs")]
    protected List<ulong> childs = new List<ulong>();
    [FieldData("SimpleChilds")]
    protected List<ulong> simpleChilds = new List<ulong>();

    public List<ulong> Scenes => scenes;

    public List<ulong> Childs => childs;

    public List<ulong> SimpleChilds => simpleChilds;

    public void Clear()
    {
      scenes.Clear();
      childs.Clear();
      simpleChilds.Clear();
    }

    public virtual void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
    {
      while (xml.Read()) {
        if (xml.NodeType == XmlNodeType.Element)
        {
          switch (xml.Name)
          {
            case "Scenes":
              scenes = EditorDataReadUtility.ReadValueList(xml, scenes);
              continue;
            case "Childs":
              childs = EditorDataReadUtility.ReadValueList(xml, childs);
              continue;
            case "SimpleChilds":
              simpleChilds = EditorDataReadUtility.ReadValueList(xml, simpleChilds);
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
