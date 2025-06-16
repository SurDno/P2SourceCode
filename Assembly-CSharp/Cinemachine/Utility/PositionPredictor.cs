// Decompiled with JetBrains decompiler
// Type: Cinemachine.Utility.PositionPredictor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Cinemachine.Utility
{
  internal class PositionPredictor
  {
    private Vector3 m_Position;
    private const float kSmoothingDefault = 10f;
    private float mSmoothing = 10f;
    private GaussianWindow1D_Vector3 m_Velocity = new GaussianWindow1D_Vector3(10f);
    private GaussianWindow1D_Vector3 m_Accel = new GaussianWindow1D_Vector3(10f);

    public float Smoothing
    {
      get => this.mSmoothing;
      set
      {
        if ((double) value == (double) this.mSmoothing)
          return;
        this.mSmoothing = value;
        int maxKernelRadius = Mathf.Max(10, Mathf.FloorToInt(value * 1.5f));
        this.m_Velocity = new GaussianWindow1D_Vector3(this.mSmoothing, maxKernelRadius);
        this.m_Accel = new GaussianWindow1D_Vector3(this.mSmoothing, maxKernelRadius);
      }
    }

    public bool IsEmpty => this.m_Velocity.IsEmpty();

    public void Reset()
    {
      this.m_Velocity.Reset();
      this.m_Accel.Reset();
    }

    public void AddPosition(Vector3 pos)
    {
      if (this.IsEmpty)
      {
        this.m_Velocity.AddValue(Vector3.zero);
      }
      else
      {
        Vector3 vector3 = this.m_Velocity.Value();
        Vector3 v = (pos - this.m_Position) / Time.deltaTime;
        this.m_Velocity.AddValue(v);
        this.m_Accel.AddValue(v - vector3);
      }
      this.m_Position = pos;
    }

    public Vector3 PredictPosition(float lookaheadTime)
    {
      int num1 = Mathf.Min(Mathf.RoundToInt(lookaheadTime / Time.deltaTime), 6);
      float num2 = lookaheadTime / (float) num1;
      Vector3 position = this.m_Position;
      Vector3 fromDirection = this.m_Velocity.IsEmpty() ? Vector3.zero : this.m_Velocity.Value();
      Vector3 vector3 = this.m_Accel.IsEmpty() ? Vector3.zero : this.m_Accel.Value();
      for (int index = 0; index < num1; ++index)
      {
        position += fromDirection * num2;
        Vector3 toDirection = fromDirection + vector3 * num2;
        vector3 = Quaternion.FromToRotation(fromDirection, toDirection) * vector3;
        fromDirection = toDirection;
      }
      return position;
    }
  }
}
