using UnityEngine;

[ExecuteAlways]
public class Body : MonoBehaviour
{
    public BodyData bodyData;

    private void Update()
    {
        if (!Application.isPlaying)
        {
            bodyData.position = transform.position;
            return;
        }

        transform.position = bodyData.position;
    }

    public void AddForce(Vector3 force, float deltaTime)
    {
        bodyData.velocity += transform.TransformVector(force) / (bodyData.mass / deltaTime);
    }
}

[System.Serializable]
public class BodyData
{
    public int index;
    public float mass = 1;
    public Vector3 position;
    public Vector3 velocity;

    public BodyData (int index, float mass, Vector3 position, Vector3 velocity)
    {
        this.index = index;
        this.mass = mass;
        this.position = position;
        this.velocity = velocity;
    }

    public void AddForce(Vector3 force, float deltaTime)
    {
        velocity += force / (mass / deltaTime);
    }

    public void AddConstantAcceleration(Vector3 direction, float acceleration, float deltaTime)
    {
        velocity += direction * acceleration * deltaTime;
    }
}