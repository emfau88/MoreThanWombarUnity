using UnityEngine;

namespace WombatLab
{
    // Visualizes existing hitstun; it does not introduce a second combat lock.
    [DefaultExecutionOrder(10)]
    public sealed class AnimationReaction : MonoBehaviour
    {
        public Animator animator;
        public CombatController clock;
        public AnimationClip hitClip, staggerClip;
        public bool Active => remaining > 0;
        public string State { get; private set; }
        float remaining, speed;
        public void Play(bool strong, float seconds)
        {
            var clip = strong ? staggerClip : hitClip;
            if (clip == null || seconds <= 0) return;
            State = strong ? "Stagger" : "Hit";
            remaining = seconds; speed = clip.length / seconds;
            animator.speed = speed; animator.Play(State, 0, 0); animator.Update(0);
        }
        void Update()
        {
            if (!Active) return;
            bool frozen = clock != null && clock.Frozen;
            animator.speed = frozen ? 0 : speed;
            remaining = Mathf.Max(0, remaining - (frozen ? 0 : Time.deltaTime));
            if (!Active) { animator.speed = 1; GetComponent<PlayerMotor>()?.ResumeLocomotion(); }
        }
        public void Clear() { remaining = 0; State = null; if (animator != null) animator.speed = 1; }
        void OnDisable() { Clear(); }
    }
}
