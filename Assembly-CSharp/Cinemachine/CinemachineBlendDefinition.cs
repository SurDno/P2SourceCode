using System;
using UnityEngine;

namespace Cinemachine;

[DocumentationSorting(10.2f, DocumentationSortingAttribute.Level.UserRef)]
[Serializable]
public struct CinemachineBlendDefinition {
	[Tooltip("Shape of the blend curve")] public Style m_Style;

	[Tooltip("Duration of the blend, in seconds")]
	public float m_Time;

	public CinemachineBlendDefinition(Style style, float time) {
		m_Style = style;
		m_Time = time;
	}

	public AnimationCurve BlendCurve {
		get {
			var timeEnd = Mathf.Max(0.0f, m_Time);
			switch (m_Style) {
				case Style.EaseInOut:
					return AnimationCurve.EaseInOut(0.0f, 0.0f, timeEnd, 1f);
				case Style.EaseIn:
					var blendCurve1 = AnimationCurve.Linear(0.0f, 0.0f, timeEnd, 1f);
					var keys1 = blendCurve1.keys;
					keys1[1].inTangent = 0.0f;
					blendCurve1.keys = keys1;
					return blendCurve1;
				case Style.EaseOut:
					var blendCurve2 = AnimationCurve.Linear(0.0f, 0.0f, timeEnd, 1f);
					var keys2 = blendCurve2.keys;
					keys2[0].outTangent = 0.0f;
					blendCurve2.keys = keys2;
					return blendCurve2;
				case Style.HardIn:
					var blendCurve3 = AnimationCurve.Linear(0.0f, 0.0f, timeEnd, 1f);
					var keys3 = blendCurve3.keys;
					keys3[0].outTangent = 0.0f;
					keys3[1].inTangent = 1.5708f;
					blendCurve3.keys = keys3;
					return blendCurve3;
				case Style.HardOut:
					var blendCurve4 = AnimationCurve.Linear(0.0f, 0.0f, timeEnd, 1f);
					var keys4 = blendCurve4.keys;
					keys4[0].outTangent = 1.5708f;
					keys4[1].inTangent = 0.0f;
					blendCurve4.keys = keys4;
					return blendCurve4;
				case Style.Linear:
					return AnimationCurve.Linear(0.0f, 0.0f, timeEnd, 1f);
				default:
					return new AnimationCurve();
			}
		}
	}

	[DocumentationSorting(10.21f, DocumentationSortingAttribute.Level.UserRef)]
	public enum Style {
		Cut,
		EaseInOut,
		EaseIn,
		EaseOut,
		HardIn,
		HardOut,
		Linear
	}
}