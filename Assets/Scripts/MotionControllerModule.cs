using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionControllerModule : MonoBehaviour
{
    public int NUM_TALKING_ANIMATIONS = 3;
    public int NUM_THINKING_ANIMATIONS = 3;
    public int NUM_LISTENING_ANIMATIONS = 1;

    public Animator animator;
    private System.Random randomGenerator = new System.Random();

    private void Awake() {
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    private void SetAnimator(string state) {
        switch (state) {
            case "ListeningTrigger":
                animator.SetInteger("ListeningIndex", randomGenerator.Next(0, NUM_LISTENING_ANIMATIONS));
                animator.SetTrigger("ListeningTrigger");
                break;
            case "ThinkingTrigger":
                animator.ResetTrigger("ListeningTrigger");
                animator.SetInteger("ThinkingIndex", randomGenerator.Next(0, NUM_THINKING_ANIMATIONS));
                animator.SetTrigger("ThinkingTrigger");
                break;
            case "TalkingTrigger":
                animator.ResetTrigger("ListeningTrigger");
                animator.ResetTrigger("ThinkingTrigger");
                animator.SetInteger("TalkingIndex", randomGenerator.Next(0, NUM_TALKING_ANIMATIONS));
                animator.SetTrigger("TalkingTrigger");
                break;
        }
    }

    public void SetAnimatorIntro() => animator.Play("Intro");

    public void SetAnimatorOutro() { }

    public void SetAnimatorListening() => SetAnimator("ListeningTrigger");
    public void SetAnimatorThinking() => SetAnimator("ThinkingTrigger");
    public void SetAnimatorSpeaking() => SetAnimator("TalkingTrigger");

}
