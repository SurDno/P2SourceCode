using Engine.Common.Commons;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI.Menu.Protagonist.HeadUpDisplay;
using Engine.Source.Audio;
using Engine.Source.Services.Notifications;
using Inspectors;
using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class SimpleNotification : UIControl, INotification
  {
    [SerializeField]
    private CanvasGroup canvasGroup;
    [SerializeField]
    private float time;
    [SerializeField]
    private float fade;
    private float progress;
    private UIService ui;
    private float alpha = -1f;

    [Inspected]
    public bool Complete { get; private set; }

    [Inspected]
    public NotificationEnum Type { get; private set; }

    private void Update()
    {
      if (!(this.ui.Active is HudWindow))
        return;
      this.progress += Time.deltaTime;
      if ((double) this.progress > (double) this.time)
        this.Complete = true;
      else
        this.SetAlpha(SoundUtility.ComputeFade(this.progress, this.time, this.fade));
    }

    protected override void Awake()
    {
      base.Awake();
      this.ui = ServiceLocator.GetService<UIService>();
      this.SetAlpha(0.0f);
    }

    public void Initialise(NotificationEnum type, object[] values) => this.Type = type;

    public void Shutdown() => Object.Destroy((Object) this.gameObject);

    private void SetAlpha(float value)
    {
      if ((double) this.alpha == (double) value)
        return;
      this.alpha = value;
      this.canvasGroup.alpha = value;
    }
  }
}
