using System;
using SteamNative;

namespace Facepunch.Steamworks;

public class Image {
	public int Id { get; internal set; }

	public int Width { get; internal set; }

	public int Height { get; internal set; }

	public byte[] Data { get; internal set; }

	public bool IsLoaded { get; internal set; }

	public bool IsError { get; internal set; }

	internal unsafe bool TryLoad(SteamUtils utils) {
		if (IsLoaded)
			return true;

		uint pnWidth = 0;
		uint pnHeight = 0;
		if (!utils.GetImageSize(Id, out pnWidth, out pnHeight)) {
			IsError = true;
			return true;
		}

		var numArray = new byte[(int)pnWidth * (int)pnHeight * 4];
		fixed (byte* pubDest = numArray) {
			if (!utils.GetImageRGBA(Id, (IntPtr)pubDest, numArray.Length)) {
				IsError = true;
				return true;
			}
		}

		Width = (int)pnWidth;
		Height = (int)pnHeight;
		Data = numArray;
		IsLoaded = true;
		IsError = false;
		return true;
	}

	public Color GetPixel(int x, int y) {
		if (!IsLoaded)
			throw new Exception("Image not loaded");
		if (x < 0 || x >= Width)
			throw new Exception("x out of bounds");
		if (y < 0 || y >= Height)
			throw new Exception("y out of bounds");
		var pixel = new Color();
		var index = (y * Width + x) * 4;
		pixel.r = Data[index];
		pixel.g = Data[index + 1];
		pixel.b = Data[index + 2];
		pixel.a = Data[index + 3];
		return pixel;
	}
}