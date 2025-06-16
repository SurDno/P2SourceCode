using Cofe.Serializations.Converters;
using System;
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
      DateTime result;
      if (this.data != null && DefaultConverter.TryParseDateTime(this.data, out result))
        this.Value = result;
      else
        this.Value = DateTime.MinValue;
    }

    public void OnBeforeSerialize()
    {
      if (this.Value != DateTime.MinValue)
        this.data = DefaultConverter.ToString(this.Value);
      else
        this.data = "";
    }
  }
}
