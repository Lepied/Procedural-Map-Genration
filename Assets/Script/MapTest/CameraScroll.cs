using UnityEditor.Rendering;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    private Vector3 dragOrigin;
    public Camera mainCamera;
    public float scrollSpeed = 20f; // 스크롤 속도

    public float minX; // 맵의 최소 X 좌표 (왼쪽)
    public float maxX; // 맵의 최대 X 좌표 (오른쪽) 
 

    void Update()
    {
        Vector3 newPosition = mainCamera.transform.position;

        // 키보드로 스크롤
        float moveX = Input.GetAxis("Horizontal") * Time.deltaTime * scrollSpeed;
        newPosition.x += moveX;

        // 드래그로 카메라 스크롤
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.GetMouseButton(0))
        {
            Vector3 difference = dragOrigin - mainCamera.ScreenToWorldPoint(Input.mousePosition);
            difference.y = 0; 
            newPosition += difference;
        }

        if (newPosition.x < minX)
        {
            newPosition.x = minX; 
        }
        else if (newPosition.x > maxX)
        {
            newPosition.x = maxX; 
        }
        newPosition.y = 0;
        mainCamera.transform.position = newPosition;

    }
}
