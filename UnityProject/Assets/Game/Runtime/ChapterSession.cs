using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace WombatLab
{
    public enum SessionPage { Home, Playing, Pause, Controls, Options, Intro, Result }

    // UI/session adapter around the existing chapter, not another progression system.
    [DefaultExecutionOrder(-100)]
    public sealed class ChapterSession : MonoBehaviour
    {
        public JunkyardChapter chapter;
        public LabHud hud;
        public SessionPage Page { get; private set; } = SessionPage.Home;
        public bool Blocked => Page != SessionPage.Playing;
        public bool HasRun { get; private set; }
        public bool UsingGamepad => gamepad && input.TouchControls?.Visible != true;
        public float RunSeconds { get; private set; }
        public const string PrefPrefix = "Wombat.Chapter.";
        LabInput input;
        PlayerMotor player;
        PlayerDefense defense;
        CombatController combat;
        GameObject canvasObject, overlay, gameplay;
        RectTransform safe, card;
        Text title, subtitle, copy, eyebrow, footer;
        Button primary, secondary, controls, options, home;
        GameObject settings;
        Slider volume, cameraSize;
        Button touchOption, helpOption, cameraOption;
        Text volumeText, cameraText;
        SessionPage returnPage;
        float originalScale, originalVolume;
        bool originalAudioPause, ready, help, calm, gamepad;
        int introStep;
        float refresh;
        Rect lastSafe;
        Vector2 lastSize;
        readonly Color ink = new Color(.05f, .10f, .12f, .98f);
        readonly Color mint = new Color(.35f, .85f, .72f);

        void Awake()
        {
            chapter.session = this; player = chapter.encounter.player;
            input = player.GetComponent<LabInput>(); input.Session = this;
            defense = player.GetComponent<PlayerDefense>(); combat = player.GetComponent<CombatController>();
            originalScale = Time.timeScale; originalVolume = AudioListener.volume; originalAudioPause = AudioListener.pause;
        }
        void Start()
        {
            CreateUI(); LoadOptions(); ready = true; Show(SessionPage.Home);
        }
        void Update()
        {
            if (!ready) return;
            UpdateSafeArea();
            var keyboard = Keyboard.current;
            foreach (var pad in Gamepad.all)
                if (pad != input.TouchControls?.Stick?.control?.device && (pad.buttonSouth.wasPressedThisFrame || pad.startButton.wasPressedThisFrame || pad.leftStick.ReadValue().sqrMagnitude > .1f)) gamepad = true;
            if (keyboard?.anyKey.wasPressedThisFrame == true) gamepad = false;
            bool pause = keyboard?.escapeKey.wasPressedThisFrame == true || keyboard?.pKey.wasPressedThisFrame == true;
            foreach (var pad in Gamepad.all) if (pad != input.TouchControls?.Stick?.control?.device && pad.startButton.wasPressedThisFrame) pause = true;
            if (pause)
            {
                if (Page == SessionPage.Playing) Pause();
                else if (Page == SessionPage.Pause) Resume();
                else if (Page == SessionPage.Options || Page == SessionPage.Controls) Show(returnPage);
            }
            if (Page == SessionPage.Playing)
            {
                bool restart = keyboard?.backspaceKey.wasPressedThisFrame == true;
                foreach (var pad in Gamepad.all) if (pad != input.TouchControls?.Stick?.control?.device && pad.selectButton.wasPressedThisFrame) restart = true;
                if (restart) { StartRun(false); return; }
                if (keyboard?.rKey.wasPressedThisFrame == true) { Retry(); return; }
                RunSeconds += Time.deltaTime;
                if (!defense.Alive || chapter.Phase == ChapterPhase.Complete) Show(SessionPage.Result);
                ApplyHelp();
            }
            if (Time.unscaledTime >= refresh)
            { refresh = Time.unscaledTime + .2f; RefreshText(); }
        }
        public void StartRun(bool introduction = true)
        {
            chapter.RestartChapter(); HasRun = true; RunSeconds = 0; introStep = 0;
            Show(introduction && PlayerPrefs.GetInt(PrefPrefix + "IntroSeen", 0) == 0 ? SessionPage.Intro : SessionPage.Playing);
        }
        public void Pause() { if (Page == SessionPage.Playing) Show(SessionPage.Pause); }
        public void Resume() { if (HasRun && defense.Alive && chapter.Phase != ChapterPhase.Complete) Show(SessionPage.Playing); }
        public void Retry()
        { chapter.RetryCheckpoint(); HasRun = true; Show(SessionPage.Playing); }
        public void GoHome() { Show(SessionPage.Home); }
        public void OpenOptions() { returnPage = Page; Show(SessionPage.Options); }
        public void OpenControls() { returnPage = Page; Show(SessionPage.Controls); }
        public void FinishIntro()
        { PlayerPrefs.SetInt(PrefPrefix + "IntroSeen", 1); PlayerPrefs.Save(); Show(SessionPage.Playing); }
        void NextIntro() { if (++introStep >= 3) FinishIntro(); else RefreshText(); }
        void Show(SessionPage page)
        {
            Page = page;
            input.ClearForMenu(); combat.ClearBufferedInput(); player.ClearJumpBuffer();
            Time.timeScale = Blocked ? 0 : originalScale;
            AudioListener.pause = Blocked || originalAudioPause;
            input.TouchControls?.SetGameplayEnabled(!Blocked);
            if (hud != null) hud.gameObject.SetActive(!Blocked);
            if (!ready) return;
            overlay.SetActive(Blocked); gameplay.SetActive(!Blocked);
            settings.SetActive(page == SessionPage.Options);
            copy.gameObject.SetActive(page != SessionPage.Options);
            ConfigureButtons(); RefreshText();
            if (EventSystem.current != null)
            { EventSystem.current.SetSelectedGameObject(null); EventSystem.current.SetSelectedGameObject(primary.gameObject); }
        }
        void ConfigureButtons()
        {
            primary.gameObject.SetActive(true); secondary.gameObject.SetActive(false);
            controls.gameObject.SetActive(Page == SessionPage.Home || Page == SessionPage.Pause);
            options.gameObject.SetActive(Page == SessionPage.Home || Page == SessionPage.Pause);
            home.gameObject.SetActive(Page == SessionPage.Pause || Page == SessionPage.Result);
            primary.onClick.RemoveAllListeners(); secondary.onClick.RemoveAllListeners();
            if (Page == SessionPage.Home)
            {
                Action(primary, HasRun && defense.Alive && chapter.Phase != ChapterPhase.Complete ? "FORTSETZEN" : "SPIELEN",
                    () => { if (HasRun && defense.Alive && chapter.Phase != ChapterPhase.Complete) Resume(); else StartRun(); });
                if (HasRun) { secondary.gameObject.SetActive(true); Action(secondary, "VON VORN", () => StartRun()); }
            }
            else if (Page == SessionPage.Pause) Action(primary, "WEITERKÄMPFEN", Resume);
            else if (Page == SessionPage.Result)
            {
                bool won = chapter.Phase == ChapterPhase.Complete;
                Action(primary, won ? "NOCHMAL SPIELEN" : "AB CHECKPOINT", () => { if (won) StartRun(false); else Retry(); });
                if (!won) { secondary.gameObject.SetActive(true); Action(secondary, "VON VORN", () => StartRun(false)); }
            }
            else if (Page == SessionPage.Intro)
            {
                Action(primary, introStep == 2 ? "LOS GEHT’S" : "WEITER", NextIntro);
                secondary.gameObject.SetActive(true); Action(secondary, "ÜBERSPRINGEN", FinishIntro);
            }
            else Action(primary, "ZURÜCK", () => Show(returnPage));
        }
        static void Action(Button button, string label, UnityEngine.Events.UnityAction action)
        { button.GetComponentInChildren<Text>().text = label; button.onClick.AddListener(action); }
        string Scheme => input.TouchControls?.Visible == true ? "TOUCH" : gamepad ? "GAMEPAD" : "TASTATUR";
        string Movement => Scheme == "TOUCH" ? "Stick: bewegen · außen: rennen · SPRUNG: springen"
            : Scheme == "GAMEPAD" ? "Linker Stick: bewegen · LT: rennen · A: springen" : "WASD / Pfeile: bewegen · CTRL: rennen · SPACE: springen";
        string Attacks => Scheme == "TOUCH" ? "COMBO / KICK / HEAVY · Luft + HEAVY: Smash (22 MP)\nSTOSS: Durchbruch (18 MP) · WELLE: Fernangriff (26 MP)"
            : Scheme == "GAMEPAD" ? "X: Combo · Y: Heavy / Luft-Smash · RB: Kick\nRT: Durchbruch · Steuerkreuz rechts: Druckwelle"
            : "J: Combo · K: Heavy / Luft-Smash · L: Kick\nE: Durchbruch · Q: Druckwelle";
        string Evade => Scheme == "TOUCH" ? "AUSWEICHEN: aus der Warnung heraus · TOR ÖFFNEN: nah am Schalter"
            : Scheme == "GAMEPAD" ? "B: ausweichen · LB: Schalter bedienen · Start: Pause" : "SHIFT: ausweichen · F: Schalter bedienen · ESC / P: Pause";
        void RefreshText()
        {
            if (!ready) return;
            eyebrow.text = "MORE THAN WOMBAT   /   SCHROTTHOF";
            footer.text = chapter.definition.areas.Length + " BEREICHE   ·   " + chapter.PlannedEnemies + " GEGNER   ·   " + Scheme;
            copy.fontSize = Page == SessionPage.Controls || Page == SessionPage.Intro ? 19 : 21;
            if (Page == SessionPage.Home)
            { title.text = "ÄRGER IM SCHROTTHOF"; subtitle.text = "Ein kurzer Brawler. Ein Hof voller Bären.";
                copy.text = "Kämpfe dich durch Anlieferung und Sortierhof bis zum Presswerk.\n\nBündle Gegner für den Boden-Smash, brich mit dem Stoß durch oder nutze die Druckwelle. Orange Warnungen geben dir Zeit zum Ausweichen.\n\nNach jedem Bereich wartet ein Checkpoint."; }
            else if (Page == SessionPage.Pause)
            { title.text = "KURZE VERSCHNAUFPAUSE"; subtitle.text = "Der Kampf wartet auf dich.";
                copy.text = chapter.definition.areas[chapter.AreaIndex].title + "   ·   " + chapter.DefeatedEnemies + " / " + chapter.PlannedEnemies + " BESIEGT\n\n" + defense.Health + " HP   ·   " + TimeLabel() + " Spielzeit\n\nSteuerung und Optionen kannst du hier jederzeit nachsehen."; }
            else if (Page == SessionPage.Controls)
            { title.text = "SO KÄMPFST DU"; subtitle.text = Scheme + "   ·   Gemeinsam bewegen und angreifen";
                copy.text = Movement + "\n\n" + Attacks + "\n\n" + Evade; }
            else if (Page == SessionPage.Options)
            { title.text = "DEIN SPIELGEFÜHL"; subtitle.text = "Änderungen gelten sofort und werden gespeichert.";
                volumeText.text = "LAUTSTÄRKE   " + Mathf.RoundToInt(volume.value * 100) + "%";
                cameraText.text = "KAMERABLICK   " + Mathf.RoundToInt(cameraSize.value) + "°";
                touchOption.GetComponentInChildren<Text>().text = "TOUCH: " + (input.TouchControls?.Visible == true ? "AN" : "AUS");
                helpOption.GetComponentInChildren<Text>().text = "HINWEISE: " + (help ? "AN" : "AUS");
                cameraOption.GetComponentInChildren<Text>().text = "KAMERA: " + (calm ? "RUHIG" : "DIREKT"); }
            else if (Page == SessionPage.Intro)
            {
                title.text = new[] { "REIN IN DEN HOF", "FINDE DEINEN RHYTHMUS", "BLEIB IN BEWEGUNG" }[introStep];
                subtitle.text = "KURZE EINFÜHRUNG   " + (introStep + 1) + " / 3   ·   " + Scheme;
                copy.text = introStep == 0 ? Movement + "\n\nLaufe nach rechts zur gelben Kampffläche. Der Kampf beginnt erst, wenn du sie betrittst."
                    : introStep == 1 ? Attacks + "\n\nSmash trifft im Kreis, Durchbruch eine Spur, Welle bis zu 3 Ziele. Spezialaktionen brauchen MP; Grundtreffer laden auf."
                    : Evade + "\n\nWeiche orange angekündigten Angriffen aus. Nach Bereich 1 öffnest du das Tor am gelben Schalter.\n\nBei Niederlage: AB CHECKPOINT für den aktuellen Abschnitt.";
                primary.GetComponentInChildren<Text>().text = introStep == 2 ? "LOS GEHT’S" : "WEITER";
            }
            else if (Page == SessionPage.Result)
            {
                bool won = chapter.Phase == ChapterPhase.Complete;
                title.text = won ? "DER HOF GEHÖRT DIR!" : "DIE BÄREN HATTEN RECHT …";
                subtitle.text = won ? chapter.PlannedEnemies + " Gegner besiegt. Schrotthof geschafft." : "Ein neuer Versuch wartet am Checkpoint.";
                copy.text = chapter.CompletedAreas + " / 3 BEREICHE GESCHAFFT   ·   " + TimeLabel() + "\n\n"
                    + (won ? "Verbleibende HP: " + defense.Health + "\n\nLust auf eine zweite Runde?" : "Ab Checkpoint beginnt der aktuelle Abschnitt neu.\nGeschaffte Bereiche und das geöffnete Tor bleiben erhalten.");
            }
        }
        string TimeLabel() { int seconds = Mathf.FloorToInt(RunSeconds); return seconds / 60 + ":" + (seconds % 60).ToString("00"); }
        void ApplyHelp()
        {
            var panel = hud != null ? hud.transform.Find("Controls") : null;
            if (panel == null) return;
            panel.gameObject.SetActive(help && input.TouchControls?.Visible != true);
            var label = panel.GetComponentInChildren<Text>(true);
            if (label != null) label.text = Scheme == "GAMEPAD" ? "X Combo  Y Heavy/Luft-Smash  RT Stoß  D-Pad → Welle  B Ausweichen  LB Tor  START Pause"
                : "J Combo  K Heavy/Luft-Smash  L Kick  E Stoß  Q Welle  SHIFT Ausweichen  F Tor  ESC Pause";
        }
        public void SetVolume(float value)
        { AudioListener.volume = Mathf.Clamp01(value); if (volume != null) volume.SetValueWithoutNotify(AudioListener.volume); PlayerPrefs.SetFloat(PrefPrefix + "Volume", AudioListener.volume); PlayerPrefs.Save(); }
        public void SetCameraSize(float value)
        { float fov = Mathf.Clamp(value, 38, 48); chapter.cameraRig.GetComponent<Camera>().fieldOfView = fov; if (cameraSize != null) cameraSize.SetValueWithoutNotify(fov); PlayerPrefs.SetFloat(PrefPrefix + "Fov", fov); PlayerPrefs.Save(); }
        void LoadOptions()
        {
            volume.SetValueWithoutNotify(Mathf.Clamp01(PlayerPrefs.GetFloat(PrefPrefix + "Volume", 1)));
            cameraSize.SetValueWithoutNotify(Mathf.Clamp(PlayerPrefs.GetFloat(PrefPrefix + "Fov", 42), 38, 48));
            AudioListener.volume = volume.value; chapter.cameraRig.GetComponent<Camera>().fieldOfView = cameraSize.value;
            help = PlayerPrefs.GetInt(PrefPrefix + "Help", 1) == 1; calm = PlayerPrefs.GetInt(PrefPrefix + "Calm", 0) == 1;
            chapter.cameraRig.follow = calm ? 2.5f : 5;
            if (PlayerPrefs.HasKey(PrefPrefix + "Touch")) input.TouchControls?.SetVisible(PlayerPrefs.GetInt(PrefPrefix + "Touch") == 1);
        }
        void OnApplicationFocus(bool focused) { if (!focused) Pause(); }
        void OnApplicationPause(bool paused) { if (paused) Pause(); }
        void OnDestroy()
        {
            Time.timeScale = originalScale; AudioListener.pause = originalAudioPause; AudioListener.volume = originalVolume;
            if (input != null && input.Session == this) { input.Session = null; input.ClearForMenu(); }
            if (chapter != null && chapter.session == this) chapter.session = null;
            if (canvasObject != null) Destroy(canvasObject);
        }

        void CreateUI()
        {
            var events = EventSystem.current;
            if (events == null)
                events = new GameObject("Chapter UI EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule)).GetComponent<EventSystem>();
            events.sendNavigationEvents = true;
            var module = events.GetComponent<InputSystemUIInputModule>();
            if (module != null && module.actionsAsset == null) module.AssignDefaultActions();
            canvasObject = new GameObject("B8 Chapter Session UI", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasObject.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 30;
            var scaler = canvasObject.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1000, 600); scaler.matchWidthOrHeight = 1;
            safe = Rect("Safe area", canvasObject.transform, Vector2.zero, Vector2.zero); Stretch(safe);
            gameplay = Rect("Pause HUD", safe, Vector2.zero, Vector2.zero).gameObject; Stretch(gameplay.GetComponent<RectTransform>());
            MakeButton("Pause", "PAUSE", gameplay.transform, new Vector2(1, 1), new Vector2(-72, -72), new Vector2(116, 42), Pause);
            overlay = Rect("Menu overlay", safe, Vector2.zero, Vector2.zero).gameObject; Stretch(overlay.GetComponent<RectTransform>());
            var dim = overlay.AddComponent<Image>(); dim.color = new Color(.02f, .04f, .055f, .73f);
            card = Rect("Menu card", overlay.transform, Vector2.one * .5f, Vector2.zero); card.sizeDelta = new Vector2(780, 508);
            card.gameObject.AddComponent<Image>().color = ink;
            var accent = Rect("Accent", card, new Vector2(.5f, 1), new Vector2(0, -3)); accent.sizeDelta = new Vector2(780, 6); accent.gameObject.AddComponent<Image>().color = mint;
            eyebrow = Label("Chapter", card, 15, new Vector2(0, 215), new Vector2(700, 30), mint);
            title = Label("Title", card, 32, new Vector2(0, 169), new Vector2(700, 50), Color.white);
            subtitle = Label("Subtitle", card, 18, new Vector2(0, 126), new Vector2(700, 32), new Color(.78f, .83f, .82f));
            copy = Label("Description", card, 21, new Vector2(0, 13), new Vector2(690, 186), Color.white);
            copy.alignment = TextAnchor.UpperLeft;
            primary = MakeButton("Primary", "SPIELEN", card, Vector2.one * .5f, new Vector2(-170, -125), new Vector2(330, 50), () => {});
            primary.GetComponent<Image>().color = new Color(.18f, .51f, .43f);
            secondary = MakeButton("Secondary", "VON VORN", card, Vector2.one * .5f, new Vector2(170, -125), new Vector2(330, 50), () => {});
            controls = MakeButton("Controls", "STEUERUNG", card, Vector2.one * .5f, new Vector2(-237, -187), new Vector2(218, 42), OpenControls);
            options = MakeButton("Options", "OPTIONEN", card, Vector2.one * .5f, new Vector2(0, -187), new Vector2(218, 42), OpenOptions);
            home = MakeButton("Home", "STARTMENÜ", card, Vector2.one * .5f, new Vector2(237, -187), new Vector2(218, 42), GoHome);
            footer = Label("Footer", card, 14, new Vector2(0, -233), new Vector2(700, 28), new Color(.58f, .67f, .67f));
            settings = Rect("Settings", card, Vector2.one * .5f, Vector2.zero).gameObject;
            settings.GetComponent<RectTransform>().sizeDelta = new Vector2(700, 210);
            volumeText = Label("Volume label", settings.transform, 17, new Vector2(-160, 77), new Vector2(340, 30), Color.white);
            volume = MakeSlider("Volume", settings.transform, new Vector2(165, 77), 0, 1, SetVolume);
            cameraText = Label("Camera label", settings.transform, 17, new Vector2(-160, 20), new Vector2(340, 30), Color.white);
            cameraSize = MakeSlider("Camera size", settings.transform, new Vector2(165, 20), 38, 48, SetCameraSize);
            touchOption = MakeButton("Touch option", "TOUCH", settings.transform, Vector2.one * .5f, new Vector2(-237, -55), new Vector2(218, 44), () => {
                input.TouchControls?.SetVisible(input.TouchControls?.Visible != true); input.TouchControls?.SetGameplayEnabled(false);
                PlayerPrefs.SetInt(PrefPrefix + "Touch", input.TouchControls?.Visible == true ? 1 : 0); PlayerPrefs.Save(); RefreshText(); });
            helpOption = MakeButton("Help option", "HINWEISE", settings.transform, Vector2.one * .5f, new Vector2(0, -55), new Vector2(218, 44), () => {
                help = !help; PlayerPrefs.SetInt(PrefPrefix + "Help", help ? 1 : 0); PlayerPrefs.Save(); RefreshText(); });
            cameraOption = MakeButton("Camera option", "KAMERA", settings.transform, Vector2.one * .5f, new Vector2(237, -55), new Vector2(218, 44), () => {
                calm = !calm; chapter.cameraRig.follow = calm ? 2.5f : 5; PlayerPrefs.SetInt(PrefPrefix + "Calm", calm ? 1 : 0); PlayerPrefs.Save(); RefreshText(); });
            UpdateSafeArea();
        }
        static RectTransform Rect(string name, Transform parent, Vector2 anchor, Vector2 position)
        { var go = new GameObject(name, typeof(RectTransform)); go.transform.SetParent(parent, false); var r = go.GetComponent<RectTransform>(); r.anchorMin = r.anchorMax = anchor; r.anchoredPosition = position; return r; }
        static void Stretch(RectTransform r) { r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one; r.offsetMin = r.offsetMax = Vector2.zero; }
        static Text Label(string name, Transform parent, int size, Vector2 position, Vector2 dimensions, Color color)
        { var r = Rect(name, parent, Vector2.one * .5f, position); r.sizeDelta = dimensions; var t = r.gameObject.AddComponent<Text>(); t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); t.fontSize = size; t.text = name; t.alignment = TextAnchor.MiddleLeft; t.color = color; t.raycastTarget = false; return t; }
        Button MakeButton(string name, string label, Transform parent, Vector2 anchor, Vector2 position, Vector2 size, UnityEngine.Events.UnityAction action)
        {
            var r = Rect(name, parent, anchor, position); r.sizeDelta = size; var image = r.gameObject.AddComponent<Image>(); image.color = new Color(.13f, .24f, .27f);
            var button = r.gameObject.AddComponent<Button>(); var colors = button.colors; colors.highlightedColor = new Color(.6f, 1, .86f); colors.selectedColor = colors.highlightedColor; colors.pressedColor = new Color(.4f, .72f, .64f); button.colors = colors;
            Label("Label", r, 18, Vector2.zero, size - new Vector2(20, 4), Color.white).alignment = TextAnchor.MiddleCenter;
            Action(button, label, action); return button;
        }
        Slider MakeSlider(string name, Transform parent, Vector2 position, float min, float max, UnityEngine.Events.UnityAction<float> callback)
        {
            var r = Rect(name, parent, Vector2.one * .5f, position); r.sizeDelta = new Vector2(290, 34);
            r.gameObject.AddComponent<Image>().color = new Color(.14f, .24f, .27f);
            var handle = Rect("Handle", r, Vector2.one * .5f, Vector2.zero); handle.sizeDelta = new Vector2(26, 34); handle.gameObject.AddComponent<Image>().color = mint;
            var slider = r.gameObject.AddComponent<Slider>(); slider.minValue = min; slider.maxValue = max; slider.handleRect = handle; slider.targetGraphic = handle.GetComponent<Image>(); slider.onValueChanged.AddListener(callback); return slider;
        }
        void UpdateSafeArea()
        {
            Vector2 size = new Vector2(Screen.width, Screen.height); var area = Screen.safeArea;
            if (size.x <= 0 || size.y <= 0 || (lastSafe == area && lastSize == size)) return;
            lastSafe = area; lastSize = size;
            safe.anchorMin = area.min / size; safe.anchorMax = area.max / size; safe.offsetMin = safe.offsetMax = Vector2.zero;
            canvasObject.GetComponent<CanvasScaler>().matchWidthOrHeight = size.x / size.y >= 1.5f ? 1 : 0;
        }
    }
}
