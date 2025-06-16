// Decompiled with JetBrains decompiler
// Type: demo_scene_control
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class demo_scene_control : MonoBehaviour
{
  public Transform c_point;
  public Transform c_point1;
  public Transform f_point;
  public Transform[] style1 = new Transform[8];
  public Transform[] style2 = new Transform[8];
  public Transform[] style3 = new Transform[8];
  public Transform[] style4 = new Transform[8];
  private GameObject current;
  private int style = 0;
  private int cur_effect = 0;
  private int max_n = 11;

  private void Start()
  {
    this.Restart();
    Application.targetFrameRate = 60;
  }

  private void Update()
  {
    this.transform.RotateAround(this.c_point1.transform.position, Vector3.up, 0.5f);
  }

  private void OnGUI()
  {
    string name = this.current.name;
    GUI.Label(new Rect(15f, 10f, 200f, 20f), name.Substring(0, name.Length - 7));
    if (GUI.Button(new Rect(290f, 30f, 90f, 30f), "Style1"))
    {
      this.style = 0;
      this.Restart();
    }
    if (GUI.Button(new Rect(390f, 30f, 90f, 30f), "Style 2"))
    {
      this.style = 1;
      this.Restart();
    }
    if (GUI.Button(new Rect(490f, 30f, 90f, 30f), "Style 3"))
    {
      this.style = 2;
      this.Restart();
    }
    if (GUI.Button(new Rect(590f, 30f, 90f, 30f), "Style 4"))
    {
      this.style = 3;
      this.Restart();
    }
    if (GUI.Button(new Rect(10f, 30f, 40f, 30f), "<-"))
    {
      if (this.cur_effect <= 0)
        this.cur_effect = this.max_n;
      else
        --this.cur_effect;
      this.Restart();
    }
    if (!GUI.Button(new Rect(60f, 30f, 40f, 30f), "->"))
      return;
    if (this.cur_effect >= this.max_n)
      this.cur_effect = 0;
    else
      ++this.cur_effect;
    this.Restart();
  }

  private void Restart()
  {
    Transform cPoint = this.c_point;
    Object.Destroy((Object) this.current);
    if (this.cur_effect == 4 || this.cur_effect == 5)
    {
      cPoint.transform.eulerAngles = new Vector3(0.0f, -90f, 0.0f);
      cPoint.transform.position = this.c_point1.transform.position + new Vector3(1f, 1f, 0.0f);
    }
    else if (this.cur_effect == 6 || this.cur_effect == 7)
    {
      cPoint.transform.eulerAngles = this.c_point1.transform.eulerAngles;
      cPoint.transform.position = this.c_point1.transform.position + new Vector3(0.0f, 1f, 0.0f);
    }
    else if (this.cur_effect == 10 || this.cur_effect == 11)
    {
      cPoint.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
      cPoint.transform.position = this.c_point1.transform.position + new Vector3(0.0f, 1f, 0.0f);
    }
    else
    {
      cPoint.transform.eulerAngles = this.c_point1.transform.eulerAngles;
      cPoint.transform.position = this.c_point1.transform.position;
    }
    if (this.style == 0)
      this.current = Object.Instantiate<Transform>(this.style1[this.cur_effect], cPoint.transform.position, cPoint.transform.rotation).gameObject;
    if (this.style == 1)
      this.current = Object.Instantiate<Transform>(this.style2[this.cur_effect], cPoint.transform.position, cPoint.transform.rotation).gameObject;
    if (this.style == 2)
      this.current = Object.Instantiate<Transform>(this.style3[this.cur_effect], cPoint.transform.position, cPoint.transform.rotation).gameObject;
    if (this.style != 3)
      return;
    this.current = Object.Instantiate<Transform>(this.style4[this.cur_effect], cPoint.transform.position, cPoint.transform.rotation).gameObject;
  }
}
