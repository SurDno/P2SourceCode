// Decompiled with JetBrains decompiler
// Type: UnityEngine.AI.NavMeshSurface
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

#nullable disable
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
      get => this.m_AgentTypeID;
      set => this.m_AgentTypeID = value;
    }

    public CollectObjects collectObjects
    {
      get => this.m_CollectObjects;
      set => this.m_CollectObjects = value;
    }

    public Vector3 size
    {
      get => this.m_Size;
      set => this.m_Size = value;
    }

    public Vector3 center
    {
      get => this.m_Center;
      set => this.m_Center = value;
    }

    public LayerMask layerMask
    {
      get => this.m_LayerMask;
      set => this.m_LayerMask = value;
    }

    public NavMeshCollectGeometry useGeometry
    {
      get => this.m_UseGeometry;
      set => this.m_UseGeometry = value;
    }

    public int defaultArea
    {
      get => this.m_DefaultArea;
      set => this.m_DefaultArea = value;
    }

    public bool ignoreNavMeshAgent
    {
      get => this.m_IgnoreNavMeshAgent;
      set => this.m_IgnoreNavMeshAgent = value;
    }

    public bool ignoreNavMeshObstacle
    {
      get => this.m_IgnoreNavMeshObstacle;
      set => this.m_IgnoreNavMeshObstacle = value;
    }

    public bool overrideTileSize
    {
      get => this.m_OverrideTileSize;
      set => this.m_OverrideTileSize = value;
    }

    public int tileSize
    {
      get => this.m_TileSize;
      set => this.m_TileSize = value;
    }

    public bool overrideVoxelSize
    {
      get => this.m_OverrideVoxelSize;
      set => this.m_OverrideVoxelSize = value;
    }

    public float voxelSize
    {
      get => this.m_VoxelSize;
      set => this.m_VoxelSize = value;
    }

    public bool buildHeightMesh
    {
      get => this.m_BuildHeightMesh;
      set => this.m_BuildHeightMesh = value;
    }

    public NavMeshData navMeshData
    {
      get => this.m_NavMeshData;
      set => this.m_NavMeshData = value;
    }

    public static List<NavMeshSurface> activeSurfaces => NavMeshSurface.s_NavMeshSurfaces;

    private void OnEnable()
    {
      NavMeshSurface.Register(this);
      this.AddData();
    }

    private void OnDisable()
    {
      this.RemoveData();
      NavMeshSurface.Unregister(this);
    }

    public void AddData()
    {
      if (this.m_NavMeshDataInstance.valid)
        return;
      if ((UnityEngine.Object) this.m_NavMeshData != (UnityEngine.Object) null)
      {
        this.m_NavMeshDataInstance = NavMesh.AddNavMeshData(this.m_NavMeshData, this.transform.position, this.transform.rotation);
        this.m_NavMeshDataInstance.owner = (UnityEngine.Object) this;
      }
      this.m_LastPosition = this.transform.position;
      this.m_LastRotation = this.transform.rotation;
    }

    public void RemoveData()
    {
      this.m_NavMeshDataInstance.Remove();
      this.m_NavMeshDataInstance = new NavMeshDataInstance();
    }

    public NavMeshBuildSettings GetBuildSettings()
    {
      NavMeshBuildSettings settingsById = NavMesh.GetSettingsByID(this.m_AgentTypeID);
      if (settingsById.agentTypeID == -1)
      {
        Debug.LogWarning((object) ("No build settings for agent type ID " + (object) this.agentTypeID), (UnityEngine.Object) this);
        settingsById.agentTypeID = this.m_AgentTypeID;
      }
      if (this.overrideTileSize)
      {
        settingsById.overrideTileSize = true;
        settingsById.tileSize = this.tileSize;
      }
      if (this.overrideVoxelSize)
      {
        settingsById.overrideVoxelSize = true;
        settingsById.voxelSize = this.voxelSize;
      }
      return settingsById;
    }

    public void BuildNavMesh()
    {
      List<NavMeshBuildSource> sources = this.CollectSources();
      Bounds bounds = new Bounds(this.m_Center, NavMeshSurface.Abs(this.m_Size));
      if (this.m_CollectObjects == CollectObjects.All || this.m_CollectObjects == CollectObjects.Children)
        bounds = this.CalculateWorldBounds(sources);
      NavMeshData navMeshData = NavMeshBuilder.BuildNavMeshData(this.GetBuildSettings(), sources, bounds, this.transform.position, this.transform.rotation);
      if (!((UnityEngine.Object) navMeshData != (UnityEngine.Object) null))
        return;
      navMeshData.name = this.gameObject.name;
      this.RemoveData();
      this.m_NavMeshData = navMeshData;
      if (this.isActiveAndEnabled)
        this.AddData();
    }

    public AsyncOperation UpdateNavMesh(NavMeshData data)
    {
      List<NavMeshBuildSource> sources = this.CollectSources();
      Bounds bounds = new Bounds(this.m_Center, NavMeshSurface.Abs(this.m_Size));
      if (this.m_CollectObjects == CollectObjects.All || this.m_CollectObjects == CollectObjects.Children)
        bounds = this.CalculateWorldBounds(sources);
      return NavMeshBuilder.UpdateNavMeshDataAsync(data, this.GetBuildSettings(), sources, bounds);
    }

    private static void Register(NavMeshSurface surface)
    {
      if (NavMeshSurface.s_NavMeshSurfaces.Count == 0)
        NavMesh.onPreUpdate += new NavMesh.OnNavMeshPreUpdate(NavMeshSurface.UpdateActive);
      if (NavMeshSurface.s_NavMeshSurfaces.Contains(surface))
        return;
      NavMeshSurface.s_NavMeshSurfaces.Add(surface);
    }

    private static void Unregister(NavMeshSurface surface)
    {
      NavMeshSurface.s_NavMeshSurfaces.Remove(surface);
      if (NavMeshSurface.s_NavMeshSurfaces.Count != 0)
        return;
      NavMesh.onPreUpdate -= new NavMesh.OnNavMeshPreUpdate(NavMeshSurface.UpdateActive);
    }

    private static void UpdateActive()
    {
      for (int index = 0; index < NavMeshSurface.s_NavMeshSurfaces.Count; ++index)
        NavMeshSurface.s_NavMeshSurfaces[index].UpdateDataIfTransformChanged();
    }

    private void AppendModifierVolumes(ref List<NavMeshBuildSource> sources)
    {
      List<NavMeshModifierVolume> meshModifierVolumeList;
      if (this.m_CollectObjects == CollectObjects.Children)
      {
        meshModifierVolumeList = new List<NavMeshModifierVolume>((IEnumerable<NavMeshModifierVolume>) this.GetComponentsInChildren<NavMeshModifierVolume>());
        meshModifierVolumeList.RemoveAll((Predicate<NavMeshModifierVolume>) (x => !x.isActiveAndEnabled));
      }
      else
        meshModifierVolumeList = NavMeshModifierVolume.activeModifiers;
      foreach (NavMeshModifierVolume meshModifierVolume in meshModifierVolumeList)
      {
        if (((int) this.m_LayerMask & 1 << meshModifierVolume.gameObject.layer) != 0 && meshModifierVolume.AffectsAgentType(this.m_AgentTypeID))
        {
          Vector3 pos = meshModifierVolume.transform.TransformPoint(meshModifierVolume.center);
          Vector3 lossyScale = meshModifierVolume.transform.lossyScale;
          Vector3 vector3 = new Vector3(meshModifierVolume.size.x * Mathf.Abs(lossyScale.x), meshModifierVolume.size.y * Mathf.Abs(lossyScale.y), meshModifierVolume.size.z * Mathf.Abs(lossyScale.z));
          sources.Add(new NavMeshBuildSource()
          {
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
      if (this.m_CollectObjects == CollectObjects.Children)
      {
        navMeshModifierList = new List<NavMeshModifier>((IEnumerable<NavMeshModifier>) this.GetComponentsInChildren<NavMeshModifier>());
        navMeshModifierList.RemoveAll((Predicate<NavMeshModifier>) (x => !x.isActiveAndEnabled));
      }
      else
        navMeshModifierList = NavMeshModifier.activeModifiers;
      foreach (NavMeshModifier navMeshModifier in navMeshModifierList)
      {
        if (((int) this.m_LayerMask & 1 << navMeshModifier.gameObject.layer) != 0 && navMeshModifier.AffectsAgentType(this.m_AgentTypeID))
          navMeshBuildMarkupList.Add(new NavMeshBuildMarkup()
          {
            root = navMeshModifier.transform,
            overrideArea = navMeshModifier.overrideArea,
            area = navMeshModifier.area,
            ignoreFromBuild = navMeshModifier.ignoreFromBuild
          });
      }
      if (this.m_CollectObjects == CollectObjects.All)
        NavMeshBuilder.CollectSources((Transform) null, (int) this.m_LayerMask, this.m_UseGeometry, this.m_DefaultArea, navMeshBuildMarkupList, sources);
      else if (this.m_CollectObjects == CollectObjects.Children)
        NavMeshBuilder.CollectSources(this.transform, (int) this.m_LayerMask, this.m_UseGeometry, this.m_DefaultArea, navMeshBuildMarkupList, sources);
      else if (this.m_CollectObjects == CollectObjects.Volume)
        NavMeshBuilder.CollectSources(NavMeshSurface.GetWorldBounds(Matrix4x4.TRS(this.transform.position, this.transform.rotation, Vector3.one), new Bounds(this.m_Center, this.m_Size)), (int) this.m_LayerMask, this.m_UseGeometry, this.m_DefaultArea, navMeshBuildMarkupList, sources);
      if (this.m_IgnoreNavMeshAgent)
        sources.RemoveAll((Predicate<NavMeshBuildSource>) (x => (UnityEngine.Object) x.component != (UnityEngine.Object) null && (UnityEngine.Object) x.component.gameObject.GetComponent<NavMeshAgent>() != (UnityEngine.Object) null));
      if (this.m_IgnoreNavMeshObstacle)
        sources.RemoveAll((Predicate<NavMeshBuildSource>) (x => (UnityEngine.Object) x.component != (UnityEngine.Object) null && (UnityEngine.Object) x.component.gameObject.GetComponent<NavMeshObstacle>() != (UnityEngine.Object) null));
      this.AppendModifierVolumes(ref sources);
      return sources;
    }

    private static Vector3 Abs(Vector3 v)
    {
      return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
    }

    private static Bounds GetWorldBounds(Matrix4x4 mat, Bounds bounds)
    {
      Vector3 vector3_1 = NavMeshSurface.Abs(mat.MultiplyVector(Vector3.right));
      Vector3 vector3_2 = NavMeshSurface.Abs(mat.MultiplyVector(Vector3.up));
      Vector3 vector3_3 = NavMeshSurface.Abs(mat.MultiplyVector(Vector3.forward));
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
            worldBounds.Encapsulate(NavMeshSurface.GetWorldBounds(matrix4x4 * source.transform, sourceObject1.bounds));
            break;
          case NavMeshBuildSourceShape.Terrain:
            TerrainData sourceObject2 = source.sourceObject as TerrainData;
            worldBounds.Encapsulate(NavMeshSurface.GetWorldBounds(matrix4x4 * source.transform, new Bounds(0.5f * sourceObject2.size, sourceObject2.size)));
            break;
          case NavMeshBuildSourceShape.Box:
          case NavMeshBuildSourceShape.Sphere:
          case NavMeshBuildSourceShape.Capsule:
          case NavMeshBuildSourceShape.ModifierBox:
            worldBounds.Encapsulate(NavMeshSurface.GetWorldBounds(matrix4x4 * source.transform, new Bounds(Vector3.zero, source.size)));
            break;
        }
      }
      worldBounds.Expand(0.1f);
      return worldBounds;
    }

    private bool HasTransformChanged()
    {
      return this.m_LastPosition != this.transform.position || this.m_LastRotation != this.transform.rotation;
    }

    private void UpdateDataIfTransformChanged()
    {
      if (!this.HasTransformChanged())
        return;
      this.RemoveData();
      this.AddData();
    }
  }
}
