using System.Xml;
using Cofe.Serializations.Data;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.Serialization;
using PLVirtualMachine.Data.SaveLoad;

namespace PLVirtualMachine.Dynamic;

public class StateInfo : ISerializeStateSave, IDynamicLoadSerializable {
	public IState state;
	public StateStack stateStack;

	public StateInfo() { }

	public StateInfo(IState state) {
		this.state = state;
		stateStack = null;
	}

	public StateInfo(StateStack stateStack) {
		state = null;
		this.stateStack = stateStack;
	}

	public void StateSave(IDataWriter writer) {
		SaveManagerUtility.Save(writer, "State", State != null ? State.BaseGuid : 0UL);
		SaveManagerUtility.Save(writer, "StateName", State != null ? State.Name : "");
		SaveManagerUtility.SaveDynamicSerializable(writer, "StateStack", stateStack);
	}

	public void LoadFromXML(XmlElement xmlNode) {
		for (var i = 0; i < xmlNode.ChildNodes.Count; ++i) {
			var childNode = (XmlElement)xmlNode.ChildNodes[i];
			if (childNode.Name == "State") {
				var id = VMSaveLoadManager.ReadUlong(childNode);
				if (id != 0UL) {
					var objectByGuid = IStaticDataContainer.StaticDataContainer.GetObjectByGuid(id);
					if (objectByGuid != null)
						state = (IState)objectByGuid;
				}
			} else if (childNode.Name == "StateStack" && childNode.ChildNodes.Count > 0) {
				stateStack = new StateStack();
				stateStack.LoadFromXML(childNode);
			}
		}
	}

	public IState State => state;

	public StateStack Stack => stateStack;
}