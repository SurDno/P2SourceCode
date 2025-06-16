namespace Engine.Source.UI.Menu.Protagonist.LockPicking
{
  public class LockPickingPin : MonoBehaviour
  {
    [SerializeField]
    private LockPickingPinSpot[] sourSpotPrototypes;
    [SerializeField]
    private LockPickingPinSpot[] sweetSpotPrototypes;
    [SerializeField]
    private GameObject sweetIndicator;
    [SerializeField]
    private GameObject sourIndicator;
    [SerializeField]
    private Vector2 rotationRange;
    private float lockPosition;
    private LockPickingSettings settings;
    private Spot[] spots;
    private float position = -1f;
    private float velocity;

    public bool InSweetSpot { get; private set; }

    public bool Locked { get; set; }

    private float Position
    {
      get => position;
      set
      {
        value = Mathf.Clamp01(value);
        if (position == (double) value)
          return;
        position = value;
        this.transform.localEulerAngles = new Vector3(0.0f, 0.0f, Mathf.Lerp(rotationRange.x, rotationRange.y, position));
      }
    }

    public void Build(LockPickingSettings settings)
    {
      Clear();
      this.settings = settings;
      LockPickingSettings.Pattern pattern = settings.Patterns[Random.Range(0, settings.Patterns.Length)];
      if (pattern.SpotCount < 1)
        pattern.SpotCount = 1;
      if (pattern.SweetSpotPosition < 0)
        pattern.SweetSpotPosition = 0;
      else if (pattern.SweetSpotPosition >= pattern.SpotCount)
        pattern.SweetSpotPosition = pattern.SpotCount - 1;
      spots = new Spot[pattern.SpotCount];
      float num1 = Random.value;
      float num2 = 0.0f;
      for (int index = 0; index < spots.Length; ++index)
      {
        bool sweet = index == pattern.SweetSpotPosition;
        spots[index].Sweet = sweet;
        spots[index].View = CreateView(sweet, index);
        spots[index].Min = Random.value;
        num1 += spots[index].Min;
        spots[index].Max = !sweet ? Random.Range(settings.SourSpotMinSize, settings.SourSpotMaxSize) : settings.SweetSpotSize;
        num2 += spots[index].Max;
      }
      float num3 = (1f - (float) (num2 + (double) settings.LowerDeadZone + settings.UpperDeadZone + settings.MiddleDeadZones * (double) (pattern.SpotCount - 1))) / num1;
      for (int index = 0; index < spots.Length; ++index)
      {
        if (index == 0)
        {
          spots[index].Min = settings.LowerDeadZone + spots[index].Min * num3;
          spots[index].Max += spots[index].Min;
        }
        else
        {
          spots[index].Min = (float) (spots[index - 1].Max + (double) settings.MiddleDeadZones + spots[index].Min * (double) num3);
          spots[index].Max += spots[index].Min;
        }
        if (spots[index].Sweet)
          lockPosition = (float) ((spots[index].Min + (double) spots[index].Max) * 0.5);
        LockPickingPinSpot view = spots[index].View;
        view.Setup((float) ((spots[index].Max + (double) spots[index].Min) * 0.5), spots[index].Max - spots[index].Min);
        view.gameObject.SetActive(true);
      }
    }

    public void Clear()
    {
      settings = null;
      Locked = false;
      Position = 0.0f;
      velocity = 0.0f;
      InSweetSpot = false;
      if (spots == null)
        return;
      foreach (Spot spot in spots)
        Object.Destroy((Object) spot.View.gameObject);
      spots = null;
    }

    public LockPickingPinSpot CreateView(bool sweet, int index)
    {
      LockPickingPinSpot original;
      if (sweet)
      {
        index %= sweetSpotPrototypes.Length;
        original = sweetSpotPrototypes[index];
      }
      else
      {
        index %= sourSpotPrototypes.Length;
        original = sourSpotPrototypes[index];
      }
      LockPickingPinSpot view = Object.Instantiate<LockPickingPinSpot>(original, original.transform.parent);
      view.transform.SetParent(original.transform.parent, false);
      return view;
    }

    private void Update()
    {
      if (settings == null)
        return;
      if (Locked)
      {
        Position = lockPosition;
        velocity = 0.0f;
      }
      else
      {
        velocity -= settings.GravityForce * Time.deltaTime;
        velocity = Mathf.Clamp(velocity, -settings.MaxVelocity, settings.MaxVelocity);
        Position += velocity * Time.deltaTime;
      }
      InSweetSpot = false;
      bool flag = false;
      for (int index = 0; index < spots.Length; ++index)
      {
        if (Position > (double) spots[index].Min && Position <= (double) spots[index].Max)
        {
          if (spots[index].Sweet)
            InSweetSpot = true;
          else
            flag = true;
        }
      }
      if (flag)
        velocity *= Mathf.Pow(0.5f, settings.SourSpotDrag * Time.deltaTime);
      if (velocity < 0.0 && Position == 0.0)
        velocity = 0.0f;
      if (velocity > 0.0 && Position == 1.0)
        velocity = 0.0f;
      sweetIndicator.SetActive(InSweetSpot);
      sourIndicator.SetActive(flag);
    }

    public void Bump()
    {
      if (Locked)
        return;
      velocity = settings.MouseForce;
    }

    private struct Spot
    {
      public bool Sweet;
      public float Min;
      public float Max;
      public LockPickingPinSpot View;
    }
  }
}
