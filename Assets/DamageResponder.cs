using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable{
	public void Damage(int damage);
	public void Heal(int heal);
	public void Die();
}

public enum DamageProperty{
	Player,
	Dice
}