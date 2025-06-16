using System.Xml;
using Cofe.Loggers;
using Engine.Common.Commons;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Data;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;
using VirtualMachine.Data;

namespace PLVirtualMachine.Base;

[TypeData(EDataType.TGameString)]
[DataFactory("GameString")]
public class VMGameString :
	VMBaseObject,
	IStub,
	IEditorDataReader,
	IGameString,
	IObject,
	IEditorBaseTemplate {
	public virtual void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext) {
		while (xml.Read()) {
			if (xml.NodeType == XmlNodeType.Element)
				switch (xml.Name) {
					case "Name":
						name = EditorDataReadUtility.ReadValue(xml, name);
						continue;
					case "Parent":
						parent = EditorDataReadUtility.ReadReference<IContainer>(xml, creator);
						continue;
					default:
						if (XMLDataLoader.Logs.Add(typeContext + " : " + xml.Name))
							Logger.AddError(typeContext + " : " + xml.Name);
						XmlReaderUtility.SkipNode(xml);
						continue;
				}

			if (xml.NodeType == XmlNodeType.EndElement)
				break;
		}
	}

	public VMGameString(ulong guid)
		: base(guid) { }

	public override EObjectCategory GetCategory() {
		return EObjectCategory.OBJECT_CATEGORY_NONE;
	}
}