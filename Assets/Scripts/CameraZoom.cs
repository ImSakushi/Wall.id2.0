using System.Collections;
using UnityEngine;
using Cinemachine;

public class CameraZoom : MonoBehaviour
{
    [Header("Cinemachine Virtual Camera")]
    public CinemachineVirtualCamera virtualCamera;

    [Header("Zoom Parameters")]
    [Tooltip("Orthographic size lors du zoom.")]
    public float zoomedOrthographicSize = 3f;
    [Tooltip("Durée du zoom.")]
    public float zoomDuration = 0.2f;
    [Tooltip("Durée du retour au zoom normal.")]
    public float returnDuration = 0.4f;
    [Tooltip("Courbe d'animation pour le zoom.")]
    public AnimationCurve zoomCurve;
    [Tooltip("Courbe d'animation pour le retour au zoom.")]
    public AnimationCurve returnCurve;

    [Header("Impulse Settings (Optionnel)")]
    public CinemachineImpulseSource impulseSource;

    private float originalOrthographicSize;
    private bool isZooming = false;

    void Start()
    {
        if (virtualCamera == null)
        {
            virtualCamera = GetComponent<CinemachineVirtualCamera>();
            if (virtualCamera == null)
            {
                Debug.LogError("CinemachineVirtualCamera non assignée dans CameraZoom.");
            }
        }

        originalOrthographicSize = virtualCamera.m_Lens.OrthographicSize;

        if (impulseSource == null)
        {
            impulseSource = GetComponent<CinemachineImpulseSource>();
            if (impulseSource == null)
            {
                Debug.LogWarning("CinemachineImpulseSource non assignée dans CameraZoom.");
            }
        }
    }

    /// <summary>
    /// Déclenche l'effet de zoom.
    /// </summary>
    public void TriggerZoom()
    {
        if (!isZooming)
        {
            StartCoroutine(ZoomRoutine());

            // Déclencher l'impulsion de secousse (Optionnel)
            if (impulseSource != null)
            {
                impulseSource.GenerateImpulse();
            }
        }
    }

    private IEnumerator ZoomRoutine()
    {
        isZooming = true;

        // Zoom In
        float elapsed = 0f;
        float startSize = originalOrthographicSize;
        float endSize = zoomedOrthographicSize;

        while (elapsed < zoomDuration)
        {
            float t = zoomCurve.Evaluate(elapsed / zoomDuration);
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(startSize, endSize, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        virtualCamera.m_Lens.OrthographicSize = endSize;

        // Zoom Out
        elapsed = 0f;
        startSize = endSize;
        endSize = originalOrthographicSize;

        while (elapsed < returnDuration)
        {
            float t = returnCurve.Evaluate(elapsed / returnDuration);
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(startSize, endSize, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        virtualCamera.m_Lens.OrthographicSize = endSize;

        isZooming = false;
    }
}
