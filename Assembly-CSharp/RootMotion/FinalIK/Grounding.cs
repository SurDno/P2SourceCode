using System;

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
    public Quality quality = Quality.Best;
    private bool initiated;

    public Leg[] legs { get; private set; }

    public Pelvis pelvis { get; private set; }

    public bool isGrounded { get; private set; }

    public Transform root { get; private set; }

    public RaycastHit rootHit { get; private set; }

    public bool rootGrounded => (double) rootHit.distance < maxStep * 2.0;

    public RaycastHit GetRootHit(float maxDistanceMlp = 10f)
    {
      RaycastHit hitInfo = new RaycastHit();
      Vector3 up = this.up;
      Vector3 zero = Vector3.zero;
      foreach (Leg leg in legs)
        zero += leg.transform.position;
      Vector3 vector3 = zero / (float) legs.Length;
      hitInfo.point = vector3 - up * maxStep * 10f;
      float num = maxDistanceMlp + 1f;
      hitInfo.distance = maxStep * num;
      if (maxStep <= 0.0)
        return hitInfo;
      if (quality != Quality.Best)
        Physics.Raycast(vector3 + up * maxStep, -up, out hitInfo, maxStep * num, (int) layers);
      else
        Physics.SphereCast(vector3 + up * maxStep, rootSphereCastRadius, -this.up, out hitInfo, maxStep * num, (int) layers);
      return hitInfo;
    }

    public bool IsValid(ref string errorMessage)
    {
      if ((UnityEngine.Object) root == (UnityEngine.Object) null)
      {
        errorMessage = "Root transform is null. Can't initiate Grounding.";
        return false;
      }
      if (legs == null)
      {
        errorMessage = "Grounding legs is null. Can't initiate Grounding.";
        return false;
      }
      if (pelvis == null)
      {
        errorMessage = "Grounding pelvis is null. Can't initiate Grounding.";
        return false;
      }
      if (legs.Length != 0)
        return true;
      errorMessage = "Grounding has 0 legs. Can't initiate Grounding.";
      return false;
    }

    public void Initiate(Transform root, Transform[] feet)
    {
      this.root = root;
      initiated = false;
      rootHit = new RaycastHit();
      if (legs == null)
        legs = new Leg[feet.Length];
      if (legs.Length != feet.Length)
        legs = new Leg[feet.Length];
      for (int index = 0; index < feet.Length; ++index)
      {
        if (legs[index] == null)
          legs[index] = new Leg();
      }
      if (pelvis == null)
        pelvis = new Pelvis();
      string empty = string.Empty;
      if (!IsValid(ref empty))
      {
        Warning.Log(empty, root);
      }
      else
      {
        if (!Application.isPlaying)
          return;
        for (int index = 0; index < feet.Length; ++index)
          legs[index].Initiate(this, feet[index]);
        pelvis.Initiate(this);
        initiated = true;
      }
    }

    public void Update()
    {
      if (!initiated)
        return;
      if ((int) layers == 0)
        LogWarning("Grounding layers are set to nothing. Please add a ground layer.");
      maxStep = Mathf.Clamp(maxStep, 0.0f, maxStep);
      footRadius = Mathf.Clamp(footRadius, 0.0001f, maxStep);
      pelvisDamper = Mathf.Clamp(pelvisDamper, 0.0f, 1f);
      rootSphereCastRadius = Mathf.Clamp(rootSphereCastRadius, 0.0001f, rootSphereCastRadius);
      maxFootRotationAngle = Mathf.Clamp(maxFootRotationAngle, 0.0f, 90f);
      prediction = Mathf.Clamp(prediction, 0.0f, prediction);
      footSpeed = Mathf.Clamp(footSpeed, 0.0f, footSpeed);
      rootHit = GetRootHit();
      float num1 = float.NegativeInfinity;
      float num2 = float.PositiveInfinity;
      isGrounded = false;
      foreach (Leg leg in legs)
      {
        leg.Process();
        if (leg.IKOffset > (double) num1)
          num1 = leg.IKOffset;
        if (leg.IKOffset < (double) num2)
          num2 = leg.IKOffset;
        if (leg.isGrounded)
          isGrounded = true;
      }
      pelvis.Process(-num1 * lowerPelvisWeight, -num2 * liftPelvisWeight, isGrounded);
    }

    public Vector3 GetLegsPlaneNormal()
    {
      if (!initiated)
        return Vector3.up;
      Vector3 up = this.up;
      Vector3 legsPlaneNormal = up;
      for (int index = 0; index < legs.Length; ++index)
      {
        Vector3 toDirection = legs[index].IKPosition - root.position;
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
      pelvis.Reset();
      foreach (Leg leg in legs)
        leg.Reset();
    }

    public void LogWarning(string message) => Warning.Log(message, root);

    public Vector3 up => useRootRotation ? root.up : Vector3.up;

    public float GetVerticalOffset(Vector3 p1, Vector3 p2)
    {
      return useRootRotation ? (Quaternion.Inverse(root.rotation) * (p1 - p2)).y : p1.y - p2.y;
    }

    public Vector3 Flatten(Vector3 v)
    {
      if (useRootRotation)
      {
        Vector3 tangent = v;
        Vector3 up = root.up;
        Vector3.OrthoNormalize(ref up, ref tangent);
        return Vector3.Project(v, tangent);
      }
      v.y = 0.0f;
      return v;
    }

    private bool useRootRotation => rotateSolver && !(root.up == Vector3.up);

    public Vector3 GetFootCenterOffset()
    {
      return root.forward * footRadius + root.forward * footCenterOffset;
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
        initiated = false;
        this.grounding = grounding;
        this.transform = transform;
        up = Vector3.up;
        IKPosition = transform.position;
        rotationOffset = Quaternion.identity;
        initiated = true;
        OnEnable();
      }

      public void OnEnable()
      {
        if (!initiated)
          return;
        lastPosition = transform.position;
        lastTime = Time.deltaTime;
      }

      public void Reset()
      {
        lastPosition = transform.position;
        lastTime = Time.deltaTime;
        IKOffset = 0.0f;
        IKPosition = transform.position;
        rotationOffset = Quaternion.identity;
      }

      public void Process()
      {
        if (!initiated || grounding.maxStep <= 0.0)
          return;
        deltaTime = Time.time - lastTime;
        lastTime = Time.time;
        if (deltaTime == 0.0)
          return;
        up = grounding.up;
        heightFromGround = float.PositiveInfinity;
        velocity = (transform.position - lastPosition) / deltaTime;
        velocity = grounding.Flatten(velocity);
        lastPosition = transform.position;
        Vector3 offsetFromHeel = velocity * grounding.prediction;
        if (grounding.footRadius <= 0.0)
          grounding.quality = Quality.Fastest;
        switch (grounding.quality)
        {
          case Quality.Fastest:
            RaycastHit raycastHit = GetRaycastHit(offsetFromHeel);
            SetFootToPoint(raycastHit.normal, raycastHit.point);
            break;
          case Quality.Simple:
            heelHit = GetRaycastHit(Vector3.zero);
            Vector3 vector3_1 = grounding.GetFootCenterOffset();
            if (invertFootCenter)
              vector3_1 = -vector3_1;
            Vector3 vector3_2 = Vector3.Cross(GetRaycastHit(vector3_1 + offsetFromHeel).point - heelHit.point, this.GetRaycastHit(grounding.root.right * grounding.footRadius * 0.5f).point - heelHit.point).normalized;
            if ((double) Vector3.Dot(vector3_2, up) < 0.0)
              vector3_2 = -vector3_2;
            SetFootToPlane(vector3_2, heelHit.point, heelHit.point);
            break;
          case Quality.Best:
            heelHit = GetRaycastHit(invertFootCenter ? -grounding.GetFootCenterOffset() : Vector3.zero);
            RaycastHit capsuleHit = GetCapsuleHit(offsetFromHeel);
            SetFootToPlane(capsuleHit.normal, capsuleHit.point, heelHit.point);
            break;
        }
        isGrounded = heightFromGround < (double) grounding.maxStep;
        float num = stepHeightFromGround;
        if (!grounding.rootGrounded)
          num = 0.0f;
        IKOffset = Interp.LerpValue(IKOffset, num, grounding.footSpeed, grounding.footSpeed);
        IKOffset = Mathf.Lerp(IKOffset, num, deltaTime * grounding.footSpeed);
        IKOffset = Mathf.Clamp(IKOffset, -Mathf.Clamp(grounding.maxStep - grounding.GetVerticalOffset(transform.position, grounding.root.position), 0.0f, grounding.maxStep), IKOffset);
        RotateFoot();
        IKPosition = transform.position - up * IKOffset;
        float footRotationWeight = grounding.footRotationWeight;
        rotationOffset = footRotationWeight >= 1.0 ? r : Quaternion.Slerp(Quaternion.identity, r, footRotationWeight);
      }

      public float stepHeightFromGround
      {
        get => Mathf.Clamp(heightFromGround, -grounding.maxStep, grounding.maxStep);
      }

      private RaycastHit GetCapsuleHit(Vector3 offsetFromHeel)
      {
        RaycastHit hitInfo = new RaycastHit();
        Vector3 vector3_1 = grounding.GetFootCenterOffset();
        if (invertFootCenter)
          vector3_1 = -vector3_1;
        Vector3 vector3_2 = transform.position + vector3_1;
        hitInfo.point = vector3_2 - up * grounding.maxStep * 2f;
        hitInfo.normal = up;
        Vector3 point1 = vector3_2 + grounding.maxStep * up;
        Vector3 point2 = point1 + offsetFromHeel;
        if (Physics.CapsuleCast(point1, point2, grounding.footRadius, -up, out hitInfo, grounding.maxStep * 3f, (int) grounding.layers) && float.IsNaN(hitInfo.point.x))
        {
          hitInfo.point = vector3_2 - up * grounding.maxStep * 2f;
          hitInfo.normal = up;
        }
        return hitInfo;
      }

      private RaycastHit GetRaycastHit(Vector3 offsetFromHeel)
      {
        RaycastHit hitInfo = new RaycastHit();
        Vector3 vector3 = transform.position + offsetFromHeel;
        hitInfo.point = vector3 - up * grounding.maxStep * 2f;
        hitInfo.normal = up;
        if (grounding.maxStep <= 0.0)
          return hitInfo;
        Physics.Raycast(vector3 + grounding.maxStep * up, -up, out hitInfo, grounding.maxStep * 3f, (int) grounding.layers);
        return hitInfo;
      }

      private Vector3 RotateNormal(Vector3 normal)
      {
        return grounding.quality == Quality.Best ? normal : Vector3.RotateTowards(up, normal, grounding.maxFootRotationAngle * ((float) Math.PI / 180f), deltaTime);
      }

      private void SetFootToPoint(Vector3 normal, Vector3 point)
      {
        toHitNormal = Quaternion.FromToRotation(up, RotateNormal(normal));
        heightFromGround = GetHeightFromGround(point);
      }

      private void SetFootToPlane(Vector3 planeNormal, Vector3 planePoint, Vector3 heelHitPoint)
      {
        planeNormal = RotateNormal(planeNormal);
        toHitNormal = Quaternion.FromToRotation(up, planeNormal);
        heightFromGround = GetHeightFromGround(V3Tools.LineToPlane(transform.position + up * grounding.maxStep, -up, planeNormal, planePoint));
        heightFromGround = Mathf.Clamp(heightFromGround, float.NegativeInfinity, GetHeightFromGround(heelHitPoint));
      }

      private float GetHeightFromGround(Vector3 hitPoint)
      {
        return grounding.GetVerticalOffset(transform.position, hitPoint) - rootYOffset;
      }

      private void RotateFoot()
      {
        r = Quaternion.Slerp(r, GetRotationOffsetTarget(), deltaTime * grounding.footRotationSpeed);
      }

      private Quaternion GetRotationOffsetTarget()
      {
        if (grounding.maxFootRotationAngle <= 0.0)
          return Quaternion.identity;
        return grounding.maxFootRotationAngle >= 180.0 ? toHitNormal : Quaternion.RotateTowards(Quaternion.identity, toHitNormal, grounding.maxFootRotationAngle);
      }

      private float rootYOffset
      {
        get
        {
          return grounding.GetVerticalOffset(transform.position, grounding.root.position - up * grounding.heightOffset);
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
        initiated = true;
        OnEnable();
      }

      public void Reset()
      {
        lastRootPosition = grounding.root.transform.position;
        lastTime = Time.deltaTime;
        IKOffset = Vector3.zero;
        heightOffset = 0.0f;
      }

      public void OnEnable()
      {
        if (!initiated)
          return;
        lastRootPosition = grounding.root.transform.position;
        lastTime = Time.time;
      }

      public void Process(float lowestOffset, float highestOffset, bool isGrounded)
      {
        if (!initiated)
          return;
        float num = Time.time - lastTime;
        lastTime = Time.time;
        if (num <= 0.0)
          return;
        float b = lowestOffset + highestOffset;
        if (!grounding.rootGrounded)
          b = 0.0f;
        heightOffset = Mathf.Lerp(heightOffset, b, num * grounding.pelvisSpeed);
        Vector3 p1 = grounding.root.position - lastRootPosition;
        lastRootPosition = grounding.root.position;
        damperF = Interp.LerpValue(damperF, isGrounded ? 1f : 0.0f, 1f, 10f);
        heightOffset -= grounding.GetVerticalOffset(p1, Vector3.zero) * grounding.pelvisDamper * damperF;
        IKOffset = grounding.up * heightOffset;
      }
    }
  }
}
