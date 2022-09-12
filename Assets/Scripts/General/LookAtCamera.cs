using UnityEngine;
using Mirror;

public class LookAtCamera : MonoBehaviour
{
    bool isActive;

    private void Start()
    {
        if (!NetworkServer.active) isActive = true;
    }
    private void Update()
    {
        if (!isActive) return;
        transform.LookAt(2 * transform.position - Camera.main.transform.position);
    }
}
