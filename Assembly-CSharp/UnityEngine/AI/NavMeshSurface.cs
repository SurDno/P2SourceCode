using System;
using System.Collections.Generic;

namespace UnityEngine.AI
{
  [ExecuteAlways]
  [DefaultExecutionOrder(-102)]
  [AddComponentMenu("Navigation/NavMeshSurface", 30)]
  [HelpURL("https://github.com/Unity-Technologies/NavMeshComponents#documentation-draft")]
  public class NavMeshSurface : MonoBehaviour
  {
    [SerializeField]
    private int m_AgentTypeID;
    [SerializeField]
    private CollectObjects m_CollectObjects = CollectObjects.All;
    [SerializeField]
    private Vector3 m_Size = new Vector3(10f, 10f, 10f);
    [SerializeField]
    private Vector3 m_Center = new Vector3(0.0f, 2f, 0.0f);
    [SerializeField]
    private LayerMask m_LayerMask = (LayerMask) -1;
    [SerializeField]
    private NavMeshCollectGeometry m_UseGeometry = NavMeshCollectGeometry.RenderMeshes;
    [SerializeField]
    private int m_DefaultArea;
    [SerializeField]
    private bool m_IgnoreNavMeshAgent = true;
    [SerializeField]
    private bool m_IgnoreNavMeshObstacle = true;
    [SerializeField]
    private bool m_OverrideTileSize;
    [SerializeField]
    private int m_TileSize = 256;
    [SerializeField]
    private bool m_OverrideVoxelSize;
    [SerializeField]
    private float m_VoxelSize;
    [SerializeField]
    private bool m_BuildHeightMesh;
    [FormerlySerializedAs("m_BakedNavMeshData")]
    [SerializeField]
    private NavMeshData m_NavMeshData;
    private NavMeshDataInstance m_NavMeshDataInstance;
    private Vector3 m_LastPosition = Vector3.zero;
    private Quaternion m_LastRotation = Quaternion.identity;
    private static readonly List<NavMeshSurface> s_NavMeshSurfaces = new List<NavMeshSurface>();

    public int agentTypeID
    {
      get => m_AgentTypeID;
      set => m_AgentTypeID = value;
    }

    public CollectObjects collectObjects
    {
      get => m_CollectObjects;
      set => m_CollectObjects = value;
    }

    public Vector3 size
    {
      get => m_Size;
      set => m_Size = value;
    }

    public Vector3 center
    {
      get => m_Center;
      set => m_Center = value;
    }

    public LayerMask layerMask
    {
      get => m_LayerMask;
      set => m_LayerMask = value;
    }

    public NavMeshCollectGeometry useGeometry
    {
      get => m_UseGeometry;
      set => m_UseGeometry = value;
    }

    public int defaultArea
    {
      get => m_DefaultArea;
      set => m_DefaultArea = value;
    }

    public bool ignoreNavMeshAgent
    {
      get => m_IgnoreNavMeshAgent;
      set => m_IgnoreNavMeshAgent = value;
    }

    public bool ignoreNavMeshObstacle
    {
      get => m_IgnoreNavMeshObstacle;
      set => m_IgnoreNavMeshObstacle = value;
    }

    public bool overrideTileSize
    {
      get => m_OverrideTileSize;
      set => m_OverrideTileSize = value;
    }

    public int tileSize
    {
      get => m_TileSize;
      set => m_TileSize = value;
    }

    public bool overrideVoxelSize
    {
      get => m_OverrideVoxelSize;
      set => m_OverrideVoxelSize = value;
    }

    public float voxelSize
    {
      get => m_VoxelSize;
      set => m_VoxelSize = value;
    }

    public bool buildHeightMesh
    {
      get => m_BuildHeightMesh;
      set => m_BuildHeightMesh = value;
    }

    public NavMeshData navMeshData
    {
      get => m_NavMeshData;
      set => m_NavMeshData = value;
    }

    public static List<NavMeshSurface> activeSurfaces => s_NavMeshSurfaces;

    private void OnEnable()
    {
      Register(this);
      AddData();
    }

    private void OnDisable()
    {
      RemoveData();
      Unregister(this);
    }

    public void AddData()
    {
      if (m_NavMeshDataInstance.valid)
        return;
      if ((UnityEngine.Object) m_NavMeshData != (UnityEngine.Object) null)
      {
        m_NavMeshDataInstance = NavMesh.AddNavMeshData(m_NavMeshData, this.transform.position, this.transform.rotation);
        m_NavMeshDataInstance.owner = (UnityEngine.Object) this;
      }
      m_LastPosition = this.transform.position;
      m_LastRotation = this.transform.rotation;
    }

    public void RemoveData()
    {
      m_NavMeshDataInstance.Remove();
      m_NavMeshDataInstance = new NavMeshDataInstance();
    }

