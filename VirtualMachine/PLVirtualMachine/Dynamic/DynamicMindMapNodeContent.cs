using System;
using System.Xml;
using Cofe.Loggers;
using Cofe.Serializations.Data;
using Engine.Common.MindMap;
using Engine.Common.Services;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.Serialization;
using PLVirtualMachine.Data.SaveLoad;
using PLVirtualMachine.GameLogic;
using PLVirtualMachine.LogicMap;

namespace PLVirtualMachine.Dynamic;

public class DynamicMindMapNodeContent :
	IDynamicMindMapObject,
	ISerializeStateSave,
	IDynamicLoadSerializable,
	INamed {
	private VMLogicMapNodeContent staticNodeContent;
	private DynamicMindMapNode parentNode;
	private IMMContent engineContent;
	private Guid dynamicGuid;
	private bool active;
	private DynamicEventBody nodeContentActivateEventBody;
	private bool firstThink = true;

	public DynamicMindMapNodeContent(
		DynamicMindMapNode parentNode,
		VMLogicMapNodeContent staticNodeContent) {
		this.parentNode = parentNode;
		this.staticNodeContent = staticNodeContent;
		dynamicGuid = DynamicMindMap.RegistrDynamicMMObject(this);
		active = false;
		if (staticNodeContent == null)
			return;
		var byNodeContentType = DynamicMindMap.GetEngineNodeContentKindByNodeContentType(staticNodeContent.ContentType);
		IMMPlaceholder mmPlaceholder = null;
		if (staticNodeContent.Picture != null) {
			var engineTemplateGuid = staticNodeContent.Picture.EngineTemplateGuid;
			mmPlaceholder = ServiceCache.TemplateService.GetTemplate<IMMPlaceholder>(engineTemplateGuid);
			if (mmPlaceholder == null)
				Logger.AddError(string.Format("Mindmap placeholder with guid={0} not found", engineTemplateGuid));
		}

		engineContent = ServiceCache.Factory.Create<IMMContent>(dynamicGuid);
		engineContent.Kind = byNodeContentType;
		engineContent.Placeholder = mmPlaceholder;
		engineContent.Description = EngineAPIManager.CreateEngineTextInstance(staticNodeContent.DescriptionText);
		ServiceCache.MindMapService.AddContent(engineContent);
		nodeContentActivateEventBody = new DynamicEventBody((VMPartCondition)staticNodeContent.ContentCondition, this,
			parentNode.GameTimeContext, OnCheckRise, false);
	}

	public string Name {
		get {
			var str = "none";
			if (staticNodeContent != null)
				str = staticNodeContent.ContentNumber.ToString();
			return string.Format("Node {0} content index {1}", parentNode.Name, str);
		}
	}

	public ulong StaticGuid => staticNodeContent == null ? 0UL : staticNodeContent.BaseGuid;

	public Guid DynamicGuid => dynamicGuid;

	public bool Active {
		get => active;
		set => active = value;
	}

	public void OnCheckRise(object newConditionValue, EEventRaisingMode raisingMode) {
		var active = Active;
		var bActive = (bool)newConditionValue;
		if (bActive == active)
			return;
		parentNode.SetContentActive(this, bActive);
	}

	public void Think() {
		if (firstThink) {
			OnCheckRise(nodeContentActivateEventBody.CalculateConditionResult(), EEventRaisingMode.ERM_ADD_TO_QUEUE);
			firstThink = false;
		} else {
			if (!NeedUpdate())
				return;
			nodeContentActivateEventBody.Think();
		}
	}

	public void Free() {
		nodeContentActivateEventBody.ClearSubscribtions();
		ServiceCache.MindMapService.RemoveContent(engineContent);
	}

	public void StateSave(IDataWriter writer) {
		SaveManagerUtility.Save(writer, "StaticGuid", staticNodeContent != null ? staticNodeContent.BaseGuid : 0UL);
		SaveManagerUtility.Save(writer, "DynamicGuid", dynamicGuid);
	}

	public void LoadFromXML(XmlElement xmlNode) {
		for (var i = 0; i < xmlNode.ChildNodes.Count; ++i) {
			var childNode = (XmlElement)xmlNode.ChildNodes[i];
			if (childNode.Name == "DynamicGuid") {
				dynamicGuid = VMSaveLoadManager.ReadGuid(childNode);
				engineContent.Id = dynamicGuid;
			}
		}

		if (staticNodeContent != null)
			return;
		Logger.AddError(string.Format(
			"SaveLoad error: dynamic map node content id={0} hasn't his static object, probably it was removed from GameData",
			dynamicGuid));
	}

	public void AfterSaveLoading() {
		if (nodeContentActivateEventBody != null)
			nodeContentActivateEventBody.AfterSaveLoading();
		Active = nodeContentActivateEventBody.CalculateConditionResult();
		firstThink = true;
	}

	public bool Modified => true;

	public void OnModify() { }

	public bool NeedUpdate() {
		return nodeContentActivateEventBody != null && nodeContentActivateEventBody.NeedUpdate();
	}
}