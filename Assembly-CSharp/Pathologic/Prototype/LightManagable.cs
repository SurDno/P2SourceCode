using Engine.Common.DateTime;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Commons;
using Inspectors;
using UnityEngine;

namespace Pathologic.Prototype;

public class LightManagable : MonoBehaviour {
	[SerializeField] private bool DisableShadowsInPlagueIntro;

	[EnumFlag(typeof(TimesOfDay))] [SerializeField]
	private TimesOfDay timesOfDay;

	[SerializeField] private Light AdditionalLight;
	[SerializeField] private GameObject BulbObject;

	[Tooltip("Time offset in minutes to make light turning on/off more random looking")] [SerializeField]
	public float TimeOffsetInMinutes;

	[Inspected] private TimesOfDay current;
	private Light light;
	private Renderer bulbRenderer;
	private bool lightEnabled;
	private static MaterialPropertyBlock mpb;

	private void Start() {
		light = GetComponent<Light>();
		if ((bool)(Object)BulbObject)
			bulbRenderer = BulbObject.GetComponent<Renderer>();
		if (light != null) {
			enabled = light.enabled;
			if (DisableShadowsInPlagueIntro) {
				var service = ServiceLocator.GetService<VirtualMachineController>();
				if (service != null && service.ProjectName == "PathologicPlagueIntro")
					light.shadows = LightShadows.None;
			}
		}

		EnableLight(enabled);
	}

	private void Update() {
		if (!InstanceByRequest<EngineApplication>.Instance.IsInitialized)
			return;
		current = ServiceLocator.GetService<ITimeService>().SolarTime.GetTimesOfDay();
		if (current == TimesOfDay.Night) {
			if (lightEnabled == TimesOfDayUtility.HasValue(timesOfDay, TimesOfDay.Night))
				return;
			EnableLight(!lightEnabled);
		} else if (current == TimesOfDay.Morning) {
			if (lightEnabled == TimesOfDayUtility.HasValue(timesOfDay, TimesOfDay.Morning))
				return;
			EnableLight(!lightEnabled);
		} else if (current == TimesOfDay.Day) {
			if (lightEnabled == TimesOfDayUtility.HasValue(timesOfDay, TimesOfDay.Day))
				return;
			EnableLight(!lightEnabled);
		} else {
			if (current != TimesOfDay.Evening ||
			    lightEnabled == TimesOfDayUtility.HasValue(timesOfDay, TimesOfDay.Evening))
				return;
			EnableLight(!lightEnabled);
		}
	}

	public void EnableLight(bool enable) {
		if (light != null)
			light.enabled = enable;
		if (AdditionalLight != null)
			AdditionalLight.enabled = enable;
		var component1 = GetComponent<LightServiceObject>();
		if (component1 != null)
			component1.enabled = enable;
		var component2 = GetComponent<LightFlicker2>();
		if (component2 != null)
			component2.enabled = enable;
		if (bulbRenderer != null) {
			if (enable)
				bulbRenderer.SetPropertyBlock(null);
			else
				bulbRenderer.SetPropertyBlock(MPB);
		}

		lightEnabled = enable;
	}

	private static MaterialPropertyBlock MPB {
		get {
			if (mpb == null) {
				mpb = new MaterialPropertyBlock();
				mpb.SetColor("_EmissionColor", Color.black);
			}

			return mpb;
		}
	}
}