using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWeapon : MonoBehaviour //The door that blocks the tutorial.
{
    [SerializeField] WeaponData weaponData;
    void Start()
    {
        GameObject chosenObject = weaponData.Weapons[Random.Range(0, weaponData.Weapons.Length)];

        GameObject weapon = Instantiate(chosenObject, transform.position, Quaternion.identity);
        Destroy(gameObject); //Destroy the random weapon component associated to the object.
    }
}