    public NavMeshBuildSettings GetBuildSettings()
    {
      NavMeshBuildSettings settingsById = NavMesh.GetSettingsByID(m_AgentTypeID);
      if (settingsById.agentTypeID == -1)
      {
        Debug.LogWarning((object) ("No build settings for agent type ID " + agentTypeID), (UnityEngine.Object) this);
        settingsById.agentTypeID = m_AgentTypeID;
      }
      if (overrideTileSize)
      {
        settingsById.overrideTileSize = true;
        settingsById.tileSize = tileSize;
      }
      if (overrideVoxelSize)
      {
        settingsById.overrideVoxelSize = true;
        settingsById.voxelSize = voxelSize;
      }
      return settingsById;
    }

    public void BuildNavMesh()
    {
      List<NavMeshBuildSource> sources = CollectSources();
      Bounds bounds = new Bounds(m_Center, Abs(m_Size));
      if (m_CollectObjects == CollectObjects.All || m_CollectObjects == CollectObjects.Children)
        bounds = CalculateWorldBounds(sources);
      NavMeshData navMeshData = NavMeshBuilder.BuildNavMeshData(GetBuildSettings(), sources, bounds, this.transform.position, this.transform.rotation);
      if (!((UnityEngine.Object) navMeshData != (UnityEngine.Object) null))
        return;
      navMeshData.name = this.gameObject.name;
      RemoveData();
      m_NavMeshData = navMeshData;
      if (this.isActiveAndEnabled)
        AddData();
    }

    public AsyncOperation UpdateNavMesh(NavMeshData data)
    {
      List<NavMeshBuildSource> sources = CollectSources();
      Bounds bounds = new Bounds(m_Center, Abs(m_Size));
      if (m_CollectObjects == CollectObjects.All || m_CollectObjects == CollectObjects.Children)
        bounds = CalculateWorldBounds(sources);
      return NavMeshBuilder.UpdateNavMeshDataAsync(data, GetBuildSettings(), sources, bounds);
    }

    private static void Register(NavMeshSurface surface)
    {
      if (s_NavMeshSurfaces.Count == 0)
        NavMesh.onPreUpdate += new NavMesh.OnNavMeshPreUpdate(UpdateActive);
      if (s_NavMeshSurfaces.Contains(surface))
        return;
      s_NavMeshSurfaces.Add(surface);
    }

    private static void Unregister(NavMeshSurface surface)
    {
      s_NavMeshSurfaces.Remove(surface);
      if (s_NavMeshSurfaces.Count != 0)
        return;
      NavMesh.onPreUpdate -= new NavMesh.OnNavMeshPreUpdate(UpdateActive);
    }

    private static void UpdateActive()
    {
      for (int index = 0; index < s_NavMeshSurfaces.Count; ++index)
        s_NavMeshSurfaces[index].UpdateDataIfTransformChanged();
    }

    private void AppendModifierVolumes(ref List<NavMeshBuildSource> sources)
    {
      List<NavMeshModifierVolume> meshModifierVolumeList;
      if (m_CollectObjects == CollectObjects.Children)
      {
        meshModifierVolumeList = new List<NavMeshModifierVolume>((IEnumerable<NavMeshModifierVolume>) this.GetComponentsInChildren<NavMeshModifierVolume>());
        meshModifierVolumeList.RemoveAll(x => !x.isActiveAndEnabled);
      }
      else
        meshModifierVolumeList = NavMeshModifierVolume.activeModifiers;
      foreach (NavMeshModifierVolume meshModifierVolume in meshModifierVolumeList)
      {
        if (((int) m_LayerMask & 1 << meshModifierVolume.gameObject.layer) != 0 && meshModifierVolume.AffectsAgentType(m_AgentTypeID))
        {
          Vector3 pos = meshModifierVolume.transform.TransformPoint(meshModifierVolume.center);
          Vector3 lossyScale = meshModifierVolume.transform.lossyScale;
          Vector3 vector3 = new Vector3(meshModifierVolume.size.x * Mathf.Abs(lossyScale.x), meshModifierVolume.size.y * Mathf.Abs(lossyScale.y), meshModifierVolume.size.z * Mathf.Abs(lossyScale.z));
          sources.Add(new NavMeshBuildSource {
            shape = NavMeshBuildSourceShape.ModifierBox,
            transform = Matrix4x4.TRS(pos, meshModifierVolume.transform.rotation, Vector3.one),
            size = vector3,
            area = meshModifierVolume.area
          });
        }
      }
    }

