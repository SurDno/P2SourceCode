using UnityEngine;

public class TOD_LoadSkyFromFile : MonoBehaviour
{
  public TOD_Sky sky;
  public TextAsset textAsset = (TextAsset) null;

  protected void Start()
  {
    if (!(bool) (Object) this.sky)
      this.sky = TOD_Sky.Instance;
    if (!(bool) (Object) this.textAsset)
      return;
    this.sky.LoadParameters(this.textAsset.text);
  }
}
