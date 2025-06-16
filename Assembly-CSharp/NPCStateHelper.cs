using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Settings.External;
using UnityEngine;
using UnityEngine.AI;

public class NPCStateHelper {
	private static LipSyncComponent previousSpeaker;

	public static void SayIdleReplic(IEntity Owner) {
		var gameObject = ((IEntityView)Owner).GameObject;
		var component1 = Owner.GetComponent<SpeakingComponent>();
		var component2 = Owner.GetComponent<LipSyncComponent>();
		if (component1 == null || component2 == null)
			return;
		var player = ServiceLocator.GetService<ISimulation>().Player;
		if (player != null && (previousSpeaker == null || previousSpeaker.IsDisposed || !previousSpeaker.IsPlaying) &&
		    (((IEntityView)player).GameObject.transform.position - gameObject.transform.position).magnitude <=
		    (double)ExternalSettingsInstance<ExternalCommonSettings>.Instance.IdleReplicsMaxRangeToPlayer) {
			previousSpeaker = component2;
			component2.Play3D(component1.InitialPhrases.Random(),
				ExternalSettingsInstance<ExternalCommonSettings>.Instance.IdleReplicsDistanceMin,
				ExternalSettingsInstance<ExternalCommonSettings>.Instance.IdleReplicsDistanceMax, true);
		}
	}

	public static void SetAgentAreaMask(NavMeshAgent agent, bool indoor) {
		var num1 = 1 << ScriptableObjectInstance<GameSettingsData>.Instance.IndoorNavigationAreaIndex;
		var num2 = 1 << ScriptableObjectInstance<GameSettingsData>.Instance.OutdoorNavigationAreaIndex;
		if (indoor)
			agent.areaMask = (agent.areaMask | num1) & (-1 ^ num2);
		else
			agent.areaMask = (agent.areaMask | num2) & (-1 ^ num1);
	}
}