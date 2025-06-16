using System.Xml;

namespace VirtualMachine.Data;

public interface IEditorDataReader {
	void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext);
}