using System.IO;
using System.IO.Compression;

namespace Scripts.Utility;

public static class CompressUtility {
	public static byte[] CompressData(byte[] data) {
		using (var memoryStream = new MemoryStream()) {
			using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress)) {
				gzipStream.Write(data, 0, data.Length);
			}

			return memoryStream.ToArray();
		}
	}

	public static byte[] DecompressData(byte[] data) {
		using (var memoryStream = new MemoryStream(data)) {
			using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress)) {
				using (var destination = new MemoryStream()) {
					gzipStream.CopyTo(destination);
					return destination.ToArray();
				}
			}
		}
	}
}