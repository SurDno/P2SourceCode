using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

internal class SourceServerQuery : IDisposable
{
  private IPEndPoint endPoint;
  private Socket socket;
  private UdpClient client;
  private int send_timeout = 2500;
  private int receive_timeout = 2500;
  private byte[] raw_data;
  private int offset;
  private readonly byte[] FFFFFFFF = new byte[4]
  {
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue
  };

  public SourceServerQuery(string ip, int port)
  {
    endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
  }

  public PlayersResponse GetPlayerList()
  {
    GetSocket();
    offset = 5;
    try
    {
      PlayersResponse playerList = new PlayersResponse();
      byte[] challenge = GetChallenge(85);
      byte[] numArray = new byte[challenge.Length + FFFFFFFF.Length + 1];
      Array.Copy(FFFFFFFF, 0, numArray, 0, FFFFFFFF.Length);
      numArray[FFFFFFFF.Length] = 85;
      Array.Copy(challenge, 0, numArray, FFFFFFFF.Length + 1, challenge.Length);
      socket.Send(numArray);
      raw_data = new byte[2048];
      socket.Receive(raw_data);
      byte num1 = ReadByte();
      for (int index = 0; index < num1; ++index)
      {
        int num2 = ReadByte();
        playerList.players.Add(new PlayersResponse.Player {
          name = ReadString(),
          score = ReadInt32(),
          playtime = ReadFloat()
        });
      }
      playerList.player_count = num1;
      return playerList;
    }
    catch (SocketException ex)
    {
      return null;
    }
  }

