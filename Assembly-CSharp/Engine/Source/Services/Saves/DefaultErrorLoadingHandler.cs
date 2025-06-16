using System;
using UnityEngine;

namespace Engine.Source.Services.Saves;

public class DefaultErrorLoadingHandler : IErrorLoadingHandler {
	public string ErrorLoading { get; private set; }

	public bool HasErrorLoading => ErrorLoading != null;

	public void LogError(string text) {
		ErrorLoading = text;
		Debug.LogError(text);
	}

	public void LogException(Exception e) {
		ErrorLoading = e.ToString();
		Debug.LogException(e);
	}
}