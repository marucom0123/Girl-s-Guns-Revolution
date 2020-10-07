using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    float count = 0;
    bool active_flag = true;

    // Start is called before the first frame update
    void Start()
    {
        count = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf == true)
        {
            active_flag = true;
            count += Time.deltaTime;
        }

        if (count >= 0.4f)
        {
            transform.position = new Vector2(300, -100);

            if (active_flag == true)
            {
                active_flag = false;
                count = 0;
                gameObject.SetActive(false);
            }
        }
    }
}
