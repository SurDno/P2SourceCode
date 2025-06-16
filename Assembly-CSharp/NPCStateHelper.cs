// Decompiled with JetBrains decompiler
// Type: NPCStateHelper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Settings.External;
using UnityEngine;
using UnityEngine.AI;

#nullable disable
public class NPCStateHelper
{
  private static LipSyncComponent previousSpeaker = (LipSyncComponent) null;

  public static void SayIdleReplic(IEntity Owner)
  {
    GameObject gameObject = ((IEntityView) Owner).GameObject;
    SpeakingComponent component1 = Owner.GetComponent<SpeakingComponent>();
    LipSyncComponent component2 = Owner.GetComponent<LipSyncComponent>();
    if (component1 == null || component2 == null)
      return;
    IEntity player = ServiceLocator.GetService<ISimulation>().Player;
    if (player != null && (NPCStateHelper.previousSpeaker == null || NPCStateHelper.previousSpeaker.IsDisposed || !NPCStateHelper.previousSpeaker.IsPlaying) && (double) (((IEntityView) player).GameObject.transform.position - gameObject.transform.position).magnitude <= (double) ExternalSettingsInstance<ExternalCommonSettings>.Instance.IdleReplicsMaxRangeToPlayer)
    {
      NPCStateHelper.previousSpeaker = component2;
      component2.Play3D(component1.InitialPhrases.Random<ILipSyncObject>(), ExternalSettingsInstance<ExternalCommonSettings>.Instance.IdleReplicsDistanceMin, ExternalSettingsInstance<ExternalCommonSettings>.Instance.IdleReplicsDistanceMax, true);
    }
  }

  public static void SetAgentAreaMask(NavMeshAgent agent, bool indoor)
  {
    int num1 = 1 << ScriptableObjectInstance<GameSettingsData>.Instance.IndoorNavigationAreaIndex;
    int num2 = 1 << ScriptableObjectInstance<GameSettingsData>.Instance.OutdoorNavigationAreaIndex;
    if (indoor)
      agent.areaMask = (agent.areaMask | num1) & (-1 ^ num2);
    else
      agent.areaMask = (agent.areaMask | num2) & (-1 ^ num1);
  }
}
