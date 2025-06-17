using Engine.Behaviours.Localization;
using Engine.Common.Commons;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI.Menu.Protagonist.HeadUpDisplay;
using Engine.Source.Audio;
using Engine.Source.Services.Notifications;
using Inspectors;
using UnityEngine;
using UnityEngine.Audio;

namespace Engine.Impl.UI.Controls
{
  public class FoundationNotification : UIControl, INotification
  {
    [SerializeField]
    private CanvasGroup canvasGroup;
    [SerializeField]
    private Localizer textRegion;
    [SerializeField]
    private ProgressView targetReputationView;
    [SerializeField]
    private ProgressView oldReputationView;
    [SerializeField]
    private HideableView downEffect;
    [SerializeField]
    private HideableView upEffect;
    [SerializeField]
    private AudioClip clip;
    [SerializeField]
    private AudioMixerGroup mixer;
    [SerializeField]
    private float time;
    [SerializeField]
    private float fade;
    [SerializeField]
    private float[] deltas = [];
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
      if (clip == null || mixer == null)
        return;
      SoundUtility.PlayAudioClip2D(clip, mixer, 1f, 0.0f, context: gameObject.GetFullName());
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
      int result = 0;
      ApplyValue(ref result, values, 0);
      float num1 = 0.0f;
      int num2 = (int) Mathf.Sign(result);
      int index = result * num2;
      if (index >= 0 && index < deltas.Length)
        num1 = (float) (deltas[index] * (double) num2 * 0.5);
      StartChange(0.5f - num1, 0.5f + num1);
    }

    private void StartChange(float oldReputation, float newReputation)
    {
      bool flag = newReputation > (double) oldReputation;
      oldReputationView.Progress = Mathf.Clamp01(oldReputation);
      targetReputationView.Progress = newReputation;
      upEffect.Visible = flag;
      downEffect.Visible = !flag;
    }

    public void Shutdown() => Destroy(gameObject);

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
