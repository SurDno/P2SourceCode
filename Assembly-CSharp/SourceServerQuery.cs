// Decompiled with JetBrains decompiler
// Type: SourceServerQuery
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

#nullable disable
internal class SourceServerQuery : IDisposable
{
  private IPEndPoint endPoint;
  private Socket socket;
  private UdpClient client;
  private int send_timeout = 2500;
  private int receive_timeout = 2500;
  private byte[] raw_data;
  private int offset = 0;
  private readonly byte[] FFFFFFFF = new byte[4]
  {
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue
  };

  public SourceServerQuery(string ip, int port)
  {
    this.endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
  }

  public SourceServerQuery.PlayersResponse GetPlayerList()
  {
    this.GetSocket();
    this.offset = 5;
    try
    {
      SourceServerQuery.PlayersResponse playerList = new SourceServerQuery.PlayersResponse();
      byte[] challenge = this.GetChallenge((byte) 85);
      byte[] numArray = new byte[challenge.Length + this.FFFFFFFF.Length + 1];
      Array.Copy((Array) this.FFFFFFFF, 0, (Array) numArray, 0, this.FFFFFFFF.Length);
      numArray[this.FFFFFFFF.Length] = (byte) 85;
      Array.Copy((Array) challenge, 0, (Array) numArray, this.FFFFFFFF.Length + 1, challenge.Length);
      this.socket.Send(numArray);
      this.raw_data = new byte[2048];
      this.socket.Receive(this.raw_data);
      byte num1 = this.ReadByte();
      for (int index = 0; index < (int) num1; ++index)
      {
        int num2 = (int) this.ReadByte();
        playerList.players.Add(new SourceServerQuery.PlayersResponse.Player()
        {
          name = this.ReadString(),
          score = this.ReadInt32(),
          playtime = this.ReadFloat()
        });
      }
      playerList.player_count = (short) num1;
      return playerList;
    }
    catch (SocketException ex)
    {
      return (SourceServerQuery.PlayersResponse) null;
    }
  }

  public Dictionary<string, string> GetRules()
  {
    this.GetClient();
    try
    {
      Dictionary<string, string> rules = new Dictionary<string, string>();
      byte[] challenge = this.GetChallenge((byte) 86, false);
      byte[] numArray1 = new byte[challenge.Length + this.FFFFFFFF.Length + 1];
      Array.Copy((Array) this.FFFFFFFF, 0, (Array) numArray1, 0, this.FFFFFFFF.Length);
      numArray1[this.FFFFFFFF.Length] = (byte) 86;
      Array.Copy((Array) challenge, 0, (Array) numArray1, this.FFFFFFFF.Length + 1, challenge.Length);
      this.client.Send(numArray1, numArray1.Length);
      byte[] numArray2 = new byte[4096];
      this.raw_data = this.client.Receive(ref this.endPoint);
      int length1 = this.raw_data.Length;
      this.offset = 0;
      int paket = this.ReadInt32();
      this.ReadInt32();
      this.offset = 4;
      byte[] numArray3;
      if (this.PacketIsSplit(paket))
      {
        int num1 = 1;
        int packetChecksum = 0;
        int num2 = 0;
        int uncompressedSize = 0;
        List<byte[]> splitPackets = new List<byte[]>();
        bool flag;
        int num3;
        int num4;
        do
        {
          flag = this.PacketIsCompressed(this.ReverseBytes(this.ReadInt32()));
          num3 = (int) this.ReadByte();
          int num5 = (int) this.ReadByte() + 1;
          short length2 = (short) ((int) this.ReadInt16() - 4);
          if (num1 == 1)
          {
            for (int index = 0; index < num3; ++index)
              splitPackets.Add(new byte[0]);
          }
          if (flag)
          {
            uncompressedSize = this.ReverseBytes(this.ReadInt32());
            packetChecksum = this.ReverseBytes(this.ReadInt32());
          }
          if (num5 == 1)
            this.ReadInt32();
          byte[] numArray4 = new byte[(int) length2];
          splitPackets[num5 - 1] = this.ReadBytes();
          if (splitPackets[num5 - 1].Length - 1 > 0 && splitPackets[num5 - 1][splitPackets[num5 - 1].Length - 1] > (byte) 0)
            splitPackets[num5 - 1][splitPackets[num5 - 1].Length - 1] = (byte) 0;
          this.offset = 0;
          if (num1 < num3)
          {
            this.raw_data = this.client.Receive(ref this.endPoint);
            num4 = this.raw_data.Length;
            num2 = this.ReadInt32();
            ++num1;
          }
          else
            num4 = 0;
        }
        while (num1 <= num3 && num4 > 0 && num2 == -2);
        numArray3 = !flag ? this.ReassemblePacket(splitPackets, false, 0, 0) : this.ReassemblePacket(splitPackets, true, uncompressedSize, packetChecksum);
      }
      else
        numArray3 = this.raw_data;
      this.raw_data = numArray3;
      ++this.offset;
      short num = this.ReadInt16();
      for (int index = 0; index < (int) num; ++index)
      {
        string key = this.ReadString();
        string str = this.ReadString();
        if (!rules.ContainsKey(key))
          rules.Add(key, str);
      }
      return rules;
    }
    catch (SocketException ex)
    {
      Console.WriteLine((object) ex);
      return (Dictionary<string, string>) null;
    }
  }

