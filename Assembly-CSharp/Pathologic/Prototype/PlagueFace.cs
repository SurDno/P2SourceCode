// Decompiled with JetBrains decompiler
// Type: Pathologic.Prototype.PlagueFace
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Components;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Inspectors;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Pathologic.Prototype
{
  public class PlagueFace : MonoBehaviour, IEntityAttachable
  {
    private IParameter<float> DamageParameter;
    private bool _isInitialized;
    private bool initedRecently = false;
    private PlagueFacePoint startingPoint;
    private float _lastAttackValue;
    private Vector3 _lastPlayerPos;
    private float _tilleNextPrototypeMessage;
    public float aggessionDecrease = 0.05f;
    [Range(0.0f, 1f)]
    public float aggresion;
    [Range(0.0f, 1f)]
    public float attack;
    public float attackDecreaseRate = 1f;
    public float attackIncreaseRate = 1f;
    public float attackMessageFrequency = 2f;
    [Header("Prototype")]
    public float attackThreshold = 0.1f;
    public float eyeExtent = 0.1f;
    public float eyeSize = 0.3f;
    [Header("Hearing")]
    public float farHearingRadius = 10f;
    [Space]
    public PlagueFace.Graphic graphic;
    public float maxPlayerVelocity = 1f;
    public PlagueFace.Navigation navigation;
    public float nearHearingRadius = 5f;
    public float noiseAggessionIncrease = 0.2f;
    [Header("State")]
    public Vector3 playerPosition;
    public float playerSize = 0.3f;
    public LayerMask sightObstacles;
    [Header("Sight and Attack")]
    public float sightRadius = 10f;
    public int sightRaysPerTick = 5;
    public PlagueFace.Sound sound;
    [Header("Init")]
    public GameObject StartingPoint;
    private bool canHearPlayer;

    [Inspected]
    public IEntity Owner { get; private set; }

    void IEntityAttachable.Attach(IEntity owner)
    {
      this.Owner = owner;
      NavigationComponent component1 = this.Owner.GetComponent<NavigationComponent>();
      if (component1 != null)
      {
        this.NavigationComponent_OnTeleport((INavigationComponent) component1, this.Owner);
        component1.OnTeleport += new Action<INavigationComponent, IEntity>(this.NavigationComponent_OnTeleport);
      }
      if (!this._isInitialized)
      {
        CrowdItemComponent component2 = this.Owner.GetComponent<CrowdItemComponent>();
        if (component2 != null)
          this.InitCrowdItem(component2);
      }
      ParametersComponent component3 = this.Owner.GetComponent<ParametersComponent>();
      if (component3 == null)
        return;
      this.DamageParameter = component3.GetByName<float>(ParameterNameEnum.InfectionDamage);
    }

    void IEntityAttachable.Detach()
    {
      NavigationComponent component = this.Owner.GetComponent<NavigationComponent>();
      if (component != null)
        component.OnTeleport -= new Action<INavigationComponent, IEntity>(this.NavigationComponent_OnTeleport);
      this.Owner = (IEntity) null;
      this.DamageParameter = (IParameter<float>) null;
    }

    private void NavigationComponent_OnTeleport(
      INavigationComponent navigationComponent,
      IEntity entity)
    {
      if (navigationComponent == null)
        return;
      IEntity setupPoint = ((NavigationComponent) navigationComponent).SetupPoint;
      if (setupPoint == null)
        return;
      this.GetPathFromGameObject(((IEntityView) setupPoint).GameObject);
    }

    private void InitCrowdItem(CrowdItemComponent crowdItem)
    {
      GameObject setupPointGo = (GameObject) null;
      if (crowdItem != null && crowdItem.Point != null)
        setupPointGo = crowdItem.Point.GameObject;
      this.GetPathFromGameObject(setupPointGo);
    }

    private void GetPathFromGameObject(GameObject setupPointGo)
    {
      if ((UnityEngine.Object) setupPointGo == (UnityEngine.Object) null)
        return;
      PlagueFacePoint componentInChildren = setupPointGo.GetComponentInChildren<PlagueFacePoint>();
      if ((UnityEngine.Object) componentInChildren == (UnityEngine.Object) null)
        return;
      this.StartingPoint = componentInChildren.gameObject;
      if ((UnityEngine.Object) this.StartingPoint == (UnityEngine.Object) null)
        return;
      this.InitializeAt(this.StartingPoint.GetComponent<PlagueFacePoint>());
    }

    private static float VisibilityWeight(Transform point, Vector3 playerPosition)
    {
      Vector3 rhs = playerPosition - point.position;
      if ((double) Vector3.Dot(point.forward, rhs) <= 0.0)
        return 0.0f;
      float magnitude = rhs.magnitude;
      return (double) magnitude == 0.0 ? float.PositiveInfinity : 1f / magnitude;
    }

    private void Awake()
    {
    }

    public void InitializeAt(PlagueFacePoint startingPoint)
    {
      if ((UnityEngine.Object) startingPoint == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) "InitializeAt(PlagueFacePoint) doesn't accept null.");
      }
      else
      {
        this.startingPoint = startingPoint;
        this.transform.SetParent(startingPoint.transform, false);
        this.transform.position = startingPoint.transform.position;
        this.transform.rotation = startingPoint.transform.rotation;
        this.transform.localScale = Vector3.one;
        this.aggresion = 0.0f;
        this.attack = 0.0f;
        this.navigation.Initialize(startingPoint);
        this.sound.Initialize();
        this.graphic.Initialize();
        this._tilleNextPrototypeMessage = 1f / this.attackMessageFrequency;
        this._isInitialized = true;
        this.enabled = true;
        this.initedRecently = true;
      }
    }

    public void FixedUpdate()
    {
      if (!this._isInitialized)
      {
        Debug.LogWarning((object) "PlagueFace must not be enabled until InitializeAt(PlagueFacePoint) is called");
        this.enabled = false;
      }
      else
      {
        if (this.initedRecently && (UnityEngine.Object) this.startingPoint != (UnityEngine.Object) null)
        {
          this.transform.position = this.startingPoint.transform.position;
          this.transform.rotation = this.startingPoint.transform.rotation;
          this.initedRecently = false;
        }
        IEntity player = ServiceLocator.GetService<ISimulation>().Player;
        if (player == null)
          return;
        GameObject gameObject = ((IEntityView) player).GameObject;
        if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
          return;
        Pivot component1 = gameObject.GetComponent<Pivot>();
        DetectorComponent component2 = (DetectorComponent) this.Owner.GetComponent<IDetectorComponent>();
        DetectableComponent component3 = player.GetComponent<DetectableComponent>();
        if (component2 != null && component3 != null)
          this.canHearPlayer = component2.Hearing.Contains((IDetectableComponent) component3);
        this.playerPosition = (UnityEngine.Object) component1 == (UnityEngine.Object) null ? gameObject.transform.position : component1.Chest.transform.position;
        this._tilleNextPrototypeMessage -= Time.fixedDeltaTime;
        if ((double) this._tilleNextPrototypeMessage < 0.0)
        {
          this._tilleNextPrototypeMessage += 1f / this.attackMessageFrequency;
          if ((double) this.attack > (double) this.attackThreshold)
            this._lastAttackValue = this.attack;
          else if ((double) this._lastAttackValue != 0.0)
            this._lastAttackValue = 0.0f;
        }
        bool isVisible = this.navigation.IsVisible(this.transform, this.playerPosition, false);
        bool wasMoved = !isVisible && this.navigation.TryMove(this.transform, this.playerPosition);
        this.PercieveAndAttack(isVisible);
        this.graphic.Update(Time.fixedDeltaTime, isVisible, wasMoved);
        this.sound.Update(this, Time.fixedDeltaTime);
        this.navigation.Navigate(this, this.aggresion);
      }
    }

    private void OnDisable() => this.navigation.Disable();

    private void OnValidate()
    {
      if ((double) this.nearHearingRadius < 0.0)
        this.nearHearingRadius = 0.0f;
      if ((double) this.farHearingRadius <= (double) this.nearHearingRadius)
      {
        this.farHearingRadius = (float) (((double) this.nearHearingRadius + (double) this.farHearingRadius) * 0.5 + 1.0 / 1000.0);
        this.nearHearingRadius = this.farHearingRadius - 1f / 500f;
      }
      if ((double) this.maxPlayerVelocity < 0.0)
        this.maxPlayerVelocity = 0.0f;
      if ((double) this.aggessionDecrease < 0.0)
        this.aggessionDecrease = 0.0f;
      if ((double) this.noiseAggessionIncrease < 0.0)
        this.noiseAggessionIncrease = 0.0f;
      if ((double) this.sightRadius < 0.0)
        this.sightRadius = 0.0f;
      if ((double) this.playerSize < 0.0)
        this.playerSize = 0.0f;
      if ((double) this.eyeExtent < 0.0)
        this.eyeExtent = 0.0f;
      if ((double) this.eyeSize < 0.0)
        this.eyeSize = 0.0f;
      if (this.sightRaysPerTick < 0)
        this.sightRaysPerTick = 0;
      if ((double) this.attackIncreaseRate < 0.0)
        this.attackIncreaseRate = 0.0f;
      if ((double) this.attackDecreaseRate < 0.0)
        this.attackDecreaseRate = 0.0f;
      this.navigation.Validate();
    }

    private void PercieveAndAttack(bool isVisible)
    {
      this.aggresion -= Time.fixedDeltaTime * this.aggessionDecrease;
      this.attack -= Time.fixedDeltaTime * this.attackDecreaseRate;
      Vector3 rhs = this.playerPosition - this.transform.position;
      float magnitude = rhs.magnitude;
      if (this.canHearPlayer)
        this.aggresion += (this.noiseAggessionIncrease + this.aggessionDecrease) * Time.fixedDeltaTime * Mathf.Min(Vector3.Distance(this.playerPosition, this._lastPlayerPos) / Time.fixedDeltaTime, this.maxPlayerVelocity) * Mathf.Min((float) (((double) this.farHearingRadius - (double) magnitude) / ((double) this.farHearingRadius - (double) this.nearHearingRadius)), 1f);
      this._lastPlayerPos = this.playerPosition;
      if ((double) magnitude < (double) this.sightRadius && (double) Vector3.Dot(this.transform.forward, rhs) > 0.0)
      {
        int num1 = 0;
        for (int index = 0; index < this.sightRaysPerTick; ++index)
        {
          Vector2 vector2 = UnityEngine.Random.insideUnitCircle * this.eyeSize;
          Vector3 vector3 = this.transform.localToWorldMatrix.MultiplyPoint(new Vector3(vector2.x, vector2.y, this.eyeExtent));
          Vector3 b = this.playerPosition + UnityEngine.Random.insideUnitSphere * this.playerSize;
          if (!Physics.Raycast(vector3, b - vector3, out RaycastHit _, Vector3.Distance(vector3, b), (int) this.sightObstacles))
            ++num1;
        }
        float num2 = (float) num1 / (float) this.sightRaysPerTick;
        if (num1 > 0)
        {
          this.aggresion += num2 * Time.fixedDeltaTime;
          if (isVisible)
            num2 = 0.0f;
          if ((double) num2 > 0.5)
            this.attack += (this.attackDecreaseRate + (float) ((double) num2 * 2.0 - 1.0) * this.attackIncreaseRate) * Time.fixedDeltaTime;
          else
            this.attack += (float) ((double) this.attackDecreaseRate * (double) num2 * 2.0) * Time.fixedDeltaTime;
        }
      }
      this.attack = Mathf.Clamp01(this.attack);
      this.aggresion = Mathf.Clamp01(this.aggresion);
      if (this.DamageParameter == null)
        return;
      this.DamageParameter.Value = this.attack;
    }

    [Serializable]
    public class Graphic
    {
      private float _lastChangeTime;
      private float _opacity;
      private MaterialPropertyBlock _properties;
      private int _textureIndex;
      public float fadeInTime = 0.5f;
      public float fadeOutTime = 5f;
      [Range(0.0f, 1f)]
      public float lowOpacity;
      public float minChangeTime = 5f;
      public Renderer renderer;
      public Texture2D[] textures;

      public void Initialize()
      {
        if (!((UnityEngine.Object) this.renderer != (UnityEngine.Object) null))
          return;
        this._opacity = 1f;
        this._properties = new MaterialPropertyBlock();
        this._properties.SetFloat("_Opacity", this._opacity);
        if (this.textures.Length != 0)
        {
          this._textureIndex = UnityEngine.Random.Range(0, this.textures.Length);
          this._properties.SetTexture("_MainTex", (Texture) this.textures[this._textureIndex]);
        }
        this._lastChangeTime = Time.time;
        this.renderer.SetPropertyBlock(this._properties);
      }

      public void Update(float deltaTime, bool isVisible, bool wasMoved)
      {
        if (!((UnityEngine.Object) this.renderer != (UnityEngine.Object) null))
          return;
        if (isVisible)
        {
          this._opacity = Mathf.MoveTowards(this._opacity, 0.0f, deltaTime / this.fadeOutTime);
        }
        else
        {
          this._opacity = Mathf.MoveTowards(this._opacity, 1f, deltaTime / this.fadeInTime);
          if (wasMoved && this.textures.Length != 0 && (double) Time.time >= (double) this._lastChangeTime + (double) this.minChangeTime)
          {
            this._textureIndex = UnityEngine.Random.Range(0, this.textures.Length);
            this._properties.SetTexture("_MainTex", (Texture) this.textures[this._textureIndex]);
            this._lastChangeTime = Time.time;
          }
        }
        this._properties.SetColor("_Color", new Color(1f, 1f, 1f, 1f));
        this._properties.SetFloat("_Opacity", Mathf.Lerp(this.lowOpacity, 1f, this._opacity));
        this.renderer.SetPropertyBlock(this._properties);
        this.renderer.GetPropertyBlock(this._properties);
      }
    }

    [Serializable]
    public class Navigation
    {
      private CullingGroup _cullingGroup;
      private BoundingSphere[] _cullingSpheres;
      private PlagueFacePoint _currentPoint;
      private float _destinationQuality;
      private bool _isInitialized;
      private float _lastJumpTime;
      private int _nextSearchPoint;
      private PathfinderAStar<PlagueFacePoint, float> _pathfinder;
      private PlagueFacePoint[] _points;
      private List<PlagueFacePoint> _route;
      public Vector4[] boundingSpheres;
      [Range(0.0f, 1f)]
      public float chaseAggresion = 0.95f;
      public bool manualDestination = false;
      public Camera playerCamera;
      [Range(0.0f, 1f)]
      public float roamAggresion = 0.5f;
      public float rotationSpeed = 45f;
      public int searchPointsPerTick = 50;
      public int searchRaysPerTick = 5;
      public float speed = 0.5f;

      private PlagueFacePoint[] CollectPointFrom(PlagueFacePoint startingPoint)
      {
        List<PlagueFacePoint> plagueFacePointList = new List<PlagueFacePoint>();
        Queue<PlagueFacePoint> plagueFacePointQueue = new Queue<PlagueFacePoint>();
        plagueFacePointQueue.Enqueue(startingPoint);
        while (plagueFacePointQueue.Count > 0)
        {
          PlagueFacePoint plagueFacePoint = plagueFacePointQueue.Dequeue();
          if (!plagueFacePointList.Contains(plagueFacePoint))
          {
            plagueFacePointList.Add(plagueFacePoint);
            for (int index = 0; index < plagueFacePoint.neighbors.Length; ++index)
            {
              if ((UnityEngine.Object) plagueFacePoint.neighbors[index] != (UnityEngine.Object) null)
                plagueFacePointQueue.Enqueue(plagueFacePoint.neighbors[index]);
            }
          }
        }
        return plagueFacePointList.ToArray();
      }

      public void Disable()
      {
        if (this._cullingGroup == null)
          return;
        this._cullingGroup.Dispose();
        this._cullingGroup = (CullingGroup) null;
      }

      public PlagueFacePoint[] GetNeighbors(PlagueFacePoint point) => point.neighbors;

      public void Initialize(PlagueFacePoint startingPoint)
      {
        this.Disable();
        if (this._isInitialized)
        {
          bool flag = false;
          for (int index = 0; index < this._points.Length; ++index)
          {
            if ((UnityEngine.Object) this._points[index] == (UnityEngine.Object) startingPoint)
            {
              flag = true;
              break;
            }
          }
          if (!flag)
            this._points = this.CollectPointFrom(startingPoint);
          this._route.Clear();
        }
        else
        {
          this._points = this.CollectPointFrom(startingPoint);
          this._route = new List<PlagueFacePoint>();
          this._cullingSpheres = new BoundingSphere[this.boundingSpheres.Length * 2];
          this._pathfinder = new PathfinderAStar<PlagueFacePoint, float>(new PathfinderAStar<PlagueFacePoint, float>.GetNeighbors(this.GetNeighbors), new PathfinderAStar<PlagueFacePoint, float>.Relation(this.TimeToJump), new PathfinderAStar<PlagueFacePoint, float>.Relation(this.TimeToJump), (PathfinderAStar<PlagueFacePoint, float>.Operation) ((x, y) => x + y), (PathfinderAStar<PlagueFacePoint, float>.Condition) ((x, y) => (double) x < (double) y), 0.0f);
          this._isInitialized = true;
        }
        this._currentPoint = startingPoint;
        this._lastJumpTime = Time.time;
        this._destinationQuality = 0.0f;
        this._nextSearchPoint = 0;
      }

      public bool IsMoving() => this._route.Count > 0;

      public bool IsVisible(Transform transform, Vector3 playerPosition, bool nextPoint)
      {
        if ((double) PlagueFace.VisibilityWeight(transform, playerPosition) <= 0.0)
          return false;
        if (this._cullingGroup == null)
          return true;
        for (int index = 0; index < this.boundingSpheres.Length; ++index)
        {
          if (this._cullingGroup.IsVisible(nextPoint ? index + this.boundingSpheres.Length : index))
            return true;
        }
        return false;
      }

      private float TimeToJump(Transform from, Transform to)
      {
        return Mathf.Max(Vector3.Distance(from.position, to.position) / this.speed, Quaternion.Angle(from.rotation, to.rotation) / this.rotationSpeed);
      }

      private float TimeToJump(PlagueFacePoint from, PlagueFacePoint to)
      {
        return this.TimeToJump(from.transform, to.transform);
      }

      public bool TryMove(Transform face, Vector3 playerPosition)
      {
        if (this._route.Count <= 0 || (double) this.TimeToJump(face, this._route[this._route.Count - 1].transform) >= (double) Time.time - (double) this._lastJumpTime || this.IsVisible(this._route[this._route.Count - 1].transform, playerPosition, true))
          return false;
        this._currentPoint = this._route[this._route.Count - 1];
        this._route.RemoveAt(this._route.Count - 1);
        face.SetParent(this._currentPoint.transform, false);
        face.localPosition = Vector3.zero;
        face.localRotation = Quaternion.identity;
        face.localScale = Vector3.one;
        this._lastJumpTime = Time.time;
        return true;
      }

      public void Navigate(PlagueFace face, float aggression)
      {
        if (!this.manualDestination && (double) aggression >= (double) this.roamAggresion)
        {
          if ((double) aggression < (double) this.chaseAggresion)
          {
            Vector3 vector3;
            if (this._route.Count == 0)
            {
              Vector3 normalized1 = (face.playerPosition - this._currentPoint.transform.position).normalized;
              int index1 = UnityEngine.Random.Range(0, this._currentPoint.neighbors.Length);
              bool flag = false;
              float num1 = float.NegativeInfinity;
              for (int index2 = 0; index2 < this._currentPoint.neighbors.Length; ++index2)
              {
                if (!flag || this._currentPoint.neighbors[index2].roamable)
                {
                  if (!flag && this._currentPoint.neighbors[index2].roamable)
                    num1 = float.NegativeInfinity;
                  vector3 = this._currentPoint.neighbors[index2].transform.position - this._currentPoint.transform.position;
                  Vector3 normalized2 = vector3.normalized;
                  float num2 = Vector3.Dot(normalized1, normalized2) * UnityEngine.Random.value;
                  if ((double) num2 > (double) num1)
                  {
                    index1 = index2;
                    flag = this._currentPoint.neighbors[index2].roamable;
                    num1 = num2;
                  }
                }
              }
              this._route.Add(this._currentPoint.neighbors[index1]);
              this._destinationQuality = 0.0f;
            }
            if (this._route.Count < 2)
            {
              vector3 = this._route[0].transform.position - this._currentPoint.transform.position;
              Vector3 normalized3 = vector3.normalized;
              int index3 = UnityEngine.Random.Range(0, this._route[0].neighbors.Length);
              bool flag = false;
              float num3 = float.NegativeInfinity;
              for (int index4 = 0; index4 < this._route[0].neighbors.Length; ++index4)
              {
                if (!flag || this._route[0].neighbors[index4].roamable)
                {
                  if (!flag && this._route[0].neighbors[index4].roamable)
                    num3 = float.NegativeInfinity;
                  vector3 = this._route[0].neighbors[index4].transform.position - this._route[0].transform.position;
                  Vector3 normalized4 = vector3.normalized;
                  float num4 = Vector3.Dot(normalized3, normalized4) * UnityEngine.Random.value;
                  if ((double) num4 > (double) num3)
                  {
                    index3 = index4;
                    flag = this._route[0].neighbors[index4].roamable;
                    num3 = num4;
                  }
                }
              }
              this._route.Add(this._route[0]);
              this._route[0] = this._route[0].neighbors[index3];
              this._destinationQuality = 0.0f;
            }
          }
          else
          {
            int num5 = Mathf.Max(this.searchPointsPerTick, this._points.Length);
            for (int searchRaysPerTick = this.searchRaysPerTick; num5 > 0 && searchRaysPerTick > 0; --num5)
            {
              if (this._nextSearchPoint >= this._points.Length)
                this._nextSearchPoint = 0;
              float num6 = PlagueFace.VisibilityWeight(this._points[this._nextSearchPoint].transform, face.playerPosition);
              if ((double) num6 > 0.0)
              {
                Vector2 vector2 = UnityEngine.Random.insideUnitCircle * face.eyeSize;
                Vector3 vector3 = this._points[this._nextSearchPoint].transform.localToWorldMatrix.MultiplyPoint(new Vector3(vector2.x, vector2.y, face.eyeExtent));
                Vector3 b = face.playerPosition + UnityEngine.Random.insideUnitSphere * face.playerSize;
                if (Physics.Raycast(vector3, b - vector3, out RaycastHit _, Vector3.Distance(vector3, b), (int) face.sightObstacles))
                  num6 = 0.0f;
                --searchRaysPerTick;
              }
              if (this._route.Count > 0 && (UnityEngine.Object) this._points[this._nextSearchPoint] == (UnityEngine.Object) this._route[0])
                this._destinationQuality = num6;
              else if ((double) num6 > (double) this._destinationQuality)
              {
                this.SetDestination(this._points[this._nextSearchPoint]);
                this._destinationQuality = num6;
              }
              ++this._nextSearchPoint;
            }
          }
        }
        if (this._cullingGroup == null)
        {
          this._cullingGroup = new CullingGroup();
          this._cullingGroup.SetBoundingSpheres(this._cullingSpheres);
        }
        if ((UnityEngine.Object) GameCamera.Instance.Camera != (UnityEngine.Object) null)
          this._cullingGroup.targetCamera = GameCamera.Instance.Camera;
        for (int index = 0; index < this.boundingSpheres.Length; ++index)
          this._cullingSpheres[index] = new BoundingSphere(this._currentPoint.transform.TransformPoint((Vector3) this.boundingSpheres[index]), this.boundingSpheres[index].w);
        if (this._route.Count > 0)
        {
          for (int index = 0; index < this.boundingSpheres.Length; ++index)
            this._cullingSpheres[index + this.boundingSpheres.Length] = new BoundingSphere(this._route[this._route.Count - 1].transform.TransformPoint((Vector3) this.boundingSpheres[index]), this.boundingSpheres[index].w);
          this._cullingGroup.SetBoundingSphereCount(this._cullingSpheres.Length);
        }
        else
          this._cullingGroup.SetBoundingSphereCount(this.boundingSpheres.Length);
      }

      public void SetDestination(PlagueFacePoint destination)
      {
        this._route.Clear();
        if ((UnityEngine.Object) destination == (UnityEngine.Object) this._currentPoint)
          return;
        this._pathfinder.AddReversedRoute(this._currentPoint, destination, this._route);
      }

      public void Validate()
      {
        if ((double) this.speed < 0.0)
          this.speed = 0.0f;
        if (this.searchPointsPerTick < 1)
          this.searchPointsPerTick = 1;
        if (this.searchRaysPerTick < 1)
          this.searchRaysPerTick = 1;
        if ((double) this.chaseAggresion >= (double) this.roamAggresion)
          return;
        this.chaseAggresion = (float) (((double) this.chaseAggresion + (double) this.roamAggresion) * 0.5);
        this.roamAggresion = this.chaseAggresion;
      }
    }

    [Serializable]
    public class Sound
    {
      private Vector3 _soundWeights;
      private Vector3 _velocity;
      private Vector3 _worldPosition;
      public Transform anchor;
      [Range(0.0f, 1f)]
      public float attackMaxVolume = 1f;
      public AudioSource attackSource;
      [Space]
      public float fadeTime = 1f;
      public float moveSmoothness = 0.5f;
      [Range(0.0f, 1f)]
      public float roamMaxVolume = 1f;
      public AudioSource roamSource;
      [Range(0.0f, 1f)]
      public float sleepMaxVolume = 1f;
      public AudioSource sleepSource;
      public Transform transform;

      public void Initialize()
      {
        if ((UnityEngine.Object) this.transform != (UnityEngine.Object) null && (UnityEngine.Object) this.anchor != (UnityEngine.Object) null)
        {
          this._worldPosition = this.anchor.position;
          this.transform.position = this._worldPosition;
        }
        this._soundWeights = new Vector3(1f, 0.0f, 0.0f);
        if ((UnityEngine.Object) this.sleepSource != (UnityEngine.Object) null)
          this.sleepSource.volume = this.sleepMaxVolume;
        if ((UnityEngine.Object) this.roamSource != (UnityEngine.Object) null)
          this.roamSource.volume = 0.0f;
        if (!((UnityEngine.Object) this.attackSource != (UnityEngine.Object) null))
          return;
        this.attackSource.volume = 0.0f;
      }

      public void Update(PlagueFace face, float deltaTime)
      {
        if ((UnityEngine.Object) this.transform != (UnityEngine.Object) null && (UnityEngine.Object) this.anchor != (UnityEngine.Object) null)
        {
          this._worldPosition = Vector3.SmoothDamp(this._worldPosition, this.anchor.position, ref this._velocity, this.moveSmoothness, float.PositiveInfinity, deltaTime);
          this.transform.position = this._worldPosition;
        }
        this._soundWeights = Vector3.MoveTowards(this._soundWeights, (double) face.attack <= 0.0 ? ((double) face.aggresion < (double) face.navigation.roamAggresion ? new Vector3(1f, 0.0f, 0.0f) : new Vector3(0.0f, 1f, 0.0f)) : new Vector3(0.0f, 0.0f, 1f), deltaTime / this.fadeTime);
        if ((UnityEngine.Object) this.sleepSource != (UnityEngine.Object) null)
          this.sleepSource.volume = this._soundWeights.x * this.sleepMaxVolume;
        if ((UnityEngine.Object) this.roamSource != (UnityEngine.Object) null)
          this.roamSource.volume = this._soundWeights.y * this.roamMaxVolume;
        if (!((UnityEngine.Object) this.attackSource != (UnityEngine.Object) null))
          return;
        this.attackSource.volume = this._soundWeights.z * this.attackMaxVolume;
      }
    }
  }
}
