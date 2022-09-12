using UnityEngine;

public class MobileUI : MonoBehaviour
{
    private void Awake()
    {
        if (SystemInfo.deviceType != DeviceType.Handheld)
        {
            gameObject.SetActive(false);
        }
    }
}
