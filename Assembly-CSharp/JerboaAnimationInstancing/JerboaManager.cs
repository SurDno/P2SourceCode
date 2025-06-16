// Decompiled with JetBrains decompiler
// Type: JerboaAnimationInstancing.JerboaManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;

#nullable disable
namespace JerboaAnimationInstancing
{
  public class JerboaManager : MonoBehaviour
  {
    [Header("Properties")]
    [Tooltip("Влияет линейно на максимальное количество тушканчиков")]
    [Range(0.0f, 1f)]
    [SerializeField]
    private float quality = 1f;
    [Tooltip("Влияет линейно на максимальное количество тушканчиков")]
    [Range(0.0f, 1f)]
    [SerializeField]
    private float weight = 1f;
    [SerializeField]
    private JerboaColorEnum colorEnum = JerboaColorEnum.Default;
    private Color color = Color.gray;
    [SerializeField]
    private JerboaColor[] jerboaColors;
    [Header("Settings")]
    [SerializeField]
    private GameObject prefab;
    private GameObject prefabInstance;
    [SerializeField]
    private GameObject groupPrefab;
    [SerializeField]
    private int groupsCount = 1;
    [SerializeField]
    private int jerboaCount = 1000;
    [SerializeField]
    private float pathSampleRadius = 12f;
    [SerializeField]
    private float teleportRadius = 20f;
    [SerializeField]
    private float groupSpreadAngle = 90f;
    [SerializeField]
    private LayerMask raycastLayerMask;
    [SerializeField]
    private int groupWalkNavigationMask = -1;
    [SerializeField]
    private int groupRayCount = 10;
    [SerializeField]
    private float groupRayDistance = 10f;
    [SerializeField]
    private float jerboaRespawnLimitPerSecond = 50f;
    private float smoothedJerboaWeight;
    [Header("Audio")]
    [SerializeField]
    private AudioMixerGroup audioMixerGroup;
    [SerializeField]
    private AudioClip[] jerboaAudioClips = new AudioClip[3];
    [Header("Animations")]
    [SerializeField]
    private TextAsset jerboaAnimationsTexture;
    private List<JerboaGroupBarycentric> groups = new List<JerboaGroupBarycentric>();
    private JerboaInstancingManager jerboaInstancingManager;
    private JerboaAnimationManager jerboaAnimationManager;
    private int currentGroupIndex;

    public bool Visible { get; set; }

    public float Quality
    {
      get => this.quality;
      set => this.quality = value;
    }

    public float Weight
    {
      get => this.weight;
      set => this.weight = value;
    }

    public JerboaColorEnum ColorEnum
    {
      get => this.colorEnum;
      set
      {
        this.colorEnum = value;
        if (!((UnityEngine.Object) this.prefabInstance != (UnityEngine.Object) null) || !((UnityEngine.Object) this.prefabInstance.GetComponentInChildren<SkinnedMeshRenderer>() != (UnityEngine.Object) null))
          return;
        foreach (JerboaColor jerboaColor in this.jerboaColors)
        {
          if (jerboaColor.ColorEnum == this.colorEnum)
          {
            this.color = jerboaColor.Color;
            return;
          }
        }
        Debug.LogWarning((object) "Jerboa color not found, defaulting to gray");
        this.color = new Color(0.5f, 0.5f, 0.5f);
      }
    }

    public Color Color => this.color;

    public float PathSampleRadius => this.pathSampleRadius;

    public float TeleportRadius => this.teleportRadius;

    public float GroupSpreadAngle => this.groupSpreadAngle;

    public LayerMask RaycasLayerMask => this.raycastLayerMask;

    public int GroupWalkNavigationMask => this.groupWalkNavigationMask;

    public int GroupRayCount => this.groupRayCount;

    public float GroupRayDistance => this.groupRayDistance;

    public float JerboaRespawnLimitPerSecond => this.jerboaRespawnLimitPerSecond;

    public float SmoothedJerboaWeight => this.smoothedJerboaWeight;

    public AudioMixerGroup AudioMixerGroup => this.audioMixerGroup;

    public AudioClip GetJerboaAudioclip(int level) => this.jerboaAudioClips[level];

    public TextAsset JerboaAnimationsTexture => this.jerboaAnimationsTexture;

    public void Syncronize()
    {
      for (int index = 0; index < this.groups.Count; ++index)
      {
        this.groups[index].TryGroupTeleport(this.transform.position, this.teleportRadius);
        this.groups[index].Syncronize();
      }
    }

