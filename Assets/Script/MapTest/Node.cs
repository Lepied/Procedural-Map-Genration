using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum NodeType
{
    None,
    Enemy,
    Shop,
    Event,
    Elite,
    BonFire,
    Boss
}

public class Node : MonoBehaviour
{
    public NodeType nodeType;
    public List<Node> connectNodes = new List<Node>(); // 연결한 노드 리스트
    public Button button;
    public Vector2 gridPos; // 노드 위치

    private void Awake()
    {
         button = GetComponent<Button>(); 
    }
    public void ConnectTo(Node otherNode)
    {
        if (!connectNodes.Contains(otherNode))
        {
            connectNodes.Add(otherNode);
            otherNode.ConnectTo(this); // 양방향 연결
        }
    }

}

