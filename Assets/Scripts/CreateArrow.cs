using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateArrow : MonoBehaviour {

    public GameObject Arrow_Prefab;

    public GameObject CreateNewArrow()
    {
        GameObject arrow = Instantiate(Arrow_Prefab, this.transform.position, this.transform.rotation);
        return arrow.gameObject;
    }
}
