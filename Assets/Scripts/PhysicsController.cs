using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteAlways]
public class PhysicsController : MonoBehaviour
{
    public static PhysicsController Instance;
    [SerializeField] private int integrator;
    [SerializeField] private double universalTime;
    [SerializeField] private int timeScale = 1;
    [SerializeField] private float deltaTime = 0.02f;
    [Header("Plots")]
    [SerializeField] private float plotTime = 100;
    [SerializeField] private float stepSize = 0.1f;
    [SerializeField] private int steps;
    [SerializeField] private float plotInterval = 1;
    private float plotUpdateTimer;
    [SerializeField] private List<Body> bodies;
    private BodyData[] bodyData;
    private BodyData[] virtualBodyData;
    List<Maneuver> maneuvers;

    private void Awake()
    {
        Instance = this;

        Time.fixedDeltaTime = deltaTime;
        Time.maximumDeltaTime = deltaTime;

        FindBodies();
        UpdateIndexes();
    }

    private void FixedUpdate() //Dosen't work make your own
    {
        for (int i = 0; i < timeScale; i++)
        {
            universalTime += Time.deltaTime;

            for (int j = 0; j < bodies.Count; j++)
            {
                bodyData[j] = bodies[j].bodyData;

                (Vector3, Vector3) data = Integrate(j, bodyData, deltaTime, (int)integrator);

                bodies[j].bodyData.position += data.Item1;
                bodies[j].bodyData.velocity += data.Item2;
            }
        }
    }

    private void Update()
    {
        steps = Mathf.RoundToInt(plotTime / stepSize);

        if (!Application.isPlaying)
        {
            FindBodies();
            UpdateIndexes();
            UpdatePlot();

            return;
        }

        plotUpdateTimer -= Time.deltaTime;
        if (plotUpdateTimer < 0)
        {
            UpdatePlot();
            plotUpdateTimer = plotInterval;
        }
    }

    private void UpdatePlot()
    {
        //Initalize
        Vector3[][] plotPoints = new Vector3[virtualBodyData.Length][];

        for (int i = 0; i < virtualBodyData.Length; i++)
        {
            virtualBodyData[i] = new BodyData(i, bodies[i].bodyData.mass, bodies[i].bodyData.position, bodies[i].bodyData.velocity);
            plotPoints[i] = new Vector3[steps];

            var mover = bodies[i].GetComponent<Mover>();
            if (mover)
            {
                maneuvers = new List<Maneuver>();

                mover.maneuvers.ForEach((item) =>
                {
                    maneuvers.Add(new Maneuver(item));
                });
            }
        }

        //Update
        for (int step = 0; step < steps; step++)
        {
            for (int i = 0; i < virtualBodyData.Length; i++)
            {
                // Only needs to happen once

                var mover = bodies[i].GetComponent<Mover>();

                if (mover)
                {
                    for (int j = 0; j < maneuvers.Count; j++)
                    {
                        if ((step * stepSize) > maneuvers[j].startTime)
                        {
                            if (maneuvers[j].duration > 0)
                            {
                                virtualBodyData[i].AddConstantAcceleration(maneuvers[j].direction, maneuvers[j].acceleration, stepSize);

                                maneuvers[j].duration -= stepSize;
                            }
                        }
                    }
                }

                (Vector3, Vector3) data = Integrate(i, virtualBodyData, stepSize, (int)integrator);

                virtualBodyData[i].position += data.Item1;
                virtualBodyData[i].velocity += data.Item2;

                Vector3 nextPosition = virtualBodyData[i].position;

                //Reference frame offset

                plotPoints[i][step] = nextPosition;
            }
        }

        //Plot data
        for (int bodyIndex = 0; bodyIndex < virtualBodyData.Length; bodyIndex++)
        {
            LineRenderer lineRenderer;

            // only add a point every fue steps and the last point

            lineRenderer = bodies[bodyIndex].GetComponent<LineRenderer>();

            if (lineRenderer)
            {
                //Reduces 1000 points to 700 points, probely better to use gpu line
                //Vector3[] newPlotPoints = Algorithms.RamerDouglasPeucker(plotPoints[bodyIndex].ToList(), 0.2f).ToArray();

                Vector3[] newPlotPoints = plotPoints[bodyIndex];

                lineRenderer.positionCount = newPlotPoints.Length;
                lineRenderer.SetPositions(newPlotPoints);
            }
        }
    }

