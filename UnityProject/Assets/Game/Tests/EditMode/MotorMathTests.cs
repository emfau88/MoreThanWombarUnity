using NUnit.Framework;
using UnityEngine;

namespace WombatLab.Tests
{
    public class MotorMathTests
    {
        [Test] public void DiagonalInputIsNotFaster()
        { Assert.That(MotorMath.PlanarInput(Vector2.one).magnitude, Is.EqualTo(1).Within(.00001)); }
        [Test] public void AnalogInputIsPreserved()
        { Assert.That(MotorMath.PlanarInput(new Vector2(.25f, 0)).x, Is.EqualTo(.25f)); }
        [Test] public void DepthDoesNotChangeHeight()
        { Assert.That(MotorMath.PlanarInput(Vector2.up), Is.EqualTo(Vector3.forward)); }
        [Test] public void BadInputCannotCorruptPosition()
        { Assert.That(MotorMath.PlanarInput(new Vector2(float.NaN, 0)), Is.EqualTo(Vector3.zero)); }
        [Test] public void BoundsClampGroundButPreserveJumpHeight()
        {
            Assert.That(MotorMath.ClampGround(new Vector3(9, 2, -7), new Vector2(-7, -3), new Vector2(7, 3)),
                Is.EqualTo(new Vector3(7, 2, -3)));
        }
        [TestCase(30)] [TestCase(60)] [TestCase(120)]
        public void JumpTrajectoryIsFrameRateIndependent(int fps)
        {
            float y = 0, v = 8, dt = 1f / fps;
            for (int i = 0; i < fps / 2; i++)
            { y += MotorMath.VerticalDisplacement(v, 23, dt); v -= 23 * dt; }
            Assert.That(y, Is.EqualTo(8 * .5f - .5f * 23 * .25f).Within(.0001f));
        }
    }
}
