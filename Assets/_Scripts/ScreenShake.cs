using Unity.Cinemachine;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    [SerializeField] private CinemachineImpulseSource impulseSource;

    private void Awake()
    {
        G.screenShake = this;
        if (impulseSource == null)
            impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void Shake(float forse = 1f)
    {
        impulseSource.GenerateImpulse(forse);
    }
}
