using UnityEngine;

namespace WombatLab
{
    public enum ChapterPhase { Arrival, Fighting, BetweenWaves, Travel, Complete }

    [DefaultExecutionOrder(-30)]
    public sealed class JunkyardChapter : MonoBehaviour
    {
        public ChapterDefinition definition;
        public EngagementCoordinator encounter;
        public ArenaCamera cameraRig;
        public ChapterGate[] entries, exits;
        public ChapterGate switchGate;
        public Transform switchPosition, checkpointMarker;
        public Renderer switchLamp;
        public ChapterSession session;
        public ChapterPhase Phase { get; private set; }
        public int AreaIndex { get; private set; }
        public int WaveIndex { get; private set; }
        public int CompletedAreas { get; private set; }
        public bool GateOpened { get; private set; }
        public int CheckpointArea { get; private set; }
        public bool CanInteract => Phase == ChapterPhase.Travel && AreaIndex == 0 && !GateOpened
            && defense.Alive && !body.Busy && !combat.Attacking && player.Grounded
            && Vector3.ProjectOnPlane(player.transform.position - switchPosition.position, Vector3.up).magnitude < 2.2f;

        PlayerMotor player;
        PlayerDefense defense;
        BodyRecovery body;
        CombatController combat;
        LabInput input;
        CharacterDefinition originalMovement, movement;
        ChapterPhase checkpointPhase;
        Vector3 checkpointSpawn;
        int checkpointHealth;
        bool checkpointGateOpened;
        float waveTimer;
        MaterialPropertyBlock lampProperties;

