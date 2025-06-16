using System.Xml;
using Cofe.Loggers;
using Cofe.Serializations.Data;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.Serialization;
using PLVirtualMachine.Data.SaveLoad;

namespace PLVirtualMachine.Dynamic;

public class StatePoint : ISerializeStateSave, IDynamicLoadSerializable {
	public IState currentState;
	public StateInfo prevState;

	public StatePoint() { }

	public StatePoint(IState current, IState prev) {
		currentState = current;
		prevState = new StateInfo(prev);
	}

	public StatePoint(IState current, StateStack prev) {
		currentState = current;
		prevState = new StateInfo(prev);
	}

	public StatePoint(IState current) {
		currentState = current;
		prevState = null;
	}

	public void StateSave(IDataWriter writer) {
		SaveManagerUtility.Save(writer, "CurrentState", currentState != null ? currentState.BaseGuid : 0UL);
		SaveManagerUtility.Save(writer, "CurrentStateName", currentState != null ? currentState.Name : "");
		SaveManagerUtility.SaveDynamicSerializable(writer, "PrevStateInfo", prevState);
	}

	public void LoadFromXML(XmlElement xmlNode) {
		for (var i = 0; i < xmlNode.ChildNodes.Count; ++i) {
			var childNode = (XmlElement)xmlNode.ChildNodes[i];
			if (childNode.Name == "CurrentState") {
				var id = VMSaveLoadManager.ReadUlong(childNode);
				if (id != 0UL) {
					var objectByGuid = IStaticDataContainer.StaticDataContainer.GetObjectByGuid(id);
					if (objectByGuid != null)
						currentState = (IState)objectByGuid;
					else
						Logger.AddError(string.Format("Saveload error: loading state id={0} not found", id));
				}
			} else if (childNode.Name == "PrevStateInfo" && childNode.ChildNodes.Count > 0) {
				prevState = new StateInfo();
				VMSaveLoadManager.LoadDynamicSerializable(childNode, prevState);
			}
		}
	}

	public bool Modified => true;

	public void OnModify() { }
}