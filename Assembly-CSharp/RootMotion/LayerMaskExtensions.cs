using System.Collections.Generic;
using UnityEngine;

namespace RootMotion;

public static class LayerMaskExtensions {
	public static bool Contains(LayerMask mask, int layer) {
		return mask == (mask | (1 << layer));
	}

	public static LayerMask Create(params string[] layerNames) {
		return NamesToMask(layerNames);
	}

	public static LayerMask Create(params int[] layerNumbers) {
		return LayerNumbersToMask(layerNumbers);
	}

	public static LayerMask NamesToMask(params string[] layerNames) {
		LayerMask mask = 0;
		foreach (var layerName in layerNames)
			mask = mask | (1 << LayerMask.NameToLayer(layerName));
		return mask;
	}

	public static LayerMask LayerNumbersToMask(params int[] layerNumbers) {
		LayerMask mask = 0;
		foreach (var layerNumber in layerNumbers)
			mask = mask | (1 << layerNumber);
		return mask;
	}

	public static LayerMask Inverse(this LayerMask original) {
		return ~original;
	}

	public static LayerMask AddToMask(this LayerMask original, params string[] layerNames) {
		return original | NamesToMask(layerNames);
	}

	public static LayerMask RemoveFromMask(this LayerMask original, params string[] layerNames) {
		return ~((LayerMask)~original | NamesToMask(layerNames));
	}

	public static string[] MaskToNames(this LayerMask original) {
		var stringList = new List<string>();
		for (var layer = 0; layer < 32; ++layer) {
			var num = 1 << layer;
			if ((original & num) == num) {
				var name = LayerMask.LayerToName(layer);
				if (!string.IsNullOrEmpty(name))
					stringList.Add(name);
			}
		}

		return stringList.ToArray();
	}

	public static int[] MaskToNumbers(this LayerMask original) {
		var intList = new List<int>();
		for (var index = 0; index < 32; ++index) {
			var num = 1 << index;
			if ((original & num) == num)
				intList.Add(index);
		}

		return intList.ToArray();
	}

	public static string MaskToString(this LayerMask original) {
		return original.MaskToString(", ");
	}

	public static string MaskToString(this LayerMask original, string delimiter) {
		return string.Join(delimiter, original.MaskToNames());
	}
}