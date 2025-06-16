using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Cofe.Meta;
using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Connections;
using Engine.Source.Services.Templates;
using Engine.Source.Test;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class CreateObjects : MonoBehaviour {
	private List<IEntity> instances;
	[SerializeField] private IEntitySerializable template;
	[SerializeField] private int count;

	private IEnumerator Start() {
		yield return new WaitForSeconds(1f);
		Test();
		yield return new WaitForSeconds(1f);
	}

	private void Test() {
		var service = ServiceLocator.GetService<IFactory>();
		ServiceLocator.GetService<IEditorTemplateService>();
		var template = this.template.Value;
		if (template == null)
			Debug.LogError("Template not found, id : " + this.template.Id);
		else {
			instances = new List<IEntity>(count);
			var stopwatch = new Stopwatch();
			stopwatch.Restart();
			for (var index = 0; index < count; ++index) {
				var target = service.Instantiate(template);
				MetaService.GetContainer(target.GetType()).GetHandler(TestAttribute.Id).Compute(target, null);
				instances.Add(target);
			}

			stopwatch.Stop();
			Debug.LogError("Complete, count : " + count + " , elapsed : " + stopwatch.Elapsed);
		}
	}
}