using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace WombatLab.Editor
{
    // One bounded actual chapter run using the same input/motor/combat components.
    public static class JunkyardChapterReview
    {
        static JunkyardChapter chapter;
        static PlayerMotor player;
        static CombatController combat;
        static PlayerDefense defense;
        static double started;
        static float nextTap;
        static int capturedArea=-1, capturedWave=-1;
        static bool capturedGate;
        static string prefix;
        static readonly StringBuilder report=new StringBuilder();
        public static string Start(string capturePrefix = "b6")
        {
            if(!EditorApplication.isPlaying)throw new InvalidOperationException("Start JunkyardChapter Play first.");
            Cancel(); prefix=capturePrefix; chapter=Object.FindAnyObjectByType<JunkyardChapter>();player=chapter.encounter.player;
            combat=player.GetComponent<CombatController>();defense=player.GetComponent<PlayerDefense>();
            if (chapter.session != null) chapter.session.StartRun(false); else chapter.RestartChapter();
            player.SetTestInput(Vector2.zero); started=EditorApplication.timeSinceStartup;
            nextTap=0;capturedArea=capturedWave=-1;capturedGate=false;report.Clear();
            report.AppendLine("Actual chapter run: standard input injection, unchanged HP/damage/AI, no direct damage or teleports.");
            LabVisualReview.Capture(prefix+"-arrival");EditorApplication.update+=Tick;return "Actual nine-wave chapter run started.";
        }
        static void Tick()
        {
            if(!EditorApplication.isPlaying||chapter==null){Cancel();return;}
            try
            {
                if(!defense.Alive||chapter.Phase==ChapterPhase.Complete||EditorApplication.timeSinceStartup-started>420)
                { Finish(); return; }
                if(chapter.Phase==ChapterPhase.Arrival || chapter.Phase==ChapterPhase.Travel)
                {
                    if(chapter.Phase==ChapterPhase.Travel && chapter.AreaIndex==0 && !chapter.GateOpened)
                    {
                        if(chapter.CanInteract)
                        {
                            player.SetTestInput(Vector2.zero);
                            if(!capturedGate){LabVisualReview.Capture(prefix+"-switch");capturedGate=true;}
                            chapter.TryInteract();report.AppendLine("Gate opened via nearby interaction; HP="+defense.Health);
                        }
                        else
                        {
                            var to=Vector3.ProjectOnPlane(chapter.switchPosition.position-player.transform.position,Vector3.up);
                            player.SetTestInput(new Vector2(to.x,to.z).normalized,run:true);
                        }
                    }
                    else player.SetTestInput(Vector2.right,run:true);
                    return;
                }
                if(chapter.Phase==ChapterPhase.BetweenWaves){player.SetTestInput(Vector2.zero);return;}
                if(capturedArea!=chapter.AreaIndex||capturedWave!=chapter.WaveIndex)
                {
                    capturedArea=chapter.AreaIndex;capturedWave=chapter.WaveIndex;
                    report.AppendLine("Area="+capturedArea+" wave="+capturedWave+" HP="+defense.Health+" position="+player.transform.position);
                    LabVisualReview.Capture(prefix+"-area-"+(capturedArea+1)+"-wave-"+(capturedWave+1));
                }
                var enemies=chapter.encounter.enemies;
                var target=enemies.Where(e=>e.target.Alive && !e.GetComponent<BodyRecovery>().Protected)
                    .OrderBy(e=>(e.transform.position-player.transform.position).sqrMagnitude).FirstOrDefault();
                if(target==null){player.SetTestInput(Vector2.zero);return;}
                var toward=Vector3.ProjectOnPlane(target.transform.position-player.transform.position,Vector3.up);
                if(!combat.Attacking && toward.sqrMagnitude>.01f)player.visual.rotation=Quaternion.LookRotation(toward);
                player.SetTestInput(toward.magnitude>1.12f?new Vector2(toward.x,toward.z).normalized:Vector2.zero);
                if(Time.time>=nextTap){combat.Queue(CombatIntent.Light);nextTap=Time.time+.15f;}
            }
            catch(Exception e){report.AppendLine(e.ToString());Finish();}
        }
        static void Finish()
        {
            report.AppendLine("End="+chapter.Phase+" completed="+chapter.CompletedAreas+" HP="+defense.Health
                +" elapsed="+(EditorApplication.timeSinceStartup-started).ToString("F1"));
            LabVisualReview.Capture(defense.Alive ? prefix+"-run-end" : prefix+"-run-defeat");
            File.WriteAllText(Path.Combine(Application.dataPath,"QA/"+prefix+"-run-report.txt"),report.ToString());
            player.ReleaseTestInput(); EditorApplication.update-=Tick;EditorApplication.isPaused=true;
        }
        public static void Cancel()
        {EditorApplication.update-=Tick;if(player!=null)player.ReleaseTestInput();}
    }
}
