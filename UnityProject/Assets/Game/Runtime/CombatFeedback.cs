using UnityEngine;

namespace WombatLab
{
    public sealed class CombatFeedback : MonoBehaviour
    {
        public Renderer flash;
        public LineRenderer spark, debugSphere;
        public CombatController combat;
        AudioSource audioSource;
        AudioClip lightSound, heavySound;
        float remaining, duration, strength;
        MaterialPropertyBlock properties;

        void Awake()
        {
            properties = new MaterialPropertyBlock();
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false; audioSource.spatialBlend = 0; audioSource.volume = .22f;
            lightSound = Tone("Original synthesized light contact", .08f, 175);
            heavySound = Tone("Original synthesized heavy contact", .14f, 90);
            Clear();
        }
        static AudioClip Tone(string name, float seconds, float frequency)
        {
            const int rate = 22050; var samples = new float[Mathf.CeilToInt(seconds * rate)];
            for (int i = 0; i < samples.Length; i++)
            {
                float t = i / (float)rate, envelope = Mathf.Exp(-t * 35) * Mathf.Min(1, t * 1000);
                samples[i] = envelope * (Mathf.Sin(2 * Mathf.PI * frequency * t) * .75f
                    + Mathf.Sin(2 * Mathf.PI * frequency * 2.73f * t) * .25f);
            }
            var clip = AudioClip.Create(name, samples.Length, 1, rate, false); clip.SetData(samples, 0); return clip;
        }
        public void Contact(Vector3 position, bool heavy)
        {
            duration = remaining = heavy ? .17f : .11f; strength = heavy ? .42f : .27f;
            flash.transform.position = position; spark.transform.position = position;
            flash.enabled = spark.enabled = true;
            spark.positionCount = 17;
            for (int i = 0; i < 17; i++)
            {
                float angle = (i / 2) * Mathf.PI / 4;
                spark.SetPosition(i, i % 2 == 0 ? Vector3.zero : new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * strength * 1.8f);
            }
            audioSource.PlayOneShot(heavy ? heavySound : lightSound);
        }
        void LateUpdate()
        {
            remaining = Mathf.Max(0, remaining - Time.unscaledDeltaTime);
            float fraction = duration > 0 ? remaining / duration : 0;
            flash.enabled = spark.enabled = fraction > 0;
            flash.transform.localScale = Vector3.one * strength * fraction;
            properties.SetColor("_BaseColor", Color.Lerp(new Color(1, .37f, .07f), new Color(1, .96f, .65f), fraction));
            flash.SetPropertyBlock(properties); spark.widthMultiplier = .045f * fraction;
            debugSphere.enabled = combat != null && combat.Attacking && combat.GetComponent<PlayerMotor>().ShowDebug;
            if (debugSphere.enabled)
            {
                var fist = combat.ContactPoint;
                debugSphere.transform.position = fist.position;
                debugSphere.startColor = debugSphere.endColor = combat.HitboxOpen ? Color.red : Color.yellow;
                debugSphere.positionCount = 33;
                for (int i = 0; i <= 32; i++)
                { float a = i * Mathf.PI / 16; debugSphere.SetPosition(i, new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0) * combat.Attack.radius); }
            }
        }
        public void Clear()
        { remaining = 0; if (flash != null) flash.enabled = false; if (spark != null) spark.enabled = false; if (debugSphere != null) debugSphere.enabled = false; audioSource?.Stop(); }
        void OnDestroy() { if (lightSound != null) Destroy(lightSound); if (heavySound != null) Destroy(heavySound); }
    }
}
