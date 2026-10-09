using System.Collections.Generic;
using UnityEngine;

namespace WombatLab
{
    public sealed class CombatProjectile : MonoBehaviour
    {
        static readonly HashSet<CombatProjectile> active = new HashSet<CombatProjectile>();
        readonly HashSet<FighterTarget> hit = new HashSet<FighterTarget>();
        readonly RaycastHit[] contacts = new RaycastHit[64];
        AttackDefinition attack;
        CombatFeedback feedback;
        Vector3 direction;
        float remaining;
        int team, damage;
        public static int ActiveCount => active.Count;
        public static void ClearAll()
        { foreach (var p in new List<CombatProjectile>(active)) if (p != null) { p.gameObject.SetActive(false); Destroy(p.gameObject); } active.Clear(); }
        public static CombatProjectile Launch(AttackDefinition attack, int team, Vector3 origin, Vector3 direction, CombatFeedback feedback, int damage = -1)
        {
            var go = new GameObject(team == 1 ? "Druckwelle" : "Schrottwurf"); go.transform.position = origin;
            go.transform.rotation = Quaternion.LookRotation(direction);
            var projectile = go.AddComponent<CombatProjectile>(); projectile.attack = attack; projectile.team = team;
            projectile.damage = damage; projectile.direction = direction.normalized; projectile.remaining = attack.projectileRange;
            projectile.feedback = feedback;
            SpecialEffects.Projectile(go.transform, team == 1, attack.radius);
            return projectile;
        }
        void OnEnable() { active.Add(this); }
        void OnDisable() { active.Remove(this); }
        void Update()
        {
            if (Time.deltaTime <= 0) return;
            float step = Mathf.Min(remaining, attack.projectileSpeed * Time.deltaTime);
            Physics.SyncTransforms();
            int count = Physics.SphereCastNonAlloc(transform.position, attack.radius, direction, contacts, step, ~0, QueryTriggerInteraction.Collide);
            // Resolve in travel order: a wall must stop the shot before targets behind it.
            for (int i = 1; i < count; i++)
            { var value = contacts[i]; int j = i - 1; while (j >= 0 && contacts[j].distance > value.distance) { contacts[j + 1] = contacts[j]; j--; } contacts[j + 1] = value; }
            for (int i = 0; i < count; i++)
            {
                var contact = contacts[i]; var target = contact.collider.GetComponentInParent<FighterTarget>();
                if (target == null)
                {
                    if (contact.collider.isTrigger) continue;
                    SpecialEffects.Impact(contact.point, false); Finish(); return;
                }
                if (target.Team == team || hit.Contains(target) || !target.Alive) continue;
                hit.Add(target);
                if (target.Receive(attack, direction, damage))
                { SpecialEffects.Impact(target.AimPoint, team == 1); feedback?.Contact(target.AimPoint, false); }
                if (hit.Count >= attack.projectileTargets) { Finish(); return; }
            }
            transform.position += direction * step; remaining -= step;
            if (remaining <= 0) Finish();
        }
        void Finish() { gameObject.SetActive(false); Destroy(gameObject); }
    }
}
