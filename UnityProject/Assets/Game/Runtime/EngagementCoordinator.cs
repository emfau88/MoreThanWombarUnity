using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace WombatLab
{
    public sealed class EngagementCoordinator : MonoBehaviour
    {
        public PlayerMotor player;
        public EnemyBrain[] enemies;
        public EnemyBrain[] chapterTemplates;
        public JunkyardChapter chapter;
        public Vector2 arenaMin = new Vector2(-6.5f, -2), arenaMax = new Vector2(6.5f, 2);
        public int maxMeleeAttackers = 1, maxOpponents = 4, initialOpponents = 1;
        public EnemyBrain Owner => owners.Count > 0 ? owners[0] : null;
        public int ActiveAttackers => owners.Count;
        readonly List<EnemyBrain> owners = new List<EnemyBrain>();
        public int Mode { get; private set; } = 1;
        public int LivingCount
        { get { int count = 0; foreach (var e in enemies) if (e.gameObject.activeSelf && e.target.Alive) count++; return count; } }
        EnemyBrain lastOwner;
        float rest;
        readonly RaycastHit[] movementHits = new RaycastHit[16];
        void Start() { if (chapter == null) SetMode(initialOpponents); }
        void Update()
        {
            rest = Mathf.Max(0, rest - Time.deltaTime);
            if (chapter != null) return;
            if (player.GetComponent<LabInput>().Read().Restart) { ResetEncounter(); return; }
            if (Gamepad.current?.dpad.up.wasPressedThisFrame == true) SetMode(Mode % Mathf.Min(maxOpponents,enemies.Length) + 1);
            var keyboard = Keyboard.current;
            if (keyboard == null) return;
            if (keyboard.digit1Key.wasPressedThisFrame) SetMode(1);
            else if (keyboard.digit2Key.wasPressedThisFrame) SetMode(2);
            else if (keyboard.digit3Key.wasPressedThisFrame) SetMode(3);
            else if (keyboard.digit4Key.wasPressedThisFrame) SetMode(4);
            else if (keyboard.digit8Key.wasPressedThisFrame) SetMode(8);
            else if (keyboard.digit0Key.wasPressedThisFrame) SetMode(10);
        }
        public bool TryAcquire(EnemyBrain enemy)
        {
            if (rest > 0 || owners.Contains(enemy) || !enemy.ReadyForAttack || !HasRoom(enemy)) return false;
            if (enemy.Opponent != null ? enemy.Opponent.Protected : player.GetComponent<BodyRecovery>()?.Protected == true) return false;
            // Rotate across every ready role instead of rewarding Update order.
            int start = (System.Array.IndexOf(enemies, lastOwner) + 1) % enemies.Length;
            for (int i = 0; i < enemies.Length; i++)
            {
                var candidate = enemies[(start + i) % enemies.Length];
                if (candidate != null && !owners.Contains(candidate) && candidate.ReadyForAttack && HasRoom(candidate)) { if (candidate != enemy) return false; break; }
            }
            owners.Add(enemy); lastOwner = enemy; rest = .28f; return true;
        }
        public bool HasRoom(EnemyBrain enemy)
        {
            int melee = 0, ranged = 0;
            foreach (var owner in owners)
            {
                if (owner.Opponent != enemy.Opponent) continue;
                if (owner.Ranged) ranged++; else melee++;
            }
            return enemy.Ranged ? ranged < 1 && melee < maxMeleeAttackers : melee < maxMeleeAttackers;
        }
        public void Release(EnemyBrain enemy)
        { if (!owners.Remove(enemy)) return; if (maxMeleeAttackers == 1) rest = .30f; }
        public void SetMode(int mode)
        {
            Mode = Mathf.Clamp(mode, 1, Mathf.Min(maxOpponents, enemies.Length));
            for (int i = 0; i < enemies.Length; i++) enemies[i].gameObject.SetActive(i < Mode);
            ResetEncounter();
        }
        public void ResetEncounter()
        {
            owners.Clear(); lastOwner = null; rest = .6f;
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
            float angle = (45 + enemy.slot * 360f / Mathf.Max(4, Mode) + (changeSide ? 35 : 0)) * Mathf.Deg2Rad;
            return MotorMath.ClampGround(enemy.TargetPosition + new Vector3(Mathf.Cos(angle) * (enemy.Ranged ? 4 : 2.3f), 0, Mathf.Sin(angle) * 1.65f),
                arenaMin + Vector2.one * .1f, arenaMax - Vector2.one * .1f);
        }
        public bool MoveEnemy(EnemyBrain enemy, Vector3 delta)
        {
            delta = Vector3.ProjectOnPlane(delta, Vector3.up);
            var wanted = MotorMath.ClampGround(enemy.transform.position + delta, arenaMin, arenaMax);
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
        public void ClearWave()
        {
            owners.Clear(); lastOwner = null; rest = .9f; CombatProjectile.ClearAll();
            foreach (var enemy in enemies)
            {
                if (enemy == null || System.Array.IndexOf(chapterTemplates, enemy) >= 0) continue;
                enemy.gameObject.SetActive(false); Destroy(enemy.gameObject);
            }
            enemies = new EnemyBrain[0];
        }
        public void SpawnWave(ChapterEnemySpawn[] wave, Vector3 origin)
        {
            ClearWave();
            for (int i = 0; i < wave.Length; i++)
                AppendEnemy(wave[i], origin + wave[i].position);
        }
        // Stage reinforcements append without resetting survivors, tokens or projectiles.
        public EnemyBrain AppendEnemy(ChapterEnemySpawn spawn, Vector3 position)
        {
                var template = System.Array.Find(chapterTemplates, e => e.role.role == spawn.role.role);
                var enemy = Instantiate(template.gameObject, transform).GetComponent<EnemyBrain>();
                enemy.role = spawn.role; enemy.attack = spawn.role.attack; enemy.player = player; enemy.coordinator = this;
                enemy.slot = enemies.Length % maxOpponents; enemy.transform.position = position;
                enemy.target.maxHealth = spawn.role.health;
                enemy.target.arenaMin = arenaMin; enemy.target.arenaMax = arenaMax;
                enemy.name = spawn.role.displayName + " " + (enemies.Length + 1);
                if (enemy.roleLabel != null) enemy.roleLabel.GetComponent<TextMesh>().text = spawn.role.displayName;
                System.Array.Resize(ref enemies, enemies.Length + 1); enemies[enemies.Length - 1] = enemy;
                enemy.gameObject.SetActive(true); enemy.ResetEnemy();
                Mode = Mathf.Min(maxOpponents, LivingCount);
                enemy.visual.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(player.transform.position - enemy.transform.position, Vector3.up));
                Physics.SyncTransforms(); return enemy;
        }
    }
}
