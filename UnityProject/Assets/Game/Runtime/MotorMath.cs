using UnityEngine;

namespace WombatLab
{
    public static class MotorMath
    {
        public static Vector3 PlanarInput(Vector2 input)
        {
            if (float.IsNaN(input.x) || float.IsNaN(input.y) ||
                float.IsInfinity(input.x) || float.IsInfinity(input.y)) return Vector3.zero;
            input = Vector2.ClampMagnitude(input, 1);
            return new Vector3(input.x, 0, input.y);
        }

        public static Vector3 ClampGround(Vector3 position, Vector2 min, Vector2 max)
        {
            return new Vector3(Mathf.Clamp(position.x, min.x, max.x), position.y,
                Mathf.Clamp(position.z, min.y, max.y));
        }

        // Exact constant-acceleration displacement avoids FPS-dependent jump height.
        public static float VerticalDisplacement(float velocity, float gravity, float dt)
            => velocity * dt - .5f * gravity * dt * dt;
    }
}
