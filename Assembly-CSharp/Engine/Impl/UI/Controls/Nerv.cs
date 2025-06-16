// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.Nerv
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Localization;
using Engine.Common.Components.Parameters;
using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class Nerv : UIControl
  {
    [SerializeField]
    [FormerlySerializedAs("_Image")]
    private Image image = (Image) null;
    [SerializeField]
    private Image baseImage;
    [SerializeField]
    [FormerlySerializedAs("_Localizer")]
    private Localizer localizer;
    [SerializeField]
    [FormerlySerializedAs("_Threshold_IsEnabled")]
    private bool thresholdIsEnabled = false;
    [Range(0.0f, 1f)]
    [SerializeField]
    [FormerlySerializedAs("_Threshold_Max")]
    private float thresholdMax = 1f;
    [Range(0.0f, 1f)]
    [SerializeField]
    [FormerlySerializedAs("_Threshold_Min")]
    private float thresholdMin = 0.0f;
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

    public ParameterNameEnum Name => this.parameterName;

    private void OnValidate() => this.ApplyProgress();

    private void ApplyProgress()
    {
      if ((UnityEngine.Object) this.image != (UnityEngine.Object) null)
      {
        this.image.fillAmount = this.progress;
        this.image.color = Color.Lerp(this.minFillColor, this.maxFillColor, this.progress);
      }
      if ((UnityEngine.Object) this.baseImage != (UnityEngine.Object) null)
      {
        this.baseImage.fillAmount = 1f - this.progress;
        this.baseImage.color = Color.Lerp(this.minBaseColor, this.maxBaseColor, this.progress);
      }
      this.Threshold = (double) this.thresholdMin <= (double) this.image.fillAmount && (double) this.image.fillAmount <= (double) this.thresholdMax;
    }

    public float Progress
    {
      get => this.progress;
      set
      {
        if (Mathf.Approximately(this.progress, value))
          return;
        this.progress = value;
        this.ApplyProgress();
      }
    }

    public Localizer Localizer
    {
      get => this.localizer;
      protected set => this.localizer = value;
    }

    public bool Threshold
    {
      get
      {
        if (!this.thresholdIsEnabled)
          this.threshold = false;
        return this.threshold;
      }
      protected set
      {
        if (this.threshold == value)
          return;
        this.threshold = this.thresholdIsEnabled && value;
        Action<Nerv> thresholdEvent = this.ThresholdEvent;
        if (thresholdEvent == null)
          return;
        thresholdEvent(this);
      }
    }

    public event Action<Nerv> ThresholdEvent;

    public static Nerv Instantiate(GameObject prefab)
    {
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab);
      gameObject.name = "[UI.Control] " + prefab.name;
      return gameObject.GetComponent<Nerv>();
    }
  }
}
