using System;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "WeaponDB", menuName = "ScriptableObjects/WeaponDB", order = 1)]
public class WeaponDB:ScriptableObject
{
    public WeaponData[] weaponDatas;
    public GameObject[] childPrefabs;
}

[Serializable]
public class WeaponData
{
    public string weaponName;
    public GameObject prefab;
    public Sprite icon;
    public int childPrefabIndex = -1;
    [Header("追加項目、基本いらない")]
    public bool enableThisElements = false;
    public float damageMultiply = 1;
    public float range;
    public float attackCT = 1;
    public Vector2 originOffset;
    public float attackSpeed;
    public float timer = 0;
}