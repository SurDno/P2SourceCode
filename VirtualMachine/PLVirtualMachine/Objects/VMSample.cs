using System;
using System.Xml;
using Cofe.Loggers;
using Engine.Common.Commons;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Data;
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
    [FieldData("SampleType")]
    private string sampleTypeStr = "";
    [FieldData("EngineID")]
    private Guid engineID;

    public virtual void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
    {
      while (xml.Read()) {
        if (xml.NodeType == XmlNodeType.Element)
        {
          switch (xml.Name)
          {
            case "SampleType":
              sampleTypeStr = EditorDataReadUtility.ReadValue(xml, sampleTypeStr);
              continue;
            case "EngineID":
              engineID = EditorDataReadUtility.ReadValue(xml, engineID);
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

    public VMSample(ulong guid) => this.guid = guid;

    public ulong BaseGuid => guid;

    public Guid EngineTemplateGuid => engineID;

    public virtual bool IsEqual(IObject other)
    {
      return other != null && typeof (VMSample) == other.GetType() && (long) BaseGuid == (long) ((VMSample) other).BaseGuid;
    }

    public string SampleType => sampleTypeStr;

    public string GuidStr => guid.ToString();
  }
}
