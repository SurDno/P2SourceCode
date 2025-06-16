using System;
using System.Diagnostics;
using System.IO;
using SteamNative;

namespace Facepunch.Steamworks;

public class Voice {
	private const int ReadBufferSize = 131072;
	internal Client client;
	internal byte[] ReadCompressedBuffer = new byte[131072];
	internal byte[] ReadUncompressedBuffer = new byte[131072];
	internal byte[] UncompressBuffer = new byte[262144];
	public Action<byte[], int> OnCompressedData;
	public Action<byte[], int> OnUncompressedData;
	private Stopwatch UpdateTimer = Stopwatch.StartNew();
	private bool _wantsrecording;
	public bool IsRecording;
	public uint DesiredSampleRate = 0;

	public uint OptimalSampleRate => client.native.user.GetVoiceOptimalSampleRate();

	public bool WantsRecording {
		get => _wantsrecording;
		set {
			_wantsrecording = value;
			if (value)
				client.native.user.StartVoiceRecording();
			else
				client.native.user.StopVoiceRecording();
		}
	}

	public DateTime LastVoiceRecordTime { get; private set; }

	public TimeSpan TimeSinceLastVoiceRecord => DateTime.Now.Subtract(LastVoiceRecordTime);

	internal Voice(Client client) {
		this.client = client;
	}

	public unsafe void Update() {
		if ((OnCompressedData == null && OnUncompressedData == null) ||
		    UpdateTimer.Elapsed.TotalSeconds < 0.10000000149011612)
			return;
		UpdateTimer.Reset();
		UpdateTimer.Start();
		uint num1 = 0;
		uint num2 = 0;
		var availableVoice = client.native.user.GetAvailableVoice(out num2, out num1,
			DesiredSampleRate == 0U ? OptimalSampleRate : DesiredSampleRate);
		if (availableVoice == VoiceResult.NotRecording || availableVoice == VoiceResult.NotInitialized)
			IsRecording = false;
		else {
			VoiceResult voice;
			fixed (byte* pDestBuffer = ReadCompressedBuffer) {
				fixed (byte* pUncompressedDestBuffer_Deprecated = ReadUncompressedBuffer) {
					voice = client.native.user.GetVoice(OnCompressedData != null, (IntPtr)pDestBuffer, 131072U,
						out num2, OnUncompressedData != null, (IntPtr)pUncompressedDestBuffer_Deprecated, 131072U,
						out num1, DesiredSampleRate == 0U ? OptimalSampleRate : DesiredSampleRate);
				}
			}

			IsRecording = true;
			if (voice == VoiceResult.OK) {
				if (OnCompressedData != null && num2 > 0U)
					OnCompressedData(ReadCompressedBuffer, (int)num2);
				if (OnUncompressedData != null && num1 > 0U)
					OnUncompressedData(ReadUncompressedBuffer, (int)num1);
				LastVoiceRecordTime = DateTime.Now;
			}

			if (voice != VoiceResult.NotRecording && voice != VoiceResult.NotInitialized)
				return;
			IsRecording = false;
		}
	}

	public bool Decompress(byte[] input, MemoryStream output, uint samepleRate = 0) {
		return Decompress(input, 0, input.Length, output, samepleRate);
	}

	public bool Decompress(byte[] input, int inputsize, MemoryStream output, uint samepleRate = 0) {
		return Decompress(input, 0, inputsize, output, samepleRate);
	}

	public unsafe bool Decompress(
		byte[] input,
		int inputoffset,
		int inputsize,
		MemoryStream output,
		uint samepleRate = 0) {
		if (inputoffset < 0 || inputoffset >= input.Length)
			throw new ArgumentOutOfRangeException(nameof(inputoffset));
		if (inputsize <= 0 || inputoffset + inputsize > input.Length)
			throw new ArgumentOutOfRangeException(nameof(inputsize));
		fixed (byte* input1 = input) {
			return Decompress((IntPtr)input1, inputoffset, inputsize, output, samepleRate);
		}
	}

	private unsafe bool Decompress(
		IntPtr input,
		int inputoffset,
		int inputsize,
		MemoryStream output,
		uint samepleRate = 0) {
		if (samepleRate == 0U)
			samepleRate = OptimalSampleRate;
		uint nBytesWritten = 0;
		VoiceResult voiceResult;
		fixed (byte* pDestBuffer = UncompressBuffer) {
			voiceResult = client.native.user.DecompressVoice((IntPtr)(void*)((IntPtr)(void*)input + inputoffset),
				(uint)inputsize, (IntPtr)pDestBuffer, (uint)UncompressBuffer.Length, out nBytesWritten, samepleRate);
		}

		if (voiceResult != VoiceResult.OK)
			return false;
		output.Write(UncompressBuffer, 0, (int)nBytesWritten);
		return true;
	}
}