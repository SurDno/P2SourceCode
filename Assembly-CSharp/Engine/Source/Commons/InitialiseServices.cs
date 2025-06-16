using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cofe.Meta;
using Cofe.Utility;
using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Services;
using Scripts.Utility;
using Debug = UnityEngine.Debug;

namespace Engine.Source.Commons;

public static class InitialiseServices {
	private static List<IInitialisable> services;

	public static bool IsInitialised { get; private set; }

	public static IEnumerator Initialise() {
		if (IsInitialised)
			throw new Exception();
		var sw = new Stopwatch();
		sw.Restart();
		var progressService = ServiceLocator.GetService<InitialiseEngineProgressService>();
		foreach (var service in ServiceLocator.GetServices()) {
			Debug.Log(ObjectInfoUtility.GetStream().Append("[Engine]").Append(" ")
				.Append(TypeUtility.GetTypeName(service.GetType())));
			MetaService.GetContainer(service.GetType()).GetHandler(FromLocatorAttribute.Id).Compute(service, null);
		}

		services = RuntimeServiceAttribute.GetServices().Select(o => o as IInitialisable).Where(o => o != null)
			.ToList();
		services = ServiceLocatorUtility.SortServices<IInitialisable, DependAttribute>(services);
		var step = 500;
		var maxCount = services.Count * step;
		foreach (var service in services) {
			if (service is IAsyncInitializable async)
				maxCount += async.AsyncCount;
			async = null;
		}

		progressService.Begin(maxCount);
		sw.Stop();
		Debug.Log(ObjectInfoUtility.GetStream().Append("[Engine]").Append(" Prepare initialise, count : ")
			.Append(services.Count).Append(" , elapsed : ").Append(sw.Elapsed));
		for (var index = 0; index < services.Count; ++index) {
			var service = services[index];
			Debug.Log(ObjectInfoUtility.GetStream().Append("[Engine]").Append(" Initialise service : ")
				.Append(TypeUtility.GetTypeName(service.GetType())));
			progressService.Progress += step;
			progressService.Update(nameof(InitialiseServices), TypeUtility.GetTypeName(service.GetType()));
			sw.Restart();
			if (service is IAsyncInitializable async)
				yield return async.AsyncInitialize();
			else
				try {
					service.Initialise();
				} catch (Exception ex) {
					Debug.LogException(ex);
				}

			sw.Stop();
			Debug.Log(ObjectInfoUtility.GetStream().Append("[Engine]").Append(" Initialise service complete : ")
				.Append(TypeUtility.GetTypeName(service.GetType())).Append(" , elapsed : ").Append(sw.Elapsed));
			yield return null;
			service = null;
			async = null;
		}

		progressService.End();
		IsInitialised = true;
	}

	public static void Terminate() {
		IsInitialised = IsInitialised ? false : throw new Exception();
		for (var index = services.Count - 1; index >= 0; --index) {
			var service = services[index];
			try {
				service.Terminate();
			} catch (Exception ex) {
				Debug.LogException(ex);
			}
		}

		services = null;
	}
}