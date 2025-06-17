using System;
using System.Collections.Generic;
using System.Linq;

namespace ParadoxNotion.Serialization.FullSerializer
{
  public struct fsResult
  {
    private static readonly string[] EmptyStringArray = [];
    private bool _success;
    private List<string> _messages;
    public static fsResult Success = new() {
      _success = true
    };

    public void AddMessage(string message)
    {
      if (_messages == null)
        _messages = [];
      _messages.Add(message);
    }

    public void AddMessages(fsResult result)
    {
      if (result._messages == null)
        return;
      if (_messages == null)
        _messages = [];
      _messages.AddRange(result._messages);
    }

    public fsResult Merge(fsResult other)
    {
      _success = _success && other._success;
      if (other._messages != null)
      {
        if (_messages == null)
          _messages = [..other._messages];
        else
          _messages.AddRange(other._messages);
      }
      return this;
    }

    public static fsResult Warn(string warning)
    {
      return new fsResult {
        _success = true,
        _messages = [warning]
      };
    }

    public static fsResult Fail(string warning)
    {
      return new fsResult {
        _success = false,
        _messages = [warning]
      };
    }

    public static fsResult operator +(fsResult a, fsResult b) => a.Merge(b);

    public bool Failed => !_success;

    public bool Succeeded => _success;

    public bool HasWarnings => _messages != null && _messages.Any();

    public fsResult AssertSuccess()
    {
      if (Failed)
        throw AsException;
      return this;
    }

    public fsResult AssertSuccessWithoutWarnings()
    {
      if (Failed || RawMessages.Any())
        throw AsException;
      return this;
    }

    public Exception AsException
    {
      get
      {
        if (!Failed && !RawMessages.Any())
          throw new Exception("Only a failed result can be converted to an exception");
        return new Exception(FormattedMessages);
      }
    }

    public IEnumerable<string> RawMessages => _messages != null ? _messages : EmptyStringArray;

    public string FormattedMessages => string.Join(",\n", RawMessages.ToArray());
  }
}
