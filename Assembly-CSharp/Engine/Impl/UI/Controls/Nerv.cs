using System;
using Engine.Behaviours.Localization;
using Engine.Common.Components.Parameters;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Engine.Impl.UI.Controls
{
  public class Nerv : UIControl
  {
    [SerializeField]
    [FormerlySerializedAs("_Image")]
    private Image image;
    [SerializeField]
    private Image baseImage;
    [SerializeField]
    [FormerlySerializedAs("_Localizer")]
    private Localizer localizer;
    [SerializeField]
    [FormerlySerializedAs("_Threshold_IsEnabled")]
    private bool thresholdIsEnabled;
    [Range(0.0f, 1f)]
    [SerializeField]
    [FormerlySerializedAs("_Threshold_Max")]
    private float thresholdMax = 1f;
    [Range(0.0f, 1f)]
    [SerializeField]
    [FormerlySerializedAs("_Threshold_Min")]
    private float thresholdMin;
    [SerializeField]
    private ParameterNameEnum parameterName;
    [SerializeField]
    private Color minBaseColor;
    [SerializeField]
    private Color minFillColor;
    [SerializeField]
    private Color maxBaseColor;
    [SerializeField]
    private Color maxFillColor;
    private float progress = 0.5f;
    private bool threshold;

    public ParameterNameEnum Name => parameterName;

    private void OnValidate() => ApplyProgress();

    private void ApplyProgress()
    {
      if (image != null)
      {
        image.fillAmount = progress;
        image.color = Color.Lerp(minFillColor, maxFillColor, progress);
      }
      if (baseImage != null)
      {
        baseImage.fillAmount = 1f - progress;
        baseImage.color = Color.Lerp(minBaseColor, maxBaseColor, progress);
      }
      Threshold = thresholdMin <= (double) image.fillAmount && image.fillAmount <= (double) thresholdMax;
    }

    public float Progress
    {
      get => progress;
      set
      {
        if (Mathf.Approximately(progress, value))
          return;
        progress = value;
        ApplyProgress();
      }
    }

    public Localizer Localizer
    {
      get => localizer;
      protected set => localizer = value;
    }

    public bool Threshold
    {
      get
      {
        if (!thresholdIsEnabled)
          threshold = false;
        return threshold;
      }
      protected set
      {
        if (threshold == value)
          return;
        threshold = thresholdIsEnabled && value;
        Action<Nerv> thresholdEvent = ThresholdEvent;
        if (thresholdEvent == null)
          return;
        thresholdEvent(this);
      }
    }

    public event Action<Nerv> ThresholdEvent;

    public static Nerv Instantiate(GameObject prefab)
    {
      GameObject gameObject = Object.Instantiate(prefab);
      gameObject.name = "[UI.Control] " + prefab.name;
      return gameObject.GetComponent<Nerv>();
    }
  }
}
