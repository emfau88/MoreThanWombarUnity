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
    // One bounded dressing pass on the saved combat scene; does not rebuild gameplay.
    public static class JunkyardPreviewBuilder
    {
        const string Root = "Assets/Game/Environment/JunkyardPreview/";
        const string Car = "Assets/ThirdParty/Kenney/CarKit/";
        const string Racing = "Assets/ThirdParty/Kenney/RacingKit/";
        const string Concrete = "Assets/ThirdParty/PolyHaven/ConcreteFloor/";
        [MenuItem("Wombat Lab/M1 Dress Junkyard Preview")]
        public static string Build()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling || EditorUtility.scriptCompilationFailed)
                throw new InvalidOperationException("Stop Play and finish compilation first.");
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (scene.path != HumanoidCombatBuilder.ScenePath || scene.isDirty)
                throw new InvalidOperationException("Load the saved HumanoidCombatLab first.");
            Directory.CreateDirectory(Root + "Materials"); AssetDatabase.Refresh();
            ConfigureTexture(Concrete + "concrete_floor_diff_1k.jpg", false);
            ConfigureTexture(Concrete + "concrete_floor_nor_gl_1k.jpg", true);
            ConfigureTexture(Racing + "Textures/net.png", false);
            var floor = Mat("Concrete", new Color(.62f,.72f,.79f));
            floor.SetTexture("_BaseMap", AssetDatabase.LoadAssetAtPath<Texture2D>(Concrete + "concrete_floor_diff_1k.jpg"));
            floor.SetTextureScale("_BaseMap", new Vector2(8,6));
            floor.SetTexture("_BumpMap", AssetDatabase.LoadAssetAtPath<Texture2D>(Concrete + "concrete_floor_nor_gl_1k.jpg"));
            floor.SetTextureScale("_BumpMap", new Vector2(8,6)); floor.SetFloat("_BumpScale",.13f); floor.EnableKeyword("_NORMALMAP");
            var steel = Mat("Weathered steel",new Color(.18f,.23f,.24f));
            var rust = Mat("Rust red",new Color(.43f,.20f,.12f));
            var teal = Mat("Faded teal",new Color(.17f,.34f,.34f));
            var sand = Mat("Workshop plaster",new Color(.52f,.47f,.37f));
            var paint = Mat("Worn yellow paint",new Color(.69f,.57f,.34f));
            var rubber = Mat("Rubber",new Color(.12f,.13f,.14f));
            var carPalette = Mat("Muted car palette",new Color(.68f,.64f,.58f));
            carPalette.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>(Car+"Textures/colormap.png"));
            var net = Mat("Fence mesh",new Color(.27f,.32f,.30f));
            net.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>(Racing+"Textures/net.png"));
            net.SetFloat("_AlphaClip",1); net.SetFloat("_Cutoff",.45f); net.EnableKeyword("_ALPHATEST_ON");
            net.SetFloat("_Cull",0); net.renderQueue=(int)RenderQueue.AlphaTest;
            var glass=Mat("Workshop windows",new Color(.14f,.24f,.27f));
            var glow=Mat("Warm work lamps",new Color(.95f,.68f,.32f));
            glow.EnableKeyword("_EMISSION");glow.SetColor("_EmissionColor",new Color(1.5f,.65f,.18f));

            var old = GameObject.Find("M1 — Junkyard environment"); if(old!=null)Object.DestroyImmediate(old);
            var root = new GameObject("M1 — Junkyard environment").transform;
            var arena = scene.GetRootGameObjects().First(g=>g.name.StartsWith("Arena"));
            // Keep the original ground/boundary collision; only replace its presentation.
            foreach(var r in arena.GetComponentsInChildren<Renderer>())r.enabled=false;
            Quad(root,"Continuous worn concrete",new Vector3(0,.003f,2.2f),new Vector2(26,20),floor);
            // Sparse painted working-area edges coincide with the existing motor limits.
            var definition=Object.FindAnyObjectByType<PlayerMotor>().definition;
            float clearance=Object.FindAnyObjectByType<PlayerMotor>().GetComponent<CharacterController>().radius+.02f;
            foreach(int side in new[]{-1,1})
            {
                float z=side<0?definition.arenaMin.y-clearance:definition.arenaMax.y+clearance;
                float x=side<0?definition.arenaMin.x-clearance:definition.arenaMax.x+clearance;
                for(int i=0;i<4;i++) Box(root,"Faded loading bay line",new Vector3(-4.95f+i*3.3f,.013f,z),new Vector3(2.3f,.007f,.075f),paint);
                Box(root,"Side working-area line",new Vector3(x,.013f,0),new Vector3(.075f,.007f,5.6f),paint);
                Box(root,"Low curb",new Vector3(side*7.65f,.09f,0),new Vector3(.28f,.18f,6.6f),steel);
            }
            Box(root,"Rear curb",new Vector3(0,.09f,3.4f),new Vector3(15.4f,.18f,.28f),steel);
            // Reuse the original container forms, with local materials and a looser arrangement.
            var containers=arena.transform.Cast<Transform>().Where(t=>t.name=="Graybox container").ToArray();
            for(int i=0;i<containers.Length;i++)
            {
                var container=Object.Instantiate(containers[i].gameObject,root).transform;
                container.name=i==0?"Old teal container":"Rust container";
                container.position=i==0?new Vector3(-4.7f,1.1f,5.3f):new Vector3(3.0f,1.1f,6.0f);
                container.rotation=Quaternion.Euler(0,i==0?-7:5,0);
                foreach(var r in container.GetComponentsInChildren<Renderer>())
                { r.enabled=true;r.sharedMaterial=r.name=="Shell"?(i==0?teal:rust):r.name=="Signal strip"?paint:steel; }
            }
            for(int i=0;i<4;i++) Prop(root,Racing+"fenceStraight.fbx",new Vector3(-9+i*6,0,8.0f),new Vector3(.6f,.44f,.6f),Vector3.zero,null,steel,glass,net);
            Prop(root,Racing+"pitsOffice.fbx",new Vector3(8.5f,0,10.5f),new Vector3(.45f,.42f,.45f),new Vector3(0,-8,0),null,sand,glass,net);
            Prop(root,Racing+"pitsGarageClosed.fbx",new Vector3(-5.8f,0,11.4f),new Vector3(.7f,.48f,.5f),Vector3.zero,null,teal,glass,net);
            // Three clusters: salvage left, sorting racks right, low loose parts at the rear.
            Prop(root,Car+"sedan.fbx",new Vector3(-8.6f,0,4.6f),Vector3.one*1.15f,new Vector3(0,32,0),carPalette,steel,glass,net);
            for(int i=0;i<5;i++) Prop(root,Car+"debris-tire.fbx",new Vector3(-7.3f+(i%2)*.7f,(i/2)*.3f,3.4f),Vector3.one*1.3f,new Vector3(0,0,90),rubber,steel,glass,net);
            Prop(root,Car+"debris-door.fbx",new Vector3(-6.7f,.1f,4.3f),Vector3.one*1.7f,new Vector3(15,35,12),carPalette,steel,glass,net);
            for(int i=0;i<3;i++)
            {
                Prop(root,Car+"debris-plate-a.fbx",new Vector3(7.1f+(i%2)*.45f,i*.28f,4.2f),Vector3.one*1.8f,new Vector3(10,30+i*18,0),carPalette,steel,glass,net);
                Prop(root,Car+"debris-tire.fbx",new Vector3(7.4f,i*.4f,2.8f),Vector3.one*1.3f,new Vector3(0,0,90),rubber,steel,glass,net);
            }
            Prop(root,Car+"debris-drivetrain.fbx",new Vector3(5.7f,0,4.0f),Vector3.one*1.9f,new Vector3(0,25,0),steel,steel,glass,net);
            Prop(root,Car+"debris-bumper.fbx",new Vector3(1.1f,0,3.9f),Vector3.one*1.8f,new Vector3(0,-15,0),carPalette,steel,glass,net);
            // Simple sign dressing uses existing Unity text, not a custom UI system.
            Box(root,"Salvage yard sign",new Vector3(-.5f,2.45f,7.9f),new Vector3(4.2f,.65f,.12f),steel);
            var sign=new GameObject("SCHROTT & SOEHNE").AddComponent<TextMesh>();sign.transform.SetParent(root,false);
            sign.transform.position=new Vector3(-.5f,2.45f,7.82f);sign.text="SCHROTT & SÖHNE";sign.fontSize=64;sign.characterSize=.1f;
            sign.anchor=TextAnchor.MiddleCenter;sign.alignment=TextAlignment.Center;sign.color=new Color(.86f,.74f,.48f);
            Prop(root,Racing+"lightPostLarge.fbx",new Vector3(6.6f,0,7.0f),Vector3.one*.6f,new Vector3(0,180,0),null,steel,glass,net);
            Box(root,"Work lamp lens",new Vector3(6.6f,3.15f,6.8f),new Vector3(.55f,.12f,.28f),glow);
            var lamp=new GameObject("Warm yard work light").AddComponent<Light>();lamp.transform.SetParent(root,false);
            lamp.transform.position=new Vector3(6.6f,3.1f,6.4f);lamp.type=LightType.Point;lamp.color=new Color(1,.66f,.32f);lamp.intensity=3;lamp.range=6;lamp.shadows=LightShadows.None;

            RenderSettings.ambientMode=AmbientMode.Trilight;
            RenderSettings.ambientSkyColor=new Color(.39f,.48f,.53f);RenderSettings.ambientEquatorColor=new Color(.32f,.35f,.34f);RenderSettings.ambientGroundColor=new Color(.22f,.18f,.14f);
            RenderSettings.fog=true;RenderSettings.fogMode=FogMode.Linear;RenderSettings.fogColor=new Color(.28f,.36f,.39f);RenderSettings.fogStartDistance=24;RenderSettings.fogEndDistance=58;
            var key=GameObject.Find("Warm key light").GetComponent<Light>();key.color=new Color(1,.89f,.76f);key.intensity=1.35f;key.transform.rotation=Quaternion.Euler(46,-38,0);key.shadows=LightShadows.Soft;
            var fill=GameObject.Find("Cool fill light").GetComponent<Light>();fill.color=new Color(.56f,.73f,.87f);fill.intensity=.42f;fill.shadows=LightShadows.None;
            var cam=Camera.main;cam.backgroundColor=RenderSettings.fogColor;cam.fieldOfView=43;
            cam.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing=false;
            cam.transform.position=new Vector3(0,8.3f,-12);cam.transform.LookAt(new Vector3(0,.7f,0));
            // Save reusable dressing; original arena collider objects remain scene-owned.
            PrefabUtility.SaveAsPrefabAssetAndConnect(root.gameObject,Root+"JunkyardEnvironment.prefab",InteractionMode.AutomatedAction);
            foreach(var mat in AssetDatabase.FindAssets("t:Material",new[]{Root}))EditorUtility.SetDirty(AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(mat)));
            EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
            return "M1 saved: continuous concrete, reused containers, imported salvage/fence/workshop, warm/cool light. Gameplay retained.";
        }
        static void ConfigureTexture(string path,bool normal)
        {
            var importer=(TextureImporter)AssetImporter.GetAtPath(path);if(importer==null)throw new InvalidOperationException("Missing "+path);
            bool dirty=importer.maxTextureSize!=1024||importer.textureType!=(normal?TextureImporterType.NormalMap:TextureImporterType.Default);
            importer.maxTextureSize=1024;importer.textureType=normal?TextureImporterType.NormalMap:TextureImporterType.Default;
            importer.wrapMode=TextureWrapMode.Repeat;if(dirty)importer.SaveAndReimport();
        }
        static Material Mat(string name,Color color)
        {
            string path=Root+"Materials/"+name+".mat";var mat=AssetDatabase.LoadAssetAtPath<Material>(path);
            if(mat==null){mat=new Material(Shader.Find("Universal Render Pipeline/Lit"));AssetDatabase.CreateAsset(mat,path);}
            mat.SetColor("_BaseColor",color);mat.SetFloat("_Smoothness",.08f);return mat;
        }
        static Transform Box(Transform parent,string name,Vector3 p,Vector3 scale,Material mat)
        {
            var go=GameObject.CreatePrimitive(PrimitiveType.Cube);go.name=name;go.transform.SetParent(parent,false);go.transform.localPosition=p;go.transform.localScale=scale;
            go.GetComponent<Renderer>().sharedMaterial=mat;Object.DestroyImmediate(go.GetComponent<Collider>());return go.transform;
        }
        static void Quad(Transform parent,string name,Vector3 p,Vector2 size,Material mat)
        {
            var go=GameObject.CreatePrimitive(PrimitiveType.Quad);go.name=name;go.transform.SetParent(parent,false);go.transform.localPosition=p;
            go.transform.localRotation=Quaternion.Euler(90,0,0);go.transform.localScale=new Vector3(size.x,size.y,1);go.GetComponent<Renderer>().sharedMaterial=mat;Object.DestroyImmediate(go.GetComponent<Collider>());
        }
        static Transform Prop(Transform parent,string path,Vector3 p,Vector3 scale,Vector3 angles,Material all,Material solid,Material glass,Material net)
        {
            var source=AssetDatabase.LoadAssetAtPath<GameObject>(path);if(source==null)throw new InvalidOperationException("Missing "+path);
            var go=(GameObject)PrefabUtility.InstantiatePrefab(source);go.transform.SetParent(parent,false);go.transform.localScale=scale;go.transform.localRotation=Quaternion.Euler(angles);
            go.transform.localPosition=p;foreach(var collider in go.GetComponentsInChildren<Collider>())Object.DestroyImmediate(collider);
            foreach(var r in go.GetComponentsInChildren<Renderer>())r.sharedMaterials=r.sharedMaterials.Select(m=>all!=null?all:m.name=="net"?net:m.name=="glass"?glass:solid).ToArray();
            // Imported pivots differ; place the lowest rendered point at the requested ground height.
            var renderers=go.GetComponentsInChildren<Renderer>();float min=renderers.Min(r=>r.bounds.min.y);go.transform.position+=Vector3.up*(p.y-min);
            return go.transform;
        }
    }
}
