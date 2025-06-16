using System.Linq;
using System.Xml;
using Cofe.Loggers;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Data.SaveLoad;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine.Dynamic.Components;

[FactoryProxy(typeof(VMSpeaking))]
public class Speaking : VMSpeaking, IInitialiseComponentFromHierarchy, IInitialiseEvents {
	public override string GetComponentTypeName() {
		return nameof(Speaking);
	}

	public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject) { }

	public void InitialiseEvent(DynamicEvent target) {
		switch (target.Name) {
			case "BeginTalkingEvent":
				BeginTalkingEvent += () => target.RaiseFromEngineImpl();
				break;
			case "EndTalkingEvent":
				EndTalkingEvent += p1 => target.RaiseFromEngineImpl(p1);
				break;
			case "OnSpeechReplyEvent":
				OnSpeechReplyEvent += p1 => target.RaiseFromEngineImpl(p1);
				break;
		}
	}

	public override void StateSave(IDataWriter writer) {
		base.StateSave(writer);
		SaveManagerUtility.Save(writer, "ContextTalkingAvailable", contextTalkingAvailable);
		SaveManagerUtility.SaveSerializable(writer, "CurrentTalking", currentTalking);
		SaveManagerUtility.SaveList(writer, "PassedOnlyOnceTalkigList", passedOnlyOnceTalkigs.Select(o => o.BaseGuid));
	}

	public override void LoadFromXML(XmlElement xmlNode) {
		base.LoadFromXML(xmlNode);
		for (var i = 0; i < xmlNode.ChildNodes.Count; ++i) {
			var childNode1 = (XmlElement)xmlNode.ChildNodes[i];
			if (childNode1.Name == "ContextTalkingAvailable")
				contextTalkingAvailable = VMSaveLoadManager.ReadBool(childNode1);
			else if (childNode1.Name == "CurrentTalking")
				currentTalking = VMSaveLoadManager.ReadValue<IStateRef>(childNode1);
			else if (childNode1.Name == "PassedOnlyOnceTalkigList") {
				passedOnlyOnceTalkigs.Clear();
				foreach (XmlNode childNode2 in childNode1.ChildNodes) {
					var stateId = VMSaveLoadManager.ReadUlong(childNode2);
					if (Parent.EditorTemplate == null) {
						Logger.AddError(
							string.Format("SaveLoad error: editor template not defined at {0}", Parent.Name));
						return;
					}

					if (Parent.EditorTemplate.StateGraph == null) {
						Logger.AddError(string.Format("SaveLoad error: state graph not defined at talking object {0}",
							Parent.Name));
						return;
					}

					var stateByGuid = Parent.EditorTemplate.StateGraph.GetStateByGuid(stateId);
					if (stateByGuid != null)
						passedOnlyOnceTalkigs.Add((IFiniteStateMachine)stateByGuid);
				}
			}
		}
	}
}