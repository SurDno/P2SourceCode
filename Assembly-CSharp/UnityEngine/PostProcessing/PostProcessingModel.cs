﻿using System;

namespace UnityEngine.PostProcessing
{
  [Serializable]
  public abstract class PostProcessingModel
  {
    [SerializeField]
    [GetSet("enabled")]
    private bool m_Enabled;

    public bool enabled
    {
      get => m_Enabled;
      set
      {
        m_Enabled = value;
        if (!value)
          return;
        OnValidate();
      }
    }

    public abstract void Reset();

    public virtual void OnValidate()
    {
    }
  }
}
