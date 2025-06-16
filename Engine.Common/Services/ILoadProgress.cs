namespace Engine.Common.Services
{
  public interface ILoadProgress
  {
    void OnBeforeLoadData();

    void OnAfterLoadData();

    void OnLoadDataComplete();

    void OnBuildHierarchy();

    void OnBeforeCreateHierarchy();

    void OnAfterCreateHierarchy();

    void OnLoadComplete();
  }
}
