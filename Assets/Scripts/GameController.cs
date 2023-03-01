using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private int targetFrameRate = 60;
    private int lastTargetFrameRate;

    private void Update()
    {
        if (lastTargetFrameRate != targetFrameRate)
        {
            Application.targetFrameRate = targetFrameRate;

            lastTargetFrameRate = targetFrameRate;
        }
    }
}
