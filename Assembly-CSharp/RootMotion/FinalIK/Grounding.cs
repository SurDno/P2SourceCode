using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
  [Serializable]
  public class Grounding
  {
    [Tooltip("Layers to ground the character to. Make sure to exclude the layer of the character controller.")]
    public LayerMask layers;
    [Tooltip("Max step height. Maximum vertical distance of Grounding from the root of the character.")]
    public float maxStep = 0.5f;
    [Tooltip("The height offset of the root.")]
    public float heightOffset;
    [Tooltip("The speed of moving the feet up/down.")]
    public float footSpeed = 2.5f;
    [Tooltip("CapsuleCast radius. Should match approximately with the size of the feet.")]
    public float footRadius = 0.15f;
    [Tooltip("Offset of the foot center along character forward axis.")]
    [HideInInspector]
    public float footCenterOffset;
    [Tooltip("Amount of velocity based prediction of the foot positions.")]
    public float prediction = 0.05f;
    [Tooltip("Weight of rotating the feet to the ground normal offset.")]
    [Range(0.0f, 1f)]
    public float footRotationWeight = 1f;
    [Tooltip("Speed of slerping the feet to their grounded rotations.")]
    public float footRotationSpeed = 7f;
    [Tooltip("Max Foot Rotation Angle. Max angular offset from the foot's rotation.")]
    [Range(0.0f, 90f)]
    public float maxFootRotationAngle = 45f;
    [Tooltip("If true, solver will rotate with the character root so the character can be grounded for example to spherical planets. For performance reasons leave this off unless needed.")]
    public bool rotateSolver;
    [Tooltip("The speed of moving the character up/down.")]
    public float pelvisSpeed = 5f;
    [Tooltip("Used for smoothing out vertical pelvis movement (range 0 - 1).")]
    [Range(0.0f, 1f)]
    public float pelvisDamper;
    [Tooltip("The weight of lowering the pelvis to the lowest foot.")]
    public float lowerPelvisWeight = 1f;
    [Tooltip("The weight of lifting the pelvis to the highest foot. This is useful when you don't want the feet to go too high relative to the body when crouching.")]
    public float liftPelvisWeight;
    [Tooltip("The radius of the spherecast from the root that determines whether the character root is grounded.")]
    public float rootSphereCastRadius = 0.1f;
    [Tooltip("The raycasting quality. Fastest is a single raycast per foot, Simple is three raycasts, Best is one raycast and a capsule cast per foot.")]
    public Grounding.Quality quality = Grounding.Quality.Best;
    private bool initiated;

    public Grounding.Leg[] legs { get; private set; }

    public Grounding.Pelvis pelvis { get; private set; }

    public bool isGrounded { get; private set; }

    public Transform root { get; private set; }

    public RaycastHit rootHit { get; private set; }

    public bool rootGrounded => (double) this.rootHit.distance < (double) this.maxStep * 2.0;

    public RaycastHit GetRootHit(float maxDistanceMlp = 10f)
    {
      RaycastHit hitInfo = new RaycastHit();
      Vector3 up = this.up;
      Vector3 zero = Vector3.zero;
      foreach (Grounding.Leg leg in this.legs)
        zero += leg.transform.position;
      Vector3 vector3 = zero / (float) this.legs.Length;
      hitInfo.point = vector3 - up * this.maxStep * 10f;
      float num = maxDistanceMlp + 1f;
      hitInfo.distance = this.maxStep * num;
      if ((double) this.maxStep <= 0.0)
        return hitInfo;
      if (this.quality != Grounding.Quality.Best)
        Physics.Raycast(vector3 + up * this.maxStep, -up, out hitInfo, this.maxStep * num, (int) this.layers);
      else
        Physics.SphereCast(vector3 + up * this.maxStep, this.rootSphereCastRadius, -this.up, out hitInfo, this.maxStep * num, (int) this.layers);
      return hitInfo;
    }

    public bool IsValid(ref string errorMessage)
    {
      if ((UnityEngine.Object) this.root == (UnityEngine.Object) null)
      {
        errorMessage = "Root transform is null. Can't initiate Grounding.";
        return false;
      }
      if (this.legs == null)
      {
        errorMessage = "Grounding legs is null. Can't initiate Grounding.";
        return false;
      }
      if (this.pelvis == null)
      {
        errorMessage = "Grounding pelvis is null. Can't initiate Grounding.";
        return false;
      }
      if (this.legs.Length != 0)
        return true;
      errorMessage = "Grounding has 0 legs. Can't initiate Grounding.";
      return false;
    }

    public void Initiate(Transform root, Transform[] feet)
    {
      this.root = root;
      this.initiated = false;
      this.rootHit = new RaycastHit();
      if (this.legs == null)
        this.legs = new Grounding.Leg[feet.Length];
      if (this.legs.Length != feet.Length)
        this.legs = new Grounding.Leg[feet.Length];
      for (int index = 0; index < feet.Length; ++index)
      {
        if (this.legs[index] == null)
          this.legs[index] = new Grounding.Leg();
      }
      if (this.pelvis == null)
        this.pelvis = new Grounding.Pelvis();
      string empty = string.Empty;
      if (!this.IsValid(ref empty))
      {
        Warning.Log(empty, root);
      }
      else
      {
        if (!Application.isPlaying)
          return;
        for (int index = 0; index < feet.Length; ++index)
          this.legs[index].Initiate(this, feet[index]);
        this.pelvis.Initiate(this);
        this.initiated = true;
      }
    }

    public void Update()
    {
      if (!this.initiated)
        return;
      if ((int) this.layers == 0)
        this.LogWarning("Grounding layers are set to nothing. Please add a ground layer.");
      this.maxStep = Mathf.Clamp(this.maxStep, 0.0f, this.maxStep);
      this.footRadius = Mathf.Clamp(this.footRadius, 0.0001f, this.maxStep);
      this.pelvisDamper = Mathf.Clamp(this.pelvisDamper, 0.0f, 1f);
      this.rootSphereCastRadius = Mathf.Clamp(this.rootSphereCastRadius, 0.0001f, this.rootSphereCastRadius);
      this.maxFootRotationAngle = Mathf.Clamp(this.maxFootRotationAngle, 0.0f, 90f);
      this.prediction = Mathf.Clamp(this.prediction, 0.0f, this.prediction);
      this.footSpeed = Mathf.Clamp(this.footSpeed, 0.0f, this.footSpeed);
      this.rootHit = this.GetRootHit();
      float num1 = float.NegativeInfinity;
      float num2 = float.PositiveInfinity;
      this.isGrounded = false;
      foreach (Grounding.Leg leg in this.legs)
      {
        leg.Process();
        if ((double) leg.IKOffset > (double) num1)
          num1 = leg.IKOffset;
        if ((double) leg.IKOffset < (double) num2)
          num2 = leg.IKOffset;
        if (leg.isGrounded)
          this.isGrounded = true;
      }
      this.pelvis.Process(-num1 * this.lowerPelvisWeight, -num2 * this.liftPelvisWeight, this.isGrounded);
    }

    public Vector3 GetLegsPlaneNormal()
    {
      if (!this.initiated)
        return Vector3.up;
      Vector3 up = this.up;
      Vector3 legsPlaneNormal = up;
      for (int index = 0; index < this.legs.Length; ++index)
      {
        Vector3 toDirection = this.legs[index].IKPosition - this.root.position;
        Vector3 normal = up;
        Vector3 tangent = toDirection;
        Vector3.OrthoNormalize(ref normal, ref tangent);
        legsPlaneNormal = Quaternion.FromToRotation(tangent, toDirection) * legsPlaneNormal;
      }
      return legsPlaneNormal;
    }

    public void Reset()
    {
      if (!Application.isPlaying)
        return;
      this.pelvis.Reset();
      foreach (Grounding.Leg leg in this.legs)
        leg.Reset();
    }

    public void LogWarning(string message) => Warning.Log(message, this.root);

    public Vector3 up => this.useRootRotation ? this.root.up : Vector3.up;

    public float GetVerticalOffset(Vector3 p1, Vector3 p2)
    {
      return this.useRootRotation ? (Quaternion.Inverse(this.root.rotation) * (p1 - p2)).y : p1.y - p2.y;
    }

    public Vector3 Flatten(Vector3 v)
    {
      if (this.useRootRotation)
      {
        Vector3 tangent = v;
        Vector3 up = this.root.up;
        Vector3.OrthoNormalize(ref up, ref tangent);
        return Vector3.Project(v, tangent);
      }
      v.y = 0.0f;
      return v;
    }

    private bool useRootRotation => this.rotateSolver && !(this.root.up == Vector3.up);

    public Vector3 GetFootCenterOffset()
    {
      return this.root.forward * this.footRadius + this.root.forward * this.footCenterOffset;
    }

    [Serializable]
    public enum Quality
    {
      Fastest,
      Simple,
      Best,
    }

    public class Leg
    {
      public Quaternion rotationOffset = Quaternion.identity;
      public bool invertFootCenter;
      private Grounding grounding;
      private float lastTime;
      private float deltaTime;
      private Vector3 lastPosition;
      private Quaternion toHitNormal;
      private Quaternion r;
      private RaycastHit heelHit;
      private Vector3 up = Vector3.up;

      public bool isGrounded { get; private set; }

      public Vector3 IKPosition { get; private set; }

      public bool initiated { get; private set; }

      public float heightFromGround { get; private set; }

      public Vector3 velocity { get; private set; }

      public Transform transform { get; private set; }

      public float IKOffset { get; private set; }

      public void Initiate(Grounding grounding, Transform transform)
      {
        this.initiated = false;
        this.grounding = grounding;
        this.transform = transform;
        this.up = Vector3.up;
        this.IKPosition = transform.position;
        this.rotationOffset = Quaternion.identity;
        this.initiated = true;
        this.OnEnable();
      }

      public void OnEnable()
      {
        if (!this.initiated)
          return;
        this.lastPosition = this.transform.position;
        this.lastTime = Time.deltaTime;
      }

      public void Reset()
      {
        this.lastPosition = this.transform.position;
        this.lastTime = Time.deltaTime;
        this.IKOffset = 0.0f;
        this.IKPosition = this.transform.position;
        this.rotationOffset = Quaternion.identity;
      }

      public void Process()
      {
        if (!this.initiated || (double) this.grounding.maxStep <= 0.0)
          return;
        this.deltaTime = Time.time - this.lastTime;
        this.lastTime = Time.time;
        if ((double) this.deltaTime == 0.0)
          return;
        this.up = this.grounding.up;
        this.heightFromGround = float.PositiveInfinity;
        this.velocity = (this.transform.position - this.lastPosition) / this.deltaTime;
        this.velocity = this.grounding.Flatten(this.velocity);
        this.lastPosition = this.transform.position;
        Vector3 offsetFromHeel = this.velocity * this.grounding.prediction;
        if ((double) this.grounding.footRadius <= 0.0)
          this.grounding.quality = Grounding.Quality.Fastest;
        switch (this.grounding.quality)
        {
          case Grounding.Quality.Fastest:
            RaycastHit raycastHit = this.GetRaycastHit(offsetFromHeel);
            this.SetFootToPoint(raycastHit.normal, raycastHit.point);
            break;
          case Grounding.Quality.Simple:
            this.heelHit = this.GetRaycastHit(Vector3.zero);
            Vector3 vector3_1 = this.grounding.GetFootCenterOffset();
            if (this.invertFootCenter)
              vector3_1 = -vector3_1;
            Vector3 vector3_2 = Vector3.Cross(this.GetRaycastHit(vector3_1 + offsetFromHeel).point - this.heelHit.point, this.GetRaycastHit(this.grounding.root.right * this.grounding.footRadius * 0.5f).point - this.heelHit.point).normalized;
            if ((double) Vector3.Dot(vector3_2, this.up) < 0.0)
              vector3_2 = -vector3_2;
            this.SetFootToPlane(vector3_2, this.heelHit.point, this.heelHit.point);
            break;
          case Grounding.Quality.Best:
            this.heelHit = this.GetRaycastHit(this.invertFootCenter ? -this.grounding.GetFootCenterOffset() : Vector3.zero);
            RaycastHit capsuleHit = this.GetCapsuleHit(offsetFromHeel);
            this.SetFootToPlane(capsuleHit.normal, capsuleHit.point, this.heelHit.point);
            break;
        }
        this.isGrounded = (double) this.heightFromGround < (double) this.grounding.maxStep;
        float num = this.stepHeightFromGround;
        if (!this.grounding.rootGrounded)
          num = 0.0f;
        this.IKOffset = Interp.LerpValue(this.IKOffset, num, this.grounding.footSpeed, this.grounding.footSpeed);
        this.IKOffset = Mathf.Lerp(this.IKOffset, num, this.deltaTime * this.grounding.footSpeed);
        this.IKOffset = Mathf.Clamp(this.IKOffset, -Mathf.Clamp(this.grounding.maxStep - this.grounding.GetVerticalOffset(this.transform.position, this.grounding.root.position), 0.0f, this.grounding.maxStep), this.IKOffset);
        this.RotateFoot();
        this.IKPosition = this.transform.position - this.up * this.IKOffset;
        float footRotationWeight = this.grounding.footRotationWeight;
        this.rotationOffset = (double) footRotationWeight >= 1.0 ? this.r : Quaternion.Slerp(Quaternion.identity, this.r, footRotationWeight);
      }

      public float stepHeightFromGround
      {
        get => Mathf.Clamp(this.heightFromGround, -this.grounding.maxStep, this.grounding.maxStep);
      }

      private RaycastHit GetCapsuleHit(Vector3 offsetFromHeel)
      {
        RaycastHit hitInfo = new RaycastHit();
        Vector3 vector3_1 = this.grounding.GetFootCenterOffset();
        if (this.invertFootCenter)
          vector3_1 = -vector3_1;
        Vector3 vector3_2 = this.transform.position + vector3_1;
        hitInfo.point = vector3_2 - this.up * this.grounding.maxStep * 2f;
        hitInfo.normal = this.up;
        Vector3 point1 = vector3_2 + this.grounding.maxStep * this.up;
        Vector3 point2 = point1 + offsetFromHeel;
        if (Physics.CapsuleCast(point1, point2, this.grounding.footRadius, -this.up, out hitInfo, this.grounding.maxStep * 3f, (int) this.grounding.layers) && float.IsNaN(hitInfo.point.x))
        {
          hitInfo.point = vector3_2 - this.up * this.grounding.maxStep * 2f;
          hitInfo.normal = this.up;
        }
        return hitInfo;
      }

      private RaycastHit GetRaycastHit(Vector3 offsetFromHeel)
      {
        RaycastHit hitInfo = new RaycastHit();
        Vector3 vector3 = this.transform.position + offsetFromHeel;
        hitInfo.point = vector3 - this.up * this.grounding.maxStep * 2f;
        hitInfo.normal = this.up;
        if ((double) this.grounding.maxStep <= 0.0)
          return hitInfo;
        Physics.Raycast(vector3 + this.grounding.maxStep * this.up, -this.up, out hitInfo, this.grounding.maxStep * 3f, (int) this.grounding.layers);
        return hitInfo;
      }

      private Vector3 RotateNormal(Vector3 normal)
      {
        return this.grounding.quality == Grounding.Quality.Best ? normal : Vector3.RotateTowards(this.up, normal, this.grounding.maxFootRotationAngle * ((float) Math.PI / 180f), this.deltaTime);
      }

      private void SetFootToPoint(Vector3 normal, Vector3 point)
      {
        this.toHitNormal = Quaternion.FromToRotation(this.up, this.RotateNormal(normal));
        this.heightFromGround = this.GetHeightFromGround(point);
      }

      private void SetFootToPlane(Vector3 planeNormal, Vector3 planePoint, Vector3 heelHitPoint)
      {
        planeNormal = this.RotateNormal(planeNormal);
        this.toHitNormal = Quaternion.FromToRotation(this.up, planeNormal);
        this.heightFromGround = this.GetHeightFromGround(V3Tools.LineToPlane(this.transform.position + this.up * this.grounding.maxStep, -this.up, planeNormal, planePoint));
        this.heightFromGround = Mathf.Clamp(this.heightFromGround, float.NegativeInfinity, this.GetHeightFromGround(heelHitPoint));
      }

      private float GetHeightFromGround(Vector3 hitPoint)
      {
        return this.grounding.GetVerticalOffset(this.transform.position, hitPoint) - this.rootYOffset;
      }

      private void RotateFoot()
      {
        this.r = Quaternion.Slerp(this.r, this.GetRotationOffsetTarget(), this.deltaTime * this.grounding.footRotationSpeed);
      }

      private Quaternion GetRotationOffsetTarget()
      {
        if ((double) this.grounding.maxFootRotationAngle <= 0.0)
          return Quaternion.identity;
        return (double) this.grounding.maxFootRotationAngle >= 180.0 ? this.toHitNormal : Quaternion.RotateTowards(Quaternion.identity, this.toHitNormal, this.grounding.maxFootRotationAngle);
      }

      private float rootYOffset
      {
        get
        {
          return this.grounding.GetVerticalOffset(this.transform.position, this.grounding.root.position - this.up * this.grounding.heightOffset);
        }
      }
    }

    public class Pelvis
    {
      private Grounding grounding;
      private Vector3 lastRootPosition;
      private float damperF;
      private bool initiated;
      private float lastTime;

      public Vector3 IKOffset { get; private set; }

      public float heightOffset { get; private set; }

      public void Initiate(Grounding grounding)
      {
        this.grounding = grounding;
        this.initiated = true;
        this.OnEnable();
      }

      public void Reset()
      {
        this.lastRootPosition = this.grounding.root.transform.position;
        this.lastTime = Time.deltaTime;
        this.IKOffset = Vector3.zero;
        this.heightOffset = 0.0f;
      }

      public void OnEnable()
      {
        if (!this.initiated)
          return;
        this.lastRootPosition = this.grounding.root.transform.position;
        this.lastTime = Time.time;
      }

      public void Process(float lowestOffset, float highestOffset, bool isGrounded)
      {
        if (!this.initiated)
          return;
        float num = Time.time - this.lastTime;
        this.lastTime = Time.time;
        if ((double) num <= 0.0)
          return;
        float b = lowestOffset + highestOffset;
        if (!this.grounding.rootGrounded)
          b = 0.0f;
        this.heightOffset = Mathf.Lerp(this.heightOffset, b, num * this.grounding.pelvisSpeed);
        Vector3 p1 = this.grounding.root.position - this.lastRootPosition;
        this.lastRootPosition = this.grounding.root.position;
        this.damperF = Interp.LerpValue(this.damperF, isGrounded ? 1f : 0.0f, 1f, 10f);
        this.heightOffset -= this.grounding.GetVerticalOffset(p1, Vector3.zero) * this.grounding.pelvisDamper * this.damperF;
        this.IKOffset = this.grounding.up * this.heightOffset;
      }
    }
  }
}
