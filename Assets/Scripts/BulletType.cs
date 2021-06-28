using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Bullet", menuName = "Level/Bullet")]
public class BulletType : ScriptableObject{    
    public TrajectoryType trajectory;

    public Sprite sprite;
    public Vector2 size;
    public float speed;
    public float lifetime;
    public int damage;

    public bool homing;
}

public enum TrajectoryType{
    Linear,
    Circular,
    Homing
}