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
    [SerializeField] float punch_force;//パンチ力
    [SerializeField] GameObject dust;//Dustのアニメーション

    private int shot_count = 0;//ショット回数

    private float key_x;//横方向の入力情報
    private bool jump_key;//ジャンプキーの入力情報
    private bool shot_key;//ショットキーの入力情報
    private bool guard_key;//ガードキーの入力情報
    private bool punch_key;//パンチキーの入力情報

    private bool is_ground = false;//接地しているか
    private bool is_wall = false;//壁に張り付いているか
    private bool jump_flag = false;//ジャンプするか
    private bool is_jump = false;//ジャンプしているか
    private bool is_double_jump = false;//二段ジャンプしているか
    private bool can_jump_flag = false;//ジャンプ可能か
    private bool can_double_jump_flag = false;//二段ジャンプ可能か
    private bool punch_flag = false;//パンチするか
    private float punch_count;//パンチのカウント
    private bool can_punch_flag = true;//パンチできるか
    private bool jump_dust_flag = true;//アニメーションを一回再生させるために使う
    private bool double_jump_dust_flag = true;
    private float dust_count;


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
        WallCheck();
        Animation();

        Debug.Log(is_wall);
    }

    private void FixedUpdate()
    {
        Move();
        Shot();
    }

    void Key()//キー入力の処理
    {
        key_x = Input.GetAxisRaw("Horizontal");//移動キー

        if (Input.GetKeyDown(KeyCode.LeftShift))//ジャンプキー
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

        if (Input.GetKey("x"))//ガードキー
        {
            guard_key = true;
        }
        else
        {
            guard_key = false;
        }

        if (Input.GetKeyDown(KeyCode.Space))//パンチキー
        {
            punch_key = true;
            punch_flag = true;
        }
        else
        {
            punch_key = false;
        }
    }

    void Move()//移動処理
    {
        if (is_wall == true)//壁に張り付いている際
        {
            rb2d.velocity = new Vector2(key_x * Time.deltaTime * 8000 , -80);
        }
        else
        {
            if (guard_key == true)//ガード中は移動が遅くなる
            {
                rb2d.velocity = new Vector2((key_x * move_speed * Time.deltaTime) / 5, rb2d.velocity.y);//左右移動
            }
            else
            {
                rb2d.velocity = new Vector2(key_x * move_speed * Time.deltaTime, rb2d.velocity.y);//左右移動
            }
        }


        if (is_ground == true)
        {
            can_jump_flag = true;
            can_punch_flag = true;
        }

        if (is_wall == true)
        {
            can_double_jump_flag = false;
        }

        if (jump_flag == true)//ジャンプ処理
        {
            if (can_double_jump_flag == true)//二段ジャンプ
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x, jump_force);
                can_double_jump_flag = false;
                is_double_jump = true;
            }

            if (is_ground == true && is_wall==false)//ジャンプ
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x, jump_force);
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

        if (punch_flag == true && shot_key == false && guard_key == false)//パンチ
        {
            punch_count += Time.deltaTime;

            if (is_ground == true)
            {
                if (transform.localScale.x >= 1)
                {
                    rb2d.velocity = new Vector2(punch_force, 0);
                }
                else
                {
                    rb2d.velocity = new Vector2(-punch_force, 0);
                }
                if (punch_count > 0.2f)
                {
                    punch_count = 0;
                    punch_flag = false;
                }
            }
            else if(can_punch_flag==true)
            {
                can_punch_flag = false;

                if (transform.localScale.x >= 1)
                {
                    rb2d.velocity = new Vector2(punch_force, 0);
                }
                else
                {
                    rb2d.velocity = new Vector2(-punch_force, 0);
                }
                if (punch_count > 0.2f)
                {
                    punch_count = 0;
                    punch_flag = false;
                }
            }
        }
    }

    void Shot()//ショット処理
    {
        shot_count++;

        if (shot_key == true && guard_key == false && is_wall == false) 
        {
            if (shot_count >= shot_delay)
            {
                shot_count = 0;
                GameObject shot = playerBulletPool.GetObject();
                if (transform.localScale.x <= -1)
                {
                    shot.GetComponent<PlayerBullet>().speed *= -1;
                    shot.transform.position = new Vector2(transform.position.x - 40, transform.position.y);
                    rb2d.AddForce(new Vector2(200, 0), ForceMode2D.Impulse);
                }
                else
                {
                    shot.transform.position = new Vector2(transform.position.x + 40, transform.position.y);
                    rb2d.AddForce(new Vector2(-200, 0), ForceMode2D.Impulse);
                }
            }
        }
    }

    void GroundCheck()//接地判定
    {
        is_ground = groundCheck.CheckGround();//GroundCheck
    }

    void WallCheck()//壁設置判定
    {
        //レイキャストで壁を検出,右左2つのレイで壁を検出
        Vector2 pos_left = new Vector2(transform.position.x - 13f, transform.position.y);
        Vector2 pos_right = new Vector2(transform.position.x + 9f, transform.position.y);
        Vector2 direction_left = new Vector2(-1, 0);
        Vector2 direction_right = new Vector2(1, 0);

        RaycastHit2D hit_left = Physics2D.Raycast(pos_left, direction_left, 1.5f);
        RaycastHit2D hit_right = Physics2D.Raycast(pos_right, direction_right, 1.5f);

        Debug.DrawRay(pos_left, direction_left * 1.5f, Color.red, 1);
        Debug.DrawRay(pos_right, direction_right * 1.5f, Color.red, 1);

        if (hit_left.collider)
        {
            if (hit_left.collider.tag == "Map_main")
            {
                is_wall = true;
            }

        }
        else if (hit_right.collider)
        {
            if (hit_right.collider.tag == "Map_main")
            {
                is_wall = true;
            }

        }
        else
        {
            is_wall = false;
        }
    }

    void Animation()//アニメーション処理
    {
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

                if (transform.localScale.x >= 1)
                {
                    effect.transform.position = new Vector2(transform.position.x - 30, transform.position.y + 15);
                }
                else if (transform.localScale.x <= -1)
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
        if (shot_key == true && guard_key == false && is_wall == false)
        {
            anim.SetBool("Shot", true);
            dust_count += Time.deltaTime;

            if (dust_count > 0.3f)
            {
                dust_count = 0;
                GameObject effect = Instantiate(dust);
                if (transform.localScale.x >= 1 && is_ground == true && key_x == 0)
                {
                    effect.transform.position = new Vector2(transform.position.x - 40, transform.position.y + 10);
                }
                else if (transform.localScale.x<= -1 && is_ground == true && key_x==0)
                {
                    effect.transform.position = new Vector2(transform.position.x + 40, transform.position.y + 10);
                    effect.transform.localScale = new Vector3(-1.4f, 1.4f, 1);
                }
            }
        }
        else
        {
            anim.SetBool("Shot", false);
        }

        //ガード
        if (guard_key == true)
        {
            anim.SetBool("Guard", true);
        }
        else
        {
            anim.SetBool("Guard", false);
        }

        //壁張り付き
        if (is_wall == true)
        {
            anim.SetBool("Wall", true);
        }
        else
        {
            anim.SetBool("Wall",false);
        }

        //パンチ
        if (punch_flag == true && shot_key == false && guard_key == false)
        {
            anim.SetBool("Punch", true);
        }
        else
        {
            anim.SetBool("Punch", false);
        }

        if(punch_flag==true && can_jump_flag == true && can_double_jump_flag == true)
        {
            anim.SetBool("Punch", true);
        }
        else
        {
            anim.SetBool("Punch", false);
        }


    }
}
