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
    private float[] deltas = new float[0];
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
      if (this.Complete || !(this.ui.Active is HudWindow))
        return;
      if (!this.play)
      {
        this.Play();
        this.play = true;
      }
      this.progress += Time.deltaTime;
      if ((double) this.progress > (double) this.time)
        this.Complete = true;
      else
        this.SetAlpha(SoundUtility.ComputeFade(this.progress, this.time, this.fade));
    }

    private void Play()
    {
      if ((Object) this.clip == (Object) null || (Object) this.mixer == (Object) null)
        return;
      SoundUtility.PlayAudioClip2D(this.clip, this.mixer, 1f, 0.0f, context: this.gameObject.GetFullName());
    }

    protected override void Awake()
    {
      base.Awake();
      this.ui = ServiceLocator.GetService<UIService>();
      this.SetAlpha(0.0f);
    }

    public void Initialise(NotificationEnum type, object[] values)
    {
      this.Type = type;
      int result = 0;
      this.ApplyValue<int>(ref result, values, 0);
      float num1 = 0.0f;
      int num2 = (int) Mathf.Sign((float) result);
      int index = result * num2;
      if (index >= 0 && index < this.deltas.Length)
        num1 = (float) ((double) this.deltas[index] * (double) num2 * 0.5);
      this.StartChange(0.5f - num1, 0.5f + num1);
    }

    private void StartChange(float oldReputation, float newReputation)
    {
      bool flag = (double) newReputation > (double) oldReputation;
      this.oldReputationView.Progress = Mathf.Clamp01(oldReputation);
      this.targetReputationView.Progress = newReputation;
      this.upEffect.Visible = flag;
      this.downEffect.Visible = !flag;
    }

    public void Shutdown() => Object.Destroy((Object) this.gameObject);

    private void SetAlpha(float value)
    {
      if ((double) this.alpha == (double) value)
        return;
      this.alpha = value;
      this.canvasGroup.alpha = this.alpha;
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