  public Dictionary<string, string> GetRules()
  {
    GetClient();
    try
    {
      Dictionary<string, string> rules = new Dictionary<string, string>();
      byte[] challenge = GetChallenge(86, false);
      byte[] numArray1 = new byte[challenge.Length + FFFFFFFF.Length + 1];
      Array.Copy(FFFFFFFF, 0, numArray1, 0, FFFFFFFF.Length);
      numArray1[FFFFFFFF.Length] = 86;
      Array.Copy(challenge, 0, numArray1, FFFFFFFF.Length + 1, challenge.Length);
      client.Send(numArray1, numArray1.Length);
      byte[] numArray2 = new byte[4096];
      raw_data = client.Receive(ref endPoint);
      int length1 = raw_data.Length;
      offset = 0;
      int paket = ReadInt32();
      ReadInt32();
      offset = 4;
      byte[] numArray3;
      if (PacketIsSplit(paket))
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
          flag = PacketIsCompressed(ReverseBytes(ReadInt32()));
          num3 = ReadByte();
          int num5 = ReadByte() + 1;
          short length2 = (short) (ReadInt16() - 4);
          if (num1 == 1)
          {
            for (int index = 0; index < num3; ++index)
              splitPackets.Add(new byte[0]);
          }
          if (flag)
          {
            uncompressedSize = ReverseBytes(ReadInt32());
            packetChecksum = ReverseBytes(ReadInt32());
          }
          if (num5 == 1)
            ReadInt32();
          byte[] numArray4 = new byte[length2];
          splitPackets[num5 - 1] = ReadBytes();
          if (splitPackets[num5 - 1].Length - 1 > 0 && splitPackets[num5 - 1][splitPackets[num5 - 1].Length - 1] > 0)
            splitPackets[num5 - 1][splitPackets[num5 - 1].Length - 1] = 0;
          offset = 0;
          if (num1 < num3)
          {
            raw_data = client.Receive(ref endPoint);
            num4 = raw_data.Length;
            num2 = ReadInt32();
            ++num1;
          }
          else
            num4 = 0;
        }
        while (num1 <= num3 && num4 > 0 && num2 == -2);
        numArray3 = !flag ? ReassemblePacket(splitPackets, false, 0, 0) : ReassemblePacket(splitPackets, true, uncompressedSize, packetChecksum);
      }
      else
        numArray3 = raw_data;
      raw_data = numArray3;
      ++offset;
      short num = ReadInt16();
      for (int index = 0; index < num; ++index)
      {
        string key = ReadString();
        string str = ReadString();
        if (!rules.ContainsKey(key))
          rules.Add(key, str);
      }
      return rules;
    }
    catch (SocketException ex)
    {
      Console.WriteLine(ex);
      return null;
    }
  }

  public void Dispose()
  {
    if (socket != null)
      socket.Close();
    if (client == null)
      return;
    client.Close();
  }

  private void GetSocket()
  {
    if (socket != null)
      return;
    socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    socket.SendTimeout = send_timeout;
    socket.ReceiveTimeout = receive_timeout;
    socket.Connect(endPoint);
  }

  private void GetClient()
  {
    if (client != null)
      return;
    client = new UdpClient();
    client.Connect(endPoint);
    client.DontFragment = true;
    client.Client.SendTimeout = send_timeout;
    client.Client.ReceiveTimeout = receive_timeout;
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
      Array.Reverse(bytes);
    return BitConverter.ToInt32(bytes, 0);
  }

  private bool PacketIsCompressed(int value) => (value & 32768) != 0;

  private bool PacketIsSplit(int paket) => paket == -2;

  private byte[] GetChallenge(byte type, bool socket = true)
  {
    byte[] numArray1 = new byte[FFFFFFFF.Length + FFFFFFFF.Length + 1];
    Array.Copy(FFFFFFFF, 0, numArray1, 0, FFFFFFFF.Length);
    numArray1[FFFFFFFF.Length] = type;
    Array.Copy(FFFFFFFF, 0, numArray1, FFFFFFFF.Length + 1, FFFFFFFF.Length);
    byte[] numArray2 = new byte[24];
    byte[] destinationArray = new byte[4];
    if (socket)
    {
      this.socket.Send(numArray1);
      this.socket.Receive(numArray2);
    }
    else
    {
      client.Send(numArray1, numArray1.Length);
      numArray2 = client.Receive(ref endPoint);
    }
    Array.Copy(numArray2, 5, destinationArray, 0, 4);
    return destinationArray;
  }

  private byte ReadByte()
  {
    byte[] destinationArray = new byte[1];
    Array.Copy(raw_data, offset, destinationArray, 0, 1);
    ++offset;
    return destinationArray[0];
  }

  private byte[] ReadBytes()
  {
    int length = raw_data.Length - offset - 4;
    if (length < 1)
      return new byte[0];
    byte[] destinationArray = new byte[length];
    Array.Copy(raw_data, offset, destinationArray, 0, raw_data.Length - offset - 4);
    offset += raw_data.Length - offset - 4;
    return destinationArray;
  }

  private int ReadInt32()
  {
    byte[] destinationArray = new byte[4];
    Array.Copy(raw_data, offset, destinationArray, 0, 4);
    offset += 4;
    return BitConverter.ToInt32(destinationArray, 0);
  }

  private short ReadInt16()
  {
    byte[] destinationArray = new byte[2];
    Array.Copy(raw_data, offset, destinationArray, 0, 2);
    offset += 2;
    return BitConverter.ToInt16(destinationArray, 0);
  }

  private float ReadFloat()
  {
    byte[] destinationArray = new byte[4];
    Array.Copy(raw_data, offset, destinationArray, 0, 4);
    offset += 4;
    return BitConverter.ToSingle(destinationArray, 0);
  }

  private string ReadString()
  {
    byte[] numArray = new byte[1] { 1 };
    string str = "";
    while (numArray[0] > 0 && offset != raw_data.Length)
    {
      Array.Copy(raw_data, offset, numArray, 0, 1);
      ++offset;
      if (numArray[0] > 0)
        str += Encoding.UTF8.GetString(numArray);
    }
    return str;
  }

  public class PlayersResponse
  {
    public short player_count;
    public List<Player> players = new List<Player>();

    public class Player
    {
      public string name { get; set; }

      public int score { get; set; }

      public float playtime { get; set; }
    }
  }
}
