using System.Collections;
using System.Xml;
using Cofe.Serializations.Data;
using Cofe.Serializations.Data.Xml;
using Cofe.Utility;
using Engine.Common;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Saves;
using Engine.Source.Services;
using Engine.Source.Services.Saves;
using Inspectors;
using JerboaAnimationInstancing;
using UnityEngine;

namespace Engine.Impl.Services;

[GameService(typeof(JerboaService), typeof(IJerboaService))]
[GenerateProxy(TypeEnum.StateSave | TypeEnum.StateLoad)]
public class JerboaService : IJerboaService, IUpdatable, IInitialisable, ISavesController {
	private GameObject prefabInstance;
	private JerboaManager jerboaManager;
	private float quality = 1f;
	private float amount;
	private JerboaColorEnum color = JerboaColorEnum.Default;

	public float Quality {
		get => quality;
		set {
			quality = value;
			if (!(jerboaManager != null))
				return;
			jerboaManager.Quality = quality;
		}
	}

	[StateSaveProxy]
	[StateLoadProxy()]
	[Inspected(Mutable = true)]
	public float Amount {
		get => amount;
		set {
			amount = value;
			if (!(jerboaManager != null))
				return;
			jerboaManager.Weight = amount;
		}
	}

	[StateSaveProxy]
	[StateLoadProxy()]
	[Inspected(Mutable = true)]
	public JerboaColorEnum Color {
		get => color;
		set {
			color = value;
			if (!(jerboaManager != null))
				return;
			jerboaManager.ColorEnum = color;
		}
	}

	public void Syncronize() {
		if (!(jerboaManager != null))
			return;
		jerboaManager.Syncronize();
	}

	public void Initialise() {
		InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable(this);
	}

	public void Terminate() {
		InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable(this);
		if (!(prefabInstance != null))
			return;
		Object.Destroy(prefabInstance);
		prefabInstance = null;
	}

	public void ComputeUpdate() {
		var player = ServiceLocator.GetService<ISimulation>().Player;
		if (player == null)
			return;
		var position = ((IEntityView)player).Position;
		if (prefabInstance == null && ScriptableObjectInstance<ResourceFromCodeData>.Instance.JerboaPrefab != null) {
			prefabInstance = Object.Instantiate(ScriptableObjectInstance<ResourceFromCodeData>.Instance.JerboaPrefab,
				position, Quaternion.identity);
			jerboaManager = prefabInstance.GetComponent<JerboaManager>();
			if (jerboaManager == null)
				Debug.Log("Jerboa prefab doesn't contain JerboaManager component");
			else {
				jerboaManager.Weight = amount;
				jerboaManager.ColorEnum = color;
				jerboaManager.Quality = quality;
				jerboaManager.Syncronize();
			}
		}

		var component = player.GetComponent<LocationItemComponent>();
		jerboaManager.Visible = component == null || !component.IsIndoor;
		if (!(prefabInstance != null))
			return;
		prefabInstance.transform.position = position;
	}

	public IEnumerator Load(IErrorLoadingHandler errorHandler) {
		yield break;
	}

	public IEnumerator Load(XmlElement element, string context, IErrorLoadingHandler errorHandler) {
		var node = element[TypeUtility.GetTypeName(GetType())];
		if (node == null)
			errorHandler.LogError(TypeUtility.GetTypeName(GetType()) + " node not found , context : " + context);
		else {
			var reader = new XmlNodeDataReader(node, context);
			((ISerializeStateLoad)this).StateLoad(reader, GetType());
			yield break;
		}
	}

	public void Unload() {
		if (!(prefabInstance != null))
			return;
		Object.Destroy(prefabInstance);
		prefabInstance = null;
	}

	public void Save(IDataWriter writer, string context) {
		DefaultStateSaveUtility.SaveSerialize(writer, TypeUtility.GetTypeName(GetType()), this);
	}
}