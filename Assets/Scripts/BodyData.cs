using UnityEngine;

[System.Serializable]
public class BodyData
{
    public int index;
    public double mass;
    public Vector3d position;
    public Vector3d velocity;

    public BodyData(int index, double mass, Vector3d position, Vector3d velocity)
    {
        this.index = index;
        this.mass = mass;
        this.position = position;
        this.velocity = velocity;
    }

    public void AddAcceleration(Vector3 direction, float acceleration, float deltaTime)
    {
        velocity += (Vector3d)(direction * acceleration * deltaTime);
    }
}
