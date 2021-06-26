using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Bullet", menuName = "Level/Bullet")]
public class BulletType : ScriptableObject{    
    public TrajectoryType trajectory;

    public float[] size;
    public float[] speed;

    public bool homing;
}

public enum TrajectoryType{
    Linear,
    Circular,
    Homing
}