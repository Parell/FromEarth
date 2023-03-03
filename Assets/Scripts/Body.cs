using UnityEngine;

[ExecuteAlways]
public class Body : MonoBehaviour
{
    public BodyData bodyData;
    public GameObject scaledObject;

    [ContextMenu("Generate Scaled Object")]
    public void GenerateScaledObject()
    {
        if (scaledObject)
        {
            DestroyImmediate(scaledObject);
        }

        scaledObject = new GameObject(name);
        scaledObject.transform.parent = GameObject.Find("Scaled").transform;
        scaledObject.transform.localScale = Vector3.one / Constants.SCALE;
        scaledObject.transform.localPosition = (Vector3)(bodyData.position / Constants.SCALE);
        scaledObject.transform.localRotation = Quaternion.identity;
        scaledObject.AddComponent<LineRenderer>();
        scaledObject.layer = 6;
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
            bodyData.position = (Vector3d)transform.position;
            return;
        }

        transform.position = (Vector3)bodyData.position;
    }

    public void AddForce(Vector3 force, float deltaTime)
    {
        bodyData.velocity += (Vector3d)(transform.TransformVector(force) * deltaTime);
    }
}
