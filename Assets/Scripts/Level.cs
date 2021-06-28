using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Bullet Pattern", menuName = "Level/Level pattern")]
public class Level : ScriptableObject{
    [System.Serializable]
    public class PatternTimer{
        public Pattern pattern;
        public float delay; // Wait time before starting next pattern
    }

    public PatternTimer[] patternTimers;
    public Vector2[] exitLocations;
}
