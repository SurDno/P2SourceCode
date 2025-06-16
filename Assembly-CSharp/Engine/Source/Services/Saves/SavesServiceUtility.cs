using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;
using Cofe.Serializations.Data.Xml;
using Engine.Source.Saves;
using UnityEngine;

namespace Engine.Source.Services.Saves;

public static class SavesServiceUtility {
	public static XmlDocument LoadDocument(string fileName) {
		if (File.Exists(fileName + ".gz"))
			try {
				using (var fileStream = File.OpenRead(fileName + ".gz")) {
					using (var inStream = new GZipStream(fileStream, CompressionMode.Decompress)) {
						var xmlDocument = new XmlDocument();
						xmlDocument.Load(inStream);
						return xmlDocument;
					}
				}
			} catch (Exception ex) {
				Debug.LogError("Error open file : " + fileName + ".gz , error : " + ex);
				return null;
			}

		if (File.Exists(fileName))
			try {
				var xmlDocument = new XmlDocument();
				xmlDocument.Load(fileName);
				return xmlDocument;
			} catch (Exception ex) {
				Debug.LogError("Error open file : " + fileName + " , error : " + ex);
				return null;
			}

		Debug.LogError("File not found : " + fileName);
		return null;
	}

	public static void SaveToFile(
		string nodeName,
		ISavesController serializable,
		string fileName,
		bool compress) {
		if (compress)
			using (var fileStream = File.Create(fileName + ".gz")) {
				using (var gzipStream = new GZipStream(fileStream, CompressionMode.Compress)) {
					using (var stream = new StreamWriter(gzipStream, Encoding.UTF8)) {
						var element = new StreamDataWriter(stream);
						element.Begin(nodeName, null, true);
						serializable.Save(element, fileName);
						element.End(nodeName, true);
					}
				}
			}
		else
			using (var stream = new StreamWriter(fileName, false, Encoding.UTF8)) {
				var element = new StreamDataWriter(stream);
				element.Begin(nodeName, null, true);
				serializable.Save(element, fileName);
				element.End(nodeName, true);
			}
	}
}