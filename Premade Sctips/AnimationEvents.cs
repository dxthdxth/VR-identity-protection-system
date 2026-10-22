using System;
using UnityEngine;
using UnityEngine.Events;
public class AnimationEvents : MonoBehaviour
{
    public event Action OnChop, OnAnimationDone, OnInteract;
    public UnityEvent OnStep;
    public void ChopAction()
    {
        OnChop?.Invoke();
    }
    public void AnimationDone()
    {
        OnAnimationDone?.Invoke();
    }

    public void Interact()
    {
        OnInteract?.Invoke();
    }

    public void Step()
    {
        OnStep?.Invoke();
    }
}
