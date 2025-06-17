using System;
using System.Collections.Generic;
using Engine.Behaviours.Localization;
using Engine.Common.Components.Parameters;
using Engine.Impl.UI.Controls;
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
    private Stat[] stats;
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
    private Dictionary<ParameterNameEnum, int> statMap = new();
    private CanvasGroup[] nameCanvasGroup;
    private CanvasGroup[] barCanvasGroup;
    private StatBar[] bars;
    private bool[] visibility;
    private Animation currentAnimation = Animation.Idle;
    private int animatedStatIndex;
    private float animationPhase;

    private void Start()
    {
      nameCanvasGroup = new CanvasGroup[stats.Length];
      barCanvasGroup = new CanvasGroup[stats.Length];
      bars = new StatBar[stats.Length];
      visibility = new bool[stats.Length];
      for (int index = 0; index < stats.Length; ++index)
      {
        statMap[stats[index].Name] = index;
        GameObject gameObject1 = Instantiate(namePrototype.gameObject);
        gameObject1.name = namePrototype.name + " " + stats[index].Name;
        gameObject1.transform.SetParent(fadedLayer.transform, false);
        nameCanvasGroup[index] = gameObject1.GetComponent<CanvasGroup>();
        string str = localizerPrefix + stats[index].Name + localizerSuffix;
        gameObject1.GetComponent<Text>().text = stats[index].Name.ToString();
        gameObject1.GetComponent<Localizer>().Signature = str;
        gameObject1.SetActive(false);
        GameObject gameObject2 = Instantiate(barPrototype.gameObject);
        gameObject2.name = barPrototype.name + " " + stats[index].Name;
        gameObject2.transform.SetParent(fadedLayer.transform, false);
        barCanvasGroup[index] = gameObject2.GetComponent<CanvasGroup>();
        bars[index] = gameObject2.GetComponent<StatBar>();
        bars[index].Image = stats[index].Image;
        bars[index].IsReversed = stats[index].IsReversed;
        bars[index].CurrentValue = stats[index].Value;
        bars[index].TailValue = stats[index].Value;
        gameObject2.SetActive(false);
      }
    }

    public void TrySetValue(ParameterNameEnum name, float value)
    {
      if (!statMap.TryGetValue(name, out int index))
        return;
      stats[index].Value = Mathf.Clamp01(value);
    }

    public void TrySetThreshold(ParameterNameEnum name, float value)
    {
      if (!statMap.TryGetValue(name, out int index))
        return;
      stats[index].VisibilityThreshold = Mathf.Clamp01(value);
    }

    private void Update()
    {
      switch (currentAnimation)
      {
        case Animation.Idle:
          for (int index = 0; index < stats.Length; ++index)
          {
            if (visibility[index] && Mathf.Abs(bars[index].CurrentValue - stats[index].Value) >= (double) alertedValueChange)
            {
              currentAnimation = Animation.Alert;
              animatedStatIndex = index;
              animationPhase = 0.0f;
              barCanvasGroup[index].transform.SetParent(transform);
              nameCanvasGroup[index].transform.SetParent(transform);
              break;
            }
            bool flag = stats[index].IsReversed ? stats[index].Value > (double) stats[index].VisibilityThreshold : stats[index].Value < (double) stats[index].VisibilityThreshold;
            if (flag && !visibility[index])
            {
              visibility[index] = true;
              currentAnimation = Animation.Show;
              animatedStatIndex = index;
              animationPhase = 0.0f;
              barCanvasGroup[index].alpha = 0.0f;
              nameCanvasGroup[index].alpha = 0.0f;
              barCanvasGroup[index].gameObject.SetActive(true);
              nameCanvasGroup[index].gameObject.SetActive(true);
              barCanvasGroup[index].transform.SetParent(transform);
              nameCanvasGroup[index].transform.SetParent(transform);
              break;
            }
            if (!flag && visibility[index])
            {
              currentAnimation = Animation.Hide;
              animatedStatIndex = index;
              animationPhase = 0.0f;
              break;
            }
          }
          if (currentAnimation != Animation.Idle)
            break;
          for (int index = 0; index < stats.Length; ++index)
            bars[index].CurrentValue = stats[index].Value;
          break;
        case Animation.Show:
          animationPhase += Time.unscaledDeltaTime;
          float visibilityPhase1 = 1f;
          float scalePhase1 = 0.0f;
          if (animationPhase < (double) showTime)
          {
            scalePhase1 = 1f;
            visibilityPhase1 = Mathf.SmoothStep(0.0f, 1f, animationPhase / showTime);
          }
          else if (animationPhase < showTime + (double) waitTime)
          {
            scalePhase1 = 1f;
            visibilityPhase1 = 1f;
          }
          else if (animationPhase < showTime + (double) waitTime + returnTime)
          {
            scalePhase1 = Mathf.SmoothStep(1f, 0.0f, (animationPhase - showTime - waitTime) / returnTime);
            visibilityPhase1 = 1f;
          }
          else
          {
            barCanvasGroup[animatedStatIndex].transform.SetParent(fadedLayer.transform, false);
            nameCanvasGroup[animatedStatIndex].transform.SetParent(fadedLayer.transform, false);
            currentAnimation = Animation.Idle;
          }
          UpdateLayout(visibilityPhase1, scalePhase1);
          bars[animatedStatIndex].CurrentValue = stats[animatedStatIndex].Value;
          break;
        case Animation.Alert:
          animationPhase += Time.unscaledDeltaTime;
          float visibilityPhase2 = 1f;
          float scalePhase2 = 0.0f;
          if (animationPhase < (double) showTime)
            scalePhase2 = Mathf.SmoothStep(0.0f, 1f, animationPhase / showTime);
          else if (animationPhase < showTime + (double) waitTime)
            scalePhase2 = 1f;
          else if (animationPhase < showTime + (double) waitTime + returnTime)
          {
            scalePhase2 = Mathf.SmoothStep(1f, 0.0f, (animationPhase - showTime - waitTime) / returnTime);
          }
          else
          {
            barCanvasGroup[animatedStatIndex].transform.SetParent(fadedLayer.transform, false);
            nameCanvasGroup[animatedStatIndex].transform.SetParent(fadedLayer.transform, false);
            currentAnimation = Animation.Idle;
          }
          UpdateLayout(visibilityPhase2, scalePhase2);
          bars[animatedStatIndex].CurrentValue = stats[animatedStatIndex].Value;
          break;
        case Animation.Hide:
          animationPhase += Time.unscaledDeltaTime;
          float visibilityPhase3 = 0.0f;
          float scalePhase3 = 0.0f;
          if (animationPhase < (double) waitTime)
          {
            visibilityPhase3 = Mathf.SmoothStep(1f, 0.0f, animationPhase / waitTime);
          }
          else
          {
            visibility[animatedStatIndex] = false;
            barCanvasGroup[animatedStatIndex].gameObject.SetActive(false);
            nameCanvasGroup[animatedStatIndex].gameObject.SetActive(false);
            currentAnimation = Animation.Idle;
          }
          UpdateLayout(visibilityPhase3, scalePhase3);
          bars[animatedStatIndex].CurrentValue = stats[animatedStatIndex].Value;
          break;
      }
    }

    public void UpdateLayout(float visibilityPhase, float scalePhase)
    {
      Vector2 firstNamePosition = this.firstNamePosition;
      Vector2 firstBarPosition = this.firstBarPosition;
      for (int index = 0; index < visibility.Length; ++index)
      {
        if (visibility[index])
        {
          nameCanvasGroup[index].GetComponent<RectTransform>().anchoredPosition = firstNamePosition;
          RectTransform component = barCanvasGroup[index].GetComponent<RectTransform>();
          if (index == animatedStatIndex)
          {
            float t = scalePhase * scalePhase;
            float num1 = 1f - scalePhase;
            float num2 = (float) (1.0 - num1 * (double) num1);
            float num3 = Mathf.Lerp(1f, barScale, t);
            component.anchoredPosition = firstBarPosition + new Vector2(0.0f, barShift * num2);
            component.localScale = new Vector3(num3, num3, num3);
            nameCanvasGroup[index].alpha = visibilityPhase;
            barCanvasGroup[index].alpha = visibilityPhase;
          }
          else
          {
            component.anchoredPosition = firstBarPosition;
            component.localScale = Vector3.one;
            nameCanvasGroup[index].alpha = 1f;
            barCanvasGroup[index].alpha = 1f;
            component.anchoredPosition = firstBarPosition;
          }
          if (index == animatedStatIndex)
          {
            firstNamePosition.y += heightStep * visibilityPhase;
            firstBarPosition.y += heightStep * visibilityPhase;
          }
          else
          {
            firstNamePosition.y += heightStep;
            firstBarPosition.y += heightStep;
          }
        }
      }
      if (scalePhase > 0.0)
      {
        fadedLayer.alpha = Mathf.Lerp(1f, fadedLayerOpacity, scalePhase * visibilityPhase);
        splashLayer.alpha = scalePhase * visibilityPhase;
        splashLayer.gameObject.SetActive(true);
      }
      else
      {
        fadedLayer.alpha = 1f;
        splashLayer.gameObject.SetActive(false);
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
