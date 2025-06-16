using System;
using System.IO;
using System.Text;
using System.Xml;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Cofe.Serializations.Data.Xml;
using Cofe.Utility;
using Engine.Common;
using Engine.Common.Commons.Converters;
using UnityEngine;

public static class SerializeUtility {
	public const string RootNodeName = "Object";

	public static void Serialize<T>(string path, T template) where T : class {
		if (template == null)
			Debug.LogError("Template is null , type : " + TypeUtility.GetTypeName(typeof(T)));
		else
			try {
				using (var stream = new StreamWriter(path, false, Encoding.UTF8)) {
					var writer = new StreamDataWriter(stream);
					if (template is ISerializeDataWrite serializeDataWrite) {
						var type = ProxyFactory.GetType(template.GetType());
						writer.Begin("Object", type, true);
						serializeDataWrite.DataWrite(writer);
						writer.End("Object", true);
					} else
						Debug.LogError("Type : " + TypeUtility.GetTypeName(template.GetType()) + " is not " +
						               typeof(ISerializeDataWrite).Name);
				}
			} catch (Exception ex) {
				Debug.LogException(ex);
			}
	}

	public static T Deserialize<T>(string path) where T : class {
		using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read)) {
			return Deserialize<T>(fileStream, path);
		}
	}

	public static T Deserialize<T>(Stream stream, string context) where T : class {
		if (stream.Length <= 0L)
			return default;
		var node = new XmlDocument();
		try {
			node.Load(stream);
		} catch (Exception ex) {
			Debug.LogError(ex + " : " + context);
			return default;
		}

		var obj = DefaultDataReadUtility.ReadSerialize<T>(new XmlNodeDataReader(node, context), "Object");
		if (obj is IFactoryProduct factoryProduct)
			factoryProduct.ConstructComplete();
		return obj;
	}

	public static T Deserialize<T>(XmlNode node, string context) where T : class {
		var obj = DefaultDataReadUtility.ReadSerialize<T>(new XmlNodeDataReader(node, context));
		if (obj is IFactoryProduct factoryProduct)
			factoryProduct.ConstructComplete();
		return obj;
	}
}