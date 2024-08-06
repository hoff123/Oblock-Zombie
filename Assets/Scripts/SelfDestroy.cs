using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    public float timeForDestruction;
    void Start()
    {
        StartCoroutine(DestroySelf(timeForDestruction));
    }

    private IEnumerator DestroySelf(float timeforDestruction)
    {
        yield return new WaitForSeconds(timeforDestruction);

        Destroy(gameObject);
    }
}
