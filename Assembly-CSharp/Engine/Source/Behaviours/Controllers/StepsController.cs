using System;
using System.Collections.Generic;
using Engine.Assets.Objects;
using Engine.Source.Audio;
using Inspectors;
using Rain;
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
    private Dictionary<StepsEvent, Info> footActions = new Dictionary<StepsEvent, Info>();
    private Dictionary<StepsReaction, float> footReactions = new Dictionary<StepsReaction, float>();
    [Inspected]
    private List<AudioSource> footLeftAudioSources = new List<AudioSource>();
    [Inspected]
    private List<AudioSource> footRightAudioSources = new List<AudioSource>();
    [Inspected]
    private List<AudioSource> footEffectAudioSources = new List<AudioSource>();
    private static List<KeyValuePair<StepsEvent, Info>> tmp = new List<KeyValuePair<StepsEvent, Info>>();

    protected abstract AudioMixerGroup FootAudioMixer { get; }

    protected abstract AudioMixerGroup FootEffectsAudioMixer { get; }

    protected void RefreshMixers()
    {
      footLeftAudioSources.ForEach(audioSource => audioSource.outputAudioMixerGroup = FootAudioMixer);
      footRightAudioSources.ForEach(audioSource => audioSource.outputAudioMixerGroup = FootAudioMixer);
      footEffectAudioSources.ForEach(audioSource => audioSource.outputAudioMixerGroup = FootEffectsAudioMixer);
    }

    protected virtual void Awake()
    {
    }

    protected void OnStep(string data, bool isIndoor)
    {
      if (stepsData == null || string.IsNullOrEmpty(data))
        return;
      UpdateMaterial(data, transform.position, isIndoor);
      float val1 = 0.0f;
      footActions.Clear();
      footReactions.Clear();
      for (int index1 = 0; index1 < stepsData.Actions.Length; ++index1)
      {
        StepsAction action = stepsData.Actions[index1];
        if (action != null && actions.ContainsKey(action.Material.name))
        {
          for (int index2 = 0; index2 < action.Events.Length; ++index2)
          {
            StepsEvent key = action.Events[index2];
            if (key != null && key.Name == data)
            {
              val1 = Math.Max(val1, key.ReactionIntensity);
              Info info = new Info {
                Action = action,
                Intensity = actions[action.Material.name]
              };
              footActions.Add(key, info);
            }
          }
        }
      }
      for (int index = 0; index < stepsData.Reactions.Length; ++index)
      {
        StepsReaction reaction = stepsData.Reactions[index];
        if (reaction != null && reactions.ContainsKey(reaction.Material.name))
          footReactions.Add(reaction, reactions[reaction.Material.name]);
      }
      int index3 = 0;
      bool flag = false;
      tmp.Clear();
      foreach (KeyValuePair<StepsEvent, Info> footAction in footActions)
        tmp.Add(footAction);
      foreach (KeyValuePair<StepsEvent, Info> keyValuePair in tmp)
      {
        if (!(keyValuePair.Value.Action.Material != puddlePhysicMaterial))
        {
          float num = !float.IsNaN(keyValuePair.Value.Intensity) ? keyValuePair.Value.Intensity : keyValuePair.Key.ActionIntensity;
          if (num < (double) keyValuePair.Value.Action.MinThesholdIntensity || num > (double) keyValuePair.Value.Action.MaxThesholdIntensity)
          {
            footActions.Remove(keyValuePair.Key);
            break;
          }
          footActions.Clear();
          footReactions.Clear();
          footActions.Add(keyValuePair.Key, keyValuePair.Value);
          break;
        }
      }
      switch (data)
      {
        case "Skeleton.Humanoid.Foot_Left":
          if (footAudioModel != null)
          {
            FillAudioModelInstance(gameObject, footAudioModel, footLeftAudioSources, FootAudioMixer, footActions.Count);
            using (Dictionary<StepsEvent, Info>.Enumerator enumerator = footActions.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                KeyValuePair<StepsEvent, Info> current = enumerator.Current;
                float num = !float.IsNaN(current.Value.Intensity) ? current.Value.Intensity : current.Key.ActionIntensity;
                if (num >= (double) current.Value.Action.MinThesholdIntensity && num <= (double) current.Value.Action.MaxThesholdIntensity)
                {
                  flag |= current.Key.HaveReaction;
                  AudioSource footLeftAudioSource = footLeftAudioSources[index3];
                  AudioClip audioClip = current.Key.Clips.Random();
                  footLeftAudioSource.volume = num;
                  footLeftAudioSource.clip = audioClip;
                  footLeftAudioSource.PlayAndCheck();
                  ++index3;
                }
              }
            }
          }

          break;
        case "Skeleton.Humanoid.Foot_Right":
          if (footAudioModel != null)
          {
            FillAudioModelInstance(gameObject, footAudioModel, footRightAudioSources, FootAudioMixer, footActions.Count);
            using (Dictionary<StepsEvent, Info>.Enumerator enumerator = footActions.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                KeyValuePair<StepsEvent, Info> current = enumerator.Current;
                float num = !float.IsNaN(current.Value.Intensity) ? current.Value.Intensity : current.Key.ActionIntensity;
                if (num >= (double) current.Value.Action.MinThesholdIntensity && num <= (double) current.Value.Action.MaxThesholdIntensity)
                {
                  flag |= current.Key.HaveReaction;
                  AudioSource rightAudioSource = footRightAudioSources[index3];
                  AudioClip audioClip = current.Key.Clips.Random();
                  rightAudioSource.volume = num;
                  rightAudioSource.clip = audioClip;
                  rightAudioSource.PlayAndCheck();
                  ++index3;
                }
              }
            }
          }

          break;
        default:
          return;
      }
      int index4 = 0;
      if (!(footEffectAudioModel != null))
        return;
      FillAudioModelInstance(gameObject, footEffectAudioModel, footEffectAudioSources, FootEffectsAudioMixer, footReactions.Count);
      foreach (KeyValuePair<StepsReaction, float> footReaction in footReactions)
      {
        float num = (!float.IsNaN(footReaction.Value) ? footReaction.Value : footReaction.Key.Intensity) * val1;
        if (num >= (double) footReaction.Key.MinThesholdIntensity && num <= (double) footReaction.Key.MaxThesholdIntensity)
        {
          AudioSource effectAudioSource = footEffectAudioSources[index4];
          AudioClip audioClip = footReaction.Key.Clips.Random();
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
      if (prefab == null || prefab.GetComponent<AudioSource>() == null)
        return;
      while (collection.Count < count)
      {
        GameObject gameObject = Instantiate(prefab);
        gameObject.transform.SetParent(parent.transform, false);
        AudioSource component = gameObject.GetComponent<AudioSource>();
        if (component == null)
          throw new Exception();
        component.loop = false;
        component.outputAudioMixerGroup = mixer;
        collection.Add(component);
      }
    }

    private void UpdateTerrainMaterial(GameObject go, Vector3 position)
    {
      if (go == null)
        return;
      Terrain component = go.GetComponent<Terrain>();
      if (component == null)
        return;
      TerrainData terrainData = component.terrainData;
      Vector3 vector3_1 = position - go.transform.position;
      if (stepsData == null)
        return;
      Vector3 vector3_2 = vector3_1 / component.terrainData.size.x * component.terrainData.alphamapResolution;
      float[,,] alphamaps = terrainData.GetAlphamaps((int) vector3_2.x, (int) vector3_2.z, 1, 1);
      int index1 = -1;
      float f1 = float.NaN;
      for (int index2 = 0; index2 < stepsData.TileLayers.Length; ++index2)
      {
        float num = alphamaps[0, 0, index2];
        if (float.IsNaN(f1) || f1 <= (double) num)
        {
          f1 = num;
          index1 = index2;
        }
      }
      if (!float.IsNaN(f1) && stepsData.TileLayers.Length != 0 && f1 > 0.0)
        actions.Add(stepsData.TileLayers[index1].name, float.NaN);
      Vector3 vector3_3 = vector3_1 / component.terrainData.size.x * component.terrainData.detailResolution;
      int index3 = -1;
      float f2 = float.NaN;
      for (int layer = 0; layer < stepsData.DetailLayers.Length; ++layer)
      {
        float num = terrainData.GetDetailLayer((int) vector3_3.x, (int) vector3_3.z, 1, 1, layer)[0, 0];
        if (float.IsNaN(f2) || f2 <= (double) num)
        {
          f2 = num;
          index3 = layer;
        }
      }
      if (!float.IsNaN(f2) && stepsData.DetailLayers.Length != 0 && f2 > 0.0)
        reactions.Add(stepsData.DetailLayers[index3].name, float.NaN);
    }

    private void UpdateMaterial(string data, Vector3 position, bool isIndoor)
    {
      actions.Clear();
      reactions.Clear();
      float maxDistance = 1f;
      LayerMask puddlesLayer = ScriptableObjectInstance<GameSettingsData>.Instance.PuddlesLayer;
      RaycastHit hitInfo1;
      if (Physics.Raycast(position + new Vector3(0.0f, maxDistance / 2f, 0.0f), new Vector3(0.0f, -maxDistance, 0.0f), out hitInfo1, maxDistance, puddlesLayer, QueryTriggerInteraction.Ignore) && hitInfo1.collider != null)
      {
        GameObject gameObject = hitInfo1.collider.gameObject;
        if (gameObject != null)
        {
          Puddle component = gameObject.GetComponent<Puddle>();
          float Wetness;
          if (component != null && component.GetWetness(hitInfo1, out Wetness))
          {
            actions.Add(puddlePhysicMaterial.name, Wetness);
            component.AddRipple(hitInfo1.point, 0.1f, 1f);
          }
        }
      }
      RainManager instance = RainManager.Instance;
      if (!(instance != null))
        return;
      LayerMask stepsLayer = ScriptableObjectInstance<GameSettingsData>.Instance.StepsLayer;
      RaycastHit hitInfo2;
      if (Physics.Raycast(position + new Vector3(0.0f, maxDistance / 2f, 0.0f), new Vector3(0.0f, -maxDistance, 0.0f), out hitInfo2, maxDistance, stepsLayer, QueryTriggerInteraction.Ignore))
      {
        Collider collider = hitInfo2.collider;
        if (collider != null)
        {
          PhysicMaterial rainPhysicMaterial = this.rainPhysicMaterial;
          if (instance.rainIntensity > 1.4012984643248171E-45 && rainPhysicMaterial != null && !isIndoor)
            reactions.Add(rainPhysicMaterial.name, instance.rainIntensity);
          if (collider as TerrainCollider != null)
            UpdateTerrainMaterial(collider.gameObject, position);
          else
            UpdateMeshMaterial(collider);
        }
      }
    }

    private void UpdateMeshMaterial(Collider collider)
    {
      if (collider == null)
        return;
      if (collider.sharedMaterial == null)
        actions.Add(collider.material.name, float.NaN);
      else
        actions.Add(collider.sharedMaterial.name, float.NaN);
    }

    public struct Info
    {
      public StepsAction Action;
      public float Intensity;
    }
  }
}