    private void OnEnable()
    {
      this.jerboaInstancingManager = new JerboaInstancingManager(this);
      this.jerboaAnimationManager = new GameObject("JerboaAnimationManager").AddComponent<JerboaAnimationManager>();
      this.jerboaAnimationManager.transform.parent = this.transform;
      this.CreateGroups();
      this.prefabInstance = UnityEngine.Object.Instantiate<GameObject>(this.prefab);
      this.prefabInstance.name = this.prefab.name;
      JerboaInstance component = this.prefabInstance.GetComponent<JerboaInstance>();
      component.InitializeAnimation(this.jerboaInstancingManager, this.jerboaAnimationManager, this.jerboaAnimationsTexture);
      component.transform.parent = this.transform;
      for (int index = 0; index < this.jerboaCount; ++index)
      {
        JerboaInstanceDescription instance = new JerboaInstanceDescription();
        instance.Source = component;
        instance.PlayAnimation(UnityEngine.Random.Range(0, 3));
        this.currentGroupIndex = (this.currentGroupIndex + 1) % this.groups.Count;
        this.groups[this.currentGroupIndex].AddInstance(instance);
        this.jerboaInstancingManager.AddBoundingSphere(instance);
        this.jerboaInstancingManager.AddInstance(instance);
      }
      this.Syncronize();
    }

    private void OnDisable()
    {
      this.DestroyGroups();
      UnityEngine.Object.Destroy((UnityEngine.Object) this.prefabInstance);
      this.jerboaInstancingManager.Dispose();
      this.jerboaInstancingManager = (JerboaInstancingManager) null;
    }

    private void Awake() => this.Visible = true;

    private void Start()
    {
    }

    private void CreateGroups()
    {
      if ((UnityEngine.Object) this.groupPrefab == (UnityEngine.Object) null)
      {
        Debug.LogError((object) "Jerboa groupPrefab is null");
      }
      else
      {
        Vector3 position = this.transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(position, out hit, 100f, this.groupWalkNavigationMask))
          position = hit.position;
        for (int index = 0; index < this.groupsCount; ++index)
        {
          GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.groupPrefab, position, Quaternion.identity);
          gameObject.name = string.Format("Jerboa_Group_{0}", (object) index);
          JerboaGroupBarycentric component = gameObject.GetComponent<JerboaGroupBarycentric>();
          component.Initialize(this);
          if ((UnityEngine.Object) component == (UnityEngine.Object) null)
          {
            Debug.LogError((object) (typeof (JerboaGroupBarycentric).Name + " component is not on group prefab"));
            break;
          }
          this.groups.Add(component);
        }
      }
    }

    private void DestroyGroups()
    {
      for (int index = 0; index < this.groups.Count; ++index)
      {
        JerboaGroupBarycentric group = this.groups[index];
        if ((UnityEngine.Object) group != (UnityEngine.Object) null)
          UnityEngine.Object.Destroy((UnityEngine.Object) group.gameObject);
      }
      this.groups.Clear();
    }

    private int SortDelegate(JerboaGroupBarycentric a, JerboaGroupBarycentric b)
    {
      return (this.transform.position - a.transform.position).sqrMagnitude.CompareTo((this.transform.position - b.transform.position).sqrMagnitude);
    }

    private void Update()
    {
      this.jerboaInstancingManager.Update(this.transform.position);
      this.smoothedJerboaWeight = Mathf.MoveTowards(this.smoothedJerboaWeight, this.weight * this.quality, (float) ((double) this.quality * (double) Time.deltaTime / 3.0));
      if ((double) UnityEngine.Random.value < (double) Time.deltaTime / 0.5)
      {
        this.groups.Sort(new Comparison<JerboaGroupBarycentric>(this.SortDelegate));
        for (int index = 0; index < this.groups.Count; ++index)
          this.groups[index].Aloud = index < 2;
      }
      for (int index = 0; index < this.groupsCount; ++index)
      {
        JerboaGroupBarycentric group = this.groups[index];
        if ((double) (this.transform.position - group.transform.position).magnitude >= (double) this.teleportRadius * 2.0 && group.TryGroupTeleport(this.transform.position, this.teleportRadius))
          break;
      }
    }

    public void RemoveInstance(JerboaInstance instance)
    {
    }
  }
}
