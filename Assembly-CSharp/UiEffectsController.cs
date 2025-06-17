using System;
using Engine.Impl.UI.Controls;
using Inspectors;
using UnityEngine;

public class UiEffectsController : MonoBehaviourInstance<UiEffectsController>
{
  [Inspected]
  [SerializeField]
  private Info[] effects = [];

  protected override void Awake()
  {
    base.Awake();
    for (int index = 0; index < effects.Length; ++index)
    {
      Info effect = effects[index];
      if (!(effect.Progress == null) && effect.Progress.gameObject.activeSelf)
      {
        effect.Visible = true;
        effect.Value = 1f;
        effect.Time = 0.0f;
        effects[index] = effect;
      }
    }
  }

  public void SetVisible(bool visible, UiEffectType type, float time, Action action = null)
  {
    for (int index = 0; index < effects.Length; ++index)
    {
      Info effect = effects[index];
      if (effect.Type == type)
      {
        Action action1 = effect.Action;
        if (action1 != null)
          action1();
        effect.Action = action;
        effect.Visible = visible;
        effect.Time = time;
        effects[index] = effect;
        return;
      }
    }
    Debug.LogError(type + " not found");
  }

  private void Update()
  {
    for (int index = 0; index < effects.Length; ++index)
    {
      Info effect = effects[index];
      if (!(effect.Progress == null))
      {
        bool flag1 = false;
        bool activeSelf = effect.Progress.gameObject.activeSelf;
        if (effect.Visible)
        {
          if (effect.Value < 1.0)
          {
            effect.Value += Time.deltaTime * (1f / effect.Time);
            if (effect.Value >= 1.0)
            {
              flag1 = true;
              effect.Value = 1f;
            }
            effects[index] = effect;
            UpdateEffect(effect);
          }
          else if (effect.Action != null)
            flag1 = true;
        }
        else if (effect.Value > 0.0)
        {
          effect.Value -= Time.deltaTime * (1f / effect.Time);
          if (effect.Value <= 0.0)
          {
            flag1 = true;
            effect.Value = 0.0f;
          }
          effects[index] = effect;
          UpdateEffect(effect);
        }
        else if (effect.Action != null)
          flag1 = true;
        bool flag2 = effect.Value > 0.0;
        if (flag2 != activeSelf)
          effect.Progress.gameObject.SetActive(flag2);
        if (flag1 && effect.Action != null)
        {
          Action action = effect.Action;
          effect.Action = null;
          effects[index] = effect;
          action();
        }
      }
    }
  }

  private void UpdateEffect(Info effect)
  {
    effect.Progress.Progress = effect.Value;
  }

  public void AddEffect(GameObject effect) => effect.transform.SetParent(transform, false);

  public void RemoveEffect(GameObject effect)
  {
  }

  [Inspected]
  private void Show() => SetVisible(true, UiEffectType.Logo, 1f);

  [Serializable]
  public struct Info
  {
    [Inspected(Header = true)]
    public UiEffectType Type;
    [Inspected]
    public ProgressViewBase Progress;
    [Inspected]
    [NonSerialized]
    public bool Visible;
    [Inspected]
    [NonSerialized]
    public float Value;
    [Inspected]
    [NonSerialized]
    public float Time;
    [Inspected]
    [NonSerialized]
    public Action Action;
  }
}
