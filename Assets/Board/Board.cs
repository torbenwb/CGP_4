using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Board : MonoBehaviour
{
    public enum LayoutType{Horizontal, Radial}

    BoxCollider2D area;
    List<BoardItem> boardItems = new List<BoardItem>();
    

    BoardItem currentTarget;
    BoardItem dragTarget;
    Vector3 dragTargetPosition;

    [Header("Layout")]
    public LayoutType layoutType = LayoutType.Radial;
    public float width;
    public float padding;
    public float radius;
    public float targetOffset = 1f;
    public Vector3 spawnPosition = Vector3.zero;

    public GameObject NewBoardItem(GameObject prefab = null){
        GameObject newGameObject;

        if (prefab) newGameObject = Instantiate(prefab, spawnPosition, Quaternion.identity);
        else newGameObject = new GameObject();

        newGameObject.transform.parent = transform;

        BoardItem boardItem = newGameObject.AddComponent<BoardItem>();
        boardItems.Add(boardItem);
        return newGameObject;
    }

    public void RemoveBoardItem(BoardItem boardItem){
        if (!boardItems.Contains(boardItem)) return;
        Destroy(boardItem.gameObject);
        boardItems.Remove(boardItem);
    }

    private void Awake()
    {
        area = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        int targetIndex = boardItems.IndexOf(currentTarget);

        switch(layoutType){
            case LayoutType.Horizontal:
                HorizontalLayout(targetIndex);
                break;
            case LayoutType.Radial:
                RadialLayout(targetIndex);
                break;
        }

        if (dragTarget) dragTarget.position = dragTargetPosition;

        dragTarget = null;
        currentTarget = null;
    }

    void HorizontalLayout(int targetIndex = -1){
        float totalWidth = (boardItems.Count * width) + (boardItems.Count - 1) * padding;
        Vector3 start = transform.position + Vector3.left * (totalWidth / 2);

        for(int i = 0; i < boardItems.Count; i++){
            Vector3 position = start + Vector3.right * (((float)i + 0.5f) * width + (i * padding));
            
            if (i == targetIndex) position += Vector3.up * targetOffset;

            boardItems[i].position = position;
        }
    }

    void RadialLayout(int targetIndex = -1){
        Vector3 origin = transform.position + Vector3.down * radius;
        for(int i = 0; i < boardItems.Count; i++){
            float angleOffset = 0f;
            float radiusOffset = 0f;
            if (targetIndex >= 0){
                if (i == targetIndex - 1) angleOffset = -1f;
                else if (i == targetIndex + 1) angleOffset = 1f;
                else if (i == targetIndex) radiusOffset = targetOffset;
            }

            boardItems[i].position = RadialLayoutPosition(origin, i, boardItems.Count, width, padding, radius, angleOffset, radiusOffset);
            boardItems[i].transform.rotation = RadialLayoutRotation(boardItems[i].transform.position, origin);
        }
    }   

    public BoardItem GetTarget(Vector3 position){
        if (!area.OverlapPoint(position)) return null;

        BoardItem target = null;
        float minDistance = 100f;
        foreach(BoardItem b in boardItems){
            float distance = (position - b.transform.position).magnitude;
            if (distance <= minDistance){
                minDistance = distance;
                target = b;
            }
        }
        return target;
    }

    public bool InArea(Vector3 position) => (area.OverlapPoint(position));

    public void SetCurrentTarget(BoardItem boardItem){
        currentTarget = boardItem;
    }

    public void DragTarget(BoardItem boardItem, Vector3 position){
        dragTarget = boardItem;
        dragTargetPosition = position;
    }

    public static Vector3 RadialLayoutPosition(Vector3 o, 
        int i, 
        int c, 
        float w, 
        float p, 
        float r, 
        float angleOffset = 0f, 
        float radiusOffset = 0f)
    {
        float circumference = Mathf.PI * 2f * r;
        float totalDistance = (c * w) + ((c - 1) * p);
        float distanceRatio = (totalDistance / circumference);
        float totalAngle = distanceRatio * 360f;
        float angle = totalAngle / c;
        float startAngle = -(totalAngle / 2f);
        Vector3 startDirection = Quaternion.AngleAxis(startAngle, Vector3.forward) * Vector2.up;
        startDirection = Quaternion.AngleAxis(((float)i + 0.5f) * angle + angleOffset, Vector3.forward) * startDirection;
        return o + startDirection * (r + radiusOffset);
    }

    Quaternion RadialLayoutRotation(Vector3 position, Vector3 origin)
    {
        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, (position - origin));
        return rotation;
    }
}
