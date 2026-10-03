"""Own Blender-authored mesh library and editable Wombat character source.

Run with Blender --background --factory-startup --python tools/Build-WombatModel.py.
No downloads or third-party character assets. JSON exports local geometry in Unity
coordinates, avoiding FBX axis/scale ambiguities and preserving native rig clips.
"""
import bpy, math, json
from pathlib import Path
from mathutils import Vector

ROOT = Path(__file__).resolve().parents[1]
SOURCE = ROOT / 'art-source' / 'wombat'
EXPORT = ROOT / 'UnityProject' / 'Assets' / 'Game' / 'Art' / 'Models'
SOURCE.mkdir(parents=True, exist_ok=True)
EXPORT.mkdir(parents=True, exist_ok=True)
bpy.ops.object.select_all(action='SELECT')
bpy.ops.object.delete(use_global=False)

def to_blender(v): return (v[0], -v[2], v[1])
def to_unity(v): return (v[0], v[2], -v[1])
def spow(v, exponent): return math.copysign(abs(v)**exponent, v)

def rounded(name, exponent=.65, pear=0):
    verts, faces = [], []
    rings, sectors = 20, 32
    for j in range(rings+1):
        lat = -math.pi/2 + math.pi*j/rings
        height = spow(math.sin(lat), exponent) * .5
        width = spow(math.cos(lat), exponent) * .5
        profile = 1 - pear * height
        for i in range(sectors):
            lon = 2*math.pi*i/sectors
            verts.append(to_blender((width*spow(math.cos(lon), exponent)*profile,
                                     height, width*spow(math.sin(lon), exponent)*profile)))
    for j in range(rings):
        for i in range(sectors):
            a=j*sectors+i; b=j*sectors+(i+1)%sectors
            faces.append((a,b,b+sectors,a+sectors))
    mesh=bpy.data.meshes.new(name); mesh.from_pydata(verts, [], faces); mesh.update()
    obj=bpy.data.objects.new(name,mesh); bpy.context.collection.objects.link(obj)
    for p in mesh.polygons: p.use_smooth=True
    return obj

library={}
for name, exp, pear in [('Body', .82, .38), ('Head', .63, -.12), ('Belly', .72, .15),
                       ('Muzzle', .48, .05), ('Nose', .38, -.10), ('Ear', .66, 0),
                       ('UpperArm', .80, -.12), ('Forearm', .61, -.20),
                       ('Thigh', .72, .10), ('Shin', .62, -.14), ('Joint', .88, 0),
                       ('Paw', .44, 0), ('Boot', .42, 0), ('Claw', .52, 0),
                       ('Shorts', .51, .12), ('Patch', .32, 0), ('Brow', .33, 0)]:
    library[name]=rounded(name,exp,pear)

def material(name,color,roughness=.65):
    mat=bpy.data.materials.new(name); mat.diffuse_color=(*color,1)
    mat.use_nodes=True; bsdf=mat.node_tree.nodes.get('Principled BSDF')
    bsdf.inputs['Base Color'].default_value=(*color,1); bsdf.inputs['Roughness'].default_value=roughness
    return mat
mats={k: material(k,v) for k,v in {
    'Chestnut':(.33,.17,.08), 'WarmFur':(.50,.28,.13), 'Cream':(.74,.56,.34),
    'Teal':(.045,.31,.29), 'Wrap':(.16,.57,.51), 'WorkNavy':(.07,.12,.15),
    'Ochre':(.88,.47,.09), 'Ink':(.035,.027,.021), 'Ivory':(.94,.86,.66)}.items()}

# Export geometry first: each mesh remains local, unit-size, with a stable name.
export=[]
for name,obj in library.items():
    mesh=obj.data.copy(); mesh.calc_loop_triangles()
    export.append({'name':name,
        'vertices':[c for v in mesh.vertices for c in to_unity(v.co)],
        'normals':[c for v in mesh.vertices for c in to_unity(v.normal)],
        'triangles':[idx for tri in mesh.loop_triangles for idx in tri.vertices]})
    bpy.data.meshes.remove(mesh)
(EXPORT/'WombatMeshLibrary.json').write_text(json.dumps({'meshes':export},separators=(',',':')),encoding='utf-8')

# Assembled reference model in the editable .blend; Unity uses the same library.
parts=[]
def part(name,shape,pos,size,mat):
    source=library[shape]; obj=bpy.data.objects.new(name, source.data.copy())
    bpy.context.collection.objects.link(obj); obj.location=to_blender(pos)
    obj.scale=(size[0],size[2],size[1]); obj.data.materials.append(mats[mat]); parts.append(obj)
    return obj
