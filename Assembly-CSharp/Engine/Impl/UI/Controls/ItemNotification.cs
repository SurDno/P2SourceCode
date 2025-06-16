using System;
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
      if (value == (double) position)
        return;
      position = value;
      ((RectTransform) transform).anchoredPosition = position * step;
    }

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
      if (progress >= (double) fadeIn)
        Complete = true;
      if (progress >= (double) time && shutdown)
      {
        Destroy(gameObject);
      }
      else
      {
        canvasGroup.alpha = SoundUtility.ComputeFade(progress, time, fadeIn, fadeOut);
        SetPosition(Mathf.MoveTowards(position, targetPosition, Time.deltaTime / fadeIn));
      }
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
      Action notificationEvent = NewNotificationEvent;
      if (notificationEvent != null)
        notificationEvent();
      NewNotificationEvent += NewNotificationListener;
    }

    private void OnDestroy()
    {
      NewNotificationEvent -= NewNotificationListener;
    }

    public void NewNotificationListener() => ++targetPosition;

    public void Initialise(NotificationEnum type, object[] values)
    {
      Type = type;
      IEntity result = null;
      ApplyValue(ref result, values, 0);
      if (result == null)
        return;
      StorableComponent component = result.GetComponent<StorableComponent>();
      if (component == null)
        return;
      InventoryPlaceholder placeholder = component.Placeholder;
      if (placeholder == null)
        return;
      image.sprite = placeholder.ImageInventorySlot.Value;
      image.gameObject.SetActive(true);
      canvasGroup.alpha = 0.0f;
      SetPosition(-1f);
      if (!(clip == null))
        return;
      clip = placeholder.SoundGroup?.GetTakeClip();
      if (clip == null)
        clip = ScriptableObjectInstance<ResourceFromCodeData>.Instance?.DefaultItemSoundGroup?.GetTakeClip();
    }

    public void Shutdown() => shutdown = true;

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