  public void Dispose()
  {
    if (this.socket != null)
      this.socket.Close();
    if (this.client == null)
      return;
    this.client.Close();
  }

  private void GetSocket()
  {
    if (this.socket != null)
      return;
    this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    this.socket.SendTimeout = this.send_timeout;
    this.socket.ReceiveTimeout = this.receive_timeout;
    this.socket.Connect((EndPoint) this.endPoint);
  }

  private void GetClient()
  {
    if (this.client != null)
      return;
    this.client = new UdpClient();
    this.client.Connect(this.endPoint);
    this.client.DontFragment = true;
    this.client.Client.SendTimeout = this.send_timeout;
    this.client.Client.ReceiveTimeout = this.receive_timeout;
  }

  private byte[] ReassemblePacket(
    List<byte[]> splitPackets,
    bool isCompressed,
    int uncompressedSize,
    int packetChecksum)
  {
    byte[] buffer1 = new byte[0];
    foreach (byte[] splitPacket in splitPackets)
    {
      if (splitPacket == null)
        throw new Exception();
      byte[] buffer2 = buffer1;
      buffer1 = new byte[buffer2.Length + splitPacket.Length];
      MemoryStream memoryStream = new MemoryStream(buffer1);
      memoryStream.Write(buffer2, 0, buffer2.Length);
      memoryStream.Write(splitPacket, 0, splitPacket.Length);
    }
    if (isCompressed)
      throw new NotImplementedException();
    return buffer1;
  }

  private int ReverseBytes(int value)
  {
    byte[] bytes = BitConverter.GetBytes(value);
    if (BitConverter.IsLittleEndian)
      Array.Reverse((Array) bytes);
    return BitConverter.ToInt32(bytes, 0);
  }

  private bool PacketIsCompressed(int value) => (value & 32768) != 0;

  private bool PacketIsSplit(int paket) => paket == -2;

  private byte[] GetChallenge(byte type, bool socket = true)
  {
    byte[] numArray1 = new byte[this.FFFFFFFF.Length + this.FFFFFFFF.Length + 1];
    Array.Copy((Array) this.FFFFFFFF, 0, (Array) numArray1, 0, this.FFFFFFFF.Length);
    numArray1[this.FFFFFFFF.Length] = type;
    Array.Copy((Array) this.FFFFFFFF, 0, (Array) numArray1, this.FFFFFFFF.Length + 1, this.FFFFFFFF.Length);
    byte[] numArray2 = new byte[24];
    byte[] destinationArray = new byte[4];
    if (socket)
    {
      this.socket.Send(numArray1);
      this.socket.Receive(numArray2);
    }
    else
    {
      this.client.Send(numArray1, numArray1.Length);
      numArray2 = this.client.Receive(ref this.endPoint);
    }
    Array.Copy((Array) numArray2, 5, (Array) destinationArray, 0, 4);
    return destinationArray;
  }

  private byte ReadByte()
  {
    byte[] destinationArray = new byte[1];
    Array.Copy((Array) this.raw_data, this.offset, (Array) destinationArray, 0, 1);
    ++this.offset;
    return destinationArray[0];
  }

  private byte[] ReadBytes()
  {
    int length = this.raw_data.Length - this.offset - 4;
    if (length < 1)
      return new byte[0];
    byte[] destinationArray = new byte[length];
    Array.Copy((Array) this.raw_data, this.offset, (Array) destinationArray, 0, this.raw_data.Length - this.offset - 4);
    this.offset += this.raw_data.Length - this.offset - 4;
    return destinationArray;
  }

  private int ReadInt32()
  {
    byte[] destinationArray = new byte[4];
    Array.Copy((Array) this.raw_data, this.offset, (Array) destinationArray, 0, 4);
    this.offset += 4;
    return BitConverter.ToInt32(destinationArray, 0);
  }

  private short ReadInt16()
  {
    byte[] destinationArray = new byte[2];
    Array.Copy((Array) this.raw_data, this.offset, (Array) destinationArray, 0, 2);
    this.offset += 2;
    return BitConverter.ToInt16(destinationArray, 0);
  }

  private float ReadFloat()
  {
    byte[] destinationArray = new byte[4];
    Array.Copy((Array) this.raw_data, this.offset, (Array) destinationArray, 0, 4);
    this.offset += 4;
    return BitConverter.ToSingle(destinationArray, 0);
  }

  private string ReadString()
  {
    byte[] numArray = new byte[1]{ (byte) 1 };
    string str = "";
    while (numArray[0] > (byte) 0 && this.offset != this.raw_data.Length)
    {
      Array.Copy((Array) this.raw_data, this.offset, (Array) numArray, 0, 1);
      ++this.offset;
      if (numArray[0] > (byte) 0)
        str += Encoding.UTF8.GetString(numArray);
    }
    return str;
  }

  public class PlayersResponse
  {
    public short player_count;
    public List<SourceServerQuery.PlayersResponse.Player> players = new List<SourceServerQuery.PlayersResponse.Player>();

    public class Player
    {
      public string name { get; set; }

      public int score { get; set; }

      public float playtime { get; set; }
    }
  }
}
