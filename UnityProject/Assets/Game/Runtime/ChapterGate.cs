using UnityEngine;

namespace WombatLab
{
    public sealed class ChapterGate : MonoBehaviour
    {
        public GameObject panel;
        public bool Closed => panel.activeSelf;
        public void SetClosed(bool closed) { panel.SetActive(closed); }
    }
}
