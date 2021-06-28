using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Single Pattern", menuName = "Level/Bullet pattern")]
public class Pattern : ScriptableObject{
    [System.Serializable]
    public class BulletTimer{
        public Vector2 startLoc;
        public Vector2 direction;
        public float delay; // wait time before firing next bullet
    }

    public BulletType type;
    public Vector2 startLoc;
    public BulletTimer[] bulletTimers;

    // Pattern spawn bullets.
}
