using UnityEngine;

[System.Serializable]
public class Maneuver
{
    public float startTime;
    public float duration;
    public float acceleration;
    public Vector3 direction;

    public Maneuver(Maneuver item)
    {
        startTime = item.startTime;
        duration = item.duration;
        acceleration = item.acceleration;
        direction = item.direction;
    }
}
