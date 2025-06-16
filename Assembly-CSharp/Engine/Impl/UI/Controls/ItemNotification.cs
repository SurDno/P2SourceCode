using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI.Menu.Protagonist.HeadUpDisplay;
using Engine.Source.Audio;
using Engine.Source.Components;
using Engine.Source.Inventory;
using Engine.Source.Services.Notifications;
using Inspectors;
using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Engine.Impl.UI.Controls
{
  public class ItemNotification : UIControl, INotification
  {
    private static Action NewNotificationEvent;
    [SerializeField]
    private CanvasGroup canvasGroup;
    [SerializeField]
    private Image image;
    [Space]
    [SerializeField]
    private AudioClip clip;
    [SerializeField]
    private AudioMixerGroup mixer;
    [Space]
    [SerializeField]
    private float time;
    [SerializeField]
    private float fadeIn;
    [SerializeField]
    private float fadeOut;
    [SerializeField]
    private Vector2 step;
    private float progress;
    private bool shutdown;
    private UIService ui;
    private bool play;
    private float position;
    private int targetPosition;

    [Inspected]
    public bool Complete { get; private set; }

    [Inspected]
    public NotificationEnum Type { get; private set; }

    private void SetPosition(float value)
    {
      if ((double) value == (double) this.position)
        return;
      this.position = value;
      ((RectTransform) this.transform).anchoredPosition = this.position * this.step;
    }

    private void Update()
    {
      if (!(this.ui.Active is HudWindow))
        return;
      if (!this.play)
      {
        this.Play();
        this.play = true;
      }
      this.progress += Time.deltaTime;
      if ((double) this.progress >= (double) this.fadeIn)
        this.Complete = true;
      if ((double) this.progress >= (double) this.time && this.shutdown)
      {
        UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
      }
      else
      {
        this.canvasGroup.alpha = SoundUtility.ComputeFade(this.progress, this.time, this.fadeIn, this.fadeOut);
        this.SetPosition(Mathf.MoveTowards(this.position, (float) this.targetPosition, Time.deltaTime / this.fadeIn));
      }
    }

    private void Play()
    {
      if ((UnityEngine.Object) this.clip == (UnityEngine.Object) null || (UnityEngine.Object) this.mixer == (UnityEngine.Object) null)
        return;
      SoundUtility.PlayAudioClip2D(this.clip, this.mixer, 1f, 0.0f, context: this.gameObject.GetFullName());
    }

    protected override void Awake()
    {
      base.Awake();
      this.ui = ServiceLocator.GetService<UIService>();
      Action notificationEvent = ItemNotification.NewNotificationEvent;
      if (notificationEvent != null)
        notificationEvent();
      ItemNotification.NewNotificationEvent += new Action(this.NewNotificationListener);
    }

    private void OnDestroy()
    {
      ItemNotification.NewNotificationEvent -= new Action(this.NewNotificationListener);
    }

    public void NewNotificationListener() => ++this.targetPosition;

    public void Initialise(NotificationEnum type, object[] values)
    {
      this.Type = type;
      IEntity result = (IEntity) null;
      this.ApplyValue<IEntity>(ref result, values, 0);
      if (result == null)
        return;
      StorableComponent component = result.GetComponent<StorableComponent>();
      if (component == null)
        return;
      InventoryPlaceholder placeholder = component.Placeholder;
      if (placeholder == null)
        return;
      this.image.sprite = placeholder.ImageInventorySlot.Value;
      this.image.gameObject.SetActive(true);
      this.canvasGroup.alpha = 0.0f;
      this.SetPosition(-1f);
      if (!((UnityEngine.Object) this.clip == (UnityEngine.Object) null))
        return;
      this.clip = placeholder.SoundGroup?.GetTakeClip();
      if ((UnityEngine.Object) this.clip == (UnityEngine.Object) null)
        this.clip = ScriptableObjectInstance<ResourceFromCodeData>.Instance?.DefaultItemSoundGroup?.GetTakeClip();
    }

    public void Shutdown() => this.shutdown = true;

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
