using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Player : MonoBehaviour
{
    [SerializeField] float move_speed;//移動速度
    [SerializeField] float jump_force;//ジャンプ力
    [SerializeField] int shot_delay;//ショットの間隔
    [SerializeField] GameObject dust;//Dustのアニメーション

    private int shot_count = 0;//ショット回数

    private float key_x;//横方向の入力情報
    private bool jump_key;//ジャンプキーの入力情報
    private bool shot_key;//ショットキーの入力情報

    private bool is_ground = false;//接地しているか
    private bool jump_flag = false;//ジャンプするか
    private bool is_jump = false;//ジャンプしているか
    private bool is_double_jump = false;//二段ジャンプしているか
    private bool can_jump_flag = false;//ジャンプ可能か
    private bool can_double_jump_flag = false;//二段ジャンプ可能か
    private bool jump_dust_flag = true;//アニメーションを一回再生させるために使う
    private bool double_jump_dust_flag = true;
    private int dust_count;


    Rigidbody2D rb2d;//Rigidbody2Dコンポーネント
    Animator anim;//Animatorコンポーネント
    GroundCheck groundCheck;//グラウンドチェックのスクリプト
    PlayerBulletPool playerBulletPool;//PlayerBulletPoolのスクリプト


    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();//Rigidbody2Dコンポーネント取得
        anim = GetComponent<Animator>();//Animatorコンポーネント取得
        groundCheck = GameObject.Find("GroundCheck").GetComponent<GroundCheck>();//GroundCheckコンポーネント取得
        playerBulletPool = GameObject.Find("PlayerBulletPool").GetComponent<PlayerBulletPool>();//PlayerBulletPoolスクリプトを取得

    }

    // Update is called once per frame
    void Update()
    {
        Key();
        GroundCheck();
        Animation();
    }

    private void FixedUpdate()
    {
        Move();
        Shot();
    }

    void Key()//キー入力の処理
    {
        key_x = Input.GetAxisRaw("Horizontal");//移動キー

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.LeftShift))//ジャンプキー
        {
            jump_key = true;
            jump_flag = true;
        }
        else
        {
            jump_key = false;
        }

        if (Input.GetKey("z"))//ショットキー
        {
            shot_key = true;
        }
        else
        {
            shot_key = false;
        }
    }

    void Move()//移動処理
    {

        rb2d.velocity = new Vector2(key_x * move_speed * Time.deltaTime, rb2d.velocity.y);//左右移動

        if (is_ground == true)
        {
            can_jump_flag = true;
        }

        if (jump_flag == true)//ジャンプ処理
        {
            if (can_double_jump_flag == true)//二段ジャンプ
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x, jump_force);
                is_ground = false;
                can_double_jump_flag = false;
                is_double_jump = true;
            }

            if (is_ground == true)//ジャンプ
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x, jump_force);
                is_ground = false;
                can_double_jump_flag = true;
                is_jump = true;
            }
            else if (can_jump_flag == true)//ジャンプせず飛び降り時に1回ジャンプできる
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x, jump_force);
                can_jump_flag = false;
                is_double_jump = true;
            }

            jump_flag = false;
        }
    }

    void Shot()//ショット処理
    {
        shot_count++;

        if (shot_key == true)
        {
            if (shot_count >= shot_delay)
            {
                shot_count = 0;
                GameObject shot = playerBulletPool.GetObject();
                if (transform.localScale.x == -1)
                {
                    shot.GetComponent<PlayerBullet>().speed *= -1;
                    shot.transform.position = new Vector2(transform.position.x - 40, transform.position.y);
                    rb2d.AddForce(new Vector2(100, 0), ForceMode2D.Impulse);
                }
                else
                {
                    shot.transform.position = new Vector2(transform.position.x + 40, transform.position.y);
                    rb2d.AddForce(new Vector2(-100, 0), ForceMode2D.Impulse);
                }
            }
        }
    }

    void GroundCheck()//接地判定
    {

        //レイキャストで実装を試みたが他の処理と相性が悪い

        /*
        //レイキャストで地面を検出,右左側2つのレイで地面を検出
        Vector2 pos_left = new Vector2(transform.position.x-7f, transform.position.y-10);        
        Vector2 pos_right = new Vector2(transform.position.x+5f, transform.position.y-10);
        Vector2 direction = new Vector2(0, -1);

        RaycastHit2D hit_left = Physics2D.Raycast(pos_left, direction, 4);
        RaycastHit2D hit_right = Physics2D.Raycast(pos_right, direction, 4);

        Debug.DrawRay(pos_left, direction * 4, Color.red, 1);
        Debug.DrawRay(pos_right, direction * 4, Color.red, 1);

        if (hit_left.collider)
        {
            if (hit_left.collider.tag == "Map_main")
            {
                isGround = true;
            }
        }
        else
        {
            isGround = false;
        }       

        if (hit_right.collider)
        {
            if (hit_right.collider.tag == "Map_main")
            {
                isGround = true;
            }
        }
        else
        {
            isGround = false;
        }
        */

        is_ground = groundCheck.CheckGround();//GroundCheck
    }

    void Animation()//アニメーション処理
    {
        Debug.Log(is_ground);
        //左右移動で左右反転
        if (key_x > 0)
        {
            transform.localScale = new Vector3(1.4f, 1.4f, 1);
        }
        else if (key_x < 0)
        {
            transform.localScale = new Vector3(-1.4f, 1.4f, 1);
        }

        //移動キー入力で走る
        if (key_x != 0)
        {
            anim.SetBool("Run", true);
        }
        else
        {
            anim.SetBool("Run", false);
        }


        //ジャンプ
        if (jump_flag == true && is_ground == true)
        {
            is_jump = false;
            anim.SetBool("Jump", true);

            if (jump_dust_flag == true)
            {
                jump_dust_flag = false;
                GameObject effect = Instantiate(dust);

                if (transform.localScale.x == 1.4f)
                {
                    effect.transform.position = new Vector2(transform.position.x - 30, transform.position.y + 15);
                }
                else if (transform.localScale.x == -1.4f)
                {
                    effect.transform.position = new Vector2(transform.position.x + 30, transform.position.y + 15);
                    effect.transform.localScale = new Vector3(-1.4f, 1.4f, 1);
                }
            }
        }
        else if (is_ground == true)
        {
            anim.SetBool("Jump", false);
            jump_dust_flag = true;
        }

        //二段ジャンプ
        if (jump_flag == true && can_double_jump_flag == true)
        {
            anim.SetTrigger("DoubleJump");
        }
        else if (jump_flag == true && can_jump_flag == true && is_ground == false)
        {
            anim.SetTrigger("DoubleJump");
        }

        //ショット
        if (shot_key == true)
        {
            anim.SetBool("Shot", true);
            dust_count++;

            if (dust_count > 20)
            {
                dust_count = 0;
                GameObject effect = Instantiate(dust);
                if (transform.localScale.x == 1.4f && is_ground == true && key_x == 0)
                {
                    effect.transform.position = new Vector2(transform.position.x - 30, transform.position.y + 10);
                }
                else if (transform.localScale.x == -1.4f && is_ground == true && key_x==0)
                {
                    effect.transform.position = new Vector2(transform.position.x + 30, transform.position.y + 10);
                    effect.transform.localScale = new Vector3(-1.4f, 1.4f, 1);
                }
            }
        }
        else
        {
            anim.SetBool("Shot", false);
        }
    }
}
