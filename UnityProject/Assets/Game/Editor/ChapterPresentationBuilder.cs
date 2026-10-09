using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Object = UnityEngine.Object;

namespace WombatLab.Editor
{
    public static class ChapterPresentationBuilder
    {
        const string Root = "Assets/Game/Environment/JunkyardChapter/";
        const string Preview = "Assets/Game/Environment/JunkyardPreview/Materials/";
        const string Car = "Assets/ThirdParty/Kenney/CarKit/";
        const string Impact = "Assets/ThirdParty/Kenney/ImpactSounds/";
        const string Interface = "Assets/ThirdParty/Kenney/InterfaceSounds/";
        [MenuItem("Wombat Lab/B7 Dress Chapter and Audio")]
        public static string Apply()
        {
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (EditorApplication.isPlaying || EditorApplication.isCompiling || EditorUtility.scriptCompilationFailed
                || scene.isDirty || scene.path != JunkyardChapterBuilder.ScenePath)
                throw new InvalidOperationException("Open the saved JunkyardChapter in idle Edit Mode.");
            Directory.CreateDirectory(Root + "Materials"); AssetDatabase.Refresh();
            var textureImporter=(TextureImporter)AssetImporter.GetAtPath(Root+"Textures/ConcretePainted.png");
            textureImporter.maxTextureSize=1024;textureImporter.wrapMode=TextureWrapMode.Repeat;textureImporter.mipmapEnabled=true;textureImporter.SaveAndReimport();
            foreach (string id in AssetDatabase.FindAssets("t:AudioClip", new[] {Impact.TrimEnd('/'), Interface.TrimEnd('/')}))
            {
                var importer = (AudioImporter)AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(id));
                importer.forceToMono = true;
                var settings = importer.defaultSampleSettings; settings.preloadAudioData = true; settings.loadType = AudioClipLoadType.DecompressOnLoad;
                settings.compressionFormat = AudioCompressionFormat.ADPCM; settings.sampleRateSetting = AudioSampleRateSetting.OverrideSampleRate;
                settings.sampleRateOverride = 22050; importer.defaultSampleSettings = settings; importer.SaveAndReimport();
            }
            var chapter = Object.FindAnyObjectByType<JunkyardChapter>();
            var world = GameObject.Find("B6 connected junkyard").transform;
            var old = world.Find("B7 presentation dressing"); if (old != null) Object.DestroyImmediate(old.gameObject);
            var dressing = new GameObject("B7 presentation dressing").transform; dressing.SetParent(world, false);
            var steel = Derived("Weathered steel", new Color(.13f,.21f,.25f));
            var teal = Derived("Faded teal", new Color(.12f,.40f,.43f));
            var rust = Derived("Rust red", new Color(.56f,.22f,.12f));
            var cream = Derived("Workshop plaster", new Color(.74f,.65f,.46f));
            var paint = Derived("Worn yellow paint", new Color(.9f,.64f,.24f));
            var rubber = Derived("Rubber", new Color(.07f,.10f,.13f));
            var glass = Derived("Workshop windows", new Color(.12f,.28f,.35f));
            var fence = Derived("Fence mesh", new Color(.16f,.24f,.26f));
            var palette = Derived("Muted car palette", new Color(.65f,.72f,.73f));
            var concrete = Derived("Concrete", new Color(.66f,.73f,.77f));
            concrete.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>(Root+"Textures/ConcretePainted.png"));
            concrete.SetTextureScale("_BaseMap", new Vector2(18,6)); concrete.SetTexture("_BumpMap",null);concrete.DisableKeyword("_NORMALMAP");
            var glow = Derived("Warm work lamps", new Color(1,.78f,.4f));
            glow.SetColor("_EmissionColor", new Color(2,.95f,.25f));
            var lines = VertexMaterial();
            var checkpoint = Flat("Checkpoint paint", new Color(.17f,.44f,.42f));
            var fur=Flat("Bear warm fur",new Color(.38f,.27f,.20f));
            var belly=Flat("Bear belly",new Color(.58f,.45f,.32f));
            var snout=Flat("Bear snout",new Color(.63f,.52f,.39f));
            var eyes=Flat("Bear eyes",new Color(.94f,.89f,.76f));
            var dark=Flat("Bear nose brows pupils",new Color(.035f,.045f,.055f));
            var ear=Flat("Bear inner ears",new Color(.53f,.31f,.28f));
            var replacements = new System.Collections.Generic.Dictionary<string,Material> {
                {"Weathered steel",steel},{"Faded teal",teal},{"Rust red",rust},{"Workshop plaster",cream},
                {"Worn yellow paint",paint},{"Rubber",rubber},{"Workshop windows",glass},{"Fence mesh",fence},
                {"Muted car palette",palette},{"Warm work lamps",glow},{"Concrete",concrete}
            };
            foreach (var renderer in world.GetComponentsInChildren<Renderer>(true))
                renderer.sharedMaterials = renderer.sharedMaterials.Select(m => m != null && replacements.TryGetValue(m.name,out var derived) ? derived : m).ToArray();

