using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class LipsyncPlugin {
	private static string sProg = "";
	private static int iLastProg = -1;

	[DllImport("UnityLipsync")]
	private static extern int _StartLipsync(string szFile);

	[DllImport("UnityLipsync")]
	private static extern int StartLipsyncFromBuffer(
		IntPtr floatArrayPtr,
		int len,
		int sampleRate,
		int numChannels);

	public static int StartLipsyncFromBuffer(float[] pcm, int sampleRate, int numChannels) {
		var gcHandle = GCHandle.Alloc(pcm, GCHandleType.Pinned);
		var num = StartLipsyncFromBuffer(gcHandle.AddrOfPinnedObject(), pcm.Length, sampleRate, numChannels);
		gcHandle.Free();
		return num;
	}

	[DllImport("UnityLipsync")]
	private static extern int _StartTextBasedLipsync(string szFile, string sText);

	[DllImport("UnityLipsync")]
	private static extern int _SetLipsyncSmooth(int smooth);

	[DllImport("UnityLipsync")]
	private static extern int _LipsyncProgress();

	[DllImport("UnityLipsync")]
	private static extern int _LipsyncAnnoResult(StringBuilder szResult);

	[DllImport("UnityLipsync")]
	private static extern bool _IsLipsyncing();

	[DllImport("UnityLipsync")]
	private static extern void _CancelLipsync();

	public static int StartLipsync(string szFile) {
		return _StartLipsync(szFile);
	}

	public static int StartTextBasedLipsync(string szFile, string sText) {
		return _StartTextBasedLipsync(szFile, sText);
	}

	public static int LipsyncProgress() {
		return _LipsyncProgress();
	}

	public static bool IsLipsyncing() {
		return _IsLipsyncing();
	}

	public static void CancelLipsync() {
		_CancelLipsync();
	}

	public static string GetLipsyncAnnoResult() {
		var capacity = _LipsyncAnnoResult(null);
		if (capacity <= 0)
			return "error not yet finished";
		var szResult = new StringBuilder(capacity);
		_LipsyncAnnoResult(szResult);
		return szResult.ToString();
	}

	public static LipsyncSmooth Smoothness {
		get => (LipsyncSmooth)_SetLipsyncSmooth(-1);
		set => _SetLipsyncSmooth((int)value);
	}

	public static string GetProgressString() {
		var num = LipsyncProgress();
		if (num != iLastProg) {
			sProg = num.ToString();
			sProg += "%";
		}

		return sProg;
	}

	[DllImport("UnityLipsync")]
	public static extern void SetRtLatency(int latency);

	[DllImport("UnityLipsync")]
	public static extern void SetRtArticWindowMilli(int ms);

	[DllImport("UnityLipsync")]
	public static extern void LipRTStart(int sampleRate, int numChannels);

	[DllImport("UnityLipsync")]
	public static extern void LipRTStop();

	[DllImport("UnityLipsync")]
	private static extern void LipRTAddBuffer(IntPtr pcm, int len);

	public static void LipRTAddBuffer(float[] pcm) {
		var gcHandle = GCHandle.Alloc(pcm, GCHandleType.Pinned);
		LipRTAddBuffer(gcHandle.AddrOfPinnedObject(), pcm.Length);
		gcHandle.Free();
	}

	[DllImport("UnityLipsync")]
	private static extern int LipRTGetArticCount();

	[DllImport("UnityLipsync")]
	private static extern float LipRTGetArtItem(int i, StringBuilder outFileBuffer);

	[DllImport("UnityLipsync")]
	private static extern void LoadSpeechHmm(IntPtr pData, int len);

	public static int LipRTGetLipsync(out phone_weight[] phns) {
		var articCount = LipRTGetArticCount();
		if (articCount == 0) {
			phns = null;
			return 0;
		}

		phns = new phone_weight[articCount];
		for (var i = 0; i < articCount; ++i) {
			var outFileBuffer = new StringBuilder(10);
			phns[i] = new phone_weight();
			phns[i].weight = LipRTGetArtItem(i, outFileBuffer);
			phns[i].phn = outFileBuffer.ToString();
		}

		return articCount;
	}

	public static void LoadSpeechHmm(TextAsset hmm) {
		var bytes = hmm.bytes;
		var gcHandle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
		LoadSpeechHmm(gcHandle.AddrOfPinnedObject(), bytes.Length);
		gcHandle.Free();
	}

	public enum LipsyncSmooth {
		Default,
		Tight,
		Tighter
	}
}