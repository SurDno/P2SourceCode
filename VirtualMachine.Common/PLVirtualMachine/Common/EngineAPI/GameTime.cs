using Cofe.Loggers;
using Cofe.Serializations.Data;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.Serialization;
using System;
using System.Globalization;
using System.Xml;

namespace PLVirtualMachine.Common.EngineAPI
{
  [VMType("GameTime")]
  public class GameTime : ISerializeStateSave, IDynamicLoadSerializable, IVMStringSerializable
  {
    protected double totalValue;
    public static GameTime Zero = new GameTime();
    private static readonly int DaySeconds = 86400;
    private static readonly int HourSeconds = 3600;
    private static readonly int MinuteSeconds = 60;

    public GameTime() => this.totalValue = 0.0;

    public GameTime(ushort days, byte hours, byte minutes, byte seconds)
    {
      this.Init(days, hours, minutes, seconds);
    }

    public GameTime(ulong totalSeconds) => this.totalValue = (double) totalSeconds;

    public GameTime(ulong totalSeconds, double lastSecond)
    {
      this.totalValue = (double) totalSeconds + lastSecond;
    }

    public GameTime(GameTime copyGameTime)
    {
      if (copyGameTime == null)
        Logger.AddError(string.Format("Invalid game time assignment at {0}", (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
      else
        this.totalValue = copyGameTime.totalValue;
    }

    public void CopyFrom(object data) => this.totalValue = ((GameTime) data).totalValue;

    public void Init() => this.totalValue = 0.0;

    public void Init(ushort days, byte hours, byte minutes, byte seconds, double lastPart = 0.0)
    {
      this.totalValue = (double) ((int) days * 24 * 3600 + (int) hours * 3600 + (int) minutes * 60 + (int) seconds);
    }

    public void Process(double elapsedTimeVal) => this.totalValue += elapsedTimeVal;

    public ushort Days
    {
      get => (ushort) ((uint) (int) Math.Floor(this.totalValue) / (uint) GameTime.DaySeconds);
      set
      {
        byte hours = this.Hours;
        byte minutes = this.Minutes;
        byte seconds = this.Seconds;
        double lastSecond = this.LastSecond;
        this.Init(value, hours, minutes, seconds, lastSecond);
      }
    }

    public byte Hours
    {
      get
      {
        return (byte) ((int) Math.Floor(this.totalValue) % GameTime.DaySeconds / GameTime.HourSeconds);
      }
      set
      {
        ushort days = this.Days;
        byte minutes = this.Minutes;
        byte seconds = this.Seconds;
        double lastSecond = this.LastSecond;
        this.Init(days, value, minutes, seconds, lastSecond);
      }
    }

    public byte Minutes
    {
      get
      {
        return (byte) ((int) Math.Floor(this.totalValue) % GameTime.HourSeconds / GameTime.MinuteSeconds);
      }
      set
      {
        ushort days = this.Days;
        byte hours = this.Hours;
        byte seconds = this.Seconds;
        double lastSecond = this.LastSecond;
        this.Init(days, hours, value, seconds, lastSecond);
      }
    }

    public byte Seconds
    {
      get => (byte) ((uint) (int) Math.Floor(this.totalValue) % (uint) GameTime.MinuteSeconds);
      set
      {
        ushort days = this.Days;
        byte hours = this.Hours;
        byte minutes = this.Minutes;
        double lastSecond = this.LastSecond;
        this.Init(days, hours, minutes, value, lastSecond);
      }
    }

    public ulong TotalSeconds
    {
      get => (ulong) Math.Floor(this.totalValue);
      set
      {
        double num = this.totalValue - Math.Floor(this.totalValue);
        this.totalValue = (double) value + num;
      }
    }

    public double TotalValue => this.totalValue;

    public double LastSecond => this.totalValue - Math.Floor(this.totalValue);

    public string Write()
    {
      Logger.AddError("Not allowed serialization data struct in virtual machine!");
      return string.Empty;
    }

    public void Read(string data)
    {
      switch (data)
      {
        case null:
          Logger.AddError(string.Format("Attempt to read null game time data at {0}", (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
          break;
        case "":
          break;
        default:
          string[] strArray = data.Split(':');
          if (strArray.Length < 4)
          {
            Logger.AddError(string.Format("Cannot convert {0} to GameTime at {1}", (object) data, (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
            break;
          }
          ushort result1 = 0;
          byte result2 = 0;
          byte result3 = 0;
          byte result4 = 0;
          double lastPart = 0.0;
          if (!ushort.TryParse(strArray[0], NumberStyles.Number, (IFormatProvider) CultureInfo.InvariantCulture, out result1))
            Logger.AddError(string.Format("Cannot convert {0} to GameTime at {1}", (object) data, (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
          if (!byte.TryParse(strArray[1], NumberStyles.Number, (IFormatProvider) CultureInfo.InvariantCulture, out result2))
            Logger.AddError(string.Format("Cannot convert {0} to GameTime at {1}", (object) data, (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
          if (!byte.TryParse(strArray[2], NumberStyles.Number, (IFormatProvider) CultureInfo.InvariantCulture, out result3))
            Logger.AddError(string.Format("Cannot convert {0} to GameTime at {1}", (object) data, (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
          if (!byte.TryParse(strArray[3], NumberStyles.Number, (IFormatProvider) CultureInfo.InvariantCulture, out result4))
            Logger.AddError(string.Format("Cannot convert {0} to GameTime at {1}", (object) data, (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
          if (strArray.Length > 4)
          {
            uint result5 = 0;
            if (!uint.TryParse(strArray[4], NumberStyles.Number, (IFormatProvider) CultureInfo.InvariantCulture, out result5))
              Logger.AddError(string.Format("Cannot convert {0} to GameTime at {1}", (object) data, (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
            lastPart = 9.9999999747524271E-07 * (double) result5;
          }
          this.Init(result1, result2, result3, result4, lastPart);
          break;
      }
    }

    public void StateSave(IDataWriter writer)
    {
      SaveManagerUtility.Save(writer, "Days", this.Days);
      SaveManagerUtility.Save(writer, "Hours", this.Hours);
      SaveManagerUtility.Save(writer, "Minutes", this.Minutes);
      SaveManagerUtility.Save(writer, "Seconds", this.Seconds);
      SaveManagerUtility.Save(writer, "LastMicroseconds", this.LastSecond);
    }

    public void LoadFromXML(XmlElement xmlNode)
    {
      ushort days = 0;
      byte hours = 0;
      byte minutes = 0;
      byte seconds = 0;
      double lastPart = 0.0;
      foreach (XmlElement childNode in xmlNode.ChildNodes)
      {
        if (childNode.Name == "Days")
          days = (ushort) StringUtility.ToByte(childNode.InnerText);
        else if (childNode.Name == "Hours")
          hours = StringUtility.ToByte(childNode.InnerText);
        else if (childNode.Name == "Minutes")
          minutes = StringUtility.ToByte(childNode.InnerText);
        else if (childNode.Name == "Seconds")
          seconds = StringUtility.ToByte(childNode.InnerText);
        else if (childNode.Name == "LastMicroseconds")
          lastPart = StringUtility.ToDouble(childNode.InnerText);
      }
      this.Init(days, hours, minutes, seconds, lastPart);
    }

    public static implicit operator TimeSpan(GameTime time)
    {
      return TimeSpan.FromSeconds((double) time.TotalSeconds);
    }

    public static implicit operator GameTime(TimeSpan time)
    {
      return new GameTime((ulong) time.TotalSeconds);
    }
  }
}
