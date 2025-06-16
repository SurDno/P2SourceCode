namespace Engine.Services.Engine.Assets;

public interface IAsset {
	bool IsError { get; }

	bool IsLoaded { get; }

	bool IsDisposed { get; }

	bool IsReadyToDispose { get; }

	bool IsValid { get; }

	string Path { get; }

	void Update();

	void Dispose(string reason);
}