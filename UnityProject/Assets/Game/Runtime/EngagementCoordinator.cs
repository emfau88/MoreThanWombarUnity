using UnityEngine;
using UnityEngine.InputSystem;

namespace WombatLab
{
    public sealed class EngagementCoordinator : MonoBehaviour
    {
        public PlayerMotor player;
        public EnemyBrain[] enemies;
        public EnemyBrain Owner { get; private set; }
        public int Mode { get; private set; } = 1;
        public int LivingCount
        { get { int count = 0; foreach (var e in enemies) if (e.gameObject.activeSelf && e.target.Alive) count++; return count; } }
        EnemyBrain lastOwner;
        float rest;
        readonly RaycastHit[] movementHits = new RaycastHit[16];
        void Start() { SetMode(1); }
        void Update()
        {
            rest = Mathf.Max(0, rest - Time.deltaTime);
            if (player.GetComponent<LabInput>().Read().Restart) { ResetEncounter(); return; }
            if (Gamepad.current?.dpad.up.wasPressedThisFrame == true) SetMode(Mode % Mathf.Min(4,enemies.Length) + 1);
            var keyboard = Keyboard.current;
            if (keyboard == null) return;
            if (keyboard.digit1Key.wasPressedThisFrame) SetMode(1);
            else if (keyboard.digit2Key.wasPressedThisFrame) SetMode(2);
            else if (keyboard.digit3Key.wasPressedThisFrame) SetMode(3);
            else if (keyboard.digit4Key.wasPressedThisFrame) SetMode(4);
        }
        public bool TryAcquire(EnemyBrain enemy)
        {
            if (Owner != null || rest > 0 || !enemy.ReadyForAttack || !player.GetComponent<PlayerDefense>().Alive) return false;
            if (player.GetComponent<BodyRecovery>()?.Protected == true) return false;
            // Rotate across every ready role instead of rewarding Update order.
            int start = (System.Array.IndexOf(enemies, lastOwner) + 1) % enemies.Length;
            for (int i = 0; i < enemies.Length; i++)
            {
                var candidate = enemies[(start + i) % enemies.Length];
                if (candidate.ReadyForAttack) { if (candidate != enemy) return false; break; }
            }
            Owner = enemy; return true;
        }
        public void Release(EnemyBrain enemy)
        { if (Owner != enemy) return; Owner = null; lastOwner = enemy; rest = .30f; }
        public void SetMode(int mode)
        {
            Mode = Mathf.Clamp(mode, 1, Mathf.Min(4, enemies.Length));
            for (int i = 0; i < enemies.Length; i++) enemies[i].gameObject.SetActive(i < Mode);
            ResetEncounter();
        }
        public void ResetEncounter()
        {
            Owner = lastOwner = null; rest = .6f;
            player.ResetToSpawn();
            foreach (var enemy in enemies) enemy.ResetEnemy();
        }
        public bool IsInView(EnemyBrain enemy)
        {
            var camera = Camera.main;
            if (camera == null) return true;
            var view = camera.WorldToViewportPoint(enemy.transform.position + Vector3.up);
            return view.z > 0 && view.x > .06f && view.x < .94f && view.y > .10f && view.y < .90f;
        }
        public Vector3 WaitingPosition(EnemyBrain enemy, bool changeSide)
        {
            float angle = (45 + enemy.slot * 90 + (changeSide && enemy.role?.role == EnemyRole.Agile ? 180 : 0)) * Mathf.Deg2Rad;
            return MotorMath.ClampGround(player.transform.position + new Vector3(Mathf.Cos(angle) * 2.3f, 0, Mathf.Sin(angle) * 1.65f),
                new Vector2(-6.3f, -1.9f), new Vector2(6.3f, 1.9f));
        }
        public bool MoveEnemy(EnemyBrain enemy, Vector3 delta)
        {
            delta = Vector3.ProjectOnPlane(delta, Vector3.up);
            var wanted = MotorMath.ClampGround(enemy.transform.position + delta, new Vector2(-6.5f, -2), new Vector2(6.5f, 2));
            Vector3 limited = wanted - enemy.transform.position;
            float distance = limited.magnitude;
            bool blocked = (limited - delta).sqrMagnitude > .000001f;
            var capsule = enemy.GetComponent<CapsuleCollider>();
            if (distance > .00001f && capsule != null)
            {
                Physics.SyncTransforms();
                Vector3 center = enemy.transform.position + capsule.center;
                float radius = Mathf.Max(.08f, capsule.radius - .02f), half = capsule.height * .5f - capsule.radius;
                int count = Physics.CapsuleCastNonAlloc(center + Vector3.up * half, center - Vector3.up * half,
                    radius, limited / distance, movementHits, distance + .02f, ~(1 << 8), QueryTriggerInteraction.Ignore);
                float allowed = distance;
                for (int i = 0; i < count; i++)
                {
                    if (movementHits[i].collider.transform.IsChildOf(enemy.transform)) continue;
                    allowed = Mathf.Min(allowed, Mathf.Max(0, movementHits[i].distance - .01f));
                }
                if (allowed < distance) { limited *= allowed / distance; blocked = true; }
            }
            enemy.transform.position += limited; return blocked;
        }
    }
}
