using SteamNative;
using System;
using System.Diagnostics;
using System.IO;

namespace Facepunch.Steamworks
{
  public class Voice
  {
    private const int ReadBufferSize = 131072;
    internal Client client;
    internal byte[] ReadCompressedBuffer = new byte[131072];
    internal byte[] ReadUncompressedBuffer = new byte[131072];
    internal byte[] UncompressBuffer = new byte[262144];
    public Action<byte[], int> OnCompressedData;
    public Action<byte[], int> OnUncompressedData;
    private Stopwatch UpdateTimer = Stopwatch.StartNew();
    private bool _wantsrecording = false;
    public bool IsRecording = false;
    public uint DesiredSampleRate = 0;

    public uint OptimalSampleRate => this.client.native.user.GetVoiceOptimalSampleRate();

    public bool WantsRecording
    {
      get => this._wantsrecording;
      set
      {
        this._wantsrecording = value;
        if (value)
          this.client.native.user.StartVoiceRecording();
        else
          this.client.native.user.StopVoiceRecording();
      }
    }

    public DateTime LastVoiceRecordTime { get; private set; }

    public TimeSpan TimeSinceLastVoiceRecord => DateTime.Now.Subtract(this.LastVoiceRecordTime);

    internal Voice(Client client) => this.client = client;

    public unsafe void Update()
    {
      if (this.OnCompressedData == null && this.OnUncompressedData == null || this.UpdateTimer.Elapsed.TotalSeconds < 0.10000000149011612)
        return;
      this.UpdateTimer.Reset();
      this.UpdateTimer.Start();
      uint num1 = 0;
      uint num2 = 0;
      VoiceResult availableVoice = this.client.native.user.GetAvailableVoice(out num2, out num1, this.DesiredSampleRate == 0U ? this.OptimalSampleRate : this.DesiredSampleRate);
      if (availableVoice == VoiceResult.NotRecording || availableVoice == VoiceResult.NotInitialized)
      {
        this.IsRecording = false;
      }
      else
      {
        VoiceResult voice;
        fixed (byte* pDestBuffer = this.ReadCompressedBuffer)
          fixed (byte* pUncompressedDestBuffer_Deprecated = this.ReadUncompressedBuffer)
            voice = this.client.native.user.GetVoice(this.OnCompressedData != null, (IntPtr) (void*) pDestBuffer, 131072U, out num2, this.OnUncompressedData != null, (IntPtr) (void*) pUncompressedDestBuffer_Deprecated, 131072U, out num1, this.DesiredSampleRate == 0U ? this.OptimalSampleRate : this.DesiredSampleRate);
        this.IsRecording = true;
        if (voice == VoiceResult.OK)
        {
          if (this.OnCompressedData != null && num2 > 0U)
            this.OnCompressedData(this.ReadCompressedBuffer, (int) num2);
          if (this.OnUncompressedData != null && num1 > 0U)
            this.OnUncompressedData(this.ReadUncompressedBuffer, (int) num1);
          this.LastVoiceRecordTime = DateTime.Now;
        }
        if (voice != VoiceResult.NotRecording && voice != VoiceResult.NotInitialized)
          return;
        this.IsRecording = false;
      }
    }

    public bool Decompress(byte[] input, MemoryStream output, uint samepleRate = 0)
    {
      return this.Decompress(input, 0, input.Length, output, samepleRate);
    }

    public bool Decompress(byte[] input, int inputsize, MemoryStream output, uint samepleRate = 0)
    {
      return this.Decompress(input, 0, inputsize, output, samepleRate);
    }

    public unsafe bool Decompress(
      byte[] input,
      int inputoffset,
      int inputsize,
      MemoryStream output,
      uint samepleRate = 0)
    {
      if (inputoffset < 0 || inputoffset >= input.Length)
        throw new ArgumentOutOfRangeException(nameof (inputoffset));
      if (inputsize <= 0 || inputoffset + inputsize > input.Length)
        throw new ArgumentOutOfRangeException(nameof (inputsize));
      fixed (byte* input1 = input)
        return this.Decompress((IntPtr) (void*) input1, inputoffset, inputsize, output, samepleRate);
    }

    private unsafe bool Decompress(
      IntPtr input,
      int inputoffset,
      int inputsize,
      MemoryStream output,
      uint samepleRate = 0)
    {
      if (samepleRate == 0U)
        samepleRate = this.OptimalSampleRate;
      uint nBytesWritten = 0;
      VoiceResult voiceResult;
      fixed (byte* pDestBuffer = this.UncompressBuffer)
        voiceResult = this.client.native.user.DecompressVoice((IntPtr) (void*) ((IntPtr) (void*) input + inputoffset), (uint) inputsize, (IntPtr) (void*) pDestBuffer, (uint) this.UncompressBuffer.Length, out nBytesWritten, samepleRate);
      if (voiceResult != VoiceResult.OK)
        return false;
      output.Write(this.UncompressBuffer, 0, (int) nBytesWritten);
      return true;
    }
  }
}
