// Decompiled with JetBrains decompiler
// Type: Engine.Source.UI.Menu.Protagonist.LockPicking.LockPickingPin
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
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
    private LockPickingPin.Spot[] spots;
    private float position = -1f;
    private float velocity;

    public bool InSweetSpot { get; private set; }

    public bool Locked { get; set; }

    private float Position
    {
      get => this.position;
      set
      {
        value = Mathf.Clamp01(value);
        if ((double) this.position == (double) value)
          return;
        this.position = value;
        this.transform.localEulerAngles = new Vector3(0.0f, 0.0f, Mathf.Lerp(this.rotationRange.x, this.rotationRange.y, this.position));
      }
    }

    public void Build(LockPickingSettings settings)
    {
      this.Clear();
      this.settings = settings;
      LockPickingSettings.Pattern pattern = settings.Patterns[Random.Range(0, settings.Patterns.Length)];
      if (pattern.SpotCount < 1)
        pattern.SpotCount = 1;
      if (pattern.SweetSpotPosition < 0)
        pattern.SweetSpotPosition = 0;
      else if (pattern.SweetSpotPosition >= pattern.SpotCount)
        pattern.SweetSpotPosition = pattern.SpotCount - 1;
      this.spots = new LockPickingPin.Spot[pattern.SpotCount];
      float num1 = Random.value;
      float num2 = 0.0f;
      for (int index = 0; index < this.spots.Length; ++index)
      {
        bool sweet = index == pattern.SweetSpotPosition;
        this.spots[index].Sweet = sweet;
        this.spots[index].View = this.CreateView(sweet, index);
        this.spots[index].Min = Random.value;
        num1 += this.spots[index].Min;
        this.spots[index].Max = !sweet ? Random.Range(settings.SourSpotMinSize, settings.SourSpotMaxSize) : settings.SweetSpotSize;
        num2 += this.spots[index].Max;
      }
      float num3 = (1f - (float) ((double) num2 + (double) settings.LowerDeadZone + (double) settings.UpperDeadZone + (double) settings.MiddleDeadZones * (double) (pattern.SpotCount - 1))) / num1;
      for (int index = 0; index < this.spots.Length; ++index)
      {
        if (index == 0)
        {
          this.spots[index].Min = settings.LowerDeadZone + this.spots[index].Min * num3;
          this.spots[index].Max += this.spots[index].Min;
        }
        else
        {
          this.spots[index].Min = (float) ((double) this.spots[index - 1].Max + (double) settings.MiddleDeadZones + (double) this.spots[index].Min * (double) num3);
          this.spots[index].Max += this.spots[index].Min;
        }
        if (this.spots[index].Sweet)
          this.lockPosition = (float) (((double) this.spots[index].Min + (double) this.spots[index].Max) * 0.5);
        LockPickingPinSpot view = this.spots[index].View;
        view.Setup((float) (((double) this.spots[index].Max + (double) this.spots[index].Min) * 0.5), this.spots[index].Max - this.spots[index].Min);
        view.gameObject.SetActive(true);
      }
    }

    public void Clear()
    {
      this.settings = (LockPickingSettings) null;
      this.Locked = false;
      this.Position = 0.0f;
      this.velocity = 0.0f;
      this.InSweetSpot = false;
      if (this.spots == null)
        return;
      foreach (LockPickingPin.Spot spot in this.spots)
        Object.Destroy((Object) spot.View.gameObject);
      this.spots = (LockPickingPin.Spot[]) null;
    }

    public LockPickingPinSpot CreateView(bool sweet, int index)
    {
      LockPickingPinSpot original;
      if (sweet)
      {
        index %= this.sweetSpotPrototypes.Length;
        original = this.sweetSpotPrototypes[index];
      }
      else
      {
        index %= this.sourSpotPrototypes.Length;
        original = this.sourSpotPrototypes[index];
      }
      LockPickingPinSpot view = Object.Instantiate<LockPickingPinSpot>(original, original.transform.parent);
      view.transform.SetParent(original.transform.parent, false);
      return view;
    }

    private void Update()
    {
      if (this.settings == null)
        return;
      if (this.Locked)
      {
        this.Position = this.lockPosition;
        this.velocity = 0.0f;
      }
      else
      {
        this.velocity -= this.settings.GravityForce * Time.deltaTime;
        this.velocity = Mathf.Clamp(this.velocity, -this.settings.MaxVelocity, this.settings.MaxVelocity);
        this.Position += this.velocity * Time.deltaTime;
      }
      this.InSweetSpot = false;
      bool flag = false;
      for (int index = 0; index < this.spots.Length; ++index)
      {
        if ((double) this.Position > (double) this.spots[index].Min && (double) this.Position <= (double) this.spots[index].Max)
        {
          if (this.spots[index].Sweet)
            this.InSweetSpot = true;
          else
            flag = true;
        }
      }
      if (flag)
        this.velocity *= Mathf.Pow(0.5f, this.settings.SourSpotDrag * Time.deltaTime);
      if ((double) this.velocity < 0.0 && (double) this.Position == 0.0)
        this.velocity = 0.0f;
      if ((double) this.velocity > 0.0 && (double) this.Position == 1.0)
        this.velocity = 0.0f;
      this.sweetIndicator.SetActive(this.InSweetSpot);
      this.sourIndicator.SetActive(flag);
    }

    public void Bump()
    {
      if (this.Locked)
        return;
      this.velocity = this.settings.MouseForce;
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
