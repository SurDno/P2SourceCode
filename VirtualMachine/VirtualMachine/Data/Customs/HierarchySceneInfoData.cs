// Decompiled with JetBrains decompiler
// Type: VirtualMachine.Data.Customs.HierarchySceneInfoData
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Loggers;
using Engine.Common.Commons;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Data;
using System.Collections.Generic;
using System.Xml;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;

#nullable disable
namespace VirtualMachine.Data.Customs
{
  [DataFactory("HierarchySceneInfoData")]
  public class HierarchySceneInfoData : IStub, IEditorDataReader
  {
    [FieldData("Scenes", DataFieldType.None)]
    protected List<ulong> scenes = new List<ulong>();
    [FieldData("Childs", DataFieldType.None)]
    protected List<ulong> childs = new List<ulong>();
    [FieldData("SimpleChilds", DataFieldType.None)]
    protected List<ulong> simpleChilds = new List<ulong>();

    public List<ulong> Scenes => this.scenes;

    public List<ulong> Childs => this.childs;

    public List<ulong> SimpleChilds => this.simpleChilds;

    public void Clear()
    {
      this.scenes.Clear();
      this.childs.Clear();
      this.simpleChilds.Clear();
    }

    public virtual void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
    {
      while (xml.Read())
      {
        if (xml.NodeType == XmlNodeType.Element)
        {
          switch (xml.Name)
          {
            case "Scenes":
              this.scenes = EditorDataReadUtility.ReadValueList(xml, this.scenes);
              continue;
            case "Childs":
              this.childs = EditorDataReadUtility.ReadValueList(xml, this.childs);
              continue;
            case "SimpleChilds":
              this.simpleChilds = EditorDataReadUtility.ReadValueList(xml, this.simpleChilds);
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
