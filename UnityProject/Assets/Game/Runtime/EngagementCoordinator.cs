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
        void Start() { SetMode(1); }
        void Update()
        {
            rest = Mathf.Max(0, rest - Time.deltaTime);
            if (player.GetComponent<LabInput>().Read().Restart) { ResetEncounter(); return; }
            var keyboard = Keyboard.current;
            if (keyboard == null) return;
            if (keyboard.digit1Key.wasPressedThisFrame) SetMode(1);
            else if (keyboard.digit2Key.wasPressedThisFrame) SetMode(2);
        }
        public bool TryAcquire(EnemyBrain enemy)
        {
            if (Owner != null || rest > 0 || !enemy.target.Alive || !player.GetComponent<PlayerDefense>().Alive) return false;
            if (player.GetComponent<BodyRecovery>()?.Protected == true) return false;
            // If another ready opponent exists, alternate rather than rewarding update order.
            if (lastOwner == enemy)
                foreach (var other in enemies)
                    if (other != enemy && other.isActiveAndEnabled && other.ReadyForAttack) return false;
            Owner = enemy; return true;
        }
        public void Release(EnemyBrain enemy)
        { if (Owner != enemy) return; Owner = null; lastOwner = enemy; rest = .30f; }
        public void SetMode(int mode)
        {
            Mode = Mathf.Clamp(mode, 1, 2);
            for (int i = 0; i < enemies.Length; i++) enemies[i].gameObject.SetActive(i < Mode);
            ResetEncounter();
        }
        public void ResetEncounter()
        {
            Owner = lastOwner = null; rest = .6f;
            player.ResetToSpawn();
            foreach (var enemy in enemies) enemy.ResetEnemy();
        }
    }
}
