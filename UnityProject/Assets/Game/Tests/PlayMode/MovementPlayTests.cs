using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace WombatLab.Tests
{
    public class MovementPlayTests
    {
        PlayerMotor player;

        [UnitySetUp] public IEnumerator Setup()
        {
            SceneManager.LoadScene("CombatLab"); yield return null;
            player = Object.FindAnyObjectByType<PlayerMotor>();
            Assert.That(player, Is.Not.Null);
            player.SetTestInput(Vector2.zero);
            yield return new WaitForSeconds(.2f);
        }

        [UnityTest] public IEnumerator GroundMotionUsesRealControllerAndWalkAnimation()
        {
            var start = player.transform.position;
            player.SetTestInput(Vector2.right);
            yield return new WaitForSeconds(.5f);
            Assert.That(player.transform.position.x - start.x, Is.InRange(1.8f, 2.7f));
            Assert.That(player.transform.position.z, Is.EqualTo(start.z).Within(.001f));
            Assert.That(player.transform.position.y, Is.EqualTo(start.y).Within(.04f));
            Assert.That(player.Grounded, Is.True);
            Assert.That(player.animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"), Is.True);
            var leftLeg = player.visual.Find("Rig/LeftLeg");
            float previousAngle = leftLeg.localEulerAngles.x;
            yield return new WaitForSeconds(.1f);
            Assert.That(Mathf.Abs(Mathf.DeltaAngle(previousAngle, leftLeg.localEulerAngles.x)), Is.GreaterThan(1));
        }

        [UnityTest] public IEnumerator JumpChangesHeightNotGroundDepthAndLands()
        {
            var start = player.transform.position;
            player.SetTestInput(Vector2.zero, true);
            yield return new WaitForSeconds(.18f);
            Assert.That(player.transform.position.y - start.y, Is.GreaterThan(.6f));
            Assert.That(player.transform.position.x, Is.EqualTo(start.x).Within(.001f));
            Assert.That(player.transform.position.z, Is.EqualTo(start.z).Within(.001f));
            Assert.That(player.Grounded, Is.False);
            yield return new WaitForSeconds(.9f);
            Assert.That(player.Grounded, Is.True);
            Assert.That(player.transform.position.y, Is.EqualTo(start.y).Within(.04f));
            Assert.That(player.State, Is.EqualTo("Idle"));
        }

        [UnityTest] public IEnumerator ArenaBoundAndRestartAreStable()
        {
            // S2 adds a solid training dummy in the original straight-line lane.
            // Test the arena boundary in a free lane, not passage through its body.
            var cc = player.GetComponent<CharacterController>(); cc.enabled = false;
            player.transform.position = new Vector3(-2.5f, .05f, -1.9f); cc.enabled = true;
            player.SetTestInput(Vector2.right);
            yield return new WaitForSeconds(2.6f);
            Assert.That(player.transform.position.x, Is.InRange(6.95f, 7.01f));
            player.ResetToSpawn();
            Assert.That(player.transform.position.x, Is.EqualTo(-2.5f).Within(.001f));
            Assert.That(player.transform.position.z, Is.EqualTo(-.35f).Within(.001f));
            Assert.That(player.VerticalVelocity, Is.LessThanOrEqualTo(0));
        }

        [UnityTest] public IEnumerator FixedCameraContainsHeroAtFarCorner()
        {
            var controller = player.GetComponent<CharacterController>();
            controller.enabled = false; player.transform.position = new Vector3(6.9f, .05f, 2.3f);
            controller.enabled = true;
            yield return new WaitForSeconds(.7f);
            var foot = Camera.main.WorldToViewportPoint(player.transform.position);
            var head = Camera.main.WorldToViewportPoint(player.transform.position + Vector3.up * 2.25f);
            Assert.That(foot.z, Is.GreaterThan(0));
            Assert.That(foot.x, Is.InRange(.04f, .96f));
            Assert.That(foot.y, Is.InRange(.10f, .85f));
            Assert.That(head.y, Is.LessThan(.85f));
        }

    }
}