    private List<NavMeshBuildSource> CollectSources()
    {
      List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();
      List<NavMeshBuildMarkup> navMeshBuildMarkupList = new List<NavMeshBuildMarkup>();
      List<NavMeshModifier> navMeshModifierList;
      if (m_CollectObjects == CollectObjects.Children)
      {
        navMeshModifierList = new List<NavMeshModifier>((IEnumerable<NavMeshModifier>) this.GetComponentsInChildren<NavMeshModifier>());
        navMeshModifierList.RemoveAll(x => !x.isActiveAndEnabled);
      }
      else
        navMeshModifierList = NavMeshModifier.activeModifiers;
      foreach (NavMeshModifier navMeshModifier in navMeshModifierList)
      {
        if (((int) m_LayerMask & 1 << navMeshModifier.gameObject.layer) != 0 && navMeshModifier.AffectsAgentType(m_AgentTypeID))
          navMeshBuildMarkupList.Add(new NavMeshBuildMarkup {
            root = navMeshModifier.transform,
            overrideArea = navMeshModifier.overrideArea,
            area = navMeshModifier.area,
            ignoreFromBuild = navMeshModifier.ignoreFromBuild
          });
      }
      if (m_CollectObjects == CollectObjects.All)
        NavMeshBuilder.CollectSources((Transform) null, (int) m_LayerMask, m_UseGeometry, m_DefaultArea, navMeshBuildMarkupList, sources);
      else if (m_CollectObjects == CollectObjects.Children)
        NavMeshBuilder.CollectSources(this.transform, (int) m_LayerMask, m_UseGeometry, m_DefaultArea, navMeshBuildMarkupList, sources);
      else if (m_CollectObjects == CollectObjects.Volume)
        NavMeshBuilder.CollectSources(GetWorldBounds(Matrix4x4.TRS(this.transform.position, this.transform.rotation, Vector3.one), new Bounds(m_Center, m_Size)), (int) m_LayerMask, m_UseGeometry, m_DefaultArea, navMeshBuildMarkupList, sources);
      if (m_IgnoreNavMeshAgent)
        sources.RemoveAll((Predicate<NavMeshBuildSource>) (x => (UnityEngine.Object) x.component != (UnityEngine.Object) null && (UnityEngine.Object) x.component.gameObject.GetComponent<NavMeshAgent>() != (UnityEngine.Object) null));
      if (m_IgnoreNavMeshObstacle)
        sources.RemoveAll((Predicate<NavMeshBuildSource>) (x => (UnityEngine.Object) x.component != (UnityEngine.Object) null && (UnityEngine.Object) x.component.gameObject.GetComponent<NavMeshObstacle>() != (UnityEngine.Object) null));
      AppendModifierVolumes(ref sources);
      return sources;
    }

    private static Vector3 Abs(Vector3 v)
    {
      return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
    }

    private static Bounds GetWorldBounds(Matrix4x4 mat, Bounds bounds)
    {
      Vector3 vector3_1 = Abs(mat.MultiplyVector(Vector3.right));
      Vector3 vector3_2 = Abs(mat.MultiplyVector(Vector3.up));
      Vector3 vector3_3 = Abs(mat.MultiplyVector(Vector3.forward));
      return new Bounds(mat.MultiplyPoint(bounds.center), vector3_1 * bounds.size.x + vector3_2 * bounds.size.y + vector3_3 * bounds.size.z);
    }

    private Bounds CalculateWorldBounds(List<NavMeshBuildSource> sources)
    {
      Matrix4x4 matrix4x4 = Matrix4x4.TRS(this.transform.position, this.transform.rotation, Vector3.one);
      matrix4x4 = matrix4x4.inverse;
      Bounds worldBounds = new Bounds();
      foreach (NavMeshBuildSource source in sources)
      {
        switch (source.shape)
        {
          case NavMeshBuildSourceShape.Mesh:
            Mesh sourceObject1 = source.sourceObject as Mesh;
            worldBounds.Encapsulate(GetWorldBounds(matrix4x4 * source.transform, sourceObject1.bounds));
            break;
          case NavMeshBuildSourceShape.Terrain:
            TerrainData sourceObject2 = source.sourceObject as TerrainData;
            worldBounds.Encapsulate(GetWorldBounds(matrix4x4 * source.transform, new Bounds(0.5f * sourceObject2.size, sourceObject2.size)));
            break;
          case NavMeshBuildSourceShape.Box:
          case NavMeshBuildSourceShape.Sphere:
          case NavMeshBuildSourceShape.Capsule:
          case NavMeshBuildSourceShape.ModifierBox:
            worldBounds.Encapsulate(GetWorldBounds(matrix4x4 * source.transform, new Bounds(Vector3.zero, source.size)));
            break;
        }
      }
      worldBounds.Expand(0.1f);
      return worldBounds;
    }

    private bool HasTransformChanged()
    {
      return m_LastPosition != this.transform.position || m_LastRotation != this.transform.rotation;
    }

    private void UpdateDataIfTransformChanged()
    {
      if (!HasTransformChanged())
        return;
      RemoveData();
      AddData();
    }
  }
}
