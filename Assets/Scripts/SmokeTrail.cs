using UnityEngine;

public class SmokeTrail : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        // Démarrer l'animation si ce n'est pas automatique
        animator.Play("SmokeAnimation");
    }

    // Appelé à la fin de l'animation via un Event ou en détectant la fin de l'animation
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
