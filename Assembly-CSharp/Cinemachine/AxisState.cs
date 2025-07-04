﻿using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Cinemachine
{
  [DocumentationSorting(6.4f, DocumentationSortingAttribute.Level.UserRef)]
  [Serializable]
  public struct AxisState(
    float maxSpeed,
    float accelTime,
    float decelTime,
    float val,
    string name,
    bool invert) {
    [NoSaveDuringPlay]
    [Tooltip("The current value of the axis.")]
    public float Value = val;
    [Tooltip("The maximum speed of this axis in units/second")]
    public float m_MaxSpeed = maxSpeed;
    [Tooltip("The amount of time in seconds it takes to accelerate to MaxSpeed with the supplied Axis at its maximum value")]
    public float m_AccelTime = accelTime;
    [Tooltip("The amount of time in seconds it takes to decelerate the axis to zero if the supplied axis is in a neutral position")]
    public float m_DecelTime = decelTime;
    [FormerlySerializedAs("m_AxisName")]
    [Tooltip("The name of this axis as specified in Unity Input manager. Setting to an empty string will disable the automatic updating of this axis")]
    public string m_InputAxisName = name;
    [NoSaveDuringPlay]
    [Tooltip("The value of the input axis.  A value of 0 means no input.  You can drive this directly from a custom input system, or you can set the Axis Name and have the value driven by the internal Input Manager")]
    public float m_InputAxisValue = 0.0f;
    [NoSaveDuringPlay]
    [Tooltip("If checked, then the raw value of the input axis will be inverted before it is used")]
    public bool m_InvertAxis = invert;
    private float mCurrentSpeed = 0.0f;
    private float mMinValue = 0.0f;
    private float mMaxValue = 0.0f;
    private bool mWrapAround = false;
    private const float Epsilon = 0.0001f;

    public void Validate()
    {
      m_MaxSpeed = Mathf.Max(0.0f, m_MaxSpeed);
      m_AccelTime = Mathf.Max(0.0f, m_AccelTime);
      m_DecelTime = Mathf.Max(0.0f, m_DecelTime);
    }

    public void SetThresholds(float minValue, float maxValue, bool wrapAround)
    {
      mMinValue = minValue;
      mMaxValue = maxValue;
      mWrapAround = wrapAround;
    }

    public bool Update(float deltaTime)
    {
      if (!string.IsNullOrEmpty(m_InputAxisName))
      {
        try
        {
          m_InputAxisValue = 0.0f;
        }
        catch (ArgumentException ex)
        {
          Debug.LogError(ex.ToString());
        }
      }
      float inputAxisValue = m_InputAxisValue;
      if (m_InvertAxis)
        inputAxisValue *= -1f;
      if (m_MaxSpeed > 9.9999997473787516E-05)
      {
        float f = inputAxisValue * m_MaxSpeed;
        if (Mathf.Abs(f) < 9.9999997473787516E-05 || Mathf.Sign(mCurrentSpeed) == (double) Mathf.Sign(f) && Mathf.Abs(f) < (double) Mathf.Abs(mCurrentSpeed))
        {
          float num = Mathf.Min(Mathf.Abs(f - mCurrentSpeed) / Mathf.Max(0.0001f, m_DecelTime) * deltaTime, Mathf.Abs(mCurrentSpeed));
          mCurrentSpeed -= Mathf.Sign(mCurrentSpeed) * num;
        }
        else
        {
          float num = Mathf.Abs(f - mCurrentSpeed) / Mathf.Max(0.0001f, m_AccelTime);
          mCurrentSpeed += Mathf.Sign(f) * num * deltaTime;
          if (Mathf.Sign(mCurrentSpeed) == (double) Mathf.Sign(f) && Mathf.Abs(mCurrentSpeed) > (double) Mathf.Abs(f))
            mCurrentSpeed = f;
        }
      }
      float maxSpeed = GetMaxSpeed();
      mCurrentSpeed = Mathf.Clamp(mCurrentSpeed, -maxSpeed, maxSpeed);
      Value += mCurrentSpeed * deltaTime;
      if (Value > (double) mMaxValue || Value < (double) mMinValue)
      {
        if (mWrapAround)
        {
          Value = Value <= (double) mMaxValue ? mMaxValue + (Value - mMinValue) : mMinValue + (Value - mMaxValue);
        }
        else
        {
          Value = Mathf.Clamp(Value, mMinValue, mMaxValue);
          mCurrentSpeed = 0.0f;
        }
      }
      return Mathf.Abs(inputAxisValue) > 9.9999997473787516E-05;
    }

    private float GetMaxSpeed()
    {
      float num1 = mMaxValue - mMinValue;
      if (!mWrapAround && num1 > 0.0)
      {
        float num2 = num1 / 10f;
        if (mCurrentSpeed > 0.0 && mMaxValue - (double) Value < num2)
          return Mathf.Lerp(0.0f, m_MaxSpeed, (mMaxValue - Value) / num2);
        if (mCurrentSpeed < 0.0 && Value - (double) mMinValue < num2)
          return Mathf.Lerp(0.0f, m_MaxSpeed, (Value - mMinValue) / num2);
      }
      return m_MaxSpeed;
    }
  }
}
