using System.Collections.Generic;
using UnityEngine;

public class NodeMapManager : MonoBehaviour
{
    public static NodeMapManager Instance { get; private set; }


    public GameObject nodePrefab;
    public RectTransform nodeParent; // ui 노드 부모
    public RectTransform lineParent; //경로 노선 그림 부모 
    public int rows;
    public int cols;
    private Node[,] grid;

    private List<Node> pathNodes = new List<Node>();
    private List<Vector2Int> pathPos = new List<Vector2Int>(); //Vector2보다 Vector2Int가 사용하는 메모리가적음
    private List<Node> canUseNodes = new List<Node>();

    private HashSet<Node> startingNodes = new HashSet<Node>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        GeneratePaths();
        GenerateMap();
        AssignStartNodes();
        AssignNodes();
        SetInitActiveButtons();
    }
    private void GeneratePaths()
    {
        HashSet<int> startingRows = new HashSet<int>();
        while (startingRows.Count < 2) // 최소 2개의 시작지점 보장
        {
            int randomRow = Random.Range(0, rows);
            startingRows.Add(randomRow);
        }

        foreach (int startRow in startingRows)
        {
            CreatePathFrom(startRow);
        }

        for (int i = 0; i < 4; i++) // 추가로 경로 생성
        {
            int startRow = Random.Range(0, rows);
            CreatePathFrom(startRow);
        }

    }
    private void CreatePathFrom(int startRow)
    {
        int currentCol = 0;
        Vector2Int currentPos = new Vector2Int(currentCol, startRow);

        while (currentCol < cols - 1)
        {
            pathPos.Add(currentPos); // 경로 위치 추가

            List<int> possibleRows = new List<int> { startRow };
            if (startRow > 0)
            { 
                possibleRows.Add(startRow - 1);
            }
            if (startRow < rows - 1)
            {
                possibleRows.Add(startRow + 1); 
            }
                
            startRow = possibleRows[Random.Range(0, possibleRows.Count)];
            currentCol++;
            currentPos = new Vector2Int(currentCol, startRow);
        }

        // 마지막 노드도 경로에 추가
        pathPos.Add(currentPos);
    }
    private void GenerateMap()
    {
        //그리드 생성
        grid = new Node[rows, cols];

        float parentWidth = nodeParent.rect.width; //부모 UI 너비,높이
        float parentHeight = nodeParent.rect.height;
        float buttonWidth = 80f;  // 버튼의 너비
        float buttonHeight = 80f; // 버튼의 높이
        float verticalSpacing = 50f;    // 버튼 간격
        float horizontalSpacing = 150f;

        // 그리드 시작 위치 계산
        float startX = horizontalSpacing + (buttonWidth / 2) - (parentWidth / 2);
        float startY = (parentHeight / 2) - (buttonHeight / 2) - verticalSpacing-100;

        HashSet<Vector2> createdPositions = new HashSet<Vector2>(); // 중복 위치 확인용 HashSet
        foreach (Vector2Int pos in pathPos)
        {
            int i = pos.x;
            int j = pos.y;

            if (createdPositions.Contains(pos)) continue; // 이미 생성된 위치면 건너뜀

            GameObject nodeObj = Instantiate(nodePrefab, nodeParent);
            RectTransform rectTransform = nodeObj.GetComponent<RectTransform>();

            // 랜덤 가중치 만들기
            float randomOffsetX = Random.Range(-30f, 30f); // X 방향 
            float randomOffsetY = Random.Range(-30f, 30f); // Y 방향 

            //노드 achored포지션 설정
            rectTransform.anchoredPosition = new Vector2(
            startX + (i * (buttonWidth + horizontalSpacing)) + randomOffsetX,
            startY + (j * -(buttonHeight + verticalSpacing)) + randomOffsetY);


            Node node = nodeObj.GetComponent<Node>();
            if (node != null)
            {
                node.gridPos = new Vector2Int(i, j);
                node.nodeType = NodeType.None;
                grid[j, i] = node;
                pathNodes.Add(node); // 경로에 포함된 노드 리스트에 추가
                node.GetComponent<NodeUI>().SetButtonActive(false);
            }
            createdPositions.Add(pos); // 중복확인 해시셋에 생성된 위치로 등록
        }
        ConnectPathNodes();

    }
    private void ConnectPathNodes()
    {
        float offsetNode = 0.5f; // 노드랑 선 오프셋

        for (int i = 0; i < pathPos.Count - 1; i++)
        {
            Vector2Int currentPos = pathPos[i];
            Vector2Int nextPos = pathPos[i + 1];

            // 오른쪽으로 한 칸, 위 또는 아래로 한 칸 안에 있는 경우에만 연결
            if (nextPos.x == currentPos.x + 1 && Mathf.Abs(nextPos.y - currentPos.y) <= 1)
            {
                Node a = grid[(int)currentPos.y, (int)currentPos.x];
                Node b = grid[(int)nextPos.y, (int)nextPos.x];

                if (a != null && b != null)
                {
                    a.connectNodes.Add(b);
                    b.connectNodes.Add(a);

                    LineRenderer line = new GameObject("Line").AddComponent<LineRenderer>();
                    line.transform.SetParent(lineParent, false); // 캔버스의 자식으로 설정

                    Vector3 aWorldPos = a.GetComponent<RectTransform>().position;
                    Vector3 bWorldPos = b.GetComponent<RectTransform>().position;

                    Vector3 direction = (bWorldPos - aWorldPos).normalized; // 방향계산

                    Vector3 startOffset = aWorldPos + direction * offsetNode; //시작점이랑 끝점 오프셋
                    Vector3 endOffset = bWorldPos - direction * offsetNode;

                    line.SetPosition(0, new Vector3(startOffset.x, startOffset.y, -1f));
                    line.SetPosition(1, new Vector3(endOffset.x, endOffset.y, -1f));

                    line.positionCount = 2;
                    line.startWidth = 0.1f;
                    line.endWidth = 0.1f;

                    line.material = Resources.Load<Material>("NodeLine"); // Resources 폴더에서 머티리얼 로드
                    line.sortingLayerName = "UI";
                    line.sortingOrder = 0;


                }
            }
        }
    }
    private void AssignStartNodes()
    {
        HashSet<int> selectedCols = new HashSet<int>();
        while (selectedCols.Count < 2) 
        {
            int randomCol = Random.Range(0, cols);
            selectedCols.Add(randomCol);
        }
        foreach (int col in selectedCols) 
        {
            startingNodes.Add(grid[0,col]);
        } 
    }

    private void AssignNodes()
    {
        //시작이랑 끝 노드 초기화
        for(int y=0; y<rows; y++)
        {
            if (grid[y, 0] != null)
                grid[y, 0].nodeType = NodeType.Enemy;

            if (grid[y, cols - 1] != null)
                grid[y, cols - 1].nodeType = NodeType.BonFire;
        }
        //나머지 노드 타입 랜덤할당
        foreach (Node node in grid)
        {
            if (node == null) continue; // null인 경우 패스

            if (node.nodeType == NodeType.None)
            {
                int randomType = Random.Range(0, 100);

                if (randomType < 70)
                    node.nodeType = NodeType.Enemy;
                else if (randomType < 80)
                    node.nodeType = NodeType.Event;
                else if (randomType < 90)
                    node.nodeType = NodeType.Elite;
                else if ((randomType < 95))
                    node.nodeType = NodeType.Shop;
                else
                    node.nodeType = NodeType.BonFire;
            }
            NodeUI nodeUI = node.GetComponent<NodeUI>();
            if (nodeUI != null)
            {
                nodeUI.nodeType = node.nodeType;
                nodeUI.UpdateNodeUI(); // UI 갱신
            }

        }
        
    }

    private void SetInitActiveButtons() //처음 노드들 설정
    {
        for (int y = 0; y < rows; y++)
        {
            if (grid[y, 0] != null)
            {

                canUseNodes.Add(grid[y, 0]);
                grid[y, 0].GetComponent<NodeUI>().SetButtonActive(true); // 처음 노드 상호작용 가능 설정
            }
        }
    }
    //이미 진행한 노드 상호작용 막고 다음 열 상호작용하게 하기
    public void UpdateActiveButtons(Node selectedNode)
    {
        selectedNode.GetComponent<NodeUI>().SetButtonActive(false); // 지금 노드 비활성화하고
        canUseNodes.Remove(selectedNode);

        int currentCol = (int)selectedNode.gridPos.x;
        int nextCol = currentCol + 1;

        foreach (Node node in canUseNodes)
        {
            if (node.gridPos.x <= currentCol)
            {
                
                node.GetComponent<NodeUI>().SetButtonActive(false);
            }
        }
        canUseNodes.RemoveAll(node => (int)node.gridPos.x <= currentCol);

        if (nextCol < cols)
        {
            foreach(Node nextNode in selectedNode.connectNodes)
            {
                if (nextNode.gridPos.x > currentCol && !canUseNodes.Contains(nextNode))
                {
                    canUseNodes.Add(nextNode);
                    nextNode.GetComponent<NodeUI>().SetButtonActive(true);
                }
            }

        }
    }
}

