using System;
using UnityEngine;
using UnityEngine.UI;

public class Lock : MonoBehaviour
{
    public event EventHandler OnUnlocked;
    public event EventHandler OnLocked;

    [SerializeField] private Image indicatorImage;
    [SerializeField, Min(0.01f)] private float unlockDuration = 3f;
    [SerializeField, Min(0.01f)] private float relockDuration = 3f;


    private bool landerInside;
    private bool isUnlocked;
    private float progress; // 0 = locked, 1 = unlocked

    private void Start()
    {
        progress = 0f;
        isUnlocked = false;
        indicatorImage.fillAmount = 0f;
        indicatorImage.color = Color.red;
    }

    private void Update()
    {
        float target = landerInside ? 1f : 0f;
        float duration = landerInside ? unlockDuration : relockDuration;

        progress = Mathf.MoveTowards(progress, target, Time.deltaTime / duration);
        indicatorImage.fillAmount = progress;

        if (!isUnlocked && progress >= 1f)
        {
            isUnlocked = true;
            indicatorImage.color = Color.green;
            OnUnlocked?.Invoke(this, EventArgs.Empty);
        }
        else if (isUnlocked && progress <= 0f)
        {
            isUnlocked = false;
            indicatorImage.color = Color.red;
            OnLocked?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Lander>(out _))
        {
            landerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<Lander>(out _))
        {
            landerInside = false;
        }
    }
}