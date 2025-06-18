using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ParadoxNotion.Serialization.FullSerializer
{
  public sealed class fsData
  {
    private object _value;
    public static readonly fsData True = new(true);
    public static readonly fsData False = new(false);
    public static readonly fsData Null = new();

    public fsData() => _value = null;

    public fsData(bool boolean) => _value = boolean;

    public fsData(double f) => _value = f;

    public fsData(long i) => _value = i;

    public fsData(string str) => _value = str;

    public fsData(Dictionary<string, fsData> dict) => _value = dict;

    public fsData(List<fsData> list) => _value = list;

    public static fsData CreateDictionary()
    {
      return new fsData(new Dictionary<string, fsData>(fsGlobalConfig.IsCaseSensitive ? StringComparer.Ordinal : (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase));
    }

    public static fsData CreateList() => new(new List<fsData>());

    public static fsData CreateList(int capacity) => new(new List<fsData>(capacity));

    internal void BecomeDictionary()
    {
      _value = new Dictionary<string, fsData>(StringComparer.Ordinal);
    }

    internal fsData Clone()
    {
      return new fsData { _value = _value };
    }

    public fsDataType Type
    {
      get
      {
        if (_value == null)
          return fsDataType.Null;
        if (_value is double)
          return fsDataType.Double;
        if (_value is long)
          return fsDataType.Int64;
        if (_value is bool)
          return fsDataType.Boolean;
        if (_value is string)
          return fsDataType.String;
        if (_value is Dictionary<string, fsData>)
          return fsDataType.Object;
        if (_value is List<fsData>)
          return fsDataType.Array;
        throw new InvalidOperationException("unknown JSON data type");
      }
    }

    public bool IsNull => _value == null;

    public bool IsDouble => _value is double;

    public bool IsInt64 => _value is long;

    public bool IsBool => _value is bool;

    public bool IsString => _value is string;

    public bool IsDictionary => _value is Dictionary<string, fsData>;

    public bool IsList => _value is List<fsData>;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public double AsDouble => Cast<double>();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public long AsInt64 => Cast<long>();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool AsBool => Cast<bool>();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public string AsString => Cast<string>();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Dictionary<string, fsData> AsDictionary => Cast<Dictionary<string, fsData>>();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public List<fsData> AsList => Cast<List<fsData>>();

    private T Cast<T>()
    {
      return _value is T ? (T) _value : throw new InvalidCastException("Unable to cast <" + this + "> (with type = " + _value.GetType() + ") to type " + typeof (T));
    }

    public override string ToString() => fsJsonPrinter.CompressedJson(this);

    public override bool Equals(object obj) => Equals(obj as fsData);

    public bool Equals(fsData other)
    {
      if (other == null || Type != other.Type)
        return false;
      switch (Type)
      {
        case fsDataType.Array:
          List<fsData> asList1 = AsList;
          List<fsData> asList2 = other.AsList;
          if (asList1.Count != asList2.Count)
            return false;
          for (int index = 0; index < asList1.Count; ++index)
          {
            if (!asList1[index].Equals(asList2[index]))
              return false;
          }
          return true;
        case fsDataType.Object:
          Dictionary<string, fsData> asDictionary1 = AsDictionary;
          Dictionary<string, fsData> asDictionary2 = other.AsDictionary;
          if (asDictionary1.Count != asDictionary2.Count)
            return false;
          foreach (string key in asDictionary1.Keys)
          {
            if (!asDictionary2.ContainsKey(key) || !asDictionary1[key].Equals(asDictionary2[key]))
              return false;
          }
          return true;
        case fsDataType.Double:
          return AsDouble == other.AsDouble || Math.Abs(AsDouble - other.AsDouble) < double.Epsilon;
        case fsDataType.Int64:
          return AsInt64 == other.AsInt64;
        case fsDataType.Boolean:
          return AsBool == other.AsBool;
        case fsDataType.String:
          return AsString == other.AsString;
        case fsDataType.Null:
          return true;
        default:
          throw new Exception("Unknown data type");
      }
    }

    public static bool operator ==(fsData a, fsData b)
    {
      if (a == (object) b)
        return true;
      if ((object) a == null || (object) b == null)
        return false;
      return a.IsDouble && b.IsDouble ? Math.Abs(a.AsDouble - b.AsDouble) < double.Epsilon : a.Equals(b);
    }

    public static bool operator !=(fsData a, fsData b) => !(a == b);

    public override int GetHashCode() => _value != null ? _value.GetHashCode() : 0;
  }
}
