using Engine.Source.Commons;
using System;
using UnityEngine;
using UnityEngine.UI;

public class BuildVersionText : MonoBehaviour
{
  private void Start()
  {
    InstanceByRequest<LabelService>.Instance.OnInvalidate += new Action(this.OnInvalidate);
    this.OnInvalidate();
  }

  private void OnInvalidate()
  {
    this.GetComponent<Text>().text = InstanceByRequest<LabelService>.Instance.Label;
  }
}
