using UnityEngine;

namespace WombatLab
{
    // Presentation observes the chapter/motor. It never drives encounter or damage rules.
    [DefaultExecutionOrder(40)]
    public sealed class ChapterPresentation : MonoBehaviour
    {
        public JunkyardChapter chapter;
        public AudioClip[] footsteps;
        public AudioClip switchClick, metalGate, waveCue, areaCue, completeCue, machinery;
        public Material markerMaterial;
        public Transform pressRam;
        public Renderer[] gateLamps;
        public ChapterGate[] gates;
        AudioSource steps, events, yard;
        PlayerMotor player;
        PlayerDefense defense;
        BodyRecovery body;
        CombatController combat;
        LineRenderer playerMarker;
        MaterialPropertyBlock lamps;
        Vector3 previous, ramRest;
        float walked, nextMachine;
        int stepIndex, area, wave;
        ChapterPhase phase;
        bool opened, initialized;

        void Awake()
        {
            player = chapter.encounter.player;
            defense = player.GetComponent<PlayerDefense>(); body = player.GetComponent<BodyRecovery>();
            combat = player.GetComponent<CombatController>();
            steps = Source("Footsteps", .20f); events = Source("Chapter cues", .32f); yard = Source("Distant press", .075f);
            previous = player.transform.position; lamps = new MaterialPropertyBlock();
            if (pressRam != null) ramRest = pressRam.localPosition;
            var ring = new GameObject("Player ground marker"); ring.transform.SetParent(transform, false);
            playerMarker = ring.AddComponent<LineRenderer>(); playerMarker.sharedMaterial = markerMaterial;
            playerMarker.useWorldSpace = false; playerMarker.loop = true; playerMarker.positionCount = 32;
            playerMarker.widthMultiplier = .028f; playerMarker.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            for (int i = 0; i < 32; i++)
            { float a = i * Mathf.PI * 2 / 32; playerMarker.SetPosition(i, new Vector3(Mathf.Cos(a) * .36f, 0, Mathf.Sin(a) * .36f)); }
            playerMarker.startColor = playerMarker.endColor = new Color(.45f, .95f, .9f, .65f);
        }
        AudioSource Source(string name, float volume)
        {
            var obj = new GameObject(name); obj.transform.SetParent(transform, false);
            var source = obj.AddComponent<AudioSource>(); source.playOnAwake = false;
            source.spatialBlend = 0; source.volume = volume; return source;
        }
        void LateUpdate()
        {
            var p = player.transform.position;
            float distance = Vector3.ProjectOnPlane(p - previous, Vector3.up).magnitude; previous = p;
            bool walking = defense.Alive && player.Grounded && !body.Busy && !combat.Attacking && !combat.Frozen
                && !defense.Evading && (player.State == "Walk" || player.State == "Run");
            if (walking && distance < 1) walked += distance; else walked = 0;
            if (footsteps != null && footsteps.Length > 0 && walked > (player.State == "Run" ? 1.15f : .85f))
            {
                walked = 0; steps.pitch = player.State == "Run" ? 1.08f : 1;
                steps.PlayOneShot(footsteps[stepIndex++ % footsteps.Length]);
            }
            playerMarker.transform.position = new Vector3(p.x, .026f, p.z);
            playerMarker.enabled = defense.Alive && player.Grounded && !body.Busy;
            if (initialized)
            {
                if (!opened && chapter.GateOpened) { events.PlayOneShot(switchClick); events.PlayOneShot(metalGate, .8f); }
                else if (chapter.Phase == ChapterPhase.Complete && phase != ChapterPhase.Complete) events.PlayOneShot(completeCue);
                else if (chapter.Phase == ChapterPhase.Travel && phase != ChapterPhase.Travel) events.PlayOneShot(areaCue);
                else if (chapter.Phase == ChapterPhase.Fighting && (phase != ChapterPhase.Fighting || area != chapter.AreaIndex || wave != chapter.WaveIndex)) events.PlayOneShot(waveCue, .65f);
                if (chapter.Phase == ChapterPhase.Arrival && phase != ChapterPhase.Arrival) { steps.Stop(); events.Stop(); yard.Stop(); }
            }
            initialized = true; phase = chapter.Phase; area = chapter.AreaIndex; wave = chapter.WaveIndex; opened = chapter.GateOpened;
            for (int i = 0; i < gateLamps.Length; i++)
            {
                Color color = gates[i].Closed ? new Color(1, .42f, .12f) : new Color(.24f, .9f, .55f);
                lamps.SetColor("_BaseColor", color); lamps.SetColor("_EmissionColor", color * 1.6f); gateLamps[i].SetPropertyBlock(lamps);
            }
            // Quiet mechanical rhythm stays behind the action and only near the press.
            float proximity = Mathf.Clamp01(1 - Mathf.Abs(p.x - 48) / 18);
            if (machinery != null && proximity > .05f && defense.Alive && phase != ChapterPhase.Complete && Time.time >= nextMachine)
            { yard.pitch = .7f; yard.PlayOneShot(machinery, proximity); nextMachine = Time.time + 4.5f; }
            if (pressRam != null) pressRam.localPosition = ramRest + Vector3.down * (.22f * (1 - Mathf.Cos(Time.time * 1.4f)) * .5f);
        }
    }
}
