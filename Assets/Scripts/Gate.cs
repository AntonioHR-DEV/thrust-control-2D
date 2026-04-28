using System;
using UnityEngine;

public class Gate : MonoBehaviour
{
    private const string TRIGGER_OPEN = "Open";
    private const string TRIGGER_CLOSE = "Close";

    [SerializeField] private Lock _lock;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _lock.OnUnlocked += Lock_OnUnclocked;
        _lock.OnLocked += Lock_OnLocked;
    }

    private void Lock_OnLocked(object sender, EventArgs e)
    {
        animator.SetTrigger(TRIGGER_CLOSE);
    }

    private void Lock_OnUnclocked(object sender, EventArgs e)
    {
        animator.SetTrigger(TRIGGER_OPEN);
    }
}
