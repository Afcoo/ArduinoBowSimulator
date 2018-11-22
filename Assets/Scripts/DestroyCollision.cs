using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyCollision : MonoBehaviour {
   
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("!");
        if (other.gameObject.tag == "Target") Destroy(other.gameObject);

        Destroy(this.transform.parent.gameObject);
    }
}
