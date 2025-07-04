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
    private readonly StringBuilder _cachedStringBuilder = new(256);

    private fsResult MakeFailure(string message)
    {
      int startIndex = Math.Max(0, _start - 20);
      int length = Math.Min(50, _input.Length - startIndex);
      return fsResult.Fail("Error while parsing: " + message + "; context = <" + _input.Substring(startIndex, length) + ">");
    }

    private bool TryMoveNext()
    {
      if (_start >= _input.Length)
        return false;
      ++_start;
      return true;
    }

    private bool HasValue() => HasValue(0);

    private bool HasValue(int offset)
    {
      return _start + offset >= 0 && _start + offset < _input.Length;
    }

    private char Character() => Character(0);

    private char Character(int offset) => _input[_start + offset];

    private void SkipSpace()
    {
      while (HasValue())
      {
        if (char.IsWhiteSpace(Character()))
        {
          TryMoveNext();
        }
        else
        {
          if (!HasValue(1) || Character(0) != '/')
            break;
          if (Character(1) == '/')
          {
            while (HasValue() && !Environment.NewLine.Contains(Character().ToString() ?? ""))
              TryMoveNext();
          }
          else if (Character(1) == '*')
          {
            TryMoveNext();
            TryMoveNext();
            while (HasValue(1))
            {
              if (Character(0) == '*' && Character(1) == '/')
              {
                TryMoveNext();
                TryMoveNext();
                TryMoveNext();
                break;
              }
              TryMoveNext();
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
        singleChar = (c1 - 48U) * multipliyer;
      else if (c1 >= 'A' && c1 <= 'F')
        singleChar = (uint) (c1 - 65 + 10) * multipliyer;
      else if (c1 >= 'a' && c1 <= 'f')
        singleChar = (uint) (c1 - 97 + 10) * multipliyer;
      return singleChar;
    }

    private uint ParseUnicode(char c1, char c2, char c3, char c4)
    {
      return ParseSingleChar(c1, 4096U) + ParseSingleChar(c2, 256U) + ParseSingleChar(c3, 16U) + ParseSingleChar(c4, 1U);
    }

    private fsResult TryUnescapeChar(out char escaped)
    {
      TryMoveNext();
      if (!HasValue())
      {
        escaped = ' ';
        return MakeFailure("Unexpected end of input after \\");
      }
      switch (Character())
      {
        case '"':
          TryMoveNext();
          escaped = '"';
          return fsResult.Success;
        case '/':
          TryMoveNext();
          escaped = '/';
          return fsResult.Success;
        case '0':
          TryMoveNext();
          escaped = char.MinValue;
          return fsResult.Success;
        case '\\':
          TryMoveNext();
          escaped = '\\';
          return fsResult.Success;
        case 'a':
          TryMoveNext();
          escaped = '\a';
          return fsResult.Success;
        case 'b':
          TryMoveNext();
          escaped = '\b';
          return fsResult.Success;
        case 'f':
          TryMoveNext();
          escaped = '\f';
          return fsResult.Success;
        case 'n':
          TryMoveNext();
          escaped = '\n';
          return fsResult.Success;
        case 'r':
          TryMoveNext();
          escaped = '\r';
          return fsResult.Success;
        case 't':
          TryMoveNext();
          escaped = '\t';
          return fsResult.Success;
        case 'u':
          TryMoveNext();
          if (IsHex(Character(0)) && IsHex(Character(1)) && IsHex(Character(2)) && IsHex(Character(3)))
          {
            uint unicode = ParseUnicode(Character(0), Character(1), Character(2), Character(3));
            TryMoveNext();
            TryMoveNext();
            TryMoveNext();
            TryMoveNext();
            escaped = (char) unicode;
            return fsResult.Success;
          }
          escaped = char.MinValue;
          return MakeFailure(string.Format("invalid escape sequence '\\u{0}{1}{2}{3}'\n", Character(0), Character(1), Character(2), Character(3)));
        default:
          escaped = char.MinValue;
          return MakeFailure(string.Format("Invalid escape sequence \\{0}", Character()));
      }
    }

    private fsResult TryParseExact(string content)
    {
      for (int index = 0; index < content.Length; ++index)
      {
        if (Character() != content[index])
          return MakeFailure("Expected " + content[index]);
        if (!TryMoveNext())
          return MakeFailure("Unexpected end of content when parsing " + content);
      }
      return fsResult.Success;
    }

    private fsResult TryParseTrue(out fsData data)
    {
      fsResult exact = TryParseExact("true");
      if (exact.Succeeded)
      {
        data = new fsData(true);
        return fsResult.Success;
      }
      data = null;
      return exact;
    }

    private fsResult TryParseFalse(out fsData data)
    {
      fsResult exact = TryParseExact("false");
      if (exact.Succeeded)
      {
        data = new fsData(false);
        return fsResult.Success;
      }
      data = null;
      return exact;
    }

    private fsResult TryParseNull(out fsData data)
    {
      fsResult exact = TryParseExact("null");
      if (exact.Succeeded)
      {
        data = new fsData();
        return fsResult.Success;
      }
      data = null;
      return exact;
    }

    private bool IsSeparator(char c) => char.IsWhiteSpace(c) || c == ',' || c == '}' || c == ']';

    private fsResult TryParseNumber(out fsData data)
    {
      int start = _start;
      do
        ;
      while (TryMoveNext() && HasValue() && !IsSeparator(Character()));
      string s = _input.Substring(start, _start - start);
      if (s.Contains(".") || s == "Infinity" || s == "-Infinity" || s == "NaN")
      {
        if (!double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
        {
          data = null;
          return MakeFailure("Bad double format with " + s);
        }
        data = new fsData(result);
        return fsResult.Success;
      }

      if (!long.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out long result1))
      {
        data = null;
        return MakeFailure("Bad Int64 format with " + s);
      }
      data = new fsData(result1);
      return fsResult.Success;
    }

    private fsResult TryParseString(out string str)
    {
      _cachedStringBuilder.Length = 0;
      if (Character() != '"' || !TryMoveNext())
      {
        str = string.Empty;
        return MakeFailure("Expected initial \" when parsing a string");
      }
      while (HasValue() && Character() != '"')
      {
        char ch = Character();
        if (ch == '\\')
        {
          fsResult fsResult = TryUnescapeChar(out char escaped);
          if (fsResult.Failed)
          {
            str = string.Empty;
            return fsResult;
          }
          _cachedStringBuilder.Append(escaped);
        }
        else
        {
          _cachedStringBuilder.Append(ch);
          if (!TryMoveNext())
          {
            str = string.Empty;
            return MakeFailure("Unexpected end of input when reading a string");
          }
        }
      }
      if (!HasValue() || Character() != '"' || !TryMoveNext())
      {
        str = string.Empty;
        return MakeFailure("No closing \" when parsing a string");
      }
      str = _cachedStringBuilder.ToString();
      return fsResult.Success;
    }

    private fsResult TryParseArray(out fsData arr)
    {
      if (Character() != '[')
      {
        arr = null;
        return MakeFailure("Expected initial [ when parsing an array");
      }
      if (!TryMoveNext())
      {
        arr = null;
        return MakeFailure("Unexpected end of input when parsing an array");
      }
      SkipSpace();
      List<fsData> list = [];
      while (HasValue() && Character() != ']')
      {
        fsResult array = RunParse(out fsData data);
        if (array.Failed)
        {
          arr = null;
          return array;
        }
        list.Add(data);
        SkipSpace();
        if (HasValue() && Character() == ',')
        {
          if (TryMoveNext())
            SkipSpace();
          else
            break;
        }
      }
      if (!HasValue() || Character() != ']' || !TryMoveNext())
      {
        arr = null;
        return MakeFailure("No closing ] for array");
      }
      arr = new fsData(list);
      return fsResult.Success;
    }

    private fsResult TryParseObject(out fsData obj)
    {
      if (Character() != '{')
      {
        obj = null;
        return MakeFailure("Expected initial { when parsing an object");
      }
      if (!TryMoveNext())
      {
        obj = null;
        return MakeFailure("Unexpected end of input when parsing an object");
      }
      SkipSpace();
      Dictionary<string, fsData> dict = new Dictionary<string, fsData>(fsGlobalConfig.IsCaseSensitive ? StringComparer.Ordinal : (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      while (HasValue() && Character() != '}')
      {
        SkipSpace();
        fsResult fsResult = TryParseString(out string str);
        if (fsResult.Failed)
        {
          obj = null;
          return fsResult;
        }
        SkipSpace();
        if (!HasValue() || Character() != ':' || !TryMoveNext())
        {
          obj = null;
          return MakeFailure("Expected : after key \"" + str + "\"");
        }
        SkipSpace();
        fsResult = RunParse(out fsData data);
        if (fsResult.Failed)
        {
          obj = null;
          return fsResult;
        }
        dict.Add(str, data);
        SkipSpace();
        if (HasValue() && Character() == ',')
        {
          if (TryMoveNext())
            SkipSpace();
          else
            break;
        }
      }
      if (!HasValue() || Character() != '}' || !TryMoveNext())
      {
        obj = null;
        return MakeFailure("No closing } for object");
      }
      obj = new fsData(dict);
      return fsResult.Success;
    }

    private fsResult RunParse(out fsData data)
    {
      SkipSpace();
      if (!HasValue())
      {
        data = null;
        return MakeFailure("Unexpected end of input");
      }
      switch (Character())
      {
        case '"':
          fsResult fsResult = TryParseString(out string str);
          if (fsResult.Failed)
          {
            data = null;
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
          return TryParseNumber(out data);
        case '[':
          return TryParseArray(out data);
        case 'f':
          return TryParseFalse(out data);
        case 'n':
          return TryParseNull(out data);
        case 't':
          return TryParseTrue(out data);
        case '{':
          return TryParseObject(out data);
        default:
          data = null;
          return MakeFailure("unable to parse; invalid token \"" + Character() + "\"");
      }
    }

    public static fsResult Parse(string input, out fsData data)
    {
      if (!string.IsNullOrEmpty(input))
        return new fsJsonParser(input).RunParse(out data);
      data = null;
      return fsResult.Fail("No input");
    }

    public static fsData Parse(string input)
    {
      Parse(input, out fsData data).AssertSuccess();
      return data;
    }

    private fsJsonParser(string input)
    {
      _input = input;
      _start = 0;
    }
  }
}
