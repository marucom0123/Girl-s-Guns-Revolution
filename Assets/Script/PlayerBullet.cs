using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float speed;

    float count;

    Rigidbody2D rb2d;
    Renderer targetRenderer;

    BulletExplosionPool bulletExplosionPool;
    GameObject player;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        bulletExplosionPool = GameObject.Find("BulletExplosionPool").GetComponent<BulletExplosionPool>();
        targetRenderer = GetComponent<Renderer>();
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        count += Time.deltaTime;

        if (gameObject.activeSelf == false)
        {
            return;
        }
        else if (count >= 0.4f)
        {
            count = 0;
            transform.position = new Vector2(0, -5000);

            if (speed < 0)
            {
                speed *= -1;
            }

            gameObject.SetActive(false);
        }
        
    }

    void LateUpdate()
    {
        rb2d.velocity = new Vector2(speed * Time.deltaTime, 0);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag != "Player" && col.tag != "PlayerBullet"&& col.tag != "EnemyBullet")
        {
            GameObject explosion = bulletExplosionPool.GetObject();
            explosion.transform.position = transform.position;

            if(speed < 0)
            {
                speed *= -1;
            }
            transform.position = new Vector2(0, -5000);
            gameObject.SetActive(false);
        }
    }
}
