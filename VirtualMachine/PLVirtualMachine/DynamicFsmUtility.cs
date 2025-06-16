using PLVirtualMachine.Common;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Common.VMSpecialAttributes;
using PLVirtualMachine.Dynamic;
using PLVirtualMachine.FSM;
using PLVirtualMachine.Objects;
using System.Collections.Generic;

namespace PLVirtualMachine
{
  public static class DynamicFsmUtility
  {
    public static VMEventLink GetStateOutputLinkByEvent(
      VMState state,
      DynamicFSM raisingFSM,
      DynamicEvent dynEvent,
      bool isOwnEvent)
    {
      if (dynEvent.FunctionalName == EngineAPIManager.GetSpecialEventName(ESpecialEventName.SEN_LOAD_GAME, typeof (VMGameComponent), true))
        return state.GetAfterExitLink();
      List<IEventLink> eventLinkList = state.GetOutputLinksByEventGuid(dynEvent.BaseGuid) ?? state.GetOutputLinksByEvent(dynEvent.StaticEvent);
      if (eventLinkList != null)
      {
        for (int index = 0; index < eventLinkList.Count; ++index)
        {
          CommonVariable eventOwner = eventLinkList[index].Event.EventOwner;
          if (eventOwner.IsNull & isOwnEvent)
            return (VMEventLink) eventLinkList[index];
          if (!eventOwner.IsBinded)
            eventOwner.Bind((IContext) state.Owner, new VMType(typeof (IObjRef)), (ILocalContext) state);
          if (eventOwner.IsBinded && ((VMVariableService) IVariableService.Instance).GetDynamicContext(eventOwner.VariableContext, (IDynamicGameObjectContext) raisingFSM).Entity.EngineGuid == dynEvent.Entity.EngineGuid)
            return (VMEventLink) eventLinkList[index];
        }
      }
      return (VMEventLink) null;
    }

    public static VMEventLink GetGraphEnterLinkByEvent(
      IFiniteStateMachine graph,
      DynamicFSM raisingFSM,
      DynamicEvent dynEvent,
      List<EventMessage> messages,
      bool isOwnEvent)
    {
      List<IEventLink> enterLinksByEvent = ((FiniteStateMachine) graph).GetEnterLinksByEvent(dynEvent);
      if (enterLinksByEvent != null)
      {
        for (int index = 0; index < enterLinksByEvent.Count; ++index)
        {
          CommonVariable eventOwner = enterLinksByEvent[index].Event.EventOwner;
          if (eventOwner.IsNull & isOwnEvent)
            return (VMEventLink) enterLinksByEvent[index];
          if (!eventOwner.IsBinded)
            eventOwner.Bind((IContext) graph.Owner, new VMType(typeof (IObjRef)), (ILocalContext) graph);
          if (eventOwner.IsBinded && ((VMVariableService) IVariableService.Instance).GetDynamicContext(eventOwner.VariableContext, (IDynamicGameObjectContext) raisingFSM).Entity.EngineGuid == dynEvent.Entity.EngineGuid)
            return (VMEventLink) enterLinksByEvent[index];
        }
      }
      if (typeof (VMBlueprint).IsAssignableFrom(graph.Owner.GetType()))
      {
        List<IBlueprint> baseBlueprints = ((VMBlueprint) graph.Owner).BaseBlueprints;
        if (baseBlueprints != null)
        {
          for (int index = 0; index < baseBlueprints.Count; ++index)
          {
            IBlueprint blueprint = baseBlueprints[index];
            if (blueprint != null)
            {
              VMEventLink enterLinkByEvent = DynamicFsmUtility.GetGraphEnterLinkByEvent(blueprint.StateGraph, raisingFSM, dynEvent, messages, isOwnEvent);
              if (enterLinkByEvent != null)
                return enterLinkByEvent;
            }
          }
        }
      }
      return (VMEventLink) null;
    }

    public static IDynamicGameObjectContext GetEventOwnerDynamicContext(
      DynamicFSM activeFSM,
      CommonVariable eventOwnerVariable,
      IBlueprint graphOwner)
    {
      if (eventOwnerVariable.IsNull)
      {
        if (activeFSM.FSMStaticObject.IsDerivedFrom(graphOwner.BaseGuid, true))
          return (IDynamicGameObjectContext) activeFSM;
      }
      else
      {
        if (!eventOwnerVariable.IsBinded)
          eventOwnerVariable.Bind((IContext) graphOwner, new VMType(typeof (IObjRef)));
        if (eventOwnerVariable.VariableContext != null)
          return ((VMVariableService) IVariableService.Instance).GetDynamicContext(eventOwnerVariable.VariableContext, (IDynamicGameObjectContext) activeFSM);
      }
      return (IDynamicGameObjectContext) null;
    }
  }
}
