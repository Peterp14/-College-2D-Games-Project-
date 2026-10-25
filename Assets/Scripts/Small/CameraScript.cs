using UnityEngine;
using System.Collections; 

public class CameraScript : MonoBehaviour
{
    public GameObject player;
    public Vector3 offset;

    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    private Transform target;

    public float followSpeed = 5f;

    void Start()
    {
        target = player.transform;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        float x = Mathf.Clamp(
            target.position.x + offset.x,
            minX,
            maxX
        );

        float y = Mathf.Clamp(
            target.position.y + offset.y,
            minY,
            maxY
        );

        Vector3 targetPosition = new Vector3(x, y, offset.z);

        if (target == player.transform)
        {
            // Original instant following
            transform.position = targetPosition;
        }
        else
        {
            // Smooth following during cutscenes
            transform.position = Vector3.Lerp(
                transform.position,
                targetPosition,
                Mathf.Clamp01(followSpeed * Time.deltaTime)
            );
        }
    }

    public IEnumerator ReturnToPlayer()
    {
        target = null;

        Vector3 destination = new Vector3(
            Mathf.Clamp(player.transform.position.x + offset.x, minX, maxX),
            Mathf.Clamp(player.transform.position.y + offset.y, minY, maxY),
            offset.z
        );

        while (Vector3.Distance(transform.position, destination) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                destination,
                followSpeed * Time.deltaTime
            );

            yield return null;
        }

        target = player.transform;
    }
}