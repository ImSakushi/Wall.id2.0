using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
  void Start()
  {
    // Destroy the game object after the length of the animation
    Destroy(gameObject, GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
  }
}
