using Engine.Behaviours.Localization;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI.Menu.Protagonist.HeadUpDisplay;
using Engine.Source.Audio;
using Engine.Source.Components.Regions;
using Engine.Source.Services.Notifications;
using Inspectors;
using RegionReputation;

namespace Engine.Impl.UI.Controls
{
  public class RegionNotification : UIControl, INotification
  {
    [SerializeField]
    private CanvasGroup canvasGroup;
    [SerializeField]
    private Localizer textRegion;
    [SerializeField]
    private Localizer textReputation;
    [SerializeField]
    private AudioClip clip;
    [SerializeField]
    private AudioMixerGroup mixer;
    [SerializeField]
    private float time;
    [SerializeField]
    private float fade;
    [SerializeField]
    private RegionReputationNames regionReputationNames;
    private float progress;
    private UIService ui;
    private bool play;
    private float alpha = -1f;

    [Inspected]
    public bool Complete { get; private set; }

    [Inspected]
    public NotificationEnum Type { get; private set; }

    private void Update()
    {
      if (!(ui.Active is HudWindow))
        return;
      if (!play)
      {
        Play();
        play = true;
      }
      progress += Time.deltaTime;
      if (progress > (double) time)
        Complete = true;
      else
        SetAlpha(SoundUtility.ComputeFade(progress, time, fade));
    }

    private void Play()
    {
      if ((Object) clip == (Object) null || (Object) mixer == (Object) null)
        return;
      SoundUtility.PlayAudioClip2D(clip, mixer, 1f, 0.0f, context: this.gameObject.GetFullName());
    }

    protected override void Awake()
    {
      base.Awake();
      ui = ServiceLocator.GetService<UIService>();
      SetAlpha(0.0f);
    }

    public void Initialise(NotificationEnum type, object[] values)
    {
      Type = type;
      IRegionComponent result = null;
      ApplyValue(ref result, values, 0);
      if (result == null)
      {
        Debug.LogError((object) "Notifications : Region : No region parameter");
      }
      else
      {
        float reputation = result.Reputation.Value;
        textRegion.Signature = RegionUtility.GetRegionTitle(result);
        textReputation.Signature = regionReputationNames.GetReputationName(result.Region, reputation);
      }
    }

    public void Shutdown() => Object.Destroy((Object) this.gameObject);

    private void SetAlpha(float value)
    {
      if (alpha == (double) value)
        return;
      alpha = value;
      canvasGroup.alpha = alpha;
    }

    private void ApplyValue<T>(ref T result, object[] values, int index)
    {
      if (index >= values.Length)
        return;
      object obj1 = values[index];
      if (obj1 == null || !(obj1 is T obj2))
        return;
      result = obj2;
    }
  }
}
