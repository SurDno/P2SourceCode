using UnityEngine;

namespace Cinemachine.Utility
{
  internal class PositionPredictor
  {
    private Vector3 m_Position;
    private const float kSmoothingDefault = 10f;
    private float mSmoothing = 10f;
    private GaussianWindow1D_Vector3 m_Velocity = new(10f);
    private GaussianWindow1D_Vector3 m_Accel = new(10f);

    public float Smoothing
    {
      get => mSmoothing;
      set
      {
        if (value == (double) mSmoothing)
          return;
        mSmoothing = value;
        int maxKernelRadius = Mathf.Max(10, Mathf.FloorToInt(value * 1.5f));
        m_Velocity = new GaussianWindow1D_Vector3(mSmoothing, maxKernelRadius);
        m_Accel = new GaussianWindow1D_Vector3(mSmoothing, maxKernelRadius);
      }
    }

    public bool IsEmpty => m_Velocity.IsEmpty();

    public void Reset()
    {
      m_Velocity.Reset();
      m_Accel.Reset();
    }

    public void AddPosition(Vector3 pos)
    {
      if (IsEmpty)
      {
        m_Velocity.AddValue(Vector3.zero);
      }
      else
      {
        Vector3 vector3 = m_Velocity.Value();
        Vector3 v = (pos - m_Position) / Time.deltaTime;
        m_Velocity.AddValue(v);
        m_Accel.AddValue(v - vector3);
      }
      m_Position = pos;
    }

    public Vector3 PredictPosition(float lookaheadTime)
    {
      int num1 = Mathf.Min(Mathf.RoundToInt(lookaheadTime / Time.deltaTime), 6);
      float num2 = lookaheadTime / num1;
      Vector3 position = m_Position;
      Vector3 fromDirection = m_Velocity.IsEmpty() ? Vector3.zero : m_Velocity.Value();
      Vector3 vector3 = m_Accel.IsEmpty() ? Vector3.zero : m_Accel.Value();
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
