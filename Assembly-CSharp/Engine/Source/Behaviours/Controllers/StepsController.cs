using Engine.Assets.Objects;
using Engine.Source.Audio;
using Inspectors;
using Rain;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Engine.Source.Behaviours.Controllers
{
  public abstract class StepsController : MonoBehaviour
  {
    [SerializeField]
    private GameObject footEffectAudioModel;
    [SerializeField]
    private GameObject footAudioModel;
    private Dictionary<string, float> actions = new Dictionary<string, float>();
    private Dictionary<string, float> reactions = new Dictionary<string, float>();
    [SerializeField]
    private PhysicMaterial puddlePhysicMaterial;
    [SerializeField]
    private PhysicMaterial rainPhysicMaterial;
    protected const string footLeftEventName = "Skeleton.Humanoid.Foot_Left";
    protected const string footRightEventName = "Skeleton.Humanoid.Foot_Right";
    [SerializeField]
    private StepsData stepsData;
    private Dictionary<StepsEvent, StepsController.Info> footActions = new Dictionary<StepsEvent, StepsController.Info>();
    private Dictionary<StepsReaction, float> footReactions = new Dictionary<StepsReaction, float>();
    [Inspected]
    private List<AudioSource> footLeftAudioSources = new List<AudioSource>();
    [Inspected]
    private List<AudioSource> footRightAudioSources = new List<AudioSource>();
    [Inspected]
    private List<AudioSource> footEffectAudioSources = new List<AudioSource>();
    private static List<KeyValuePair<StepsEvent, StepsController.Info>> tmp = new List<KeyValuePair<StepsEvent, StepsController.Info>>();

    protected abstract AudioMixerGroup FootAudioMixer { get; }

    protected abstract AudioMixerGroup FootEffectsAudioMixer { get; }

    protected void RefreshMixers()
    {
      this.footLeftAudioSources.ForEach((Action<AudioSource>) (audioSource => audioSource.outputAudioMixerGroup = this.FootAudioMixer));
      this.footRightAudioSources.ForEach((Action<AudioSource>) (audioSource => audioSource.outputAudioMixerGroup = this.FootAudioMixer));
      this.footEffectAudioSources.ForEach((Action<AudioSource>) (audioSource => audioSource.outputAudioMixerGroup = this.FootEffectsAudioMixer));
    }

    protected virtual void Awake()
    {
    }

    protected void OnStep(string data, bool isIndoor)
    {
      if ((UnityEngine.Object) this.stepsData == (UnityEngine.Object) null || string.IsNullOrEmpty(data))
        return;
      this.UpdateMaterial(data, this.transform.position, isIndoor);
      float val1 = 0.0f;
      this.footActions.Clear();
      this.footReactions.Clear();
      for (int index1 = 0; index1 < this.stepsData.Actions.Length; ++index1)
      {
        StepsAction action = this.stepsData.Actions[index1];
        if (action != null && this.actions.ContainsKey(action.Material.name))
        {
          for (int index2 = 0; index2 < action.Events.Length; ++index2)
          {
            StepsEvent key = action.Events[index2];
            if (key != null && key.Name == data)
            {
              val1 = Math.Max(val1, key.ReactionIntensity);
              StepsController.Info info = new StepsController.Info()
              {
                Action = action,
                Intensity = this.actions[action.Material.name]
              };
              this.footActions.Add(key, info);
            }
          }
        }
      }
      for (int index = 0; index < this.stepsData.Reactions.Length; ++index)
      {
        StepsReaction reaction = this.stepsData.Reactions[index];
        if (reaction != null && this.reactions.ContainsKey(reaction.Material.name))
          this.footReactions.Add(reaction, this.reactions[reaction.Material.name]);
      }
      int index3 = 0;
      bool flag = false;
      StepsController.tmp.Clear();
      foreach (KeyValuePair<StepsEvent, StepsController.Info> footAction in this.footActions)
        StepsController.tmp.Add(footAction);
      foreach (KeyValuePair<StepsEvent, StepsController.Info> keyValuePair in StepsController.tmp)
      {
        if (!((UnityEngine.Object) keyValuePair.Value.Action.Material != (UnityEngine.Object) this.puddlePhysicMaterial))
        {
          float num = !float.IsNaN(keyValuePair.Value.Intensity) ? keyValuePair.Value.Intensity : keyValuePair.Key.ActionIntensity;
          if ((double) num < (double) keyValuePair.Value.Action.MinThesholdIntensity || (double) num > (double) keyValuePair.Value.Action.MaxThesholdIntensity)
          {
            this.footActions.Remove(keyValuePair.Key);
            break;
          }
          this.footActions.Clear();
          this.footReactions.Clear();
          this.footActions.Add(keyValuePair.Key, keyValuePair.Value);
          break;
        }
      }
      switch (data)
      {
        case "Skeleton.Humanoid.Foot_Left":
          if ((UnityEngine.Object) this.footAudioModel != (UnityEngine.Object) null)
          {
            this.FillAudioModelInstance(this.gameObject, this.footAudioModel, (IList<AudioSource>) this.footLeftAudioSources, this.FootAudioMixer, this.footActions.Count);
            using (Dictionary<StepsEvent, StepsController.Info>.Enumerator enumerator = this.footActions.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                KeyValuePair<StepsEvent, StepsController.Info> current = enumerator.Current;
                float num = !float.IsNaN(current.Value.Intensity) ? current.Value.Intensity : current.Key.ActionIntensity;
                if ((double) num >= (double) current.Value.Action.MinThesholdIntensity && (double) num <= (double) current.Value.Action.MaxThesholdIntensity)
                {
                  flag |= current.Key.HaveReaction;
                  AudioSource footLeftAudioSource = this.footLeftAudioSources[index3];
                  AudioClip audioClip = ((IEnumerable<AudioClip>) current.Key.Clips).Random<AudioClip>();
                  footLeftAudioSource.volume = num;
                  footLeftAudioSource.clip = audioClip;
                  footLeftAudioSource.PlayAndCheck();
                  ++index3;
                }
              }
              break;
            }
          }
          else
            break;
        case "Skeleton.Humanoid.Foot_Right":
          if ((UnityEngine.Object) this.footAudioModel != (UnityEngine.Object) null)
          {
            this.FillAudioModelInstance(this.gameObject, this.footAudioModel, (IList<AudioSource>) this.footRightAudioSources, this.FootAudioMixer, this.footActions.Count);
            using (Dictionary<StepsEvent, StepsController.Info>.Enumerator enumerator = this.footActions.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                KeyValuePair<StepsEvent, StepsController.Info> current = enumerator.Current;
                float num = !float.IsNaN(current.Value.Intensity) ? current.Value.Intensity : current.Key.ActionIntensity;
                if ((double) num >= (double) current.Value.Action.MinThesholdIntensity && (double) num <= (double) current.Value.Action.MaxThesholdIntensity)
                {
                  flag |= current.Key.HaveReaction;
                  AudioSource rightAudioSource = this.footRightAudioSources[index3];
                  AudioClip audioClip = ((IEnumerable<AudioClip>) current.Key.Clips).Random<AudioClip>();
                  rightAudioSource.volume = num;
                  rightAudioSource.clip = audioClip;
                  rightAudioSource.PlayAndCheck();
                  ++index3;
                }
              }
              break;
            }
          }
          else
            break;
        default:
          return;
      }
      int index4 = 0;
      if (!((UnityEngine.Object) this.footEffectAudioModel != (UnityEngine.Object) null))
        return;
      this.FillAudioModelInstance(this.gameObject, this.footEffectAudioModel, (IList<AudioSource>) this.footEffectAudioSources, this.FootEffectsAudioMixer, this.footReactions.Count);
      foreach (KeyValuePair<StepsReaction, float> footReaction in this.footReactions)
      {
        float num = (!float.IsNaN(footReaction.Value) ? footReaction.Value : footReaction.Key.Intensity) * val1;
        if ((double) num >= (double) footReaction.Key.MinThesholdIntensity && (double) num <= (double) footReaction.Key.MaxThesholdIntensity)
        {
          AudioSource effectAudioSource = this.footEffectAudioSources[index4];
          AudioClip audioClip = ((IEnumerable<AudioClip>) footReaction.Key.Clips).Random<AudioClip>();
          effectAudioSource.volume = num;
          effectAudioSource.clip = audioClip;
          effectAudioSource.PlayAndCheck();
          ++index4;
        }
      }
    }

    private void FillAudioModelInstance(
      GameObject parent,
      GameObject prefab,
      IList<AudioSource> collection,
      AudioMixerGroup mixer,
      int count)
    {
      if ((UnityEngine.Object) prefab == (UnityEngine.Object) null || (UnityEngine.Object) prefab.GetComponent<AudioSource>() == (UnityEngine.Object) null)
        return;
      while (collection.Count < count)
      {
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab);
        gameObject.transform.SetParent(parent.transform, false);
        AudioSource component = gameObject.GetComponent<AudioSource>();
        if ((UnityEngine.Object) component == (UnityEngine.Object) null)
          throw new Exception();
        component.loop = false;
        component.outputAudioMixerGroup = mixer;
        collection.Add(component);
      }
    }

    private void UpdateTerrainMaterial(GameObject go, Vector3 position)
    {
      if ((UnityEngine.Object) go == (UnityEngine.Object) null)
        return;
      Terrain component = go.GetComponent<Terrain>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
        return;
      TerrainData terrainData = component.terrainData;
      Vector3 vector3_1 = position - go.transform.position;
      if ((UnityEngine.Object) this.stepsData == (UnityEngine.Object) null)
        return;
      Vector3 vector3_2 = vector3_1 / component.terrainData.size.x * (float) component.terrainData.alphamapResolution;
      float[,,] alphamaps = terrainData.GetAlphamaps((int) vector3_2.x, (int) vector3_2.z, 1, 1);
      int index1 = -1;
      float f1 = float.NaN;
      for (int index2 = 0; index2 < this.stepsData.TileLayers.Length; ++index2)
      {
        float num = alphamaps[0, 0, index2];
        if (float.IsNaN(f1) || (double) f1 <= (double) num)
        {
          f1 = num;
          index1 = index2;
        }
      }
      if (!float.IsNaN(f1) && this.stepsData.TileLayers.Length != 0 && (double) f1 > 0.0)
        this.actions.Add(this.stepsData.TileLayers[index1].name, float.NaN);
      Vector3 vector3_3 = vector3_1 / component.terrainData.size.x * (float) component.terrainData.detailResolution;
      int index3 = -1;
      float f2 = float.NaN;
      for (int layer = 0; layer < this.stepsData.DetailLayers.Length; ++layer)
      {
        float num = (float) terrainData.GetDetailLayer((int) vector3_3.x, (int) vector3_3.z, 1, 1, layer)[0, 0];
        if (float.IsNaN(f2) || (double) f2 <= (double) num)
        {
          f2 = num;
          index3 = layer;
        }
      }
      if (!float.IsNaN(f2) && this.stepsData.DetailLayers.Length != 0 && (double) f2 > 0.0)
        this.reactions.Add(this.stepsData.DetailLayers[index3].name, float.NaN);
    }

    private void UpdateMaterial(string data, Vector3 position, bool isIndoor)
    {
      this.actions.Clear();
      this.reactions.Clear();
      float maxDistance = 1f;
      LayerMask puddlesLayer = ScriptableObjectInstance<GameSettingsData>.Instance.PuddlesLayer;
      RaycastHit hitInfo1;
      if (Physics.Raycast(position + new Vector3(0.0f, maxDistance / 2f, 0.0f), new Vector3(0.0f, -maxDistance, 0.0f), out hitInfo1, maxDistance, (int) puddlesLayer, QueryTriggerInteraction.Ignore) && (UnityEngine.Object) hitInfo1.collider != (UnityEngine.Object) null)
      {
        GameObject gameObject = hitInfo1.collider.gameObject;
        if ((UnityEngine.Object) gameObject != (UnityEngine.Object) null)
        {
          Puddle component = gameObject.GetComponent<Puddle>();
          float Wetness;
          if ((UnityEngine.Object) component != (UnityEngine.Object) null && component.GetWetness(hitInfo1, out Wetness))
          {
            this.actions.Add(this.puddlePhysicMaterial.name, Wetness);
            component.AddRipple(hitInfo1.point, 0.1f, 1f);
          }
        }
      }
      RainManager instance = RainManager.Instance;
      if (!((UnityEngine.Object) instance != (UnityEngine.Object) null))
        return;
      LayerMask stepsLayer = ScriptableObjectInstance<GameSettingsData>.Instance.StepsLayer;
      RaycastHit hitInfo2;
      if (Physics.Raycast(position + new Vector3(0.0f, maxDistance / 2f, 0.0f), new Vector3(0.0f, -maxDistance, 0.0f), out hitInfo2, maxDistance, (int) stepsLayer, QueryTriggerInteraction.Ignore))
      {
        Collider collider = hitInfo2.collider;
        if ((UnityEngine.Object) collider != (UnityEngine.Object) null)
        {
          PhysicMaterial rainPhysicMaterial = this.rainPhysicMaterial;
          if ((double) instance.rainIntensity > 1.4012984643248171E-45 && (UnityEngine.Object) rainPhysicMaterial != (UnityEngine.Object) null && !isIndoor)
            this.reactions.Add(rainPhysicMaterial.name, instance.rainIntensity);
          if ((UnityEngine.Object) (collider as TerrainCollider) != (UnityEngine.Object) null)
            this.UpdateTerrainMaterial(collider.gameObject, position);
          else
            this.UpdateMeshMaterial(collider);
        }
      }
    }

    private void UpdateMeshMaterial(Collider collider)
    {
      if ((UnityEngine.Object) collider == (UnityEngine.Object) null)
        return;
      if ((UnityEngine.Object) collider.sharedMaterial == (UnityEngine.Object) null)
        this.actions.Add(collider.material.name, float.NaN);
      else
        this.actions.Add(collider.sharedMaterial.name, float.NaN);
    }

    public struct Info
    {
      public StepsAction Action;
      public float Intensity;
    }
  }
}
