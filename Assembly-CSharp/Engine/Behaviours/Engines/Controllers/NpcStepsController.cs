using Engine.Behaviours.Components;
using Engine.Source.Behaviours.Controllers;
using Engine.Source.Commons;
using Engine.Source.Services;
using Engine.Source.Settings.External;
using Inspectors;
using UnityEngine;
using UnityEngine.Audio;

namespace Engine.Behaviours.Engines.Controllers
{
  [DisallowMultipleComponent]
  public class NpcStepsController : StepsController
  {
    private Pivot pivot;

    [Inspected]
    private bool Indoor { get; set; }

    [Inspected]
    protected override AudioMixerGroup FootAudioMixer => Indoor ? ScriptableObjectInstance<GameSettingsData>.Instance.NpcFootIndoorMixer : ScriptableObjectInstance<GameSettingsData>.Instance.NpcFootOutdoorMixer;

    [Inspected]
    protected override AudioMixerGroup FootEffectsAudioMixer => Indoor ? ScriptableObjectInstance<GameSettingsData>.Instance.NpcFootEffectsIndoorMixer : ScriptableObjectInstance<GameSettingsData>.Instance.NpcFootEffectsOutdoorMixer;

    protected void Start()
    {
      pivot = gameObject.GetComponent<Pivot>();
      if (pivot == null)
      {
        Debug.LogError("Please move " + typeof (NpcStepsController).Name + " to root of prefab and make sure " + typeof (Pivot).Name + " is also at the same level.", gameObject);
      }
      else
      {
        pivot.GetAnimatorEventProxy().AnimatorEventEvent += AnimatorEvent;
        pivot.IndoorChangedEvent += Pivot_IndoorChangedEvent;
        Pivot_IndoorChangedEvent(pivot.Indoor);
      }
    }

    protected void OnDestroy()
    {
      if (!(pivot != null))
        return;
      pivot.IndoorChangedEvent -= Pivot_IndoorChangedEvent;
    }

    private void Pivot_IndoorChangedEvent(bool indoor)
    {
      Indoor = indoor;
      RefreshMixers();
    }

    protected void AnimatorEvent(string data)
    {
      string str = data;
      if (!(str == "Skeleton.Humanoid.Foot_Left") && !(str == "Skeleton.Humanoid.Foot_Right") || !DetectorUtility.CheckDistance(EngineApplication.PlayerPosition, transform.position, ExternalSettingsInstance<ExternalCommonSettings>.Instance.StepsDistance))
        return;
      OnStep(data, Indoor);
    }
  }
}
