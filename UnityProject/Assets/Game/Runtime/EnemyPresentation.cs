using UnityEngine;

namespace WombatLab
{
    // Shared by chapter role templates; cloned references stay local to each enemy.
    public sealed class EnemyPresentation : MonoBehaviour
    {
        public EnemyBrain enemy;
        public Material barMaterial;
        public AudioClip standardWarning, agileWarning, heavyWarning, knockdown;
        AudioSource cues;
        LineRenderer healthBack, health;
        string previous;
        BodyRecovery body;
        RecoveryState priorBody;
        void Awake()
        {
            body = GetComponent<BodyRecovery>(); priorBody = body.State;
            cues = gameObject.AddComponent<AudioSource>(); cues.playOnAwake = false; cues.spatialBlend = 0; cues.volume = .20f;
            healthBack = Bar("Health background", .055f, new Color(.055f, .07f, .09f));
            health = Bar("Health", .032f, enemy.role.color);
        }
        LineRenderer Bar(string name, float width, Color color)
        {
            var go = new GameObject(name); go.transform.SetParent(enemy.roleLabel, false);
            var line = go.AddComponent<LineRenderer>(); line.useWorldSpace = false; line.positionCount = 2;
            line.sharedMaterial = barMaterial; line.startColor = line.endColor = color; line.widthMultiplier = width;
            line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; return line;
        }
        void LateUpdate()
        {
            bool alive = enemy.target.Alive;
            enemy.roleLabel.gameObject.SetActive(alive);
            if (alive)
            {
                bool warning = enemy.State == "TELEGRAPH";
                enemy.roleLabel.GetComponent<MeshRenderer>().enabled = warning || enemy.player.ShowDebug;
                health.enabled = healthBack.enabled = warning || enemy.target.Health < enemy.target.maxHealth;
                // Camera-aligned labels and bars remain compact above the body.
                if (Camera.main != null) enemy.roleLabel.rotation = Camera.main.transform.rotation;
                healthBack.SetPosition(0, new Vector3(-.38f, -.17f, 0)); healthBack.SetPosition(1, new Vector3(.38f, -.17f, 0));
                health.SetPosition(0, new Vector3(-.38f, -.17f, -.002f));
                health.SetPosition(1, new Vector3(-.38f + .76f * enemy.target.Health / enemy.target.maxHealth, -.17f, -.002f));
                if (enemy.State == "TELEGRAPH" && previous != enemy.State)
                {
                    bool heavy = enemy.role.role == EnemyRole.Heavy;
                    bool ranged = enemy.role.role == EnemyRole.Agile || enemy.role.role == EnemyRole.Thrower;
                    cues.pitch = heavy ? .85f : ranged ? 1.2f : 1;
                    cues.PlayOneShot(heavy ? heavyWarning : ranged ? agileWarning : standardWarning);
                }
            }
            if (body.State != priorBody && (body.State == RecoveryState.Falling || body.State == RecoveryState.Dead))
            { cues.pitch = enemy.role.role == EnemyRole.Heavy ? .85f : 1; cues.PlayOneShot(knockdown, .7f); }
            previous = enemy.State; priorBody = body.State;
        }
    }
}