part('Torso','Body',(0,1.03,0),(1.15,1.34,.87),'Chestnut')
part('Belly','Belly',(0,.93,.34),(.78,.92,.24),'WarmFur')
part('Head','Head',(0,1.73,.035),(1.02,.69,.78),'Chestnut')
part('Muzzle','Muzzle',(0,1.59,.37),(.76,.36,.37),'Cream')
part('Nose','Nose',(0,1.67,.575),(.36,.18,.17),'Ink')
part('Shorts','Shorts',(0,.43,-.01),(1.00,.42,.72),'WorkNavy')
part('Belt','Patch',(0,.58,.35),(.78,.11,.075),'Teal')
part('Buckle','Patch',(0,.58,.405),(.16,.12,.075),'Ochre')
for side in [-1,1]:
    suffix='L' if side<0 else 'R'
    part('Ear_'+suffix,'Ear',(side*.40,2.01,.005),(.28,.31,.16),'Chestnut')
    part('InnerEar_'+suffix,'Ear',(side*.40,2.015,.07),(.16,.20,.045),'Cream')
    part('Eye_'+suffix,'Joint',(side*.255,1.795,.352),(.17,.125,.065),'Ivory')
    part('Pupil_'+suffix,'Joint',(side*.255,1.785,.395),(.072,.095,.035),'Ink')
    brow=part('Brow_'+suffix,'Brow',(side*.25,1.88,.36),(.24,.07,.075),'WarmFur')
    brow.rotation_euler.y=side*math.radians(10)
    part('UpperArm_'+suffix,'UpperArm',(side*.60,1.06,.02),(.37,.46,.37),'Chestnut')
    part('Forearm_'+suffix,'Forearm',(side*.69,.86,.13),(.36,.39,.36),'WarmFur')
    part('Paw_'+suffix,'Paw',(side*.70,.78,.24),(.50,.40,.48),'Teal')
    part('Thigh_'+suffix,'Thigh',(side*.32,.39,-.025),(.39,.38,.38),'Chestnut')
    part('Shin_'+suffix,'Shin',(side*.32,.22,.04),(.33,.30,.34),'WarmFur')
    part('Foot_'+suffix,'Boot',(side*.32,.13,.09),(.49,.25,.56),'Ink')
    for finger in range(3):
        part('PawPad_'+suffix+str(finger),'Patch',(side*.70+(finger-1)*.105,.75,.475),(.09,.22,.055),'Wrap')
        part('Toe_'+suffix+str(finger),'Claw',(side*.32+(finger-1)*.10,.145,.37),(.08,.09,.11),'Cream')

# Explicit editable source skeleton, in the same rest pose and meter units.
arm=bpy.data.armatures.new('Wombat_Articulated_Rig'); rig=bpy.data.objects.new('Wombat_Rig',arm)
bpy.context.collection.objects.link(rig); bpy.context.view_layer.objects.active=rig
rig.select_set(True); bpy.ops.object.mode_set(mode='EDIT')
def bone(name,head,tail,parent=None):
    b=arm.edit_bones.new(name); b.head=to_blender(head); b.tail=to_blender(tail)
    if parent: b.parent=arm.edit_bones[parent]
    return b
bone('Root',(0,0,0),(0,.20,0)); bone('Pelvis',(0,.43,0),(0,.72,0),'Root')
bone('Spine',(0,.72,0),(0,1.25,0),'Pelvis'); bone('Head',(0,1.4,0),(0,1.95,0),'Spine')
for s in [-1,1]:
    n='L' if s<0 else 'R'
    bone('Shoulder_'+n,(s*.50,1.25,0),(s*.68,1.0,0),'Spine')
    bone('Elbow_'+n,(s*.68,1.0,0),(s*.70,.78,.24),'Shoulder_'+n)
    bone('Hand_'+n,(s*.70,.78,.24),(s*.70,.78,.45),'Elbow_'+n)
    bone('Hip_'+n,(s*.30,.60,0),(s*.34,.35,-.02),'Pelvis')
    bone('Knee_'+n,(s*.34,.35,-.02),(s*.32,.13,.09),'Hip_'+n)
    bone('Foot_'+n,(s*.32,.13,.09),(s*.32,.13,.35),'Knee_'+n)
bpy.ops.object.mode_set(mode='OBJECT'); rig.show_in_front=True
for obj in parts:
    # Rigid weights are suitable for deliberately rounded overlapping segments.
    name=obj.name; suffix='L' if '_L' in name else 'R'
    binding='Head' if any(k in name for k in ('Head','Ear','Eye','Pupil','Brow','Muzzle','Nose')) else 'Spine'
    if 'UpperArm' in name: binding='Shoulder_'+suffix
    elif 'Forearm' in name: binding='Elbow_'+suffix
    elif 'Paw' in name: binding='Hand_'+suffix
    elif 'Thigh' in name: binding='Hip_'+suffix
    elif 'Shin' in name: binding='Knee_'+suffix
    elif 'Foot' in name or 'Toe' in name: binding='Foot_'+suffix
    elif any(k in name for k in ('Shorts','Belt','Buckle')): binding='Pelvis'
    group=obj.vertex_groups.new(name=binding); group.add(list(range(len(obj.data.vertices))),1,'REPLACE')
    mod=obj.modifiers.new('Articulated deformation','ARMATURE'); mod.object=rig

# The library is kept in its own hidden collection, so the source opens cleanly.
assets=bpy.data.collections.new('Local mesh library — exported to Unity'); bpy.context.scene.collection.children.link(assets)
for obj in library.values():
    for col in list(obj.users_collection): col.objects.unlink(obj)
    assets.objects.link(obj)
assets.hide_viewport=True; assets.hide_render=True
bpy.context.scene.world.color=(.08,.10,.12)
bpy.ops.wm.save_as_mainfile(filepath=str(SOURCE/'Wombat_B2.blend'))
print('WOMBAT_EXPORT',len(export),'meshes',sum(len(m['vertices'])//3 for m in export),'vertices',str(EXPORT))
