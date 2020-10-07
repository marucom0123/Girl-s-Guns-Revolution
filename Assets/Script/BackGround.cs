using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    [SerializeField] float rate;
    GameObject player;
    Vector3 player_pos;
    Vector3 camera_pos;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        player_pos = player.transform.localPosition;
        camera_pos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = (player.transform.localPosition - player_pos) * rate;
        transform.localPosition = new Vector3(camera_pos.x + pos.x,0,100);

        if (transform.localPosition.x >= 2200)
        {
            transform.localPosition = new Vector3(-1000, 0, 10);
            player_pos = player.transform.localPosition;
            camera_pos = transform.localPosition;

        }
        else if (transform.localPosition.x < -2200)
        {
            transform.localPosition = new Vector3(1000, 0, 10);
            player_pos = player.transform.localPosition;
            camera_pos = transform.localPosition;
        }
    }
}
