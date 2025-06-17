using System;
using Cofe.Serializations.Converters;
using UnityEngine;

namespace Engine.Source.Connections
{
  [Serializable]
  public struct DateTimeField : ISerializationCallbackReceiver
  {
    [SerializeField]
    private string data;

    public DateTime Value { get; set; }

    public void OnAfterDeserialize()
    {
      if (data != null && DefaultConverter.TryParseDateTime(data, out DateTime result))
        Value = result;
      else
        Value = DateTime.MinValue;
    }

    public void OnBeforeSerialize()
    {
      if (Value != DateTime.MinValue)
        data = DefaultConverter.ToString(Value);
      else
        data = "";
    }
  }
}
