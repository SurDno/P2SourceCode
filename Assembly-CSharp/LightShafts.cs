using Engine.Common;
using Engine.Source.Commons;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof (MeshFilter), typeof (ParticleSystem))]
public class LightShafts : MonoBehaviour, IUpdatable
{
  public static bool isLightActive = false;
  private static Vector3 _lightDirection = Vector3.forward;
  public static LayerMask shadowCastingColliders;
  public static bool isPlayerSet = false;
  public static Vector3 playerPosition = Vector3.zero;
  public static float nearDistance = 5f;
  public static float farDistance = 100f;
  public static float nearUpdateTime = 0.5f;
  private static MaterialPropertyBlock _materialProperties;
  private static int _opacityShaderProperty;
  private static int thisFrameIndex = -1;
  private static int thisFrameRebuilds;
  public float brightness = 0.25f;
  public float fadeIn = 0.5f;
  [Header("Raycasts")]
  public float indoorBias = 0.5f;
  public float length = 5f;
  public float outdoorBias = 0.5f;
  public float outdoorDistance = 50f;
  [Space]
  public float particlesPerUnit = 1f;
  [Space]
  public Vector3[] points = new Vector3[0];
  [Header("Rays")]
  public float radius = 0.1f;
  private GameObject _child;
  private MeshFilter _childMeshFilter;
  private MeshRenderer _childMeshRenderer;
  private MeshFilter _meshFilter;
  private MeshRenderer _meshRenderer;
  private ParticleSystem _particleSystem;
  private float _lastUpdateTime;
  private float _phase;

  public static Vector3 LightDirection
  {
    get => LightShafts._lightDirection;
    set => LightShafts._lightDirection = value.normalized;
  }

  private MeshFilter meshFilter
  {
    get
    {
      if ((UnityEngine.Object) this._meshFilter == (UnityEngine.Object) null)
        this._meshFilter = this.GetComponent<MeshFilter>();
      return this._meshFilter;
    }
  }

  private MeshRenderer meshRenderer
  {
    get
    {
      if ((UnityEngine.Object) this._meshRenderer == (UnityEngine.Object) null)
        this._meshRenderer = this.GetComponent<MeshRenderer>();
      return this._meshRenderer;
    }
  }

  private ParticleSystem particleSystem
  {
    get
    {
      if ((UnityEngine.Object) this._particleSystem == (UnityEngine.Object) null)
        this._particleSystem = this.GetComponent<ParticleSystem>();
      return this._particleSystem;
    }
  }

  private void CheckChild()
  {
    if (!((UnityEngine.Object) this._child == (UnityEngine.Object) null))
      return;
    this._child = new GameObject("Light Shaft Fade", new System.Type[2]
    {
      typeof (MeshFilter),
      typeof (MeshRenderer)
    });
    this._child.layer = this.gameObject.layer;
    this._child.tag = this.gameObject.tag;
    this._child.transform.SetParent(this.transform, false);
    this._child.transform.localPosition = Vector3.zero;
    this._child.transform.localRotation = Quaternion.identity;
    this._child.transform.localScale = Vector3.one;
    this._child.hideFlags = HideFlags.DontSave;
    this._childMeshRenderer = this._child.GetComponent<MeshRenderer>();
    this._childMeshRenderer.sharedMaterial = this.meshRenderer.sharedMaterial;
    this._childMeshFilter = this._child.GetComponent<MeshFilter>();
  }

