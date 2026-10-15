using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject player;
    public Vector3 offset;

    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    private void LateUpdate()
    {
        if (player != null)
        {
            float x = Mathf.Clamp(
                player.transform.position.x + offset.x,
                minX,
                maxX
            );

            float y = Mathf.Clamp(
                player.transform.position.y + offset.y,
                minY,
                maxY
            );

            transform.position = new Vector3(x, y, offset.z);
        }
    }
}