using UnityEngine;

public class OriginFrameController : MonoBehaviour
{
    public static OriginFrameController Instance;
    [SerializeField] private int targetThreshold = 5000;
    [SerializeField] private Transform localCamera;
    [SerializeField] private Transform scaledCamera;
    private Vector3 localPosition;
    private Vector3 scaledPosition;
    private Vector3 originPosition;
    private Vector3 scaledOriginPosition;

    private void Start()
    {
        Instance = this;
    }

    public static Vector3 LocalPosition
    {
        get { return Instance.localPosition; }
    }

    public static Vector3 ScaledPosition
    {
        get { return Instance.scaledPosition; }
    }

    public static Vector3 OriginPosition
    {
        get { return Instance.originPosition; }
    }

    public static Vector3 ScaledOriginPosition
    {
        get { return Instance.scaledOriginPosition; }
    }

    private void FixedUpdate()
    {
        // Local space objects
        localPosition = (Vector3)localCamera.position + originPosition;

        if (localCamera.position.magnitude > targetThreshold)
        {
            originPosition += (Vector3)localCamera.position;

            localCamera.position -= localCamera.position;
        }

        // Scaled space objects
        scaledPosition = (Vector3)scaledCamera.position + scaledOriginPosition;

        if (scaledCamera.position.magnitude > targetThreshold)
        {
            scaledOriginPosition += (Vector3)scaledCamera.position;
        }
    }
}
