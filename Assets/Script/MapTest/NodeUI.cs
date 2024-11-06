using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class NodeUI : MonoBehaviour
{
    public NodeType nodeType;
    public Button nodeButton;
    public Image nodeImage;

    public Sprite enemyNode;
    public Sprite shopNode;
    public Sprite eventNode;
    public Sprite eliteNode;
    public Sprite bonFireNode;

    private Node node;

    private void Start()
    {
        node = GetComponent<Node>();
        UpdateNodeUI();
        nodeButton.onClick.AddListener(OnNodeClick);
    }

    public void UpdateNodeUI()
    {
        if (nodeImage == null)
        {
            Debug.LogWarning("NodeUI: nodeImage or nodeText is not assigned!");
            return;
        }
        switch (nodeType)
        {
            case NodeType.Enemy:
                nodeImage.sprite = enemyNode;
                break;

            case NodeType.Event:
                nodeImage.sprite = enemyNode;
                break;

            case NodeType.Elite:
                nodeImage.sprite = eliteNode;
                break;
            case NodeType.Shop:
                nodeImage.sprite = shopNode;
                break;
            case NodeType.BonFire:
                nodeImage.sprite = bonFireNode;
                break;

        }

    }

    private void OnNodeClick()
    {

        NodeMapManager.Instance.UpdateActiveButtons(node);
        Debug.Log($"Node{nodeType} click");//버튼 눌렷을때 타입별로 나눠서 처리하기

    }
    public void SetButtonActive(bool active)
    {
        if (nodeButton != null)
        {
            nodeButton.interactable = active;
        }
    }
}
