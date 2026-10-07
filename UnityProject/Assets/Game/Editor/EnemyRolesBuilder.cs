using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace WombatLab.Editor
{
    public static class EnemyRolesBuilder
    {
        const string Root = HumanoidCombatBuilder.Root;
        [MenuItem("Wombat Lab/B5 Apply Enemy Roles")]
        public static string Apply()
        {
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (EditorApplication.isPlaying || EditorApplication.isCompiling || EditorUtility.scriptCompilationFailed || scene.isDirty
                || scene.path != HumanoidCombatBuilder.ScenePath) throw new InvalidOperationException("Open the saved HumanoidCombatLab in idle Edit Mode.");
            var encounter = Object.FindAnyObjectByType<EngagementCoordinator>();
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(Root + "RobotReactions.controller");
            var standardAttack = CopyAttack("Enemy_Standard", "Assets/Game/Data/Light2_Cross.asset", controller);
            standardAttack.damage = 8; standardAttack.knocksDown = false; standardAttack.radius = .38f;
            var heavyAttack = CopyAttack("Enemy_RoleHeavy", Root + "Enemy_Heavy.asset", controller);
            heavyAttack.damage = 18; heavyAttack.radius = .46f; heavyAttack.knocksDown = true;
            var rush = CopyAttack("Enemy_Rush", "Assets/Game/Data/Light2_Cross.asset", controller);
            rush.clip = RushClip(); rush.chargeDistance = 2.35f; rush.damage = 10; rush.radius = .40f;
            rush.knocksDown = rush.heavy = false; rush.startupSeconds = .10f; rush.activeSeconds = .24f; rush.recoverySeconds = .18f;
            HumanoidCombatBuilder.State(controller, rush.stateName, rush.clip).writeDefaultValues = true;
            var standard = Role(EnemyRole.Standard, "STANDARD", standardAttack, 65, 8, 2.2f, .45f, .35f, .75f, 1.30f, 1.20f, new Color(.16f,.72f,.68f));
            var agile = Role(EnemyRole.Agile, "AGILE", rush, 55, 10, 3.3f, .80f, .60f, 1.05f, 3.20f, 2.50f, new Color(.67f,.38f,.86f));
            agile.minimumAttackDistance = 1.6f;
            var heavy = Role(EnemyRole.Heavy, "HEAVY", heavyAttack, 80, 18, 1.6f, .75f, .60f, 1.05f, 1.30f, 1.20f, new Color(1,.62f,.12f));
            var enemies = encounter.enemies.ToList();
            while (enemies.Count < 4) enemies.Add(Object.Instantiate(enemies[0].gameObject, enemies[0].transform.parent).GetComponent<EnemyBrain>());
            encounter.enemies = enemies.ToArray();
            var roles = new[] { heavy, standard, agile, standard };
            var spawns = new[] { new Vector3(2.7f,0,-.35f), new Vector3(-3.4f,0,.8f), new Vector3(4.6f,0,1.15f), new Vector3(-5.2f,0,-1.1f) };
            for (int i = 0; i < 4; i++)
            {
                var enemy = enemies[i]; var role = roles[i];
                enemy.gameObject.name = "B5 " + role.displayName + " " + (i+1); enemy.slot = i; enemy.role = role;
                enemy.attack = role.attack; enemy.coordinator = encounter; enemy.player = encounter.player;
                enemy.engagementDistance = role.engagementDistance; enemy.preferredDistance = role.preferredDistance;
                enemy.target.maxHealth = role.health; enemy.target.clock = encounter.player.GetComponent<CombatController>();
                enemy.transform.position = spawns[i]; enemy.gameObject.SetActive(i == 0);
                float width = role.role == EnemyRole.Heavy ? 1 : role.role == EnemyRole.Agile ? .72f : .87f;
                float height = role.role == EnemyRole.Heavy ? 1 : role.role == EnemyRole.Agile ? .85f : .94f;
                enemy.visual.localScale = new Vector3(width, height, width);
                var body = enemy.GetComponent<CapsuleCollider>(); body.radius = .3f * width; body.height = 1.7f * height; body.center = Vector3.up * .85f * height;
                var hurt = enemy.GetComponentsInChildren<CapsuleCollider>(true).First(c => c.isTrigger);
                hurt.radius = .37f * width; hurt.height = 1.22f * height; hurt.center = Vector3.up * 1.14f * height;
                var glove = Material("B5_" + role.displayName, role.color);
                foreach (var renderer in enemy.visual.GetComponentsInChildren<Renderer>()) if (renderer.name == "Fist") renderer.sharedMaterial = glove;
                ConfigureLabel(enemy, height, role.color);
                var oldLane = enemy.transform.Find("Charge lane"); if (oldLane != null) Object.DestroyImmediate(oldLane.gameObject);
                enemy.chargeWarning = null;
                if (role.role == EnemyRole.Agile)
                {
                    var lane = GameObject.CreatePrimitive(PrimitiveType.Cube); lane.name = "Charge lane"; lane.transform.SetParent(enemy.transform, false);
                    Object.DestroyImmediate(lane.GetComponent<Collider>());
                    lane.transform.localScale = new Vector3(.55f, .018f, rush.chargeDistance);
                    lane.GetComponent<Renderer>().sharedMaterial = Material("B5_RushLane", new Color(1,.40f,.12f));
                    enemy.chargeWarning = lane.transform; lane.SetActive(false);
                }
                EditorUtility.SetDirty(enemy); EditorUtility.SetDirty(enemy.target);
            }
            var hud = Object.FindAnyObjectByType<LabHud>();
            foreach (var label in hud.GetComponentsInChildren<Text>(true))
            {
                if (label.transform.parent.name == "Header" && label != hud.stateText) label.text = "MORE THAN WOMBAT / SCHROTTHOF — GEGNERROLLEN";
                if (label.transform.parent.name == "Controls") label.text = "WASD Gehen  CTRL Rennen  SPACE Sprung  J Combo  K Heavy  L Kick  E Stoß  SHIFT Ausweichen  1–4 Gegner  R Neustart";
            }
            foreach (var asset in new UnityEngine.Object[] {standardAttack,heavyAttack,rush,standard,agile,heavy,controller,encounter}) EditorUtility.SetDirty(asset);
            EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene); AssetDatabase.SaveAssets();
            return "B5: Heavy/Standard/Agile/Standard, 1–4 mode, role data/labels, locked rush warning, shared rotating attack token.";
        }
        static AttackDefinition CopyAttack(string name, string source, AnimatorController controller)
        {
            var original = AssetDatabase.LoadAssetAtPath<AttackDefinition>(source);
            var attack = AssetDatabase.LoadAssetAtPath<AttackDefinition>(Root + name + ".asset");
            if (attack == null) { attack = Object.Instantiate(original); AssetDatabase.CreateAsset(attack, Root + name + ".asset"); }
            else EditorUtility.CopySerialized(original, attack);
            attack.name = attack.stateName = name;
            HumanoidCombatBuilder.State(controller, name, attack.clip).writeDefaultValues = true; return attack;
        }
        static EnemyRoleDefinition Role(EnemyRole kind, string name, AttackDefinition attack, int hp, int damage, float speed,
            float warning, float recovery, float cooldown, float range, float preferred, Color color)
        {
            string path = Root + "Role_" + name + ".asset";
            var role = AssetDatabase.LoadAssetAtPath<EnemyRoleDefinition>(path);
            if (role == null) { role = ScriptableObject.CreateInstance<EnemyRoleDefinition>(); AssetDatabase.CreateAsset(role, path); }
            role.role = kind; role.displayName = name; role.attack = attack; role.health = hp; role.damage = damage; role.moveSpeed = speed;
            role.telegraphSeconds = warning; role.recoverySeconds = recovery; role.cooldownSeconds = cooldown;
            role.engagementDistance = range; role.preferredDistance = preferred; role.minimumAttackDistance = 0; role.color = color; return role;
        }
        static AnimationClip RushClip()
        {
            var source = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Game/Art/Animations/Walk.anim"); var clip = Object.Instantiate(source);
            foreach (var binding in AnimationUtility.GetCurveBindings(source))
            {
                var old = AnimationUtility.GetEditorCurve(source, binding);
                AnimationUtility.SetEditorCurve(clip, binding, new AnimationCurve(old.keys.Select(k => new Keyframe(k.time / source.length * .52f, k.value)).ToArray()));
            }
            float[] t = {0,.10f,.18f,.34f,.52f};
            AnimationUtility.SetEditorCurve(clip, EditorCurveBinding.FloatCurve("Rig/Torso", typeof(Transform), "localEulerAnglesRaw.x"), new AnimationCurve(t.Select((time,i)=>new Keyframe(time,new[]{0f,20f,28f,24f,0f}[i])).ToArray()));
            foreach (string side in new[] {"Left","Right"})
            {
                float x = side == "Right" ? .65f : -.65f;
                AnimationUtility.SetEditorCurve(clip,EditorCurveBinding.FloatCurve("Rig/"+side+"Arm",typeof(Transform),"m_LocalPosition.x"),AnimationCurve.Constant(0,.52f,x));
                AnimationUtility.SetEditorCurve(clip,EditorCurveBinding.FloatCurve("Rig/"+side+"Arm",typeof(Transform),"m_LocalPosition.y"),AnimationCurve.Constant(0,.52f,1.25f));
                AnimationUtility.SetEditorCurve(clip,EditorCurveBinding.FloatCurve("Rig/"+side+"Arm",typeof(Transform),"m_LocalPosition.z"),new AnimationCurve(t.Select((time,i)=>new Keyframe(time,new[]{.06f,.4f,.70f,.65f,.06f}[i])).ToArray()));
            }
            var settings = AnimationUtility.GetAnimationClipSettings(clip); settings.loopTime=false;settings.startTime=0;settings.stopTime=.52f;AnimationUtility.SetAnimationClipSettings(clip,settings);
            return HumanoidCombatBuilder.SaveClip(clip,"Enemy_Rush");
        }
        static Material Material(string name, Color color)
        {
            var material = AssetDatabase.LoadAssetAtPath<Material>(Root + name + ".mat");
            if (material == null) { material = new Material(Shader.Find("Universal Render Pipeline/Lit")); AssetDatabase.CreateAsset(material, Root + name + ".mat"); }
            material.SetColor("_BaseColor",color); EditorUtility.SetDirty(material); return material;
        }
        static void ConfigureLabel(EnemyBrain enemy, float height, Color color)
        {
            var existing = enemy.transform.Find("Role label");
            var label = existing != null ? existing.GetComponent<TextMesh>() : new GameObject("Role label").AddComponent<TextMesh>();
            label.transform.SetParent(enemy.transform,false); label.transform.localPosition = Vector3.up * (2.55f * height);
            label.text = enemy.RoleName; label.fontSize = 48; label.characterSize = .06f;
            label.anchor = TextAnchor.MiddleCenter; label.color = color; enemy.roleLabel = label.transform;
        }
    }
}
