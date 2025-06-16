using System.Collections.Generic;
using Engine.Common.Services;

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
      get => quality;
      set => quality = value;
    }

    public float Weight
    {
      get => weight;
      set => weight = value;
    }

    public JerboaColorEnum ColorEnum
    {
      get => colorEnum;
      set
      {
        colorEnum = value;
        if (!((UnityEngine.Object) prefabInstance != (UnityEngine.Object) null) || !((UnityEngine.Object) prefabInstance.GetComponentInChildren<SkinnedMeshRenderer>() != (UnityEngine.Object) null))
          return;
        foreach (JerboaColor jerboaColor in jerboaColors)
        {
          if (jerboaColor.ColorEnum == colorEnum)
          {
            color = jerboaColor.Color;
            return;
          }
        }
        Debug.LogWarning((object) "Jerboa color not found, defaulting to gray");
        color = new Color(0.5f, 0.5f, 0.5f);
      }
    }

    public Color Color => color;

    public float PathSampleRadius => pathSampleRadius;

    public float TeleportRadius => teleportRadius;

    public float GroupSpreadAngle => groupSpreadAngle;

    public LayerMask RaycasLayerMask => raycastLayerMask;

    public int GroupWalkNavigationMask => groupWalkNavigationMask;

    public int GroupRayCount => groupRayCount;

    public float GroupRayDistance => groupRayDistance;

    public float JerboaRespawnLimitPerSecond => jerboaRespawnLimitPerSecond;

    public float SmoothedJerboaWeight => smoothedJerboaWeight;

    public AudioMixerGroup AudioMixerGroup => audioMixerGroup;

    public AudioClip GetJerboaAudioclip(int level) => jerboaAudioClips[level];

    public TextAsset JerboaAnimationsTexture => jerboaAnimationsTexture;

    public void Syncronize()
    {
      for (int index = 0; index < groups.Count; ++index)
      {
        groups[index].TryGroupTeleport(this.transform.position, teleportRadius);
        groups[index].Syncronize();
      }
    }

    private void OnEnable()
    {
      jerboaInstancingManager = new JerboaInstancingManager(this);
      jerboaAnimationManager = new GameObject("JerboaAnimationManager").AddComponent<JerboaAnimationManager>();
      jerboaAnimationManager.transform.parent = this.transform;
      CreateGroups();
      prefabInstance = UnityEngine.Object.Instantiate<GameObject>(prefab);
      prefabInstance.name = prefab.name;
      JerboaInstance component = prefabInstance.GetComponent<JerboaInstance>();
      component.InitializeAnimation(jerboaInstancingManager, jerboaAnimationManager, jerboaAnimationsTexture);
      component.transform.parent = this.transform;
      for (int index = 0; index < jerboaCount; ++index)
      {
        JerboaInstanceDescription instance = new JerboaInstanceDescription();
        instance.Source = component;
        instance.PlayAnimation(UnityEngine.Random.Range(0, 3));
        currentGroupIndex = (currentGroupIndex + 1) % groups.Count;
        groups[currentGroupIndex].AddInstance(instance);
        jerboaInstancingManager.AddBoundingSphere(instance);
        jerboaInstancingManager.AddInstance(instance);
      }
      Syncronize();
    }

    private void OnDisable()
    {
      DestroyGroups();
      UnityEngine.Object.Destroy((UnityEngine.Object) prefabInstance);
      jerboaInstancingManager.Dispose();
      jerboaInstancingManager = null;
    }

    private void Awake() => Visible = true;

    private void Start()
    {
    }

    private void CreateGroups()
    {
      if ((UnityEngine.Object) groupPrefab == (UnityEngine.Object) null)
      {
        Debug.LogError((object) "Jerboa groupPrefab is null");
      }
      else
      {
        Vector3 position = this.transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(position, out hit, 100f, groupWalkNavigationMask))
          position = hit.position;
        for (int index = 0; index < groupsCount; ++index)
        {
          GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(groupPrefab, position, Quaternion.identity);
          gameObject.name = string.Format("Jerboa_Group_{0}", index);
          JerboaGroupBarycentric component = gameObject.GetComponent<JerboaGroupBarycentric>();
          component.Initialize(this);
          if ((UnityEngine.Object) component == (UnityEngine.Object) null)
          {
            Debug.LogError((object) (typeof (JerboaGroupBarycentric).Name + " component is not on group prefab"));
            break;
          }
          groups.Add(component);
        }
      }
    }

    private void DestroyGroups()
    {
      for (int index = 0; index < groups.Count; ++index)
      {
        JerboaGroupBarycentric group = groups[index];
        if ((UnityEngine.Object) group != (UnityEngine.Object) null)
          UnityEngine.Object.Destroy((UnityEngine.Object) group.gameObject);
      }
      groups.Clear();
    }

    private int SortDelegate(JerboaGroupBarycentric a, JerboaGroupBarycentric b)
    {
      return (this.transform.position - a.transform.position).sqrMagnitude.CompareTo((this.transform.position - b.transform.position).sqrMagnitude);
    }

    private void Update()
    {
      jerboaInstancingManager.Update(this.transform.position);
      smoothedJerboaWeight = Mathf.MoveTowards(smoothedJerboaWeight, weight * quality, (float) (quality * (double) Time.deltaTime / 3.0));
      if ((double) UnityEngine.Random.value < (double) Time.deltaTime / 0.5)
      {
        groups.Sort(SortDelegate);
        for (int index = 0; index < groups.Count; ++index)
          groups[index].Aloud = index < 2;
      }
      for (int index = 0; index < groupsCount; ++index)
      {
        JerboaGroupBarycentric group = groups[index];
        if ((double) (this.transform.position - group.transform.position).magnitude >= teleportRadius * 2.0 && group.TryGroupTeleport(this.transform.position, teleportRadius))
          break;
      }
    }

    public void RemoveInstance(JerboaInstance instance)
    {
    }
  }
}
