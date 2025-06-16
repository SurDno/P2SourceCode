using Engine.Common.MindMap;
using Engine.Impl.MindMap;
using UnityEngine;
using UnityEngine.UI;

namespace Engine.Source.UI.Menu.Protagonist.MindMap
{
  public class MMPageView : MonoBehaviour
  {
    private const float CanvasHeight = 400f;
    private MMWindow mindMap;
    private MMPage page;
    private RectTransform nodeRoot;
    private RectTransform linkRoot;
    private RectTransform[] nodeObjects;
    private RectTransform[] linkObjects;
    private MMNode startingNode = (MMNode) null;
    private Rect bounds;

    public static RectTransform AddChildRect(RectTransform parent)
    {
      RectTransform transform = (RectTransform) new GameObject(string.Empty, new System.Type[1]
      {
        typeof (RectTransform)
      }).transform;
      transform.SetParent((Transform) parent, false);
      return transform;
    }

    public static MMPageView Create(MMWindow mindMap, MMPage page, MMNode startingNode = null)
    {
      RectTransform rectTransform = MMPageView.AddChildRect(mindMap.ContentRect);
      rectTransform.name = "Page";
      MMPageView mmPageView = rectTransform.gameObject.AddComponent<MMPageView>();
      mmPageView.mindMap = mindMap;
      mmPageView.page = page;
      mmPageView.startingNode = startingNode;
      return mmPageView;
    }

    public void CallMap(MMNodeView nodeView) => this.mindMap.CallMap((IMMNode) nodeView.Node);

    public void HideNodeInfo(MMNodeView nodeView) => this.mindMap.HideNodeInfo(nodeView);

