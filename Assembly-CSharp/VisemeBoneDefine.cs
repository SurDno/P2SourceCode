using System;
using System.Collections;
using System.Xml;
using UnityEngine;

[Serializable]
public class VisemeBoneDefine {
	public string m_visemeLabel = "";
	public BonePose[] m_BonePoses;

	public VisemeBoneDefine() { }

	public VisemeBoneDefine(string vis) {
		m_visemeLabel = vis;
	}

	public string Name {
		get => m_visemeLabel;
		set => m_visemeLabel = value;
	}

	public bool HasPose => m_BonePoses != null && m_BonePoses.Length != 0;

	public void RecordBonePositions(Transform[] boneList) {
		m_BonePoses = new BonePose[boneList.Length];
		for (var index = 0; index < boneList.Length; ++index) {
			m_BonePoses[index] = new BonePose();
			m_BonePoses[index].InitializeBone(boneList[index]);
		}
	}

	public void ResetBonePose(BonePose[] basePoses) {
		foreach (var bonePose in m_BonePoses) {
			foreach (var basePose in basePoses)
				if (basePose.m_bone == bonePose.m_bone)
					bonePose.ResetToThisPose(basePose);
		}
	}

	public void ReadViseme(XmlTextReader reader, string endTag, bool flipX, float scale) {
		var flag = false;
		var num = 0;
		var arrayList = new ArrayList();
		while (!flag && reader.Read())
			if (reader.Name == endTag && reader.NodeType == XmlNodeType.EndElement)
				flag = true;
			else if (reader.Name == "label")
				m_visemeLabel = XMLUtils.ReadXMLInnerText(reader, "label");
			else if (reader.Name == "bone") {
				++num;
				var bonePose = new BonePose();
				bonePose.ReadBonePose(reader, flipX, scale);
				arrayList.Add(bonePose);
			}

		m_BonePoses = new BonePose[arrayList.Count];
		var index = 0;
		foreach (BonePose bonePose in arrayList) {
			m_BonePoses[index] = bonePose;
			++index;
		}
	}

	public void LoadUnityBones(Transform gObject, Hashtable cache, bool bKeepLocalRotations) {
		foreach (var bonePose in m_BonePoses)
			bonePose.LoadUnityBone(gObject, cache, bKeepLocalRotations);
	}

	public void ResetToThisPose() {
		foreach (var bonePose in m_BonePoses)
			bonePose.ResetToThisTransform();
	}

	public void Print() {
		foreach (var bonePose in m_BonePoses)
			bonePose.Print();
	}

	public void ConvertPosesToOffsetsFromBase(VisemeBoneDefine defaultModel) {
		if (this == defaultModel)
			return;
		foreach (var bonePose1 in m_BonePoses) {
			var bonePose2 = defaultModel.GetBonePose(bonePose1.boneName);
			if (bonePose2 != null)
				bonePose1.ConvertToOffsets(bonePose2);
			else
				Debug.Log("VisemeBoneDefine::ConvertPosesToOffsetsFromBase - can't find bone in base pose " +
				          bonePose1.boneName);
		}
	}

	private BonePose GetBonePose(string boneName) {
		foreach (var bonePose in m_BonePoses)
			if (bonePose.boneName == boneName)
				return bonePose;
		return null;
	}

	public void Deform(float weight, BoneDeformMemento memento) {
		foreach (var bonePose in m_BonePoses)
			bonePose.Deform(weight, memento);
	}
}