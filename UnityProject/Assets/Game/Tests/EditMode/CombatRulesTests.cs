using NUnit.Framework;

namespace WombatLab.Tests
{
    public class CombatRulesTests
    {
        [TestCase(30)] [TestCase(60)] [TestCase(120)]
        public void EveryFrameRateFindsActiveWindow(int fps)
        {
            float previous = 0; int contacts = 0;
            for (int frame = 1; frame <= fps; frame++)
            {
                float now = frame / (float)fps;
                if (AttackRules.WindowCrossed(previous, now, .25f, .46f)) contacts++;
                previous = now;
            }
            Assert.That(contacts, Is.GreaterThan(0));
        }
        [Test] public void WholeWindowSkippedByOneFrameStillResolves()
        { Assert.That(AttackRules.WindowCrossed(.1f, .7f, .25f, .46f), Is.True); }
        [Test] public void StartupCannotHit()
        { Assert.That(AttackRules.WindowCrossed(0, .1f, .25f, .46f), Is.False); }
        [Test] public void RecoveryCannotHit()
        { Assert.That(AttackRules.WindowCrossed(.6f, .8f, .25f, .46f), Is.False); }
        [Test] public void RewoundClipCannotOpenWindow()
        { Assert.That(AttackRules.WindowCrossed(.7f, .2f, .25f, .46f), Is.False); }
        [Test] public void HitstopDoesNotExpireBufferedInput()
        {
            var buffer = new CombatBuffer(); buffer.Submit(CombatIntent.Light);
            buffer.Tick(.15f); for (int i = 0; i < 120; i++) buffer.Tick(0);
            Assert.That(buffer.Pending, Is.EqualTo(CombatIntent.Light));
            Assert.That(buffer.Remaining, Is.EqualTo(.15f).Within(.0001f));
        }
        [Test] public void BufferExpiresAndConsumesOnlyOnce()
        {
            var buffer = new CombatBuffer(); buffer.Submit(CombatIntent.Heavy);
            Assert.That(buffer.Consume(), Is.EqualTo(CombatIntent.Heavy));
            Assert.That(buffer.Consume(), Is.EqualTo(CombatIntent.None));
            buffer.Submit(CombatIntent.Light); buffer.Tick(.31f);
            Assert.That(buffer.Consume(), Is.EqualTo(CombatIntent.None));
        }
        [TestCase(1, 1, true, 1, false)]
        [TestCase(1, 2, false, 1, false)]
        [TestCase(1, 2, true, -1, false)]
        [TestCase(1, 2, true, 1, true)]
        public void TeamDeathAndFacingFilter(int attacker, int target, bool alive, float dot, bool valid)
        { Assert.That(AttackRules.ValidTarget(attacker, target, alive, dot), Is.EqualTo(valid)); }
    }
}