    private (Vector3, Vector3) Integrate(int index, BodyData[] bodyData, float stepSize, int integrator)
    {
        Vector3 Gravity(Vector3 position)
        {
            Vector3 acceleration = Vector3.zero;

            for (int i = 0; i < bodyData.Length; i++)
            {
                if (i == index) { continue; }

                Vector3 r = bodyData[i].position - position;

                acceleration += r * (Constant.G * bodyData[i].mass) / Mathf.Pow(r.magnitude, 3);
            }

            return acceleration;
        }

        Vector3 Velocity(Vector3 position, float stepSize)
        {
            return bodyData[index].velocity + Gravity(position) * stepSize;
        }

        Vector3 Acceleration(Vector3 velocity, float stepSize)
        {
            return Gravity(bodyData[index].position) + Gravity(bodyData[index].position + velocity * stepSize) * stepSize;
        }

        switch (integrator)
        {
            case 0:
                {
                    Vector3 position, velocity;
                    {
                        velocity = stepSize * Acceleration(bodyData[index].velocity, 0);
                        position = stepSize * Velocity(bodyData[index].position, 0);

                        return (position, velocity);
                    }
                }
            case 1:
                {
                    Vector3 k1, k2, position, velocity;
                    {

                        k1 = Acceleration(bodyData[index].velocity, 0);
                        k2 = Acceleration(bodyData[index].velocity + stepSize * 0.5f * k1, stepSize * 0.5f);

                        velocity = stepSize * k2;
                    }
                    {
                        k1 = Velocity(bodyData[index].position, 0);
                        k2 = Velocity(bodyData[index].position + stepSize * 0.5f * k1, stepSize * 0.5f);

                        position = stepSize * k2;

                        return (position, velocity);
                    }
                }
            case 2:
                {
                    Vector3 k1, k2, k3, k4, position, velocity;
                    {
                        k1 = Acceleration(bodyData[index].velocity, 0);
                        k2 = Acceleration(bodyData[index].velocity + stepSize * 0.5f * k1, stepSize * 0.5f);
                        k3 = Acceleration(bodyData[index].velocity + stepSize * 0.5f * k2, stepSize * 0.5f);
                        k4 = Acceleration(bodyData[index].velocity + stepSize * -k3, stepSize);

                        velocity = stepSize / 6 * (k1 + 2 * k2 + 2 * k3 + k4);
                    }
                    {
                        k1 = Velocity(bodyData[index].position, 0);
                        k2 = Velocity(bodyData[index].position + stepSize * 0.5f * k1, stepSize * 0.5f);
                        k3 = Velocity(bodyData[index].position + stepSize * 0.5f * k2, stepSize * 0.5f);
                        k4 = Velocity(bodyData[index].position + stepSize * -k3, stepSize);

                        position = stepSize / 6 * (k1 + 2 * k2 + 2 * k3 + k4);

                        return (position, velocity);
                    }
                }
        }
        return default;
    }

    public void AddBody(Body body)
    {
        bodies.Add(body);

        UpdateIndexes();
    }

    public void RemoveBody(Body body, bool destroyObject)
    {
        if (destroyObject)
        {
            Destroy(body.gameObject);
        }

        bodies.Remove(body);

        UpdateIndexes();
    }

    public void RemoveBodyAt(int index, bool destroyObject)
    {
        if (destroyObject)
        {
            Destroy(bodies[index].gameObject);
        }

        bodies.RemoveAt(index);

        UpdateIndexes();
    }

    public void CreateBody(GameObject newObject, float mass, Vector3 position, Vector3 velocity)
    {
        var body = Instantiate(newObject).AddComponent<Body>();
        body.bodyData.mass = mass;
        body.bodyData.position = position;
        body.bodyData.velocity = velocity;

        AddBody(body);
    }

    public void FindBodies()
    {
        bodies = FindObjectsOfType<Body>().ToList();
    }

    public void UpdateIndexes()
    {
        bodyData = new BodyData[bodies.Count];
        virtualBodyData = new BodyData[bodies.Count];

        for (int i = 0; i < bodies.Count; i++)
        {
            bodies[i].bodyData.index = i;

            bodyData[i] = bodies[i].bodyData;
        }
    }

    public static double UniversalTime
    {
        get { return Instance.universalTime; }
    }

    public static float DeltaTime
    {
        get { return Instance.deltaTime; }
    }
}
