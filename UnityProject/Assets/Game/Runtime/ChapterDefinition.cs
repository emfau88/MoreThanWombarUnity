using System;
using UnityEngine;

namespace WombatLab
{
    [Serializable] public class ChapterEnemySpawn
    {
        public EnemyRoleDefinition role;
        public Vector3 position;
    }
    [Serializable] public class ChapterWave
    {
        public string title;
        public float triggerOffset;
        public ChapterEnemySpawn[] enemies;
    }
    [Serializable] public class ChapterArea
    {
        public string title;
        public float center;
        public ChapterWave[] waves;
    }
    [CreateAssetMenu(menuName = "Wombat Lab/Chapter")]
    public sealed class ChapterDefinition : ScriptableObject
    {
        public Vector3 start = new Vector3(-6, .05f, 0);
        public Vector2 minimum = new Vector2(-7, -2.35f), maximum = new Vector2(55, 2.35f);
        public float waveBreak = 2.2f;
        public int areaHeal = 30;
        public float areaEnergy = 30;
        public int activeLimit = 8, throwerLimit = 2;
        public float spawnWarningSeconds = 1.1f, spawnInterval = .35f;
        public ChapterArea[] areas;
    }
}
