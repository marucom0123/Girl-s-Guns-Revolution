using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    private bool is_ground = false;
    private bool is_ground_enter, is_ground_stay, is_ground_exit;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool CheckGround()
    {
        if (is_ground_enter || is_ground_stay)
        {
            is_ground = true;
        }
        else if (is_ground_exit)
        {
            is_ground = false;
        }

        is_ground_enter = false;
        is_ground_stay = false;
        is_ground_exit = false;

        return is_ground;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Map_main")
        {
            is_ground_enter = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Map_main")
        {
            is_ground_stay = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Map_main")
        {
            is_ground_exit = true;
        }
    }
}
