using System.Xml;
using Cofe.Loggers;
using Engine.Common.Commons;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Data;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;
using VirtualMachine.Data;

namespace PLVirtualMachine.LogicMap;

[TypeData(EDataType.TLogicMapLink)]
[DataFactory("MindMapLink")]
public class VMLogicLink :
	VMBaseObject,
	IStub,
	IEditorDataReader,
	ILink,
	IContainer,
	IObject,
	IEditorBaseTemplate,
	INamedElement,
	INamed,
	IStaticUpdateable {
	[FieldData("Source", DataFieldType.Reference)]
	private VMLogicMapNode source;

	[FieldData("Destination", DataFieldType.Reference)]
	private VMLogicMapNode destination;

	public virtual void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext) {
		while (xml.Read()) {
			if (xml.NodeType == XmlNodeType.Element)
				switch (xml.Name) {
					case "Source":
						source = EditorDataReadUtility.ReadReference<VMLogicMapNode>(xml, creator);
						continue;
					case "Destination":
						destination = EditorDataReadUtility.ReadReference<VMLogicMapNode>(xml, creator);
						continue;
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

	public VMLogicLink(ulong guid)
		: base(guid) { }

	public override EObjectCategory GetCategory() {
		return EObjectCategory.OBJECT_CATEGORY_GRAPH_ELEMENT;
	}

	public IGraphObject Source => source;

	public IGraphObject Destination => destination;

	public override void Clear() {
		base.Clear();
		source = null;
		destination = null;
	}
}