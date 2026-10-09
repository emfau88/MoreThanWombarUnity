using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace WombatLab.Tests
{
    public class JunkyardChapterPlayTests
    {
        JunkyardChapter chapter;
        EngagementCoordinator encounter;
        PlayerMotor player;
        PlayerDefense defense;
        CombatController combat;
        [UnitySetUp] public IEnumerator Setup()
        {
            SceneManager.LoadScene("JunkyardChapter"); yield return null;
            chapter=Object.FindAnyObjectByType<JunkyardChapter>(); encounter=chapter.encounter; player=encounter.player;
            if (chapter.session != null) chapter.session.StartRun(false);
            defense=player.GetComponent<PlayerDefense>(); combat=player.GetComponent<CombatController>();
            player.SetTestInput(Vector2.zero); yield return null;
        }
        IEnumerator WalkTo(float x)
        {
            float end=Time.time+15;
            while(Mathf.Abs(player.transform.position.x-x)>.16f && Time.time<end)
            {
                player.SetTestInput(new Vector2(Mathf.Sign(x-player.transform.position.x),0),run:true);
                yield return null;
            }
            player.SetTestInput(Vector2.zero);
            Assert.That(player.transform.position.x,Is.EqualTo(x).Within(.2f),"Walkable route / collider");
        }
        IEnumerator DefeatWave()
        {
            // Progression check uses the real damage/death receiver. Gameplay contact is
            // covered separately by ActualMixedGroup... and the existing duel cases.
            foreach(var enemy in encounter.enemies) enemy.enabled=false;
            foreach(var enemy in encounter.enemies)
            {
                float end=Time.time+8;
                while(enemy.target.Alive && Time.time<end)
                {
                    if(!enemy.GetComponent<BodyRecovery>().Protected)enemy.target.ReceiveHit(combat.lights[2],Vector3.zero);
                    yield return new WaitForSecondsRealtime(.18f); // Final result pauses scaled time.
                }
                Assert.That(enemy.target.Alive,Is.False);
            }
            yield return null; yield return null;
        }
        [UnityTest] public IEnumerator FullRouteWavesSwitchAndChapterRestart()
        {
            Assert.That(chapter.Phase,Is.EqualTo(ChapterPhase.Arrival)); Assert.That(encounter.LivingCount,Is.Zero);
            Assert.That(chapter.TryInteract(),Is.False); yield return WalkTo(-4);
            Assert.That(chapter.Phase,Is.EqualTo(ChapterPhase.Fighting));
            for(int area=0;area<3;area++)
            {
                Assert.That(chapter.AreaIndex,Is.EqualTo(area)); Assert.That(chapter.entries[area].Closed,Is.True);
                Assert.That(chapter.exits[area].Closed,Is.True);
                for(int wave=0;wave<chapter.definition.areas[area].waves.Length;wave++)
                {
                    Assert.That(chapter.WaveIndex,Is.EqualTo(wave));
                    Assert.That(encounter.LivingCount,Is.EqualTo(chapter.definition.areas[area].waves[wave].enemies.Length));
                    foreach(var enemy in encounter.enemies)
                    {
                        Assert.That(enemy.transform.position.x,Is.InRange(area*24-6.5f,area*24+6.5f));
                        Assert.That(enemy.target.Health,Is.EqualTo(enemy.role.health));
                    }
                    yield return DefeatWave();
                    if(wave+1<chapter.definition.areas[area].waves.Length)
                    {
                        Assert.That(chapter.Phase,Is.EqualTo(ChapterPhase.BetweenWaves));
                        yield return new WaitForSeconds(chapter.definition.waveBreak+.15f);
                    }
                }
                Assert.That(chapter.CompletedAreas,Is.EqualTo(area+1)); Assert.That(chapter.exits[area].Closed,Is.False);
                if(area==2) break;
                Assert.That(chapter.Phase,Is.EqualTo(ChapterPhase.Travel));
                if(area==0)
                {
                    Assert.That(chapter.GateOpened,Is.False); yield return WalkTo(10.5f);
                    Assert.That(chapter.TryInteract(),Is.True); Assert.That(chapter.switchGate.Closed,Is.False);
                }
                yield return WalkTo(chapter.definition.areas[area+1].center-4);
            }
            Assert.That(chapter.Phase,Is.EqualTo(ChapterPhase.Complete)); Assert.That(encounter.LivingCount,Is.Zero);
            Assert.That(chapter.Hint(false),Does.Contain("geschafft"));
            if (chapter.session != null)
            {
                Assert.That(chapter.session.Page,Is.EqualTo(SessionPage.Result));
                Assert.That(Time.timeScale,Is.Zero);
                var replay=GameObject.Find("B8 Chapter Session UI").transform.Find("Safe area/Menu overlay/Menu card/Primary").GetComponent<UnityEngine.UI.Button>();
                Assert.That(replay.GetComponentInChildren<UnityEngine.UI.Text>().text,Is.EqualTo("NOCHMAL SPIELEN"));
                replay.onClick.Invoke();
                Assert.That(chapter.session.Page,Is.EqualTo(SessionPage.Playing));
            }
            else chapter.RestartChapter();
            yield return null;
            Assert.That(chapter.Phase,Is.EqualTo(ChapterPhase.Arrival)); Assert.That(chapter.CompletedAreas,Is.Zero);
            Assert.That(chapter.switchGate.Closed,Is.True); Assert.That(defense.Health,Is.EqualTo(100));
            Assert.That(player.transform.position.x,Is.EqualTo(chapter.definition.start.x).Within(.05f));
        }
        [UnityTest] public IEnumerator CheckpointPreservesCompletedAreaGateHealthAndResetsCombatDeath()
        {
            yield return WalkTo(-4);
            for(int i=0;i<3;i++)
            {yield return DefeatWave();if(i<2)yield return new WaitForSeconds(chapter.definition.waveBreak+.15f);}
            yield return WalkTo(10.5f); Assert.That(chapter.TryInteract(),Is.True);
            defense.ReceiveDamage(18,Vector3.zero); yield return WalkTo(20);
            int entryHealth=defense.Health; Assert.That(chapter.CheckpointArea,Is.EqualTo(1));
            var abandoned=encounter.enemies[0]; abandoned.target.ReceiveHit(combat.lights[0],Vector3.right);
            Assert.That(defense.ReceiveDamage(1000,Vector3.zero,true),Is.True);
            Assert.That(defense.Alive,Is.False); Assert.That(player.GetComponent<BodyRecovery>().Busy,Is.True);
            if (chapter.session != null) chapter.session.Retry(); else chapter.RetryCheckpoint();
            player.SetTestInput(Vector2.zero); yield return null;
            Assert.That(chapter.CompletedAreas,Is.EqualTo(1)); Assert.That(chapter.AreaIndex,Is.EqualTo(1));
            Assert.That(chapter.WaveIndex,Is.Zero); Assert.That(chapter.GateOpened,Is.True);
            Assert.That(chapter.Phase,Is.EqualTo(ChapterPhase.Fighting)); Assert.That(encounter.Owner,Is.Null);
            Assert.That(defense.Health,Is.EqualTo(entryHealth)); Assert.That(combat.Attacking,Is.False);
            Assert.That(player.GetComponent<BodyRecovery>().Busy,Is.False); Assert.That(encounter.LivingCount,Is.EqualTo(2));
            Assert.That(player.transform.position.x,Is.EqualTo(18.4f).Within(.1f));
            Assert.That(abandoned==null,Is.True); // old wave was removed, never resurrected at a stale spawn.
            foreach(var enemy in encounter.enemies)Assert.That(enemy.target.Health,Is.EqualTo(enemy.role.health));
            yield return WalkTo(24);
            Assert.That(Camera.main.WorldToViewportPoint(player.transform.position+Vector3.up).x,Is.InRange(.08f,.92f));
        }
        [UnityTest] public IEnumerator ActualMixedGroupCanBeFoughtAtSecondAreaAndTouchUsesChapterFlow()
        {
            yield return WalkTo(-4);
            for(int i=0;i<3;i++)
            {yield return DefeatWave();if(i<2)yield return new WaitForSeconds(chapter.definition.waveBreak+.15f);}
            yield return WalkTo(10.5f);
            player.GetComponent<MobileTouchControls>().SetVisible(true);
            var buttons=player.GetComponentsInChildren<UnityEngine.UI.Button>(true);
            var interaction=System.Array.Find(buttons,x=>x.name=="Interact");
            Assert.That(interaction,Is.Not.Null); interaction.onClick.Invoke(); Assert.That(chapter.GateOpened,Is.True);
            Assert.That(System.Array.Find(buttons,x=>x.name=="Opponent count"),Is.Null);
            yield return WalkTo(20); float end=Time.time+22, nextTap=0;
            while(chapter.Phase==ChapterPhase.Fighting && defense.Alive && Time.time<end)
            {
                var target=System.Array.Find(encounter.enemies,x=>x.target.Alive);
                var toward=Vector3.ProjectOnPlane(target.transform.position-player.transform.position,Vector3.up);
                if(!combat.Attacking && toward.sqrMagnitude>.01f)player.visual.rotation=Quaternion.LookRotation(toward);
                player.SetTestInput(toward.magnitude>1.15f?new Vector2(toward.x,toward.z).normalized:Vector2.zero);
                if(Time.time>=nextTap){combat.Queue(CombatIntent.Light);nextTap=Time.time+.15f;}
                int committed=0;foreach(var enemy in encounter.enemies)if(enemy.Committed)committed++;
                Assert.That(committed,Is.LessThanOrEqualTo(1)); yield return null;
            }
            player.SetTestInput(Vector2.zero);
            Assert.That(defense.Alive,Is.True,"Actual standard/agile fight should be winnable with existing contacts at world x=24");
            Assert.That(chapter.Phase,Is.EqualTo(ChapterPhase.BetweenWaves));
        }
    }
}
