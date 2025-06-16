using SRF;
using System;

namespace SRDebugger.Services
{
  public class Info : ISystemInfo
  {
    private Func<object> _valueGetter;

    public string Title { get; set; }

    public object Value
    {
      get
      {
        try
        {
          return this._valueGetter();
        }
        catch (Exception ex)
        {
          return (object) "Error ({0})".Fmt((object) ex.GetType().Name);
        }
      }
    }

    public bool IsPrivate { get; set; }

    public static Info Create(string name, Func<object> getter, bool isPrivate = false)
    {
      return new Info()
      {
        Title = name,
        _valueGetter = getter,
        IsPrivate = isPrivate
      };
    }

    public static Info Create(string name, object value, bool isPrivate = false)
    {
      return new Info()
      {
        Title = name,
        _valueGetter = (Func<object>) (() => value),
        IsPrivate = isPrivate
      };
    }
  }
}
