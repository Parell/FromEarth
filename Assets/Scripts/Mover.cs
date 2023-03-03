using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField] private Vector3 force;
    private Body body;

    private void Start()
    {
        body = GetComponent<Body>();
    }

    private void FixedUpdate()
    {
        body.AddForce(force, PhysicsController.DeltaTime);
    }
}
