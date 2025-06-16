using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Cofe.Serializations.Data;
using Engine.Common.MindMap;
using Engine.Common.Services;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.Serialization;
using PLVirtualMachine.Data.SaveLoad;
using PLVirtualMachine.LogicMap;

namespace PLVirtualMachine.Dynamic;

public class DynamicMindMapNode :
	IDynamicMindMapObject,
	ISerializeStateSave,
	IDynamicLoadSerializable {
	private DynamicMindMap parentMindMap;
	private VMLogicMapNode staticNode;
	private Guid dynamicGuid;
	private List<DynamicMindMapNodeContent> nodeContentList = new();
	private DynamicMindMapNodeContent activeContent;
	private bool undiscovered;

	public DynamicMindMapNode(DynamicMindMap parentMindMap, VMLogicMapNode staticNode) {
		this.parentMindMap = parentMindMap;
		this.staticNode = staticNode;
		dynamicGuid = DynamicMindMap.RegistrDynamicMMObject(this);
		var linksByDestNode = parentMindMap.GetLinksByDestNode(this);
		linksByDestNode.AddRange(linksByDestNode);
		var linksBySourceNode = parentMindMap.GetLinksBySourceNode(this);
		linksBySourceNode.AddRange(linksBySourceNode);
		LoadContent();
		activeContent = null;
	}

	public ulong StaticGuid => staticNode == null ? 0UL : staticNode.BaseGuid;

	public Guid DynamicGuid => dynamicGuid;

	public VMLogicMapNode StaticNode => staticNode;

	public IMMNode EngineNode { get; set; }

	public void Think() {
		DynamicFSM.SetCurrentDebugState(StaticNode);
		for (var index = 0; index < nodeContentList.Count; ++index)
			nodeContentList[index].Think();
		DynamicFSM.SetCurrentDebugState(null);
	}

	public void SetContentActive(DynamicMindMapNodeContent content, bool bActive) {
		content.Active = bActive;
		MakeCurrentActiveContent();
	}

	public IGameMode GameTimeContext => parentMindMap.GameTimeContext;

	public void Free() {
		for (var index = 0; index < nodeContentList.Count; ++index)
			nodeContentList[index].Free();
	}

	public void StateSave(IDataWriter writer) {
		SaveManagerUtility.Save(writer, "StaticGuid", staticNode.BaseGuid);
		SaveManagerUtility.Save(writer, "DynamicGuid", dynamicGuid);
		SaveManagerUtility.Save(writer, "Undiscovered", EngineNode.Undiscovered);
		SaveManagerUtility.SaveDynamicSerializableList(writer, "MMNodeContentList", nodeContentList);
	}

	public void LoadFromXML(XmlElement xmlNode) {
		for (var i = 0; i < xmlNode.ChildNodes.Count; ++i) {
			var childNode1 = (XmlElement)xmlNode.ChildNodes[i];
			if (childNode1.Name == "DynamicGuid") {
				dynamicGuid = VMSaveLoadManager.ReadGuid(childNode1);
				EngineNode.Id = dynamicGuid;
			} else if (childNode1.Name == "Undiscovered")
				undiscovered = VMSaveLoadManager.ReadBool(childNode1);
			else if (childNode1.Name == "MMNodeContentList")
				foreach (XmlElement childNode2 in childNode1.ChildNodes) {
					var num = VMSaveLoadManager.ReadUlong(childNode2.FirstChild);
					if (num != 0UL && staticNode.GetContentByGuid(num) != null)
						((DynamicMindMapNodeContent)DynamicMindMap.GetDynamicMMObjectByStaticguid(num))?.LoadFromXML(
							childNode2);
				}
		}
	}

	public void AfterSaveLoading() {
		for (var index = 0; index < nodeContentList.Count; ++index)
			nodeContentList[index].AfterSaveLoading();
		MakeCurrentActiveContent();
		EngineNode.Undiscovered = undiscovered;
	}

	public string Name => string.Format("{0}.{1}", parentMindMap.Name, staticNode.Name);

	public bool Modified => true;

	public void OnModify() { }

	private void MakeCurrentActiveContent() {
		activeContent = null;
		for (var index = 0; index < nodeContentList.Count; ++index)
			if (nodeContentList[index].Active)
				activeContent = nodeContentList[index];
		var mindMapService = ServiceCache.MindMapService;
		if (activeContent != null)
			EngineNode.Content = mindMapService.Contents.FirstOrDefault(o => o.Id == activeContent.DynamicGuid);
		else
			EngineNode.Content = null;
	}

	private void LoadContent() {
		if (staticNode == null)
			return;
		for (var index = 0; index < staticNode.Contents.Count; ++index)
			nodeContentList.Add(new DynamicMindMapNodeContent(this, staticNode.Contents[index]));
	}
}