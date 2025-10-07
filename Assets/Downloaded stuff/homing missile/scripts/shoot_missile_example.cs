using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HomingMissile
{
   public class shoot_missile_example : MonoBehaviour
   {
      public GameObject missile_prefab;
      public GameObject target;
      public Transform[] spawn_points;
      // button in the inspector to call this function
      //shoot missile if space is pressed
      public void Update()
      {
         if (Input.GetKeyDown(KeyCode.Space))
         {
            shoot_missile();
         }
      }
      [ContextMenu("Shoot Missile")]
      public void shoot_missile()
      {
         GameObject missile = Instantiate(missile_prefab, spawn_points[0].position, spawn_points[0].rotation);
         var missileScript = missile.GetComponent<homing_missile>();
         var missileTowerDamage = GetComponent<TowerBehaviour>().currentDamage;
         missileScript.target = target;
         missileScript.explosionDamage = missileTowerDamage;
         missileScript.targetpointer.GetComponent<homing_missile_pointer>().target = target;
         missileScript.usemissile();
      }
   }
}