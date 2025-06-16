using Engine.Impl.UI.Controls;
using FlowCanvas;
using FlowCanvas.Nodes;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints.Effects
{
  [Category("Effects")]
  public class SpriteEffectNode : FlowControlNode, IUpdatable
  {
    [Port("Value")]
    private ValueInput<float> valueInput;
    [Port("Angle")]
    private ValueInput<float> angleInput;
    [Port("Position")]
    private ValueInput<Vector2> positionInput;
    [Port("Scale", new object[] {1f, 1f, 1f})]
    private ValueInput<Vector3> scaleInput;
    [Port("Sprite")]
    private ValueInput<ProgressViewBase> spriteInput;
    private ProgressViewBase sprite;
    private float prevValue;

    public void Update()
    {
      float num = this.valueInput.value;
      if ((double) this.prevValue == (double) num)
        return;
      this.prevValue = num;
      if ((double) num != 0.0)
      {
        this.CreateSprite();
        if ((Object) this.sprite != (Object) null)
          this.sprite.Progress = num;
      }
      else
        this.DestroySprite();
    }

    public override void OnDestroy()
    {
      this.DestroySprite();
      base.OnDestroy();
    }

    private void CreateSprite()
    {
      if (!((Object) this.sprite == (Object) null))
        return;
      ProgressViewBase prefab = this.spriteInput.value;
      if ((Object) prefab != (Object) null)
      {
        this.sprite = UnityFactory.Instantiate<ProgressViewBase>(prefab, "[Effects]");
        MonoBehaviourInstance<UiEffectsController>.Instance.AddEffect(this.sprite.gameObject);
        this.SetAngle();
        this.SetOffset();
        this.SetScale();
      }
    }

    private void SetAngle()
    {
      if (!((Object) this.sprite != (Object) null))
        return;
      this.sprite.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, this.angleInput.value);
    }

    private void SetOffset()
    {
      if (!((Object) this.sprite != (Object) null))
        return;
      Rect rect = ((RectTransform) this.sprite.transform.parent).rect;
      Vector2 vector2 = this.positionInput.value;
      float z = this.sprite.transform.localPosition.z;
      this.sprite.transform.localPosition = new Vector3(vector2.x * rect.width, vector2.y * rect.height, z);
    }

    private void SetScale()
    {
      if (!((Object) this.sprite != (Object) null))
        return;
      this.sprite.transform.localScale = this.scaleInput.value;
    }

    private void DestroySprite()
    {
      if (!((Object) this.sprite != (Object) null))
        return;
      MonoBehaviourInstance<UiEffectsController>.Instance.RemoveEffect(this.sprite.gameObject);
      UnityFactory.Destroy((MonoBehaviour) this.sprite);
      this.sprite = (ProgressViewBase) null;
    }
  }
}
