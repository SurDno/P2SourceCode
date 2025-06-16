using Cofe.Serializations.Converters;
using System;
using UnityEngine;

namespace Engine.Source.Connections
{
  [Serializable]
  public struct TimeSpanField : ISerializationCallbackReceiver
  {
    [SerializeField]
    private string data;

    public TimeSpan Value { get; set; }

    public void OnAfterDeserialize()
    {
      TimeSpan result;
      if (this.data != null && DefaultConverter.TryParseTimeSpan(this.data, out result))
        this.Value = result;
      else
        this.Value = TimeSpan.Zero;
    }

    public void OnBeforeSerialize()
    {
      if (this.Value != TimeSpan.Zero)
        this.data = DefaultConverter.ToString(this.Value);
      else
        this.data = "";
    }
  }
}
