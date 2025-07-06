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
      [ContextMenu("Shoot Missile")]
      public void shoot_missile()
      {
         GameObject missile = Instantiate(missile_prefab, spawn_points[0].position, spawn_points[0].rotation);
         missile.GetComponent<homing_missile>().target = target;
         missile.GetComponent<homing_missile>().targetpointer.GetComponent<homing_missile_pointer>().target = target;
         // missile.GetComponent<homing_missile>().shooter = this.gameObject;
         missile.GetComponent<homing_missile>().usemissile();
      }
   }
}