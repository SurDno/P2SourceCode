using System;

namespace Engine.Source.Services.Saves;

public interface IErrorLoadingHandler {
	string ErrorLoading { get; }

	bool HasErrorLoading { get; }

	void LogError(string text);

	void LogException(Exception e);
}