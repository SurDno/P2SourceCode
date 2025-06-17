using System;
using Cofe.Serializations.Converters;
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
      if (data != null && DefaultConverter.TryParseTimeSpan(data, out TimeSpan result))
        Value = result;
      else
        Value = TimeSpan.Zero;
    }

    public void OnBeforeSerialize()
    {
      if (Value != TimeSpan.Zero)
        data = DefaultConverter.ToString(Value);
      else
        data = "";
    }
  }
}
