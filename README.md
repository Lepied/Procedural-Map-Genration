# Procedural-Map-Genration
 For Unity

# 팀 프로젝트 'Dorim-Survival' 구현 중 제작한 기능
# 절차적 맵 생성 알고리즘 구현

전체적인 디자인은 **'슬레이 더 스파이어'**(Slay the Spire)라는 게임의 맵 스타일을 참고하여 제작하였으며, 맵을 생성하는 절차는 다음과 같이 구성

## 1. 그리드 생성
맵의 전체 구조를 나타낼 **그리드(Grid)**를 특정 크기로 생성. 이 그리드는 기본적으로 경로와 노드를 배치할 영역을 지정해주는 역할

- **그리드 크기**는 가로와 세로로 일정한 칸 수를 가지며, 크기는 col, row값을 조절하는 것으로 변경 가능
- 그리드는 노드 간의 경로를 연결할 좌표 기반의 기준점

## 2. 경로 생성
그리드 내부에서 **진행 경로(Path)**를 랜덤으로 생성 이 경로는 플레이어가 시작 지점에서 목적지까지 이동하는 흐름

- 시작 지점에서 그리드의 마지막 지점까지 무작위로 경로 형성
- 경로는 시작노드에서 다음 노드, 다음 노드의 위, 다음 노드의 아래로 형성될 수 있으며 이를 반복하는 것으로 형성 

## 3. 노드 생성 및 배치
경로가 결정된 후, 각 경로 위에 **노드(Node)**를 생성합니다. 노드는 플레이어가 이동하거나 상호작용할 수 있는 포인트, 버튼 오브젝트로 형성되어 이벤트를 진행할 수 있음

- 각 노드의 **타입**은 enum을 통해 미리 제시
- 노드의 타입은 확률에 따라 결정, 경로상에 고정된 노드를 만들기 위해서 그리드의 위치를 참조해 수정하는 것으로 특정 위치에는 특정 노드가 배치되도록 설정가능

## 4. 노드 상호작용 설정
플레이어가 이동할 수 있는 경로에 따라 **상호작용이 가능한 노드**와 **불가능한 노드**를 설정

- 플레이어가 이미 지나온 경로는 비활성화된 상태로 업데이트되고, 진행방향에 있어 이동할 수 있는 노드는 플레이어의 진행에 따라 활성화
- 각 노드의 타입에 맞게 Node.cs코드를 수정하는 것으로 이벤트 트리거 생성 가능

