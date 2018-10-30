using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterController : MonoBehaviour
{
    // Update is called once per frame
    private void Update()
    {
        if (GameManager.hasLost)
        {
            transform.position += Vector3.down * 2 * Time.deltaTime;
        }
    }
}