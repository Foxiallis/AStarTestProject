using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Class", menuName = "Character Class")]
public class CharacterClass : ScriptableObject
{
    public int id;
    public float speed;
    public float turnRate;
    public float stamina;
    public Material pawnMaterial;
}
