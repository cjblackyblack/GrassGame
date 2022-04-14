using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "GunData")]
public class GunData : ScriptableObject
{
	public int MaxTime;
	public BulletData[] Bullets;
}

[System.Serializable]
public class BulletData 
{
	public int Frame;
	public GameObject Bullet;
	public Vector3 Direction;
}