using UnityEngine;

namespace WombatLab
{
    public sealed class ArenaCamera : MonoBehaviour
    {
        public Transform target;
        public Vector3 offset = new Vector3(0, 7.6f, -12);
        public float follow = 4;
        Vector3 focus = new Vector3(0, .7f, 0);
        void LateUpdate()
        {
            if (target == null) return;
            var desired = new Vector3(Mathf.Clamp(target.position.x * .25f, -1.6f, 1.6f), .7f,
                Mathf.Clamp(target.position.z * .15f, -.35f, .35f));
            focus = Vector3.Lerp(focus, desired, 1 - Mathf.Exp(-follow * Time.deltaTime));
            transform.position = focus + offset;
            transform.rotation = Quaternion.LookRotation(focus - transform.position, Vector3.up);
        }
    }
}
