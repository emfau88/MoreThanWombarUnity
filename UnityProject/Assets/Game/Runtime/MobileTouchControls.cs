using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace WombatLab
{
    // Presentation/input adapter only: Unity's on-screen controls feed the existing gamepad map.
    [RequireComponent(typeof(LabInput))]
    public sealed class MobileTouchControls : MonoBehaviour
    {
        public bool Visible => controls != null && controls.activeSelf;
        public bool Running => Visible && Stick.control is Vector2Control axis && axis.ReadValue().magnitude >= .9f;
        public bool BlocksPointerAttacks => Visible || (Mouse.current != null && toggle != null &&
            RectTransformUtility.RectangleContainsScreenPoint(toggle, Mouse.current.position.ReadValue()));
        public OnScreenStick Stick { get; private set; }
        public RectTransform SafeArea => safeArea;
        RectTransform safeArea, toggle;
        GameObject canvasObject, controls, portraitHint;
        Text toggleText, stateText;
        Text opponentCountText;
        EngagementCoordinator modeEncounter;
        JunkyardChapter chapter;
        Button interactButton;
        Transform keyboardHelp, header;
        int originalStateFont;
        Sprite disc;
        Texture2D discTexture;
        bool userSelected;
        Rect lastSafe;
        Vector2 lastSize;

        void Awake() { EnsureCreated(); }
        public void EnsureCreated()
        {
            if (canvasObject != null) return;
            GetComponent<LabInput>().TouchControls = this;
            if (EventSystem.current == null)
            {
                var events = new GameObject("Touch UI EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
                events.transform.SetParent(transform, false);
                events.GetComponent<EventSystem>().sendNavigationEvents = false;
                var module = events.GetComponent<InputSystemUIInputModule>();
                module.pointerBehavior = UIPointerBehavior.AllPointersAsIs;
                module.deselectOnBackgroundClick = false;
            }
            discTexture = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            discTexture.name = "Touch disc";
            for (int y = 0; y < 64; y++) for (int x = 0; x < 64; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(31.5f, 31.5f));
                discTexture.SetPixel(x, y, new Color(1, 1, 1, Mathf.Clamp01(31.5f - distance)));
            }
            discTexture.Apply(); disc = Sprite.Create(discTexture, new Rect(0, 0, 64, 64), Vector2.one * .5f);
            canvasObject = new GameObject("Mobile Touch HUD", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasObject.transform.SetParent(transform, false);
            var canvas = canvasObject.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 10;
            var scaler = canvasObject.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 600); scaler.matchWidthOrHeight = 1;
            safeArea = Rect("Safe area", canvasObject.transform); Stretch(safeArea);
            var toggleButton = Button("Touch toggle", "TOUCH", safeArea, new Vector2(1, 1), new Vector2(-76, -24), new Vector2(116, 42), false);
            toggle = toggleButton.GetComponent<RectTransform>(); toggleText = toggleButton.GetComponentInChildren<Text>();
            toggleButton.onClick.AddListener(() => { userSelected = true; SetVisible(!Visible); });
            controls = Rect("Touch controls", safeArea).gameObject; Stretch(controls.GetComponent<RectTransform>());
            controls.SetActive(false);

            var baseDisc = Image("Stick base", controls.transform, new Vector2(0, 0), new Vector2(122, 116), new Vector2(188, 188), new Color(.06f, .12f, .15f, .65f));
            baseDisc.raycastTarget = false;
            var knob = Image("Move stick", baseDisc.transform, Vector2.one * .5f, Vector2.zero, new Vector2(96, 96), new Color(.32f, .82f, .72f, .85f));
            Stick = knob.gameObject.AddComponent<OnScreenStick>(); Stick.controlPath = "<Gamepad>/leftStick";
            Stick.movementRange = 62; Stick.behaviour = OnScreenStick.Behaviour.RelativePositionWithStaticOrigin;
            Label(baseDisc.transform, "BEWEGEN", 18, new Vector2(0, -93), new Vector2(190, 30));
            Label(baseDisc.transform, "Außen: Rennen", 16, new Vector2(0, 112), new Vector2(200, 28));

            Action("Heavy", "HEAVY", "buttonNorth", new Vector2(-266, 174), 88, new Color(.80f, .40f, .24f, .85f));
            Action("Kick", "KICK", "rightShoulder", new Vector2(-165, 190), 88, new Color(.32f, .44f, .53f, .85f));
            Action("Jump", "SPRUNG", "buttonSouth", new Vector2(-66, 172), 88, new Color(.32f, .44f, .53f, .85f));
            Action("Light", "COMBO", "buttonWest", new Vector2(-205, 69), 108, new Color(.25f, .72f, .61f, .90f));
            Action("Evade", "AUSWEICHEN", "buttonEast", new Vector2(-85, 61), 102, new Color(.32f, .44f, .53f, .85f));
            Action("Charge", "STOSS", "rightTrigger", new Vector2(-325, 66), 92, new Color(.68f, .48f, .22f, .9f));
            chapter = FindAnyObjectByType<JunkyardChapter>();
            var reset = Button("Restart", chapter != null ? "CHECKPOINT" : "NEUSTART", controls.transform, new Vector2(0, 1), new Vector2(85, -24), new Vector2(140, 42), false);
            reset.gameObject.AddComponent<OnScreenButton>().controlPath = "<Gamepad>/start";
            var encounter = FindAnyObjectByType<EngagementCoordinator>();
            if (chapter != null)
            {
                var restartChapter = Button("Chapter restart", "VON VORN", controls.transform, new Vector2(0,1), new Vector2(255,-24), new Vector2(170,42), false);
                restartChapter.gameObject.AddComponent<OnScreenButton>().controlPath = "<Gamepad>/select";
                interactButton = Button("Interact", "TOR ÖFFNEN", controls.transform, new Vector2(.5f,0), new Vector2(0,64), new Vector2(190,52), false);
                interactButton.onClick.AddListener(() => chapter.TryInteract());
            }
            else if (encounter != null)
            {
                modeEncounter = encounter;
                var mode = Button("Opponent count", "GEGNER: 1", controls.transform, new Vector2(0,1), new Vector2(255,-24), new Vector2(170,42), false);
                opponentCountText = mode.GetComponentInChildren<Text>();
                mode.onClick.AddListener(() => encounter.SetMode(encounter.Mode % Mathf.Min(4,encounter.enemies.Length) + 1));
            }
            var hint = Rect("Portrait hint", controls.transform); Stretch(hint);
            portraitHint = hint.gameObject;
            Label(hint, "Für den Kampf bitte quer halten", 26, Vector2.zero, new Vector2(540, 64));
            var hud = FindAnyObjectByType<LabHud>();
            if (hud != null)
            {
                keyboardHelp = hud.transform.Find("Controls"); header = hud.transform.Find("Header");
                stateText = hud.stateText; if (stateText != null) originalStateFont = stateText.fontSize;
            }
            SetVisible(Application.isMobilePlatform || Touchscreen.current != null);
            UpdateSafeArea();
        }

        void Action(string name, string label, string binding, Vector2 position, float size, Color color)
        {
            var button = Button(name, label, controls.transform, new Vector2(1, 0), position, Vector2.one * size, true);
            button.GetComponent<Image>().color = color;
            button.gameObject.AddComponent<OnScreenButton>().controlPath = "<Gamepad>/" + binding;
        }
        Button Button(string name, string label, Transform parent, Vector2 anchor, Vector2 position, Vector2 size, bool round)
        {
            var image = Image(name, parent, anchor, position, size, new Color(.08f, .16f, .19f, .9f));
            if (!round) image.sprite = null;
            var button = image.gameObject.AddComponent<Button>();
            button.navigation = new Navigation { mode = Navigation.Mode.None };
            var colors = button.colors; colors.pressedColor = new Color(.62f, 1, .88f); colors.highlightedColor = Color.white; button.colors = colors;
            Label(image.transform, label, label == "AUSWEICHEN" ? 14 : label == "SPRUNG" ? 17 : 19, Vector2.zero, size - Vector2.one * 8);
            return button;
        }
        Image Image(string name, Transform parent, Vector2 anchor, Vector2 position, Vector2 size, Color color)
        {
            var rect = Rect(name, parent); rect.anchorMin = rect.anchorMax = anchor; rect.sizeDelta = size; rect.anchoredPosition = position;
            var image = rect.gameObject.AddComponent<Image>(); image.sprite = disc; image.color = color; return image;
        }
        static RectTransform Rect(string name, Transform parent)
        { var go = new GameObject(name, typeof(RectTransform)); go.transform.SetParent(parent, false); return go.GetComponent<RectTransform>(); }
        static void Stretch(RectTransform rect)
        { rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one; rect.offsetMin = rect.offsetMax = Vector2.zero; }
        static void Label(Transform parent, string text, int size, Vector2 position, Vector2 dimensions)
        {
            var rect = Rect(text, parent); rect.anchorMin = rect.anchorMax = Vector2.one * .5f; rect.anchoredPosition = position; rect.sizeDelta = dimensions;
            var label = rect.gameObject.AddComponent<Text>(); label.text = text; label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = size; label.color = Color.white; label.alignment = TextAnchor.MiddleCenter; label.raycastTarget = false;
        }
        public void SetVisible(bool visible)
        {
            controls.SetActive(visible); toggleText.text = visible ? "TOUCH AUS" : "TOUCH";
            if (keyboardHelp != null) keyboardHelp.gameObject.SetActive(!visible);
            if (header != null) foreach (var title in header.GetComponentsInChildren<Text>(true))
                if (title != stateText) title.gameObject.SetActive(!visible);
            if (stateText != null) stateText.fontSize = visible ? 22 : originalStateFont;
            portraitHint.SetActive(visible && Screen.height > Screen.width);
        }
        void Update()
        {
            if (interactButton != null) interactButton.gameObject.SetActive(chapter != null && chapter.CanInteract);
            if (opponentCountText != null && modeEncounter != null) opponentCountText.text = "GEGNER: " + modeEncounter.Mode;
            if (!userSelected && !Visible && (Application.isMobilePlatform || Touchscreen.current != null)) SetVisible(true);
            UpdateSafeArea(); portraitHint.SetActive(Visible && Screen.height > Screen.width);
        }
        void UpdateSafeArea()
        {
            var size = new Vector2(Screen.width, Screen.height); var area = Screen.safeArea;
            if (size.x <= 0 || size.y <= 0 || (lastSafe == area && lastSize == size)) return;
            lastSafe = area; lastSize = size;
            canvasObject.GetComponent<CanvasScaler>().matchWidthOrHeight = size.x >= size.y ? 1 : 0;
            safeArea.anchorMin = area.min / size; safeArea.anchorMax = area.max / size;
            safeArea.offsetMin = safeArea.offsetMax = Vector2.zero;
        }
        // Disabling the official controls releases/removes their virtual device, including held pointers.
        public void ReleasePointers() { if (Visible) { controls.SetActive(false); controls.SetActive(true); } }
        void OnApplicationFocus(bool focused) { if (!focused) ReleasePointers(); }
        void OnApplicationPause(bool paused) { if (paused) ReleasePointers(); }
        void OnDisable() { if (controls != null) SetVisible(false); }
        void OnDestroy()
        {
            if (disc != null) Destroy(disc); if (discTexture != null) Destroy(discTexture);
            var input = GetComponent<LabInput>(); if (input != null && input.TouchControls == this) input.TouchControls = null;
        }
    }
}
