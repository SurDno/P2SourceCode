// Decompiled with JetBrains decompiler
// Type: TOD_LoadSkyFromFile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
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
