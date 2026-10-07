using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace WombatLab.Editor
{
    public static class JunkyardChapterBuilder
    {
        public const string ScenePath = "Assets/Game/Scenes/JunkyardChapter.unity";
        const string Root = "Assets/Game/Chapters/Junkyard/";
        const string Combat = "Assets/Game/Characters/HumanoidCombat/";
        const string Materials = "Assets/Game/Environment/JunkyardPreview/Materials/";
        [MenuItem("Wombat Lab/B6 Build Junkyard Chapter")]
        public static string Build()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling || EditorUtility.scriptCompilationFailed
                || UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty)
                throw new InvalidOperationException("Use idle compiled Editor with a saved scene.");
            Directory.CreateDirectory(Root); AssetDatabase.Refresh();
            var scene = EditorSceneManager.OpenScene(HumanoidCombatBuilder.ScenePath);
            // Fork the saved lab first. The lab and its combat assets remain the reference scene.
            EditorSceneManager.SaveScene(scene, ScenePath);
            var encounter = Object.FindAnyObjectByType<EngagementCoordinator>();
            var player = encounter.player;
            var chapter = new GameObject("B6 Junkyard chapter").AddComponent<JunkyardChapter>();
            var standard = AssetDatabase.LoadAssetAtPath<EnemyRoleDefinition>(Combat + "Role_STANDARD.asset");
            var agile = AssetDatabase.LoadAssetAtPath<EnemyRoleDefinition>(Combat + "Role_AGILE.asset");
            var heavy = AssetDatabase.LoadAssetAtPath<EnemyRoleDefinition>(Combat + "Role_HEAVY.asset");
            var elite = AssetDatabase.LoadAssetAtPath<EnemyRoleDefinition>(Root + "Foreman.asset");
            if (elite == null) { elite = Object.Instantiate(heavy); AssetDatabase.CreateAsset(elite, Root + "Foreman.asset"); }
            else EditorUtility.CopySerialized(heavy, elite);
            elite.name = elite.displayName = "VORARBEITER"; elite.health = 100; elite.damage = 20;
            elite.moveSpeed = 1.9f; elite.telegraphSeconds = .7f; elite.cooldownSeconds = .85f;
            var data = AssetDatabase.LoadAssetAtPath<ChapterDefinition>(Root + "JunkyardChapter.asset");
            if (data == null) { data = ScriptableObject.CreateInstance<ChapterDefinition>(); AssetDatabase.CreateAsset(data,Root + "JunkyardChapter.asset"); }
            data.areas = new[] {
                Area("ANLIEFERUNG",0, Wave(standard), Wave(standard,standard), Wave(standard,standard)),
                Area("SORTIERHOF",24, Wave(standard,agile), Wave(agile,standard,standard), Wave(heavy,agile,standard)),
                Area("PRESSWERK",48, Wave(heavy,standard,standard), Wave(heavy,agile,standard,standard), Wave(elite,agile,standard))
            };
            chapter.definition = data; chapter.encounter = encounter;
            chapter.cameraRig = Camera.main.GetComponent<ArenaCamera>();
            chapter.cameraRig.scrolling = true; chapter.cameraRig.minimumX = -5; chapter.cameraRig.maximumX = 51;
            chapter.cameraRig.target = player.transform;
            encounter.chapter = chapter; player.chapter = chapter;
            var templates = new GameObject("Chapter enemy templates").transform;
            foreach (var enemy in encounter.enemies)
            {
                enemy.gameObject.SetActive(false); enemy.transform.SetParent(templates, true);
            }
            encounter.chapterTemplates = encounter.enemies.Take(3).ToArray();
            // The fourth lab clone is not a separate role template.
            Object.DestroyImmediate(encounter.enemies[3].gameObject);
            encounter.enemies = new EnemyBrain[0];
            var originalArena = scene.GetRootGameObjects().First(x => x.name.StartsWith("Arena"));
            Object.DestroyImmediate(originalArena);
            var world = new GameObject("B6 connected junkyard").transform;
            Material steel = Material("Weathered steel"), paint = Material("Worn yellow paint"), rust = Material("Rust red");
            var ground = Box(world,"Continuous physical floor",new Vector3(24,-.2f,0),new Vector3(66,.4f,7),steel,true);
            ground.GetComponent<Renderer>().enabled = false;
            foreach (int side in new[]{-1,1})
                Box(world,"Physical lane boundary",new Vector3(24,.15f,side*3.45f),new Vector3(66,.3f,.18f),steel,true);
            var originalEnvironment = GameObject.Find("M1 — Junkyard environment");
            Object.DestroyImmediate(originalEnvironment);
            var environment = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Game/Environment/JunkyardPreview/JunkyardEnvironment.prefab");
            chapter.entries = new ChapterGate[3]; chapter.exits = new ChapterGate[3];
            for (int i = 0; i < 3; i++)
            {
                float center = data.areas[i].center;
                var room = (GameObject)PrefabUtility.InstantiatePrefab(environment);
                PrefabUtility.UnpackPrefabInstance(room,PrefabUnpackMode.Completely,InteractionMode.AutomatedAction);
                room.name = "Area " + (i+1) + " " + data.areas[i].title; room.transform.SetParent(world,false); room.transform.position = Vector3.right * center;
                // Side curbs/lines belonged to a closed test arena; remove them along the new walkable route.
                foreach (var child in room.GetComponentsInChildren<Transform>(true).Where(t => t.name == "Low curb" || t.name == "Side working-area line").ToArray())
                    Object.DestroyImmediate(child.gameObject);
                var sign = room.GetComponentsInChildren<TextMesh>().FirstOrDefault();
                if(sign != null) sign.text = (i+1) + " / " + data.areas[i].title;
                if(i == 1)
                {
                    foreach(var t in room.GetComponentsInChildren<Transform>().Where(t=>t.name=="Old teal container"||t.name=="Rust container"))
                        t.localRotation = Quaternion.Euler(0,0,0);
                    for(int n=0;n<3;n++) Box(room.transform,"Sorting rack",new Vector3(-4+n*3.5f,.25f,3.8f),new Vector3(2.5f,.5f,1.1f),steel,false);
                }
                if(i == 2)
                {
                    Box(room.transform,"Press frame",new Vector3(0,3.2f,5.7f),new Vector3(5,.4f,1.8f),rust,false);
                    foreach(int side in new[]{-1,1})Box(room.transform,"Press column",new Vector3(side*2.3f,1.6f,5.7f),new Vector3(.5f,3.2f,1.8f),steel,false);
                    Box(room.transform,"Press ram",new Vector3(0,2.1f,5.7f),new Vector3(3.6f,.55f,1.5f),paint,false);
                }
                chapter.entries[i] = Gate(world,"Entry "+i,center-7.5f,steel,paint);
                chapter.exits[i] = Gate(world,"Exit "+i,center+7.5f,steel,paint);
            }
            for(int n=0;n<2;n++)
            {
                float x=12+n*24;
                for(int mark=0;mark<5;mark++)Box(world,"Route arrow",new Vector3(x-3+mark*1.5f,.014f,0),new Vector3(.5f,.01f,.12f),paint,false);
                Box(world,"Connector curb",new Vector3(x,.10f,3.4f),new Vector3(10,.2f,.25f),steel,false);
                Sign(world,"WEITER →",new Vector3(x,2,3.7f),paint.color);
            }
            chapter.switchGate = Gate(world,"Switch-operated gate",13,steel,paint);
            chapter.switchGate.SetClosed(true);
            var console=Box(world,"Gate switch",new Vector3(10.5f,.65f,1.35f),new Vector3(.55f,1.3f,.5f),steel,false);
            var lamp=Box(world,"Switch lamp",new Vector3(10.5f,1.4f,1.35f),new Vector3(.4f,.16f,.36f),paint,false);
            chapter.switchPosition=console.transform; chapter.switchLamp=lamp.GetComponent<Renderer>();
            Sign(world,"TOR-SCHALTER",new Vector3(10.5f,2.1f,1.35f),paint.color);
            chapter.checkpointMarker=Box(world,"Checkpoint marker",new Vector3(-6,.012f,0),new Vector3(1.3f,.015f,1.8f),
                AssetDatabase.LoadAssetAtPath<Material>(Combat+"B5_STANDARD.mat"),false).transform;
            player.transform.position=data.start; player.visual.rotation=Quaternion.LookRotation(Vector3.right);
            chapter.cameraRig.Snap();
            var hud=Object.FindAnyObjectByType<LabHud>(); hud.chapter=chapter;
            foreach(var label in hud.GetComponentsInChildren<Text>(true))
            {
                if(label.transform.parent.name=="Header" && label!=hud.stateText)label.text="MORE THAN WOMBAT / SCHROTTHOF-KAPITEL";
                if(label.transform.parent.name=="Controls")label.text="WASD Gehen  CTRL Rennen  SPACE Sprung  J Combo  K Heavy  L Kick  E Stoß  SHIFT Ausweichen  F Schalter  R Checkpoint  BACKSPACE Von vorn";
            }
            hud.stateText.text="1/3 ANLIEFERUNG · Zum gelben Kampffeld →";
            EditorUtility.SetDirty(data); EditorUtility.SetDirty(elite); AssetDatabase.SaveAssets();
            EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
            var buildScenes=EditorBuildSettings.scenes.ToList();
            if(!buildScenes.Any(s=>s.path==ScenePath)){buildScenes.Add(new EditorBuildSettingsScene(ScenePath,true));EditorBuildSettings.scenes=buildScenes.ToArray();}
            return "B6 saved: 3 connected areas, 9 waves / 23 opponents, switch gate, checkpoints, elite finale; HumanoidCombatLab retained.";
        }
        static ChapterArea Area(string name,float center,params ChapterWave[] waves)=>new ChapterArea{title=name,center=center,waves=waves};
        static ChapterWave Wave(params EnemyRoleDefinition[] roles)
        {
            var positions=new[]{new Vector3(2.7f,0,-.45f),new Vector3(-2,0,1.35f),new Vector3(4.6f,0,1.1f),new Vector3(-3.7f,0,-1.2f)};
            return new ChapterWave{enemies=roles.Select((role,i)=>new ChapterEnemySpawn{role=role,position=positions[i]}).ToArray()};
        }
        static Material Material(string name)=>AssetDatabase.LoadAssetAtPath<Material>(Materials+name+".mat");
        static GameObject Box(Transform parent,string name,Vector3 position,Vector3 scale,Material material,bool collision)
        {
            var obj=GameObject.CreatePrimitive(PrimitiveType.Cube);obj.name=name;obj.transform.SetParent(parent,false);obj.transform.localPosition=position;obj.transform.localScale=scale;
            obj.GetComponent<Renderer>().sharedMaterial=material;if(!collision)Object.DestroyImmediate(obj.GetComponent<Collider>());return obj;
        }
        static ChapterGate Gate(Transform world,string name,float x,Material steel,Material paint)
        {
            var root=new GameObject(name).transform;root.SetParent(world,false);root.position=Vector3.right*x;
            foreach(int side in new[]{-1,1})Box(root,"Gate post",new Vector3(0,1.25f,side*3.15f),new Vector3(.32f,2.5f,.32f),steel,false);
            var panel=new GameObject("Closed gate").transform;panel.SetParent(root,false);
            Box(panel,"Fence collision",new Vector3(0,1.25f,0),new Vector3(.16f,2.5f,6.1f),Material("Fence mesh"),true);
            Box(panel,"Safety bar",new Vector3(0,1.35f,0),new Vector3(.19f,.14f,6.1f),paint,false);
            var gate=root.gameObject.AddComponent<ChapterGate>();gate.panel=panel.gameObject;gate.SetClosed(false);return gate;
        }
        static void Sign(Transform parent,string text,Vector3 position,Color color)
        {
            var label=new GameObject(text).AddComponent<TextMesh>();label.transform.SetParent(parent,false);label.transform.localPosition=position;
            label.text=text;label.fontSize=48;label.characterSize=.06f;label.anchor=TextAnchor.MiddleCenter;label.color=color;
        }
    }
}
