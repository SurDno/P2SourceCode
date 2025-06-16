using System.Collections.Generic;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Common.VMSpecialAttributes;
using PLVirtualMachine.Dynamic;
using PLVirtualMachine.FSM;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine;

public static class DynamicFsmUtility {
	public static VMEventLink GetStateOutputLinkByEvent(
		VMState state,
		DynamicFSM raisingFSM,
		DynamicEvent dynEvent,
		bool isOwnEvent) {
		if (dynEvent.FunctionalName ==
		    EngineAPIManager.GetSpecialEventName(ESpecialEventName.SEN_LOAD_GAME, typeof(VMGameComponent), true))
			return state.GetAfterExitLink();
		var eventLinkList = state.GetOutputLinksByEventGuid(dynEvent.BaseGuid) ??
		                    state.GetOutputLinksByEvent(dynEvent.StaticEvent);
		if (eventLinkList != null)
			for (var index = 0; index < eventLinkList.Count; ++index) {
				var eventOwner = eventLinkList[index].Event.EventOwner;
				if (eventOwner.IsNull & isOwnEvent)
					return (VMEventLink)eventLinkList[index];
				if (!eventOwner.IsBinded)
					eventOwner.Bind((IContext)state.Owner, new VMType(typeof(IObjRef)), state);
				if (eventOwner.IsBinded &&
				    ((VMVariableService)IVariableService.Instance)
				    .GetDynamicContext(eventOwner.VariableContext, raisingFSM).Entity.EngineGuid ==
				    dynEvent.Entity.EngineGuid)
					return (VMEventLink)eventLinkList[index];
			}

		return null;
	}

	public static VMEventLink GetGraphEnterLinkByEvent(
		IFiniteStateMachine graph,
		DynamicFSM raisingFSM,
		DynamicEvent dynEvent,
		List<EventMessage> messages,
		bool isOwnEvent) {
		var enterLinksByEvent = ((FiniteStateMachine)graph).GetEnterLinksByEvent(dynEvent);
		if (enterLinksByEvent != null)
			for (var index = 0; index < enterLinksByEvent.Count; ++index) {
				var eventOwner = enterLinksByEvent[index].Event.EventOwner;
				if (eventOwner.IsNull & isOwnEvent)
					return (VMEventLink)enterLinksByEvent[index];
				if (!eventOwner.IsBinded)
					eventOwner.Bind((IContext)graph.Owner, new VMType(typeof(IObjRef)), graph);
				if (eventOwner.IsBinded &&
				    ((VMVariableService)IVariableService.Instance)
				    .GetDynamicContext(eventOwner.VariableContext, raisingFSM).Entity.EngineGuid ==
				    dynEvent.Entity.EngineGuid)
					return (VMEventLink)enterLinksByEvent[index];
			}

		if (typeof(VMBlueprint).IsAssignableFrom(graph.Owner.GetType())) {
			var baseBlueprints = ((VMBlueprint)graph.Owner).BaseBlueprints;
			if (baseBlueprints != null)
				for (var index = 0; index < baseBlueprints.Count; ++index) {
					var blueprint = baseBlueprints[index];
					if (blueprint != null) {
						var enterLinkByEvent = GetGraphEnterLinkByEvent(blueprint.StateGraph, raisingFSM, dynEvent,
							messages, isOwnEvent);
						if (enterLinkByEvent != null)
							return enterLinkByEvent;
					}
				}
		}

		return null;
	}

	public static IDynamicGameObjectContext GetEventOwnerDynamicContext(
		DynamicFSM activeFSM,
		CommonVariable eventOwnerVariable,
		IBlueprint graphOwner) {
		if (eventOwnerVariable.IsNull) {
			if (activeFSM.FSMStaticObject.IsDerivedFrom(graphOwner.BaseGuid, true))
				return activeFSM;
		} else {
			if (!eventOwnerVariable.IsBinded)
				eventOwnerVariable.Bind(graphOwner, new VMType(typeof(IObjRef)));
			if (eventOwnerVariable.VariableContext != null)
				return ((VMVariableService)IVariableService.Instance).GetDynamicContext(
					eventOwnerVariable.VariableContext, activeFSM);
		}

		return null;
	}
}