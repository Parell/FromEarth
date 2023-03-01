using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
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

    public List<Maneuver> maneuvers;

    private void Update()
    {
        //maneuvers[0].duration = (maneuvers[0].startMass * maneuvers[0].isp * Constant.G0 / maneuvers[0].thrust) * (1 - Mathf.Exp(-maneuvers[0].deltaV.magnitude / (maneuvers[0].isp * Constant.G0)));
        //maneuvers[0].endMass = maneuvers[0].startMass / Mathf.Exp(maneuvers[0].deltaV.magnitude / (maneuvers[0].isp * Constant.G0));

        maneuvers[0].direction = maneuvers[0].deltaV.normalized;
    }
}
