using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    float count;//カウント

    // Start is called before the first frame update
    void Awake()
    {
        transform.position = new Vector2(0, -1000);
    }

    // Update is called once per frame
    void Update()
    {
        count += Time.deltaTime;
        if(count > 2)
        {
            Destroy(gameObject);
        }
    }
}
