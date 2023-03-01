using UnityEngine;

[System.Serializable]
public class Maneuver
{
    public float startTime;
    public Vector3 deltaV;
    public float duration;
    public Vector3 direction;
    public float isp;
    public float thrust;
    public float acceleration;
    public float startMass;
    public float endMass;

    public Maneuver(Maneuver item)
    {
        startTime = item.startTime;
        deltaV = item.deltaV;
        duration = item.duration;
        direction = item.direction;
        isp = item.isp;
        thrust = item.thrust;
        acceleration = item.acceleration;
        startMass = item.startMass;
        endMass = item.endMass;
    }
}
