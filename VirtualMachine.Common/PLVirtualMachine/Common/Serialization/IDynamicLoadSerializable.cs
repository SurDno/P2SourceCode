using System.Xml;

namespace PLVirtualMachine.Common.Serialization
{
  public interface IDynamicLoadSerializable
  {
    void LoadFromXML(XmlElement xmlNode);
  }
}
