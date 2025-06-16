using System;
using System.Runtime.InteropServices;
using Engine.Source.Settings.External;
using UnityEngine;

namespace Engine.Source.Utility;

public static class AffinityUtility {
	[DllImport("Affinity")]
	private static extern bool GetAffinity(ref long value);

	[DllImport("Affinity")]
	private static extern bool SetAffinity(long value);

	public static void ComputeAffinity() {
		try {
			Debug.Log("ComputeAffinity , try compute");
			var affinity = ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.Affinity;
			switch (affinity) {
				case -1:
					Debug.Log("ComputeAffinity , not requred");
					break;
				case 0:
					Debug.Log("ComputeAffinity , try disable ht");
					var numberOfCores = SystemInfoUtility.NumberOfCores;
					var logicalProcessors1 = SystemInfoUtility.NumberOfLogicalProcessors;
					Debug.Log("ComputeAffinity , NumberOfCores : " + numberOfCores);
					Debug.Log("ComputeAffinity , NumberOfLogicalProcessors : " + logicalProcessors1);
					if (ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.MinAffinityCount >
					    logicalProcessors1) {
						Debug.Log("ComputeAffinity , min : " +
						          ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.MinAffinityCount +
						          " > current : " + logicalProcessors1);
						break;
					}

					long num1 = 0;
					if (!GetAffinity(ref num1)) {
						Debug.LogError("ComputeAffinity , Error invoke : GetAffinity");
						break;
					}

					Debug.Log("ComputeAffinity , current value : " + ToBinary(num1, logicalProcessors1));
					if (numberOfCores == logicalProcessors1) {
						Debug.Log("ComputeAffinity , no ht detected");
						break;
					}

					if (numberOfCores * 2 == logicalProcessors1) {
						Debug.Log("ComputeAffinity , try compute x2");
						for (long index = 0; index < numberOfCores; ++index) {
							var num2 = index * 2L + 1L;
							num1 &= ~(1 << (int)num2);
						}

						if (!SetAffinity(num1)) {
							Debug.LogError("ComputeAffinity , Error invoke : SetAffinity");
							break;
						}

						if (!GetAffinity(ref num1)) {
							Debug.LogError("ComputeAffinity , Error invoke : GetAffinity");
							break;
						}

						Debug.Log("ComputeAffinity , new value : " + ToBinary(num1, logicalProcessors1));
						break;
					}

					Debug.Log("ComputeAffinity , is not support");
					break;
				default:
					if (affinity > 0) {
						Debug.Log("ComputeAffinity , try set value : " + affinity);
						var logicalProcessors2 = SystemInfoUtility.NumberOfLogicalProcessors;
						long num3 = 0;
						if (!GetAffinity(ref num3)) {
							Debug.LogError("ComputeAffinity , Error invoke : GetAffinity");
							break;
						}

						Debug.Log("ComputeAffinity , current value : " + ToBinary(num3, logicalProcessors2));
						long num4 = affinity;
						if (!SetAffinity(num4)) {
							Debug.LogError("ComputeAffinity , Error invoke : SetAffinity");
							break;
						}

						if (!GetAffinity(ref num4)) {
							Debug.LogError("ComputeAffinity , Error invoke : GetAffinity");
							break;
						}

						Debug.Log("ComputeAffinity , new value : " + ToBinary(num4, logicalProcessors2));
						break;
					}

					Debug.Log("ComputeAffinity , wrong value");
					break;
			}
		} catch (Exception ex) {
			Debug.LogException(ex);
		}
	}

	public static string ToBinary(long value, int count) {
		return "0b" + Convert.ToString(value, 2).PadLeft(count, '0');
	}
}