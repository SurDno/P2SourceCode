// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.CompareMeterLine
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class CompareMeterLine : MonoBehaviour
  {
    [SerializeField]
    private Transform Arrow = (Transform) null;
    [SerializeField]
    private RectTransform Bar = (RectTransform) null;
    [SerializeField]
    private RectTransform leftBorder = (RectTransform) null;
    [SerializeField]
    private RectTransform rightBorder = (RectTransform) null;
    [SerializeField]
    private GameObject priceItem;
    [SerializeField]
    private Image priceCoin;
    [SerializeField]
    private Sprite coinSprite;
    [SerializeField]
    private Sprite handSprite;
    [SerializeField]
    private Text priceText;
    [SerializeField]
    private Color priceColorEnabled;
    [SerializeField]
    private Color priceColorDisabled;
    [SerializeField]
    private GameObject leftArrow;
    [SerializeField]
    private GameObject rightArrow;
    private float currentValue = float.NaN;
    private float factor;
    private float oldFactor = 1f;
    private float targetValue = float.NaN;
    private bool barterMode = false;
    private int storedCoins = 0;
    private int marketCoins = 0;

    public float TargetValue
    {
      get => this.targetValue;
      set
      {
        this.targetValue = value;
        this.Calculate();
      }
    }

    public float CurrentValue
    {
      get => this.currentValue;
      set
      {
        this.currentValue = value;
        this.Calculate();
      }
    }

    public int StoredCoins
    {
      get => this.storedCoins;
      set => this.storedCoins = value;
    }

    public int MarketCoins
    {
      get => this.marketCoins;
      set => this.marketCoins = value;
    }

    public void Reset()
    {
      this.currentValue = 0.0f;
      this.targetValue = 0.0f;
      this.ShowResult(0.0f);
      this.ShowPrices();
    }

    protected void Start()
    {
      this.currentValue = 0.0f;
      this.targetValue = 0.0f;
      this.Calculate();
    }

    public void BarterMode(bool b)
    {
      this.barterMode = b;
      this.priceCoin.sprite = b ? this.handSprite : this.coinSprite;
      this.priceCoin.SetNativeSize();
    }

    protected void Calculate()
    {
      if (Mathf.Approximately(this.currentValue, this.targetValue))
        this.factor = 1f;
      else if ((double) this.currentValue < (double) this.targetValue)
      {
        this.factor = this.currentValue / this.targetValue;
      }
      else
      {
        this.factor = this.targetValue / this.currentValue;
        this.factor = (float) (1.0 - (double) this.factor + 1.0);
      }
      --this.factor;
      if ((double) this.factor != (double) this.oldFactor && !float.IsNaN(this.factor))
      {
        this.oldFactor = this.factor;
        this.ShowResult(this.factor);
      }
      this.ShowPrices();
    }

    private void ShowPrices()
    {
      this.priceItem.SetActive((double) this.currentValue != 0.0 || (double) this.targetValue != 0.0);
      this.leftArrow.SetActive((double) this.currentValue < (double) this.targetValue);
      this.rightArrow.SetActive((double) this.currentValue > (double) this.targetValue);
      if (this.barterMode)
        this.priceText.text = " " + Mathf.Abs(this.currentValue - this.targetValue).ToString() + " ";
      else if ((double) this.currentValue - (double) this.targetValue > (double) this.marketCoins)
        this.priceText.text = " " + (object) this.marketCoins + " / " + Mathf.Abs(this.currentValue - this.targetValue).ToString() + " ";
      else
        this.priceText.text = " " + Mathf.Abs(this.currentValue - this.targetValue).ToString() + " ";
      this.priceText.color = (double) this.storedCoins >= (double) this.targetValue - (double) this.currentValue ? this.priceColorEnabled : this.priceColorDisabled;
    }

    private void ShowResult(float result)
    {
      Vector2 vector2_1 = new Vector2(0.0f, this.Arrow.localPosition.y);
      Vector2 vector2_2 = new Vector2(0.0f, this.Arrow.localPosition.y);
      Vector2 a = new Vector2(0.0f, this.Arrow.localPosition.y);
      Vector2 vector2_3 = new Vector2(this.leftBorder.localPosition.x, this.Arrow.localPosition.y);
      Vector2 vector2_4 = new Vector2(this.rightBorder.localPosition.x, this.Arrow.localPosition.y);
      Vector2 current1;
      Vector2 current2;
      Vector2 vector2_5;
      if ((double) result < 0.0)
      {
        current1 = Vector2.zero;
        float f = Mathf.Abs(result) * Vector2.Distance(a, vector2_3);
        current2 = Vector2.MoveTowards(current1, vector2_3, Mathf.Abs(f));
        vector2_5 = current2;
      }
      else
      {
        current2 = Vector2.zero;
        float f = Mathf.Abs(result) * Vector2.Distance(a, vector2_4);
        current1 = Vector2.MoveTowards(current2, vector2_4, Mathf.Abs(f));
        vector2_5 = current1;
      }
      this.Arrow.localPosition = (Vector3) vector2_5;
      this.Bar.localPosition = (Vector3) new Vector2(current2.x, this.Bar.localPosition.y);
      this.Bar.sizeDelta = new Vector2((current1 - current2).x, this.Bar.rect.height);
    }
  }
}
