using Engine.Impl.UI.Controls;
using FlowCanvas;
using FlowCanvas.Nodes;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

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
    [Port("Scale", 1f, 1f, 1f)]
    private ValueInput<Vector3> scaleInput;
    [Port("Sprite")]
    private ValueInput<ProgressViewBase> spriteInput;
    private ProgressViewBase sprite;
    private float prevValue;

    public void Update()
    {
      float num = valueInput.value;
      if (prevValue == (double) num)
        return;
      prevValue = num;
      if (num != 0.0)
      {
        CreateSprite();
        if ((Object) sprite != (Object) null)
          sprite.Progress = num;
      }
      else
        DestroySprite();
    }

    public override void OnDestroy()
    {
      DestroySprite();
      base.OnDestroy();
    }

    private void CreateSprite()
    {
      if (!((Object) sprite == (Object) null))
        return;
      ProgressViewBase prefab = spriteInput.value;
      if ((Object) prefab != (Object) null)
      {
        sprite = UnityFactory.Instantiate<ProgressViewBase>(prefab, "[Effects]");
        MonoBehaviourInstance<UiEffectsController>.Instance.AddEffect(sprite.gameObject);
        SetAngle();
        SetOffset();
        SetScale();
      }
    }

    private void SetAngle()
    {
      if (!((Object) sprite != (Object) null))
        return;
      sprite.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, angleInput.value);
    }

    private void SetOffset()
    {
      if (!((Object) sprite != (Object) null))
        return;
      Rect rect = ((RectTransform) sprite.transform.parent).rect;
      Vector2 vector2 = positionInput.value;
      float z = sprite.transform.localPosition.z;
      sprite.transform.localPosition = new Vector3(vector2.x * rect.width, vector2.y * rect.height, z);
    }

    private void SetScale()
    {
      if (!((Object) sprite != (Object) null))
        return;
      sprite.transform.localScale = scaleInput.value;
    }

    private void DestroySprite()
    {
      if (!((Object) sprite != (Object) null))
        return;
      MonoBehaviourInstance<UiEffectsController>.Instance.RemoveEffect(sprite.gameObject);
      UnityFactory.Destroy((MonoBehaviour) sprite);
      sprite = null;
    }
  }
}