    private void Start()
    {
      Vector2 vector2_1 = new Vector2(float.MaxValue, float.MaxValue);
      Vector2 vector2_2 = new Vector2(float.MinValue, float.MinValue);
      this.linkRoot = MMPageView.AddChildRect(this.transform as RectTransform);
      this.linkRoot.name = "Links";
      this.nodeRoot = MMPageView.AddChildRect(this.transform as RectTransform);
      this.nodeRoot.name = "Nodes";
      this.nodeObjects = new RectTransform[this.page.NodesCount];
      this.linkObjects = new RectTransform[this.page.LinksCount];
      bool[] flagArray1 = new bool[this.nodeObjects.Length];
      bool[] flagArray2 = new bool[this.nodeObjects.Length];
      bool[] flagArray3 = new bool[this.nodeObjects.Length];
      RectTransform node1 = (RectTransform) null;
      for (int index1 = 0; index1 < this.linkObjects.Length; ++index1)
      {
        MMLink link = this.page.GetLink(index1);
        IMMNode origin = link.Origin;
        IMMNode target = link.Target;
        link.Kind = MMLinkKind.Direct;
        bool flag = false;
        if (link.Kind == MMLinkKind.Direct)
        {
          IMMContent content = ((MMNode) origin).Content;
          int index2 = this.page.Nodes.IndexOf<IMMNode>(origin);
          flagArray1[index2] = true;
          if (content != null && content.Kind == MMContentKind.Normal)
          {
            flag = true;
            if (target.Content == null)
            {
              flagArray2[index2] = true;
              int index3 = this.page.Nodes.IndexOf<IMMNode>(target);
              flagArray3[index3] = true;
            }
          }
        }
        else if (((MMNode) origin).Content != null && ((MMNode) target).Content != null)
        {
          if (link.Kind == MMLinkKind.DirectHidden)
          {
            if (((MMNode) origin).Content.Kind == MMContentKind.Normal)
              flag = true;
          }
          else
            flag = true;
        }
        if (flag)
        {
          RectTransform rectTransform = MMPageView.AddChildRect(this.linkRoot);
          rectTransform.name = "Link";
          this.linkObjects[index1] = rectTransform;
          Vector2 vector2_3 = origin.Position.To() * 400f;
          Vector2 vector2_4 = target.Position.To() * 400f;
          Vector2 a = Vector2.MoveTowards(vector2_3, vector2_4, this.mindMap.RadiusByKind(origin.NodeKind));
          Vector2 b = Vector2.MoveTowards(vector2_4, vector2_3, this.mindMap.RadiusByKind(target.NodeKind));
          float x = Vector2.Distance(a, b);
          rectTransform.sizeDelta = new Vector2(x, this.mindMap.LinkThickness);
          Image image = rectTransform.gameObject.AddComponent<Image>();
          image.material = this.mindMap.Material;
          image.sprite = this.mindMap.LinkImage;
          image.color = this.mindMap.ColorByKind(MMContentKind.Normal) * this.mindMap.InactiveTint;
          float z = Mathf.Atan2(b.y - a.y, b.x - a.x) * 57.29578f;
          rectTransform.anchoredPosition = new Vector2((float) (((double) a.x + (double) b.x) * 0.5), (float) (((double) a.y + (double) b.y) * 0.5));
          rectTransform.localRotation = Quaternion.Euler(0.0f, 0.0f, z);
        }
      }
      for (int index = 0; index < this.nodeObjects.Length; ++index)
      {
        MMNode node2 = this.page.GetNode(index);
        if (node2.Content != null || flagArray3[index])
        {
          RectTransform parent = MMPageView.AddChildRect(this.nodeRoot);
          parent.name = "Node";
          this.nodeObjects[index] = parent;
          float num1 = this.mindMap.RadiusByKind(node2.NodeKind);
          float num2 = num1 * this.mindMap.RadiusToSpriteSize;
          parent.sizeDelta = new Vector2(num2, num2);
          Vector2 vector2_5 = node2.Position.To() * 400f;
          parent.anchoredPosition = vector2_5;
          Vector2 vector2_6 = new Vector2(vector2_5.x - num1, vector2_5.y - num1);
          Vector2 vector2_7 = new Vector2(vector2_5.x + num1, vector2_5.y + num1);
          if ((double) vector2_1.x > (double) vector2_6.x)
            vector2_1.x = vector2_6.x;
          if ((double) vector2_1.y > (double) vector2_6.y)
            vector2_1.y = vector2_6.y;
          if ((double) vector2_2.x < (double) vector2_7.x)
            vector2_2.x = vector2_7.x;
          if ((double) vector2_2.y < (double) vector2_7.y)
            vector2_2.y = vector2_7.y;
          if (this.startingNode == node2)
            node1 = parent;
          IMMContent content = node2.Content;
          if (content != null)
          {
            bool flag1 = this.mindMap.MapContainsNode((IMMNode) node2);
            if (flag1)
            {
              parent.name = "Glow";
              Image image = parent.gameObject.AddComponent<Image>();
              image.sprite = this.mindMap.ActiveGlowImage;
              image.color = this.mindMap.ActiveGlowColor;
              parent = MMPageView.AddChildRect(parent);
              parent.name = "Node";
              parent.sizeDelta = new Vector2(num2, num2);
            }
            RawImage rawImage1 = parent.gameObject.AddComponent<RawImage>();
            rawImage1.texture = !(content.Placeholder is MMPlaceholder placeholder) ? (Texture) null : placeholder.Image.Value;
            rawImage1.material = this.mindMap.Material;
            bool flag2 = flag1 || node2.Undiscovered || content.Kind != MMContentKind.Normal && content.Kind != MMContentKind.Success || node2.NodeKind != MMNodeKind.Normal || flagArray2[index] || !flagArray1[index];
            rawImage1.color = this.mindMap.ColorByKind(content.Kind);
            if (!flag2)
            {
              RawImage rawImage2 = rawImage1;
              rawImage2.color = rawImage2.color * this.mindMap.InactiveTint;
            }
            MMNodeView mmNodeView = parent.gameObject.AddComponent<MMNodeView>();
            mmNodeView.Node = node2;
            mmNodeView.HasMapItem = flag1;
            if (node2.Undiscovered)
            {
              GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.mindMap.NewNodeIndicatorPrefab);
              gameObject.transform.SetParent((Transform) parent, false);
              mmNodeView.NewIndicator = gameObject;
            }
          }
          else
          {
            RawImage rawImage = parent.gameObject.AddComponent<RawImage>();
            rawImage.texture = this.mindMap.PredictionIcon;
            rawImage.material = this.mindMap.Material;
            rawImage.color = this.mindMap.ColorByKind(MMContentKind.Normal) * this.mindMap.InactiveTint;
          }
        }
      }
      this.bounds = Rect.MinMaxRect(vector2_1.x, vector2_1.y, vector2_2.x, vector2_2.y);
      this.mindMap.SetContentSize(this.bounds.size);
      ((RectTransform) this.transform).anchoredPosition = -this.bounds.center;
      if (!((UnityEngine.Object) node1 != (UnityEngine.Object) null))
        return;
      this.mindMap.FocusOnNode(node1);
    }

    public void ShowNodeInfo(MMNodeView nodeView) => this.mindMap.ShowNodeInfo(nodeView);
  }
}