            // One continuous surface removes seams between the copied room ground quads.
            foreach (var floor in world.GetComponentsInChildren<Transform>().Where(t=>t.name=="Continuous worn concrete").ToArray())
                Object.DestroyImmediate(floor.gameObject);
            Quad(dressing,"Continuous chapter concrete",new Vector3(24,.003f,8),new Vector2(112,40),concrete);
            var distant=Flat("Distant blue steel",new Color(.21f,.33f,.38f));
            for(int n=0;n<7;n++)
            {
                var source=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ThirdParty/Kenney/RacingKit/pitsGarageClosed.fbx");
                var warehouse=(GameObject)PrefabUtility.InstantiatePrefab(source);warehouse.name="Distant warehouse "+n;warehouse.transform.SetParent(dressing,false);
                warehouse.transform.localPosition=new Vector3(-12+n*12,0,17+n%2*2);warehouse.transform.localScale=new Vector3(.75f,.5f+(n%3)*.09f,.62f);
                foreach(var r in warehouse.GetComponentsInChildren<Renderer>())r.sharedMaterials=r.sharedMaterials.Select(m=>distant).ToArray();
                foreach(var c in warehouse.GetComponentsInChildren<Collider>())Object.DestroyImmediate(c);
            }
            for (int i = 0; i < 3; i++)
            {
                var room = world.Find("Area " + (i+1) + " " + chapter.definition.areas[i].title);
                float center = chapter.definition.areas[i].center;
                var sign = room.GetComponentsInChildren<TextMesh>().First();
                sign.text = (i+1)+" / "+chapter.definition.areas[i].title; sign.fontSize=64; sign.characterSize=.063f;
                sign.color = new Color(1,.86f,.59f);
                room.Find("Salvage yard sign").localScale = new Vector3(6.1f,.76f,.15f);
                // The first room remains a loading dock, the other two get distinct silhouettes.
                if (i == 1)
                {
                    Place(room,"Old teal container",new Vector3(-7.8f,1.1f,6.6f),.7f);
                    Place(room,"Rust container",new Vector3(8.4f,1.1f,7.3f),.6f);
                    foreach (var rack in room.GetComponentsInChildren<Transform>().Where(t=>t.name=="Sorting rack").ToArray()) Object.DestroyImmediate(rack.gameObject);
                    for (int n=0;n<3;n++)
                    {
                        float x=center-4.6f+n*4;
                        for (int side=-1;side<=1;side+=2) Box(dressing,"Rack upright",new Vector3(x+side*1.4f,1.25f,5.6f),new Vector3(.13f,2.5f,.9f),teal);
                        for (int shelf=0;shelf<2;shelf++)
                        {
                            Box(dressing,"Sorting shelf",new Vector3(x,.5f+shelf*1.0f,5.6f),new Vector3(3,.16f,1.25f),steel);
                            Prop(dressing,"debris-plate-a",new Vector3(x-.6f,.58f+shelf,5.6f),1.0f,new Vector3(0,n*34,0),shelf==0?rust:cream);
                            Prop(dressing,"debris-tire",new Vector3(x+.65f,.58f+shelf,5.6f),1.05f,new Vector3(0,0,90),rubber);
                        }
                        Sign(dressing,new[]{"BLECH","REIFEN","RESTE"}[n],new Vector3(x,2.55f,5.45f),.038f,new Color(.73f,.88f,.88f));
                    }
                }
                if (i == 2)
                {
                    Place(room,"Old teal container",new Vector3(-7.6f,1.1f,6.2f),.65f);
                    Place(room,"Rust container",new Vector3(7.6f,1.1f,6.6f),.65f);
                    Box(dressing,"Press foundation",new Vector3(center,.12f,5.7f),new Vector3(5.4f,.24f,2.7f),steel);
                    foreach (int side in new[]{-1,1})
                    {
                        Cylinder(dressing,"Hydraulic sleeve",new Vector3(center+side*1.75f,2.5f,5.8f),new Vector3(.36f,.9f,.36f),steel);
                        Cylinder(dressing,"Hydraulic piston",new Vector3(center+side*1.75f,1.75f,5.8f),new Vector3(.17f,.5f,.17f),cream);
                        Box(dressing,"Press warning trim",new Vector3(center+side*2.3f,1.2f,4.76f),new Vector3(.32f,2.2f,.025f),paint);
                    }
                    Sign(dressing,"HÄNDE WEG",new Vector3(center,.95f,4.65f),.05f,new Color(1,.68f,.3f));
                }
                // Keep low dressing behind the usable z lane rather than against fighters.
                foreach (var tire in room.GetComponentsInChildren<Transform>().Where(t=>t.name=="debris-tire" && t.localPosition.z<3.5f))
                { var p=tire.localPosition;p.z=3.75f;tire.localPosition=p; }
                var roomLamp = room.GetComponentsInChildren<Light>().FirstOrDefault();
                if (roomLamp != null) { roomLamp.range=8;roomLamp.intensity=3.5f;roomLamp.color=i==1?new Color(.5f,.8f,1):new Color(1,.65f,.3f); }
                var pool=new GameObject("Area light "+(i+1)).AddComponent<Light>();pool.transform.SetParent(dressing,false);
                pool.transform.position=new Vector3(center,4.3f,1.3f);pool.type=LightType.Point;pool.range=9;pool.intensity=4;
                pool.color=i==1?new Color(.62f,.84f,1):new Color(1,.79f,.48f);pool.shadows=LightShadows.None;
                // Faded hazard bands sit outside the fight lane.
                for(int n=0;n<8;n++)
                {var stripe=Box(dressing,"Hazard band",new Vector3(center-5.7f+n*1.6f,.012f,3.04f),new Vector3(.62f,.009f,.13f),paint);stripe.localRotation=Quaternion.Euler(0,28,0);}
            }
            var player = chapter.encounter.player;
            chapter.cameraRig.offset = new Vector3(0,9.2f,-12.8f); chapter.cameraRig.follow=5;
            var cam=Camera.main;cam.fieldOfView=42;cam.allowHDR=true;cam.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing=true;
            RenderSettings.ambientSkyColor=new Color(.35f,.46f,.56f);RenderSettings.ambientEquatorColor=new Color(.32f,.39f,.43f);
            RenderSettings.ambientGroundColor=new Color(.20f,.22f,.25f);RenderSettings.fogColor=new Color(.22f,.34f,.42f);
            RenderSettings.fogStartDistance=29;RenderSettings.fogEndDistance=65;cam.backgroundColor=RenderSettings.fogColor;
            var key=GameObject.Find("Warm key light").GetComponent<Light>();key.color=new Color(1,.88f,.72f);key.intensity=1.7f;
            key.transform.rotation=Quaternion.Euler(52,-32,0);key.shadowStrength=.75f;
            var fill=GameObject.Find("Cool fill light").GetComponent<Light>();fill.intensity=.55f;fill.color=new Color(.46f,.73f,1);
            ConfigureVolume(dressing);
            var presentation=chapter.GetComponent<ChapterPresentation>();if(presentation==null)presentation=chapter.gameObject.AddComponent<ChapterPresentation>();
            presentation.chapter=chapter;presentation.markerMaterial=lines;
            presentation.footsteps=new[]{Audio(Impact,"footstep_concrete_000"),Audio(Impact,"footstep_concrete_001"),Audio(Impact,"footstep_concrete_002")};
            presentation.switchClick=Audio(Interface,"switch_001");presentation.metalGate=Audio(Impact,"impactMetal_medium_000");
            presentation.waveCue=Audio(Interface,"select_001");presentation.areaCue=Audio(Interface,"confirmation_002");
            presentation.completeCue=Audio(Impact,"impactBell_heavy_000");presentation.machinery=Audio(Impact,"impactPlate_heavy_000");
            presentation.pressRam=world.Find("Area 3 PRESSWERK/Press ram");
            presentation.gates=chapter.entries.Concat(chapter.exits).Concat(new[]{chapter.switchGate}).ToArray();
            presentation.gateLamps=presentation.gates.Select(g=>{
                // Front gate posts are deliberately shorter to reduce foreground occlusion.
                foreach(var post in g.GetComponentsInChildren<Transform>(true).Where(t=>t.name=="Gate post" && t.localPosition.z<0))
                {post.localPosition=new Vector3(0,.55f,-3.15f);post.localScale=new Vector3(.22f,1.1f,.22f);}
                var lamp=Box(dressing,"Gate status light",g.transform.position+new Vector3(0,1.17f,-3.15f),new Vector3(.25f,.1f,.23f),glow);
                return lamp.GetComponent<Renderer>();
            }).ToArray();
            chapter.checkpointMarker.GetComponent<Renderer>().sharedMaterial=checkpoint;
            var feedback=player.GetComponent<CombatController>().feedback;feedback.lightContact=Audio(Impact,"impactPunch_medium_000");feedback.heavyContact=Audio(Impact,"impactPunch_heavy_000");
            foreach(var enemy in chapter.encounter.chapterTemplates)
            {
                foreach(var r in enemy.visual.GetComponentsInChildren<Renderer>(true))
                    if(r.name!="Fist")r.sharedMaterial=r.name=="Belly"?belly:r.name=="Muzzle"?snout:r.name=="Eye"?eyes:
                        r.name=="Pupil"||r.name=="Nose"||r.name=="Brow"?dark:r.name=="Inner ear"?ear:fur;
                var label=enemy.roleLabel.GetComponent<TextMesh>();label.characterSize=.045f;label.fontSize=64;
                var cues=enemy.GetComponent<EnemyPresentation>();if(cues==null)cues=enemy.gameObject.AddComponent<EnemyPresentation>();
                cues.enemy=enemy;cues.barMaterial=lines;cues.standardWarning=Audio(Interface,"question_001");
                cues.agileWarning=Audio(Interface,"select_001");cues.heavyWarning=Audio(Impact,"impactMetal_medium_000");cues.knockdown=Audio(Impact,"impactPunch_heavy_000");
            }
            chapter.cameraRig.Snap();
            foreach(var mat in replacements.Values)EditorUtility.SetDirty(mat);
            AssetDatabase.SaveAssets();EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);
            return "B7 saved: distinct loading/sorting/press areas, derived materials/light/grade, clearer camera/markers, 12 CC0 cues; gameplay data and lab retained.";
        }
        static AudioClip Audio(string root,string name)
        {var clip=AssetDatabase.LoadAssetAtPath<AudioClip>(root+name+".ogg");if(clip==null)throw new InvalidOperationException("Missing audio "+name);return clip;}
        static Material Derived(string name,Color color)
        {
            var source=AssetDatabase.LoadAssetAtPath<Material>(Preview+name+".mat");
            var mat=AssetDatabase.LoadAssetAtPath<Material>(Root+"Materials/"+name+".mat");
            if(mat==null){mat=Object.Instantiate(source);AssetDatabase.CreateAsset(mat,Root+"Materials/"+name+".mat");}else EditorUtility.CopySerialized(source,mat);
            mat.name=name;mat.SetColor("_BaseColor",color);mat.SetFloat("_Smoothness",.12f);return mat;
        }
        static Material Flat(string name,Color color)
        {
            var mat=AssetDatabase.LoadAssetAtPath<Material>(Root+"Materials/"+name+".mat");
            if(mat==null){mat=new Material(Shader.Find("Universal Render Pipeline/Lit"));AssetDatabase.CreateAsset(mat,Root+"Materials/"+name+".mat");}
            mat.SetColor("_BaseColor",color);mat.SetFloat("_Smoothness",.05f);EditorUtility.SetDirty(mat);return mat;
        }
        static Material VertexMaterial()
        {
            var mat=AssetDatabase.LoadAssetAtPath<Material>(Root+"Materials/Markers.mat");
            if(mat==null){mat=new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit"));AssetDatabase.CreateAsset(mat,Root+"Materials/Markers.mat");}
            mat.SetColor("_BaseColor",Color.white);mat.SetFloat("_Surface",1);mat.SetFloat("_Blend",0);
            mat.SetFloat("_SrcBlend",(float)BlendMode.SrcAlpha);mat.SetFloat("_DstBlend",(float)BlendMode.OneMinusSrcAlpha);
            mat.SetFloat("_ZWrite",0);mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");mat.renderQueue=(int)RenderQueue.Transparent;EditorUtility.SetDirty(mat);return mat;
        }
        static void ConfigureVolume(Transform parent)
        {
            var profile=AssetDatabase.LoadAssetAtPath<VolumeProfile>(Root+"ChapterGrade.asset");
            if(profile==null){profile=ScriptableObject.CreateInstance<VolumeProfile>();AssetDatabase.CreateAsset(profile,Root+"ChapterGrade.asset");}
            if(!profile.TryGet<ColorAdjustments>(out var grade)){grade=profile.Add<ColorAdjustments>();AssetDatabase.AddObjectToAsset(grade,profile);}
            grade.postExposure.Override(.35f);grade.contrast.Override(12);grade.saturation.Override(8);
            if(!profile.TryGet<Tonemapping>(out var tone)){tone=profile.Add<Tonemapping>();AssetDatabase.AddObjectToAsset(tone,profile);}tone.mode.Override(TonemappingMode.ACES);
            if(!profile.TryGet<Bloom>(out var bloom)){bloom=profile.Add<Bloom>();AssetDatabase.AddObjectToAsset(bloom,profile);}bloom.threshold.Override(1.25f);bloom.intensity.Override(.12f);
            var volume=new GameObject("Chapter colour grade").AddComponent<Volume>();volume.transform.SetParent(parent,false);volume.isGlobal=true;volume.sharedProfile=profile;
            EditorUtility.SetDirty(profile);foreach(var c in profile.components)EditorUtility.SetDirty(c);
        }
        static void Place(Transform room,string name,Vector3 p,float scale)
        {var obj=room.Find(name);obj.localPosition=p;obj.localScale=Vector3.one*scale;obj.localRotation=Quaternion.identity;}
        static Transform Box(Transform parent,string name,Vector3 p,Vector3 scale,Material mat)
        {return Primitive(parent,name,p,scale,mat,PrimitiveType.Cube);}
        static void Cylinder(Transform parent,string name,Vector3 p,Vector3 scale,Material mat)
        {Primitive(parent,name,p,scale,mat,PrimitiveType.Cylinder);}
        static Transform Primitive(Transform parent,string name,Vector3 p,Vector3 scale,Material mat,PrimitiveType type)
        {
            var go=GameObject.CreatePrimitive(type);go.name=name;go.transform.SetParent(parent,false);go.transform.localPosition=p;go.transform.localScale=scale;
            go.GetComponent<Renderer>().sharedMaterial=mat;Object.DestroyImmediate(go.GetComponent<Collider>());return go.transform;
        }
        static void Quad(Transform parent,string name,Vector3 p,Vector2 size,Material mat)
        {var t=Primitive(parent,name,p,new Vector3(size.x,size.y,1),mat,PrimitiveType.Quad);t.localRotation=Quaternion.Euler(90,0,0);}
        static void Sign(Transform parent,string text,Vector3 p,float size,Color color)
        {var label=new GameObject(text).AddComponent<TextMesh>();label.transform.SetParent(parent,false);label.transform.localPosition=p;label.text=text;label.fontSize=64;label.characterSize=size;label.anchor=TextAnchor.MiddleCenter;label.color=color;}
        static void Prop(Transform parent,string name,Vector3 p,float scale,Vector3 angles,Material mat)
        {
            var source=AssetDatabase.LoadAssetAtPath<GameObject>(Car+name+".fbx");var go=(GameObject)PrefabUtility.InstantiatePrefab(source);go.transform.SetParent(parent,false);
            go.transform.localScale=Vector3.one*scale;go.transform.localRotation=Quaternion.Euler(angles);go.transform.localPosition=p;
            foreach(var r in go.GetComponentsInChildren<Renderer>())r.sharedMaterials=r.sharedMaterials.Select(m=>mat).ToArray();
            foreach(var c in go.GetComponentsInChildren<Collider>())Object.DestroyImmediate(c);
            float min=go.GetComponentsInChildren<Renderer>().Min(r=>r.bounds.min.y);go.transform.position+=Vector3.up*(p.y-min);
        }
    }
}
