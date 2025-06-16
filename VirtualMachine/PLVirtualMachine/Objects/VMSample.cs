using Cofe.Loggers;
using Engine.Common.Commons;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Data;
using System;
using System.Xml;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;
using VirtualMachine.Data;

namespace PLVirtualMachine.Objects
{
  [TypeData(EDataType.TSample)]
  [DataFactory("Sample")]
  public class VMSample : 
    IStub,
    IEditorDataReader,
    IObject,
    IEditorBaseTemplate,
    ISample,
    IEngineTemplated
  {
    private ulong guid;
    [FieldData("SampleType", DataFieldType.None)]
    private string sampleTypeStr = "";
    [FieldData("EngineID", DataFieldType.None)]
    private Guid engineID;

    public virtual void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
    {
      while (xml.Read())
      {
        if (xml.NodeType == XmlNodeType.Element)
        {
          switch (xml.Name)
          {
            case "SampleType":
              this.sampleTypeStr = EditorDataReadUtility.ReadValue(xml, this.sampleTypeStr);
              continue;
            case "EngineID":
              this.engineID = EditorDataReadUtility.ReadValue(xml, this.engineID);
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

    public VMSample(ulong guid) => this.guid = guid;

    public ulong BaseGuid => this.guid;

    public Guid EngineTemplateGuid => this.engineID;

    public virtual bool IsEqual(IObject other)
    {
      return other != null && typeof (VMSample) == other.GetType() && (long) this.BaseGuid == (long) ((VMSample) other).BaseGuid;
    }

    public string SampleType => this.sampleTypeStr;

    public string GuidStr => this.guid.ToString();
  }
}
