using System;
using Cofe.Serializations.Converters;

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
      if (data != null && DefaultConverter.TryParseDateTime(data, out result))
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
