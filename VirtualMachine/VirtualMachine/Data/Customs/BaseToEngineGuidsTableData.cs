using System;
using System.Collections.Generic;
using System.Xml;
using Cofe.Serializations.Converters;
using Engine.Common.Comparers;
using VirtualMachine.Common.Data;

namespace VirtualMachine.Data.Customs;

public class BaseToEngineGuidsTableData : IEditorDataReader {
	private Dictionary<ulong, Guid> idToGuid;
	private Dictionary<Guid, ulong> guidToId;

	public void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext) {
		var capacity = 0;
		if (!xml.IsEmptyElement) {
			xml.MoveToAttribute("count");
			capacity = DefaultConverter.ParseInt(xml.Value);
		}

		idToGuid = new Dictionary<ulong, Guid>(capacity, UlongComparer.Instance);
		guidToId = new Dictionary<Guid, ulong>(capacity, GuidComparer.Instance);
		if (xml.IsEmptyElement)
			return;
		while (xml.Read())
			if (xml.NodeType == XmlNodeType.Element) {
				xml.MoveToAttribute("key");
				var key = DefaultConverter.ParseUlong(xml.Value);
				var guid = DefaultConverter.ParseGuid(XmlReaderUtility.ReadContent(xml));
				idToGuid.Add(key, guid);
				guidToId.Add(guid, key);
			} else if (xml.NodeType == XmlNodeType.EndElement)
				break;
	}

	public ulong GetBaseGuidByEngineTemplateGuid(Guid guid) {
		ulong engineTemplateGuid = 0;
		guidToId.TryGetValue(guid, out engineTemplateGuid);
		return engineTemplateGuid;
	}

	public Guid GetEngineTemplateGuidByBaseGuid(ulong id) {
		Guid templateGuidByBaseGuid;
		idToGuid.TryGetValue(id, out templateGuidByBaseGuid);
		return templateGuidByBaseGuid;
	}
}