        void Awake()
        {
            player = encounter.player; player.chapter = this; encounter.chapter = this;
            defense = player.GetComponent<PlayerDefense>(); body = player.GetComponent<BodyRecovery>();
            combat = player.GetComponent<CombatController>(); input = player.GetComponent<LabInput>();
            originalMovement = player.definition;
            movement = Instantiate(originalMovement); player.definition = movement;
            lampProperties = new MaterialPropertyBlock();
        }
        void Start() { RestartChapter(); }
        void Update()
        {
            if (input.GameplayBlocked) return;
            var frame = input.Read();
            if (frame.ChapterRestart) { RestartChapter(); return; }
            if (frame.Restart) { RetryCheckpoint(); return; }
            if (!defense.Alive || Phase == ChapterPhase.Complete) return;
            if (frame.Interact) TryInteract();
            if (Phase == ChapterPhase.Arrival)
            {
                if (player.transform.position.x >= definition.areas[0].center - 4.8f) BeginArea(0);
            }
            else if (Phase == ChapterPhase.Fighting && encounter.LivingCount == 0)
            {
                if (WaveIndex + 1 < definition.areas[AreaIndex].waves.Length)
                { Phase = ChapterPhase.BetweenWaves; waveTimer = definition.waveBreak; }
                else ClearArea();
            }
            else if (Phase == ChapterPhase.BetweenWaves)
            {
                waveTimer -= combat.Frozen ? 0 : Time.deltaTime;
                if (waveTimer <= 0) { WaveIndex++; SpawnWave(); }
            }
            else if (Phase == ChapterPhase.Travel && (AreaIndex != 0 || GateOpened)
                && player.transform.position.x >= definition.areas[AreaIndex + 1].center - 4.8f)
                BeginArea(AreaIndex + 1);
        }
        void BeginArea(int index)
        {
            AreaIndex = index; WaveIndex = 0;
            float center = definition.areas[index].center;
            movement.arenaMin = new Vector2(center - 7, definition.minimum.y);
            movement.arenaMax = new Vector2(center + 7, definition.maximum.y);
            encounter.arenaMin = new Vector2(center - 6.5f, -2);
            encounter.arenaMax = new Vector2(center + 6.5f, 2);
            cameraRig.scrolling = false; cameraRig.center = center;
            SetCombatGates(index);
            SaveCheckpoint(ChapterPhase.Fighting, new Vector3(center - 5.6f, .05f, 0));
            SpawnWave();
        }
        void SpawnWave()
        {
            Phase = ChapterPhase.Fighting;
            var area = definition.areas[AreaIndex];
            encounter.SpawnWave(area.waves[WaveIndex].enemies, new Vector3(area.center, 0, 0));
        }
        void ClearArea()
        {
            CompletedAreas = AreaIndex + 1;
            defense.RestoreHealth(definition.areaHeal);
            SetCombatGates(-1); cameraRig.scrolling = true;
            if (CompletedAreas == definition.areas.Length)
            {
                Phase = ChapterPhase.Complete;
                movement.arenaMin = definition.minimum; movement.arenaMax = definition.maximum;
                SaveCheckpoint(ChapterPhase.Complete, player.transform.position);
            }
            else
            {
                Phase = ChapterPhase.Travel;
                SetTravelBounds();
                SaveCheckpoint(ChapterPhase.Travel, new Vector3(definition.areas[AreaIndex].center + 8.5f, .05f, 0));
            }
        }
        void SaveCheckpoint(ChapterPhase phase, Vector3 position)
        {
            CheckpointArea = AreaIndex; checkpointPhase = phase; checkpointSpawn = position;
            checkpointHealth = defense.Health; checkpointGateOpened = GateOpened;
            checkpointMarker.position = new Vector3(position.x, .012f, position.z);
        }
        void SetTravelBounds()
        {
            movement.arenaMin = definition.minimum;
            movement.arenaMax = new Vector2(definition.areas[AreaIndex + 1].center + 7, definition.maximum.y);
        }
        void SetCombatGates(int active)
        {
            for (int i = 0; i < entries.Length; i++)
            { entries[i].SetClosed(i == active); exits[i].SetClosed(i == active); }
        }
        void SetSwitch(bool opened)
        {
            GateOpened = opened; switchGate.SetClosed(!opened);
            lampProperties.SetColor("_BaseColor", opened ? new Color(.22f,.85f,.55f) : new Color(1,.7f,.12f));
            switchLamp.SetPropertyBlock(lampProperties);
        }
        public bool TryInteract()
        {
            if (!CanInteract) return false;
            SetSwitch(true); checkpointGateOpened = true; return true;
        }
        public void RetryCheckpoint()
        {
            encounter.ClearWave();
            AreaIndex = CheckpointArea; WaveIndex = 0;
            SetSwitch(checkpointGateOpened); SetCombatGates(-1);
            player.ResetAt(checkpointSpawn, Quaternion.LookRotation(Vector3.right));
            defense.ResetDefense(checkpointHealth);
            if (checkpointPhase == ChapterPhase.Fighting)
            { CompletedAreas = AreaIndex; BeginArea(AreaIndex); }
            else
            {
                Phase = checkpointPhase;
                CompletedAreas = Phase == ChapterPhase.Arrival ? 0 : AreaIndex + 1;
                cameraRig.scrolling = true;
                if (Phase == ChapterPhase.Travel) SetTravelBounds();
                else
                {
                    movement.arenaMin = definition.minimum;
                    movement.arenaMax = Phase == ChapterPhase.Arrival
                        ? new Vector2(definition.areas[0].center + 7, definition.maximum.y) : definition.maximum;
                }
            }
            cameraRig.Snap(); Physics.SyncTransforms();
        }
        public void RestartChapter()
        {
            encounter.ClearWave(); AreaIndex = WaveIndex = CompletedAreas = CheckpointArea = 0;
            SetCombatGates(-1); SetSwitch(false);
            movement.arenaMin = definition.minimum;
            movement.arenaMax = new Vector2(definition.areas[0].center + 7, definition.maximum.y);
            player.ResetAt(definition.start, Quaternion.LookRotation(Vector3.right));
            Phase = ChapterPhase.Arrival; cameraRig.scrolling = true;
            cameraRig.minimumX = definition.minimum.x + 2; cameraRig.maximumX = definition.maximum.x - 4;
            cameraRig.Snap(); SaveCheckpoint(ChapterPhase.Arrival, definition.start);
            Physics.SyncTransforms();
        }
        public string Hint(bool touch, bool gamepad = false)
        {
            string retry = touch ? "CHECKPOINT" : gamepad ? "Checkpoint im Menü" : "R: Checkpoint";
            if (!defense.Alive) return "Besiegt · " + retry + " · " + (touch ? "VON VORN" : "BACKSPACE: Von vorn");
            if (Phase == ChapterPhase.Complete) return "Schrotthof geschafft! · " + (touch ? "VON VORN: Nochmal" : "BACKSPACE: Nochmal");
            string prefix = (AreaIndex + 1) + "/3 " + definition.areas[AreaIndex].title + " · ";
            if (Phase == ChapterPhase.Arrival) return prefix + "Zum gelben Kampffeld →";
            if (Phase == ChapterPhase.Travel) return prefix + (AreaIndex == 0 && !GateOpened
                ? CanInteract ? (touch ? "TOR ÖFFNEN" : gamepad ? "LB: Schalter — Tor öffnen" : "F: Schalter — Tor öffnen") : "Zum gelben Schalter →"
                : "Checkpoint gesetzt · Weiter nach rechts →");
            if (Phase == ChapterPhase.BetweenWaves) return prefix + "Nächste Welle ...";
            string warning = encounter.Owner?.State == "TELEGRAPH" ? (encounter.Owner.role.role == EnemyRole.Agile ? "Aus der Spur!" : "Ausweichen / unterbrechen!") : "";
            return prefix + "Welle " + (WaveIndex + 1) + "/" + definition.areas[AreaIndex].waves.Length
                + " · " + encounter.LivingCount + " Gegner" + (warning.Length > 0 ? " · " + warning : "");
        }
        void OnDestroy()
        {
            if (player != null && player.definition == movement) player.definition = originalMovement;
            if (movement != null) Destroy(movement);
        }
    }
}
