﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour {
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("!");
        Destroy(other.transform.parent.gameObject);
        Destroy(this.gameObject);
    }
}