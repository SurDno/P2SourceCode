public class ExporterRuntime : MonoBehaviour
{
  public void ExportObj(GameObject mainObj, string path, string name)
  {
    ExporterRuntimeS.ExportGOToOBJ(mainObj.GetComponentsInChildren<Transform>(), path, name);
  }
}
