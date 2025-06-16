﻿// Decompiled with JetBrains decompiler
// Type: ParadoxNotion.Serialization.FullSerializer.fsResult
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace ParadoxNotion.Serialization.FullSerializer
{
  public struct fsResult
  {
    private static readonly string[] EmptyStringArray = new string[0];
    private bool _success;
    private List<string> _messages;
    public static fsResult Success = new fsResult()
    {
      _success = true
    };

    public void AddMessage(string message)
    {
      if (this._messages == null)
        this._messages = new List<string>();
      this._messages.Add(message);
    }

    public void AddMessages(fsResult result)
    {
      if (result._messages == null)
        return;
      if (this._messages == null)
        this._messages = new List<string>();
      this._messages.AddRange((IEnumerable<string>) result._messages);
    }

    public fsResult Merge(fsResult other)
    {
      this._success = this._success && other._success;
      if (other._messages != null)
      {
        if (this._messages == null)
          this._messages = new List<string>((IEnumerable<string>) other._messages);
        else
          this._messages.AddRange((IEnumerable<string>) other._messages);
      }
      return this;
    }

    public static fsResult Warn(string warning)
    {
      return new fsResult()
      {
        _success = true,
        _messages = new List<string>() { warning }
      };
    }

    public static fsResult Fail(string warning)
    {
      return new fsResult()
      {
        _success = false,
        _messages = new List<string>() { warning }
      };
    }

    public static fsResult operator +(fsResult a, fsResult b) => a.Merge(b);

    public bool Failed => !this._success;

    public bool Succeeded => this._success;

    public bool HasWarnings => this._messages != null && this._messages.Any<string>();

    public fsResult AssertSuccess()
    {
      if (this.Failed)
        throw this.AsException;
      return this;
    }

    public fsResult AssertSuccessWithoutWarnings()
    {
      if (this.Failed || this.RawMessages.Any<string>())
        throw this.AsException;
      return this;
    }

    public Exception AsException
    {
      get
      {
        if (!this.Failed && !this.RawMessages.Any<string>())
          throw new Exception("Only a failed result can be converted to an exception");
        return new Exception(this.FormattedMessages);
      }
    }

    public IEnumerable<string> RawMessages
    {
      get
      {
        return this._messages != null ? (IEnumerable<string>) this._messages : (IEnumerable<string>) fsResult.EmptyStringArray;
      }
    }

    public string FormattedMessages => string.Join(",\n", this.RawMessages.ToArray<string>());
  }
}
