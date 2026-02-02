using UnityEngine;

public class MouseCursorFollow : MonoBehaviour
{
    public Camera cam;
    public float zOffset = 0f;
    public bool clampDistance = false;
    public float maxDistance = 5f;
    public Transform player;

    void Start()
    {
        if (!cam) cam = Camera.main;
        Cursor.visible = false; // ẩn cursor hệ thống
    }

    void Update()
    {
        // if (!cam) return;

        // Vector3 mouseScreen = Input.mousePosition;
        // Vector3 mouseWorld = cam.ScreenToWorldPoint(mouseScreen);
        // mouseWorld.z = zOffset;

        // if (clampDistance && player)
        // {
        //     Vector2 dir = mouseWorld - player.position;
        //     if (dir.magnitude > maxDistance)
        //         mouseWorld = player.position + (Vector3)(dir.normalized * maxDistance);
        // }
        // transform.position = mouseWorld;

    }
    public void UpdateCursor(Vector3 pos)
    {
        transform.position = pos;
    }
}
