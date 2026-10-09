using UnityEngine;

namespace WombatLab
{
    // Small, bounded geometric effects using one shared unlit material; no external VFX package.
    public sealed class SpecialEffects : MonoBehaviour
    {
        static Material material;
        LineRenderer line;
        float lifetime, remaining, width;
        static Material Material
        {
            get
            {
                if (material == null) material = Resources.Load<Material>("CombatSignals");
                return material;
            }
        }
        static LineRenderer Line(GameObject go, Color color, float width)
        {
            var line = go.AddComponent<LineRenderer>(); line.sharedMaterial = Material;
            line.useWorldSpace = false; line.startColor = line.endColor = color; line.widthMultiplier = width;
            line.numCapVertices = 3; line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            line.receiveShadows = false;
            var properties = new MaterialPropertyBlock(); properties.SetColor("_BaseColor", color); line.SetPropertyBlock(properties);
            return line;
        }
        public static void Ring(Vector3 position, float radius)
        {
            var go = new GameObject("Smash impact radius"); go.transform.position = position + Vector3.up * .035f;
            var line = Line(go, new Color(1, .68f, .2f), .09f); line.positionCount = 49;
            for (int i = 0; i <= 48; i++) { float a = i * Mathf.PI * 2 / 48; line.SetPosition(i, new Vector3(Mathf.Cos(a), 0, Mathf.Sin(a)) * radius); }
            Fade(go, line, .3f);
        }
        public static void Impact(Vector3 position, bool player)
        {
            var go = new GameObject("Individual contact"); go.transform.position = position;
            var line = Line(go, player ? new Color(1, .86f, .48f) : new Color(1, .4f, .16f), .065f);
            line.positionCount = 9;
            for (int i = 0; i < 9; i++) { float a = i / 2 * Mathf.PI / 2; line.SetPosition(i, i % 2 == 0 ? Vector3.zero : new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0) * .3f); }
            Fade(go, line, .15f);
        }
        public static void Trail(Vector3 from, Vector3 to)
        {
            var go = new GameObject("Breakthrough trail"); go.transform.position = from + Vector3.up * .08f;
            var line = Line(go, new Color(.25f, .85f, .78f), .16f); line.positionCount = 2;
            line.SetPosition(0, Vector3.zero); line.SetPosition(1, to - from); Fade(go, line, .16f);
        }
        public static void Projectile(Transform root, bool player, float radius)
        {
            var line = Line(root.gameObject, player ? new Color(.25f, .95f, .85f) : new Color(1, .5f, .12f), .1f);
            line.positionCount = 17;
            for (int i = 0; i <= 16; i++) { float a = i * Mathf.PI / 8; line.SetPosition(i, new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0) * radius); }
        }
        static void Fade(GameObject go, LineRenderer line, float seconds)
        { var effect = go.AddComponent<SpecialEffects>(); effect.line = line; effect.lifetime = effect.remaining = seconds; effect.width = line.widthMultiplier; }
        void Update()
        {
            remaining -= Time.deltaTime; if (remaining <= 0) { Destroy(gameObject); return; }
            line.widthMultiplier = width * remaining / lifetime;
        }
    }
}
