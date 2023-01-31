using UnityEngine;

public class BoardItem : MonoBehaviour
{
    public float moveSpeed = 15f;
    public Vector3 position;
    public Vector3 offset;

    // Update is called once per frame
    void Update()
    {
        float distance = (transform.position - (position + offset)).magnitude;
        transform.position = Vector3.MoveTowards(transform.position, position + offset, distance * moveSpeed * Time.deltaTime);   
    }
}