  private void NullChild()
  {
    if (!((UnityEngine.Object) this._child != (UnityEngine.Object) null))
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) this._child);
    this._child = (GameObject) null;
    this._childMeshFilter = (MeshFilter) null;
    this._childMeshRenderer = (MeshRenderer) null;
  }

  private void CheckMesh()
  {
    if ((UnityEngine.Object) this.meshFilter.sharedMesh != (UnityEngine.Object) null)
      return;
    Mesh mesh = new Mesh();
    mesh.MarkDynamic();
    mesh.hideFlags = HideFlags.DontSave;
    this.meshFilter.sharedMesh = mesh;
    ParticleSystem.ShapeModule shape = this.particleSystem.shape with
    {
      shapeType = ParticleSystemShapeType.Mesh,
      mesh = mesh
    };
    this.particleSystem.Play();
    this.meshRenderer.enabled = true;
  }

  public void NullMesh()
  {
    if ((UnityEngine.Object) this.meshFilter.sharedMesh != (UnityEngine.Object) null)
    {
      UnityEngine.Object.Destroy((UnityEngine.Object) this.meshFilter.sharedMesh);
      this.meshFilter.sharedMesh = (Mesh) null;
    }
    this.particleSystem.Stop();
    ParticleSystem.ShapeModule shape = this.particleSystem.shape with
    {
      shapeType = ParticleSystemShapeType.Sphere,
      mesh = (Mesh) null
    };
    this.meshRenderer.enabled = false;
  }

  private void Reset()
  {
    this.NullChild();
    this.NullMesh();
    this._phase = 1f;
  }

  private void OnEnable()
  {
    LightShaftsBuilder.Occupy();
    InstanceByRequest<UpdateService>.Instance.LightShaftsUpdater.AddUpdatable((IUpdatable) this);
    this.Reset();
  }

  private void OnDisable()
  {
    InstanceByRequest<UpdateService>.Instance.LightShaftsUpdater.RemoveUpdatable((IUpdatable) this);
    LightShaftsBuilder.Vacate();
    this.Reset();
  }

  public void ComputeUpdate()
  {
    if (!LightShafts.isLightActive)
    {
      this.Reset();
    }
    else
    {
      if ((double) LightShafts.nearUpdateTime <= 0.0)
      {
        ++this._phase;
      }
      else
      {
        float num1 = 1f / LightShafts.nearUpdateTime;
        if (LightShafts.isPlayerSet)
        {
          float num2 = Vector3.Distance(this.transform.position, LightShafts.playerPosition);
          if ((double) num2 >= (double) LightShafts.farDistance)
            num1 = 0.0f;
          else if ((double) num2 > (double) LightShafts.nearDistance)
            num1 *= (float) (((double) LightShafts.farDistance - (double) num2) / ((double) LightShafts.farDistance - (double) LightShafts.nearDistance));
        }
        this._phase += num1 * (Time.unscaledTime - this._lastUpdateTime);
      }
      if ((double) this._phase >= 1.0)
      {
        if (LightShafts.thisFrameIndex != Time.frameCount)
        {
          LightShafts.thisFrameIndex = Time.frameCount;
          LightShafts.thisFrameRebuilds = 0;
        }
        if (LightShafts.thisFrameRebuilds < 4 || (double) this._phase >= 2.0)
        {
          if ((UnityEngine.Object) this.meshFilter.sharedMesh == (UnityEngine.Object) null)
          {
            this.NullChild();
          }
          else
          {
            this.CheckChild();
            Mesh sharedMesh = this._childMeshFilter.sharedMesh;
            this._childMeshFilter.sharedMesh = this.meshFilter.sharedMesh;
            this.meshFilter.sharedMesh = sharedMesh;
          }
          this.UpdateMesh();
          this._phase = 0.0f;
          ++LightShafts.thisFrameRebuilds;
        }
      }
      if ((UnityEngine.Object) this.meshFilter.sharedMesh != (UnityEngine.Object) null)
        this.UpdateRenderer(this.meshRenderer, this._phase);
      if ((UnityEngine.Object) this._childMeshRenderer != (UnityEngine.Object) null)
        this.UpdateRenderer(this._childMeshRenderer, 1f - this._phase);
    }
    this._lastUpdateTime = Time.unscaledTime;
  }

  private void UpdateRenderer(MeshRenderer renderer, float phase)
  {
    if (LightShafts._materialProperties == null)
    {
      LightShafts._materialProperties = new MaterialPropertyBlock();
      LightShafts._opacityShaderProperty = Shader.PropertyToID("_Opacity");
    }
    phase = Mathf.Clamp01(phase);
    LightShafts._materialProperties.SetFloat(LightShafts._opacityShaderProperty, phase * this.brightness);
    renderer.SetPropertyBlock(LightShafts._materialProperties);
  }

  private void UpdateMesh()
  {
    if (!LightShafts.isLightActive || (double) this.length <= 0.0)
    {
      this.NullMesh();
    }
    else
    {
      float opacity = Mathf.Clamp01(Vector3.Dot(LightShafts.LightDirection, this.transform.forward));
      if ((double) opacity <= 0.0)
      {
        this.NullMesh();
      }
      else
      {
        LightShaftsBuilder instance = LightShaftsBuilder.Instance;
        instance.Prepare(this, opacity, this.points.Length);
        Matrix4x4 localToWorldMatrix = this.transform.localToWorldMatrix;
        bool flag = false;
        float num = 0.0f;
        for (int index = 0; index < this.points.Length; ++index)
        {
          Vector3 origin = localToWorldMatrix.MultiplyPoint(this.points[index]);
          if ((double) this.outdoorDistance > (double) this.outdoorBias && Physics.Raycast(origin - LightShafts._lightDirection * this.outdoorBias, -LightShafts._lightDirection, this.outdoorDistance - this.outdoorBias, (int) LightShafts.shadowCastingColliders, QueryTriggerInteraction.Ignore))
          {
            float length = 0.0f;
            instance.AddRay(origin, length);
          }
          else
          {
            RaycastHit hitInfo;
            float length1;
            if ((double) this.length > (double) this.indoorBias && Physics.Raycast(origin + LightShafts._lightDirection * this.indoorBias, LightShafts._lightDirection, out hitInfo, this.length - this.indoorBias, (int) LightShafts.shadowCastingColliders, QueryTriggerInteraction.Ignore))
            {
              length1 = hitInfo.distance + this.indoorBias + this.radius;
              if ((double) length1 > (double) this.length)
                length1 = this.length;
            }
            else
              length1 = this.length;
            if ((double) length1 <= (double) this.radius)
            {
              float length2 = 0.0f;
              instance.AddRay(origin, length2);
            }
            else
            {
              instance.AddRay(origin, length1);
              num += length1;
              flag = true;
            }
          }
        }
        if (!flag)
        {
          this.NullMesh();
        }
        else
        {
          this.CheckMesh();
          instance.BuildTo(this.meshFilter.sharedMesh);
          this.particleSystem.emission.rateOverTime = new ParticleSystem.MinMaxCurve(num * this.particlesPerUnit);
          ParticleSystem.ShapeModule shape = this.particleSystem.shape with
          {
            shapeType = ParticleSystemShapeType.Mesh,
            mesh = this.meshFilter.sharedMesh
          };
        }
      }
    }
  }
}
