using Engine.Common.Commons;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI.Menu.Protagonist.HeadUpDisplay;
using Engine.Source.Audio;
using Engine.Source.Services.Notifications;
using Inspectors;

namespace Engine.Impl.UI.Controls
{
  public class Notification : UIControl, INotification
  {
    [SerializeField]
    private CanvasGroup canvasGroup;
    [SerializeField]
    private AudioClip clip;
    [SerializeField]
    private AudioMixerGroup mixer;
    [SerializeField]
    private float time;
    [SerializeField]
    private float fade;
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
      if (Complete || !(ui.Active is HudWindow))
        return;
      if (!play)
      {
        SimplePlayerWindowSwapper.SetNotificationTempWindow(Type);
        Play();
        play = true;
      }
      progress += Time.deltaTime;
      if (progress >= (double) time)
      {
        SetAlpha(0.0f);
        Complete = true;
        SimplePlayerWindowSwapper.SetNotificationTempWindow(NotificationEnum.None);
      }
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

    public void Initialise(NotificationEnum type, object[] values) => Type = type;

    public void Shutdown() => Object.Destroy((Object) this.gameObject);

    private void SetAlpha(float value)
    {
      if (alpha == (double) value)
        return;
      alpha = value;
      canvasGroup.alpha = value;
    }
  }
}
