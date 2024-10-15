using UnityEngine;
using UnityEngine.UI;

public class RepeatingCountdownTimer : MonoBehaviour
{
    public Text timerText;
    private float countdownTime = 60f;
    private float timeBetweenUpdates = 1f; // Délai entre les mises à jour du temps
    private float timer;

    void Start()
    {
        timer = timeBetweenUpdates;
        UpdateTimerText();
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            countdownTime -= 1;
            if (countdownTime <= 0)
            {
                countdownTime = 60; // Réinitialise à 60 une fois arrivé à 0
            }
            UpdateTimerText();
            timer = timeBetweenUpdates;
        }
    }

    void UpdateTimerText()
    {
        timerText.text = countdownTime.ToString();
    }
}
