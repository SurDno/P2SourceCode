﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ParadoxNotion.Serialization.FullSerializer
{
  public class fsJsonParser
  {
    private int _start;
    private string _input;
    private readonly StringBuilder _cachedStringBuilder = new StringBuilder(256);

    private fsResult MakeFailure(string message)
    {
      int startIndex = Math.Max(0, this._start - 20);
      int length = Math.Min(50, this._input.Length - startIndex);
      return fsResult.Fail("Error while parsing: " + message + "; context = <" + this._input.Substring(startIndex, length) + ">");
    }

    private bool TryMoveNext()
    {
      if (this._start >= this._input.Length)
        return false;
      ++this._start;
      return true;
    }

    private bool HasValue() => this.HasValue(0);

    private bool HasValue(int offset)
    {
      return this._start + offset >= 0 && this._start + offset < this._input.Length;
    }

    private char Character() => this.Character(0);

    private char Character(int offset) => this._input[this._start + offset];

    private void SkipSpace()
    {
      while (this.HasValue())
      {
        if (char.IsWhiteSpace(this.Character()))
        {
          this.TryMoveNext();
        }
        else
        {
          if (!this.HasValue(1) || this.Character(0) != '/')
            break;
          if (this.Character(1) == '/')
          {
            while (this.HasValue() && !Environment.NewLine.Contains(this.Character().ToString() ?? ""))
              this.TryMoveNext();
          }
          else if (this.Character(1) == '*')
          {
            this.TryMoveNext();
            this.TryMoveNext();
            while (this.HasValue(1))
            {
              if (this.Character(0) == '*' && this.Character(1) == '/')
              {
                this.TryMoveNext();
                this.TryMoveNext();
                this.TryMoveNext();
                break;
              }
              this.TryMoveNext();
            }
          }
        }
      }
    }

    private bool IsHex(char c)
    {
      return c >= '0' && c <= '9' || c >= 'a' && c <= 'f' || c >= 'A' && c <= 'F';
    }

    private uint ParseSingleChar(char c1, uint multipliyer)
    {
      uint singleChar = 0;
      if (c1 >= '0' && c1 <= '9')
        singleChar = ((uint) c1 - 48U) * multipliyer;
      else if (c1 >= 'A' && c1 <= 'F')
        singleChar = (uint) ((int) c1 - 65 + 10) * multipliyer;
      else if (c1 >= 'a' && c1 <= 'f')
        singleChar = (uint) ((int) c1 - 97 + 10) * multipliyer;
      return singleChar;
    }

    private uint ParseUnicode(char c1, char c2, char c3, char c4)
    {
      return this.ParseSingleChar(c1, 4096U) + this.ParseSingleChar(c2, 256U) + this.ParseSingleChar(c3, 16U) + this.ParseSingleChar(c4, 1U);
    }

    private fsResult TryUnescapeChar(out char escaped)
    {
      this.TryMoveNext();
      if (!this.HasValue())
      {
        escaped = ' ';
        return this.MakeFailure("Unexpected end of input after \\");
      }
      switch (this.Character())
      {
        case '"':
          this.TryMoveNext();
          escaped = '"';
          return fsResult.Success;
        case '/':
          this.TryMoveNext();
          escaped = '/';
          return fsResult.Success;
        case '0':
          this.TryMoveNext();
          escaped = char.MinValue;
          return fsResult.Success;
        case '\\':
          this.TryMoveNext();
          escaped = '\\';
          return fsResult.Success;
        case 'a':
          this.TryMoveNext();
          escaped = '\a';
          return fsResult.Success;
        case 'b':
          this.TryMoveNext();
          escaped = '\b';
          return fsResult.Success;
        case 'f':
          this.TryMoveNext();
          escaped = '\f';
          return fsResult.Success;
        case 'n':
          this.TryMoveNext();
          escaped = '\n';
          return fsResult.Success;
        case 'r':
          this.TryMoveNext();
          escaped = '\r';
          return fsResult.Success;
        case 't':
          this.TryMoveNext();
          escaped = '\t';
          return fsResult.Success;
        case 'u':
          this.TryMoveNext();
          if (this.IsHex(this.Character(0)) && this.IsHex(this.Character(1)) && this.IsHex(this.Character(2)) && this.IsHex(this.Character(3)))
          {
            uint unicode = this.ParseUnicode(this.Character(0), this.Character(1), this.Character(2), this.Character(3));
            this.TryMoveNext();
            this.TryMoveNext();
            this.TryMoveNext();
            this.TryMoveNext();
            escaped = (char) unicode;
            return fsResult.Success;
          }
          escaped = char.MinValue;
          return this.MakeFailure(string.Format("invalid escape sequence '\\u{0}{1}{2}{3}'\n", (object) this.Character(0), (object) this.Character(1), (object) this.Character(2), (object) this.Character(3)));
        default:
          escaped = char.MinValue;
          return this.MakeFailure(string.Format("Invalid escape sequence \\{0}", (object) this.Character()));
      }
    }

    private fsResult TryParseExact(string content)
    {
      for (int index = 0; index < content.Length; ++index)
      {
        if ((int) this.Character() != (int) content[index])
          return this.MakeFailure("Expected " + content[index].ToString());
        if (!this.TryMoveNext())
          return this.MakeFailure("Unexpected end of content when parsing " + content);
      }
      return fsResult.Success;
    }

    private fsResult TryParseTrue(out fsData data)
    {
      fsResult exact = this.TryParseExact("true");
      if (exact.Succeeded)
      {
        data = new fsData(true);
        return fsResult.Success;
      }
      data = (fsData) null;
      return exact;
    }

    private fsResult TryParseFalse(out fsData data)
    {
      fsResult exact = this.TryParseExact("false");
      if (exact.Succeeded)
      {
        data = new fsData(false);
        return fsResult.Success;
      }
      data = (fsData) null;
      return exact;
    }

    private fsResult TryParseNull(out fsData data)
    {
      fsResult exact = this.TryParseExact("null");
      if (exact.Succeeded)
      {
        data = new fsData();
        return fsResult.Success;
      }
      data = (fsData) null;
      return exact;
    }

    private bool IsSeparator(char c) => char.IsWhiteSpace(c) || c == ',' || c == '}' || c == ']';

    private fsResult TryParseNumber(out fsData data)
    {
      int start = this._start;
      do
        ;
      while (this.TryMoveNext() && this.HasValue() && !this.IsSeparator(this.Character()));
      string s = this._input.Substring(start, this._start - start);
      if (s.Contains(".") || s == "Infinity" || s == "-Infinity" || s == "NaN")
      {
        double result;
        if (!double.TryParse(s, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result))
        {
          data = (fsData) null;
          return this.MakeFailure("Bad double format with " + s);
        }
        data = new fsData(result);
        return fsResult.Success;
      }
      long result1;
      if (!long.TryParse(s, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result1))
      {
        data = (fsData) null;
        return this.MakeFailure("Bad Int64 format with " + s);
      }
      data = new fsData(result1);
      return fsResult.Success;
    }

    private fsResult TryParseString(out string str)
    {
      this._cachedStringBuilder.Length = 0;
      if (this.Character() != '"' || !this.TryMoveNext())
      {
        str = string.Empty;
        return this.MakeFailure("Expected initial \" when parsing a string");
      }
      while (this.HasValue() && this.Character() != '"')
      {
        char ch = this.Character();
        if (ch == '\\')
        {
          char escaped;
          fsResult fsResult = this.TryUnescapeChar(out escaped);
          if (fsResult.Failed)
          {
            str = string.Empty;
            return fsResult;
          }
          this._cachedStringBuilder.Append(escaped);
        }
        else
        {
          this._cachedStringBuilder.Append(ch);
          if (!this.TryMoveNext())
          {
            str = string.Empty;
            return this.MakeFailure("Unexpected end of input when reading a string");
          }
        }
      }
      if (!this.HasValue() || this.Character() != '"' || !this.TryMoveNext())
      {
        str = string.Empty;
        return this.MakeFailure("No closing \" when parsing a string");
      }
      str = this._cachedStringBuilder.ToString();
      return fsResult.Success;
    }

    private fsResult TryParseArray(out fsData arr)
    {
      if (this.Character() != '[')
      {
        arr = (fsData) null;
        return this.MakeFailure("Expected initial [ when parsing an array");
      }
      if (!this.TryMoveNext())
      {
        arr = (fsData) null;
        return this.MakeFailure("Unexpected end of input when parsing an array");
      }
      this.SkipSpace();
      List<fsData> list = new List<fsData>();
      while (this.HasValue() && this.Character() != ']')
      {
        fsData data;
        fsResult array = this.RunParse(out data);
        if (array.Failed)
        {
          arr = (fsData) null;
          return array;
        }
        list.Add(data);
        this.SkipSpace();
        if (this.HasValue() && this.Character() == ',')
        {
          if (this.TryMoveNext())
            this.SkipSpace();
          else
            break;
        }
      }
      if (!this.HasValue() || this.Character() != ']' || !this.TryMoveNext())
      {
        arr = (fsData) null;
        return this.MakeFailure("No closing ] for array");
      }
      arr = new fsData(list);
      return fsResult.Success;
    }

    private fsResult TryParseObject(out fsData obj)
    {
      if (this.Character() != '{')
      {
        obj = (fsData) null;
        return this.MakeFailure("Expected initial { when parsing an object");
      }
      if (!this.TryMoveNext())
      {
        obj = (fsData) null;
        return this.MakeFailure("Unexpected end of input when parsing an object");
      }
      this.SkipSpace();
      Dictionary<string, fsData> dict = new Dictionary<string, fsData>(fsGlobalConfig.IsCaseSensitive ? (IEqualityComparer<string>) StringComparer.Ordinal : (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      while (this.HasValue() && this.Character() != '}')
      {
        this.SkipSpace();
        string str;
        fsResult fsResult = this.TryParseString(out str);
        if (fsResult.Failed)
        {
          obj = (fsData) null;
          return fsResult;
        }
        this.SkipSpace();
        if (!this.HasValue() || this.Character() != ':' || !this.TryMoveNext())
        {
          obj = (fsData) null;
          return this.MakeFailure("Expected : after key \"" + str + "\"");
        }
        this.SkipSpace();
        fsData data;
        fsResult = this.RunParse(out data);
        if (fsResult.Failed)
        {
          obj = (fsData) null;
          return fsResult;
        }
        dict.Add(str, data);
        this.SkipSpace();
        if (this.HasValue() && this.Character() == ',')
        {
          if (this.TryMoveNext())
            this.SkipSpace();
          else
            break;
        }
      }
      if (!this.HasValue() || this.Character() != '}' || !this.TryMoveNext())
      {
        obj = (fsData) null;
        return this.MakeFailure("No closing } for object");
      }
      obj = new fsData(dict);
      return fsResult.Success;
    }

    private fsResult RunParse(out fsData data)
    {
      this.SkipSpace();
      if (!this.HasValue())
      {
        data = (fsData) null;
        return this.MakeFailure("Unexpected end of input");
      }
      switch (this.Character())
      {
        case '"':
          string str;
          fsResult fsResult = this.TryParseString(out str);
          if (fsResult.Failed)
          {
            data = (fsData) null;
            return fsResult;
          }
          data = new fsData(str);
          return fsResult.Success;
        case '+':
        case '-':
        case '.':
        case '0':
        case '1':
        case '2':
        case '3':
        case '4':
        case '5':
        case '6':
        case '7':
        case '8':
        case '9':
        case 'I':
        case 'N':
          return this.TryParseNumber(out data);
        case '[':
          return this.TryParseArray(out data);
        case 'f':
          return this.TryParseFalse(out data);
        case 'n':
          return this.TryParseNull(out data);
        case 't':
          return this.TryParseTrue(out data);
        case '{':
          return this.TryParseObject(out data);
        default:
          data = (fsData) null;
          return this.MakeFailure("unable to parse; invalid token \"" + this.Character().ToString() + "\"");
      }
    }

    public static fsResult Parse(string input, out fsData data)
    {
      if (!string.IsNullOrEmpty(input))
        return new fsJsonParser(input).RunParse(out data);
      data = (fsData) null;
      return fsResult.Fail("No input");
    }

    public static fsData Parse(string input)
    {
      fsData data;
      fsJsonParser.Parse(input, out data).AssertSuccess();
      return data;
    }

    private fsJsonParser(string input)
    {
      this._input = input;
      this._start = 0;
    }
  }
}
