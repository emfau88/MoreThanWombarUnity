using UnityEngine;

namespace WombatLab
{
    public sealed class TrainingDummy : MonoBehaviour
    {
        public int team = 2, maxHealth = 150;
        public Transform pad;
        public Renderer padRenderer;
        public CombatController clock;
        public int Health { get; private set; }
        public int HitCount { get; private set; }
        public float Hitstun { get; private set; }
        public bool Alive => Health > 0;
        Vector3 spawn, velocity;
        Color baseColor;
        MaterialPropertyBlock properties;
        float flash;
        Transform fallenRig;
        Animator enemyAnimator;
        Collider[] colliders;
        bool[] colliderEnabled;
        Vector3 rigRestPosition;
        Quaternion rigRestRotation, padRestRotation;
        float deathProgress;

        void Awake()
        {
            properties = new MaterialPropertyBlock();
            spawn = transform.position;
            baseColor = padRenderer.sharedMaterial.GetColor("_BaseColor");
            var brain = GetComponent<EnemyBrain>();
            fallenRig = brain != null ? brain.visual.Find("Rig") : null;
            enemyAnimator = brain != null ? brain.animator : null;
            if (fallenRig != null) { rigRestPosition = fallenRig.localPosition; rigRestRotation = fallenRig.localRotation; }
            padRestRotation = pad.localRotation;
            colliders = GetComponentsInChildren<Collider>(true);
            colliderEnabled = new bool[colliders.Length];
            for (int i = 0; i < colliders.Length; i++) colliderEnabled[i] = colliders[i].enabled;
            ResetTraining();
        }

        public bool ReceiveHit(AttackDefinition attack, Vector3 direction)
        {
            if (!Alive) return false;
            Health = Mathf.Max(0, Health - attack.damage); HitCount++;
            Hitstun = attack.hitstun; flash = .12f;
            velocity = Vector3.ProjectOnPlane(direction, Vector3.up).normalized * attack.knockback * 6;
            GetComponent<EnemyBrain>()?.Interrupt();
            if (!Alive)
            {
                if (enemyAnimator != null) { enemyAnimator.Play("Idle", 0, 0); enemyAnimator.Update(0); enemyAnimator.enabled = false; }
                foreach (var collider in colliders) collider.enabled = false;
            }
            return true;
        }

        void Update()
        {
            float dt = clock != null && clock.Frozen ? 0 : Time.deltaTime;
            Hitstun = Mathf.Max(0, Hitstun - dt); flash = Mathf.Max(0, flash - dt);
            transform.position = MotorMath.ClampGround(transform.position + velocity * dt,
                new Vector2(-6.6f, -2.1f), new Vector2(6.6f, 2.1f));
            velocity *= Mathf.Exp(-9 * dt);
            if (!Alive && fallenRig != null)
            {
                deathProgress = Mathf.Min(1, deathProgress + Time.deltaTime / .42f);
                float fall = Mathf.SmoothStep(0, 1, deathProgress);
                fallenRig.localRotation = rigRestRotation * Quaternion.Euler(-90 * fall, 0, 12 * Mathf.Sin(fall * Mathf.PI));
                fallenRig.localPosition = rigRestPosition + new Vector3(0, .52f * fall, -.12f * fall);
            }
            else if (fallenRig == null)
            {
                float tilt = Alive ? (Hitstun > 0 ? -12 : 0) : -70;
                pad.localRotation = Quaternion.Slerp(pad.localRotation, padRestRotation * Quaternion.Euler(tilt, 0, 0), 1 - Mathf.Exp(-18 * dt));
            }
            properties.SetColor("_BaseColor", flash > 0 ? new Color(1, .94f, .67f) : Alive ? baseColor : baseColor * .35f);
            padRenderer.SetPropertyBlock(properties);
        }

        public void ResetTraining()
        {
            Health = maxHealth; HitCount = 0; Hitstun = flash = 0;
            velocity = Vector3.zero; transform.position = spawn;
            deathProgress = 0;
            if (fallenRig != null) { fallenRig.localPosition = rigRestPosition; fallenRig.localRotation = rigRestRotation; }
            if (enemyAnimator != null) { enemyAnimator.enabled = true; enemyAnimator.speed = 1; enemyAnimator.Play("Idle", 0, 0); }
            for (int i = 0; i < colliders.Length; i++) colliders[i].enabled = colliderEnabled[i];
            pad.localRotation = padRestRotation;
            properties.SetColor("_BaseColor", baseColor); padRenderer.SetPropertyBlock(properties);
        }
    }
}
