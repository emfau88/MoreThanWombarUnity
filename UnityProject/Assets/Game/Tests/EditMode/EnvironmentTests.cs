using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace WombatLab.Tests
{
    public class EnvironmentTests
    {
        [Test] public void UsesPinnedUnitySixEditor()
        {
            Assert.That(Application.unityVersion, Is.EqualTo("6000.4.0f1"));
        }

        [Test] public void ProjectIsSeparateFromBrowsergame()
        {
            Assert.That(Application.dataPath, Does.EndWith("MoreThanWombarUnity/UnityProject/Assets"));
            Assert.That(AssetDatabase.IsValidFolder("Assets/Game"), Is.True);
        }

        [Test] public void UrpPipelineIsAssigned()
        {
            Assert.That(UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline, Is.Not.Null);
        }
    }
}
