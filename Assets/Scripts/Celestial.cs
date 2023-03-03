using UnityEngine;

[RequireComponent(typeof(Body)), ExecuteAlways]
public class Celestial : MonoBehaviour
{
    [SerializeField] private float radius = 1f;
    [SerializeField] private float surfaceGravity = 1f;
    [SerializeField] private Mesh mesh;
    [SerializeField] private GameObject sphere;
    [SerializeField] private GameObject scaledSphere;
    [SerializeField] private Material material;
    private Body body;

    private void OnEnable()
    {
        body = GetComponent<Body>();

        CalculateMass();
        GenerateSphere();
    }

    [ContextMenu("Calculate Mass")]
    private void CalculateMass()
    {
        body.bodyData.mass = surfaceGravity * (radius * radius) / Constants.G;
    }

    [ContextMenu("Generate Sphere")]
    private void GenerateSphere()
    {
        if (!sphere)
        {
            sphere = new GameObject("Sphere");  
            sphere.AddComponent<MeshFilter>().mesh = mesh;
            sphere.AddComponent<MeshRenderer>().material = material;
        }
        else
        {
            sphere.GetComponent<MeshFilter>().mesh = mesh;
            sphere.GetComponent<MeshRenderer>().material = material;
        }

        sphere.transform.parent = gameObject.transform;
        sphere.transform.localPosition = Vector3.zero;
        sphere.transform.localScale = Vector3.one * radius;
        sphere.layer = 0;

        body.GenerateScaledObject();

        if (body.scaledObject)
        {
            if (!scaledSphere)
            {
                scaledSphere = new GameObject("Sphere");
                scaledSphere.AddComponent<MeshFilter>().mesh = mesh;
                scaledSphere.AddComponent<MeshRenderer>().material = material;
            }
            else
            {
                scaledSphere.GetComponent<MeshFilter>().mesh = mesh;
                scaledSphere.GetComponent<MeshRenderer>().material = material;
            }

            scaledSphere.transform.parent = body.scaledObject.transform;
            scaledSphere.transform.localPosition = Vector3.zero;
            scaledSphere.transform.localScale = Vector3.one * radius;
            scaledSphere.layer = 6;
        }
    }
}
