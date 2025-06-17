using System;
using UnityEngine;

namespace Cinemachine
{
  [DocumentationSorting(9f, DocumentationSortingAttribute.Level.UserRef)]
  public sealed class NoiseSettings : ScriptableObject
  {
    [SerializeField]
    [Tooltip("These are the noise channels for the virtual camera's position. Convincing noise setups typically mix low, medium and high frequencies together, so start with a size of 3")]
    private TransformNoiseParams[] m_Position = [];
    [SerializeField]
    [Tooltip("These are the noise channels for the virtual camera's orientation. Convincing noise setups typically mix low, medium and high frequencies together, so start with a size of 3")]
    private TransformNoiseParams[] m_Orientation = [];

    public TransformNoiseParams[] PositionNoise => m_Position;

    public TransformNoiseParams[] OrientationNoise => m_Orientation;

    public void CopyFrom(NoiseSettings other)
    {
      m_Position = new TransformNoiseParams[other.m_Position.Length];
      other.m_Position.CopyTo(m_Position, 0);
      m_Orientation = new TransformNoiseParams[other.m_Orientation.Length];
      other.m_Orientation.CopyTo(m_Orientation, 0);
    }

    [DocumentationSorting(9.1f, DocumentationSortingAttribute.Level.UserRef)]
    [Serializable]
    public struct NoiseParams
    {
      [Tooltip("The amplitude of the noise for this channel.  Larger numbers vibrate higher.")]
      public float Amplitude;
      [Tooltip("The frequency of noise for this channel.  Higher magnitudes vibrate faster.")]
      public float Frequency;
    }

    [DocumentationSorting(9.2f, DocumentationSortingAttribute.Level.UserRef)]
    [Serializable]
    public struct TransformNoiseParams
    {
      [Tooltip("Noise definition for X-axis")]
      public NoiseParams X;
      [Tooltip("Noise definition for Y-axis")]
      public NoiseParams Y;
      [Tooltip("Noise definition for Z-axis")]
      public NoiseParams Z;
    }
  }
}
