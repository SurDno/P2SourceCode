using Engine.Behaviours.Localization;
using Engine.Common.Components.Parameters;
using Engine.Impl.UI.Controls;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Engine.Impl.UI.Menu.Protagonist.HeadUpDisplay
{
  public class HUDStats : MonoBehaviour
  {
    [SerializeField]
    [FormerlySerializedAs("FadedLayer")]
    private CanvasGroup fadedLayer;
    [SerializeField]
    [FormerlySerializedAs("SplashLayer")]
    private CanvasGroup splashLayer;
    [SerializeField]
    [FormerlySerializedAs("NamePrototype")]
    private Localizer namePrototype;
    [SerializeField]
    [FormerlySerializedAs("BarPrototype")]
    private StatBar barPrototype;
    [SerializeField]
    [FormerlySerializedAs("LocalizerPrefix")]
    private string localizerPrefix;
    [SerializeField]
    [FormerlySerializedAs("LocalizerSuffix")]
    private string localizerSuffix;
    [Space]
    [SerializeField]
    [FormerlySerializedAs("Stats")]
    private HUDStats.Stat[] stats;
    [Header("Layout")]
    [SerializeField]
    [FormerlySerializedAs("FirstNamePosition")]
    private Vector2 firstNamePosition;
    [SerializeField]
    [FormerlySerializedAs("FirstBarPosition")]
    private Vector2 firstBarPosition;
    [SerializeField]
    [FormerlySerializedAs("HeightStep")]
    private float heightStep;
    [Header("Animation")]
    [SerializeField]
    [FormerlySerializedAs("AlertedValueChange")]
    private float alertedValueChange = 0.2f;
    [SerializeField]
    [FormerlySerializedAs("ShowTime")]
    private float showTime = 0.25f;
    [SerializeField]
    [FormerlySerializedAs("WaitTime")]
    private float waitTime = 1f;
    [SerializeField]
    [FormerlySerializedAs("ReturnTime")]
    private float returnTime = 1.5f;
    [SerializeField]
    [FormerlySerializedAs("BarScale")]
    private float barScale = 2f;
    [SerializeField]
    [FormerlySerializedAs("BarShift")]
    private float barShift = -20f;
    [SerializeField]
    [FormerlySerializedAs("FadedLayerOpacity")]
    private float fadedLayerOpacity = 0.25f;
    private Dictionary<ParameterNameEnum, int> statMap = new Dictionary<ParameterNameEnum, int>();
    private CanvasGroup[] nameCanvasGroup;
    private CanvasGroup[] barCanvasGroup;
    private StatBar[] bars;
    private bool[] visibility;
    private HUDStats.Animation currentAnimation = HUDStats.Animation.Idle;
    private int animatedStatIndex;
    private float animationPhase;

    private void Start()
    {
      this.nameCanvasGroup = new CanvasGroup[this.stats.Length];
      this.barCanvasGroup = new CanvasGroup[this.stats.Length];
      this.bars = new StatBar[this.stats.Length];
      this.visibility = new bool[this.stats.Length];
      for (int index = 0; index < this.stats.Length; ++index)
      {
        this.statMap[this.stats[index].Name] = index;
        GameObject gameObject1 = UnityEngine.Object.Instantiate<GameObject>(this.namePrototype.gameObject);
        gameObject1.name = this.namePrototype.name + " " + (object) this.stats[index].Name;
        gameObject1.transform.SetParent(this.fadedLayer.transform, false);
        this.nameCanvasGroup[index] = gameObject1.GetComponent<CanvasGroup>();
        string str = this.localizerPrefix + (object) this.stats[index].Name + this.localizerSuffix;
        gameObject1.GetComponent<Text>().text = this.stats[index].Name.ToString();
        gameObject1.GetComponent<Localizer>().Signature = str;
        gameObject1.SetActive(false);
        GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(this.barPrototype.gameObject);
        gameObject2.name = this.barPrototype.name + " " + (object) this.stats[index].Name;
        gameObject2.transform.SetParent(this.fadedLayer.transform, false);
        this.barCanvasGroup[index] = gameObject2.GetComponent<CanvasGroup>();
        this.bars[index] = gameObject2.GetComponent<StatBar>();
        this.bars[index].Image = this.stats[index].Image;
        this.bars[index].IsReversed = this.stats[index].IsReversed;
        this.bars[index].CurrentValue = this.stats[index].Value;
        this.bars[index].TailValue = this.stats[index].Value;
        gameObject2.SetActive(false);
      }
    }

    public void TrySetValue(ParameterNameEnum name, float value)
    {
      int index;
      if (!this.statMap.TryGetValue(name, out index))
        return;
      this.stats[index].Value = Mathf.Clamp01(value);
    }

    public void TrySetThreshold(ParameterNameEnum name, float value)
    {
      int index;
      if (!this.statMap.TryGetValue(name, out index))
        return;
      this.stats[index].VisibilityThreshold = Mathf.Clamp01(value);
    }

    private void Update()
    {
      switch (this.currentAnimation)
      {
        case HUDStats.Animation.Idle:
          for (int index = 0; index < this.stats.Length; ++index)
          {
            if (this.visibility[index] && (double) Mathf.Abs(this.bars[index].CurrentValue - this.stats[index].Value) >= (double) this.alertedValueChange)
            {
              this.currentAnimation = HUDStats.Animation.Alert;
              this.animatedStatIndex = index;
              this.animationPhase = 0.0f;
              this.barCanvasGroup[index].transform.SetParent(this.transform);
              this.nameCanvasGroup[index].transform.SetParent(this.transform);
              break;
            }
            bool flag = this.stats[index].IsReversed ? (double) this.stats[index].Value > (double) this.stats[index].VisibilityThreshold : (double) this.stats[index].Value < (double) this.stats[index].VisibilityThreshold;
            if (flag && !this.visibility[index])
            {
              this.visibility[index] = true;
              this.currentAnimation = HUDStats.Animation.Show;
              this.animatedStatIndex = index;
              this.animationPhase = 0.0f;
              this.barCanvasGroup[index].alpha = 0.0f;
              this.nameCanvasGroup[index].alpha = 0.0f;
              this.barCanvasGroup[index].gameObject.SetActive(true);
              this.nameCanvasGroup[index].gameObject.SetActive(true);
              this.barCanvasGroup[index].transform.SetParent(this.transform);
              this.nameCanvasGroup[index].transform.SetParent(this.transform);
              break;
            }
            if (!flag && this.visibility[index])
            {
              this.currentAnimation = HUDStats.Animation.Hide;
              this.animatedStatIndex = index;
              this.animationPhase = 0.0f;
              break;
            }
          }
          if (this.currentAnimation != HUDStats.Animation.Idle)
            break;
          for (int index = 0; index < this.stats.Length; ++index)
            this.bars[index].CurrentValue = this.stats[index].Value;
          break;
        case HUDStats.Animation.Show:
          this.animationPhase += Time.unscaledDeltaTime;
          float visibilityPhase1 = 1f;
          float scalePhase1 = 0.0f;
          if ((double) this.animationPhase < (double) this.showTime)
          {
            scalePhase1 = 1f;
            visibilityPhase1 = Mathf.SmoothStep(0.0f, 1f, this.animationPhase / this.showTime);
          }
          else if ((double) this.animationPhase < (double) this.showTime + (double) this.waitTime)
          {
            scalePhase1 = 1f;
            visibilityPhase1 = 1f;
          }
          else if ((double) this.animationPhase < (double) this.showTime + (double) this.waitTime + (double) this.returnTime)
          {
            scalePhase1 = Mathf.SmoothStep(1f, 0.0f, (this.animationPhase - this.showTime - this.waitTime) / this.returnTime);
            visibilityPhase1 = 1f;
          }
          else
          {
            this.barCanvasGroup[this.animatedStatIndex].transform.SetParent(this.fadedLayer.transform, false);
            this.nameCanvasGroup[this.animatedStatIndex].transform.SetParent(this.fadedLayer.transform, false);
            this.currentAnimation = HUDStats.Animation.Idle;
          }
          this.UpdateLayout(visibilityPhase1, scalePhase1);
          this.bars[this.animatedStatIndex].CurrentValue = this.stats[this.animatedStatIndex].Value;
          break;
        case HUDStats.Animation.Alert:
          this.animationPhase += Time.unscaledDeltaTime;
          float visibilityPhase2 = 1f;
          float scalePhase2 = 0.0f;
          if ((double) this.animationPhase < (double) this.showTime)
            scalePhase2 = Mathf.SmoothStep(0.0f, 1f, this.animationPhase / this.showTime);
          else if ((double) this.animationPhase < (double) this.showTime + (double) this.waitTime)
            scalePhase2 = 1f;
          else if ((double) this.animationPhase < (double) this.showTime + (double) this.waitTime + (double) this.returnTime)
          {
            scalePhase2 = Mathf.SmoothStep(1f, 0.0f, (this.animationPhase - this.showTime - this.waitTime) / this.returnTime);
          }
          else
          {
            this.barCanvasGroup[this.animatedStatIndex].transform.SetParent(this.fadedLayer.transform, false);
            this.nameCanvasGroup[this.animatedStatIndex].transform.SetParent(this.fadedLayer.transform, false);
            this.currentAnimation = HUDStats.Animation.Idle;
          }
          this.UpdateLayout(visibilityPhase2, scalePhase2);
          this.bars[this.animatedStatIndex].CurrentValue = this.stats[this.animatedStatIndex].Value;
          break;
        case HUDStats.Animation.Hide:
          this.animationPhase += Time.unscaledDeltaTime;
          float visibilityPhase3 = 0.0f;
          float scalePhase3 = 0.0f;
          if ((double) this.animationPhase < (double) this.waitTime)
          {
            visibilityPhase3 = Mathf.SmoothStep(1f, 0.0f, this.animationPhase / this.waitTime);
          }
          else
          {
            this.visibility[this.animatedStatIndex] = false;
            this.barCanvasGroup[this.animatedStatIndex].gameObject.SetActive(false);
            this.nameCanvasGroup[this.animatedStatIndex].gameObject.SetActive(false);
            this.currentAnimation = HUDStats.Animation.Idle;
          }
          this.UpdateLayout(visibilityPhase3, scalePhase3);
          this.bars[this.animatedStatIndex].CurrentValue = this.stats[this.animatedStatIndex].Value;
          break;
      }
    }

    public void UpdateLayout(float visibilityPhase, float scalePhase)
    {
      Vector2 firstNamePosition = this.firstNamePosition;
      Vector2 firstBarPosition = this.firstBarPosition;
      for (int index = 0; index < this.visibility.Length; ++index)
      {
        if (this.visibility[index])
        {
          this.nameCanvasGroup[index].GetComponent<RectTransform>().anchoredPosition = firstNamePosition;
          RectTransform component = this.barCanvasGroup[index].GetComponent<RectTransform>();
          if (index == this.animatedStatIndex)
          {
            float t = scalePhase * scalePhase;
            float num1 = 1f - scalePhase;
            float num2 = (float) (1.0 - (double) num1 * (double) num1);
            float num3 = Mathf.Lerp(1f, this.barScale, t);
            component.anchoredPosition = firstBarPosition + new Vector2(0.0f, this.barShift * num2);
            component.localScale = new Vector3(num3, num3, num3);
            this.nameCanvasGroup[index].alpha = visibilityPhase;
            this.barCanvasGroup[index].alpha = visibilityPhase;
          }
          else
          {
            component.anchoredPosition = firstBarPosition;
            component.localScale = Vector3.one;
            this.nameCanvasGroup[index].alpha = 1f;
            this.barCanvasGroup[index].alpha = 1f;
            component.anchoredPosition = firstBarPosition;
          }
          if (index == this.animatedStatIndex)
          {
            firstNamePosition.y += this.heightStep * visibilityPhase;
            firstBarPosition.y += this.heightStep * visibilityPhase;
          }
          else
          {
            firstNamePosition.y += this.heightStep;
            firstBarPosition.y += this.heightStep;
          }
        }
      }
      if ((double) scalePhase > 0.0)
      {
        this.fadedLayer.alpha = Mathf.Lerp(1f, this.fadedLayerOpacity, scalePhase * visibilityPhase);
        this.splashLayer.alpha = scalePhase * visibilityPhase;
        this.splashLayer.gameObject.SetActive(true);
      }
      else
      {
        this.fadedLayer.alpha = 1f;
        this.splashLayer.gameObject.SetActive(false);
      }
    }

    private enum Animation
    {
      Idle,
      Show,
      Alert,
      Hide,
    }

    [Serializable]
    public class Stat
    {
      public ParameterNameEnum Name;
      public Sprite Image;
      public bool IsReversed;
      [Range(0.0f, 1f)]
      public float VisibilityThreshold = 0.5f;
      [Range(0.0f, 1f)]
      public float Value = 0.5f;
    }
  }
}
