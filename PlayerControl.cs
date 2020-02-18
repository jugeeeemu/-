using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float Gravity = 20.0f;//重力(マップ環境設定用のスクリプトに移動予定)(変更しても適用されないのは何故...?)

    Rigidbody rb;

    public class Character
    {
        public  float force;//未使用
        private float velocity_x;//未使用
        private float velocity_y;
        private float velocity_z;//未使用
        public  float scalar;
        private Vector3 vector;
        public  float degree_x;//未使用
        public  float degree_y;
        public  float degree_z;//未使用
        public  int direction_H;
        public  int direction_V;
        public  bool Moving;//移動中かどうか
        public bool Intheair;//空中にいるかどうか

        private float MoveAcceleration = 5.0f;//移動加速度
        private float MoveLimit = 6.0f;//移動速度制限
        private float Brake = 20.0f;//移動キーを話した際の停止するまでの時間

        public void AddGravity(float Add)//加速度に重力を加算
        {
            vector.y=0;
            if (Intheair || Moving){vector.y = -Add;}
            if (!Intheair && !Moving){vector.y -= velocity_y *Brake;}//坂道でブレーキすると慣性で浮くのを防止
            if (velocity_y<-20){vector.y=0;}
        }

        public void Setvelocity(Rigidbody rb)
        {
            velocity_x = rb.velocity.x;
            velocity_y = rb.velocity.y;
            velocity_z = rb.velocity.z;
        }

        public Vector3 Getvector()
        {
            return vector;
        }
        public void Setvector(Rigidbody rb)
        {
            float cos_y = Mathf.Cos(degree_y);
            float sin_y = Mathf.Sin(degree_y);

            int direction_H_sub=0;
            int direction_V_sub=0;

            if(cos_y>0){direction_H_sub= 1;}
            if(cos_y<0){direction_H_sub=-1;}
            if(sin_y>0){direction_V_sub= 1;}
            if(sin_y<0){direction_V_sub=-1;}

            if (direction_H != 0) { vector.x = MoveAcceleration * (MoveLimit * direction_H_sub - velocity_x) * Mathf.Abs(cos_y); }else{ vector.x = -velocity_x *Brake;}
            if (direction_V != 0) { vector.z = MoveAcceleration * (MoveLimit * direction_V_sub - velocity_z) * Mathf.Abs(sin_y); }else{ vector.z = -velocity_z *Brake;}
        }
    }

    Character player;

    // Start is called before the first frame update
    void Start()
    {
        player = new Character();
        rb = GetComponent<Rigidbody>();
    }

    public void Intheaircheck()//空中にいるかどうかの判定(当たり判定(床)から呼び出し)
    {
        player.Intheair=false;
    }

    // Update is called once per frame
    void Update()
    {
//プレイヤーの移動================================================================================
        player.direction_H = 0;
        player.direction_V = 0;
        player.Moving=false;
            if (Input.GetKey(KeyCode.W)) { player.direction_V =  1; player.Moving = true; }
            if (Input.GetKey(KeyCode.S)) { player.direction_V = -1; player.Moving = true; }
            if (Input.GetKey(KeyCode.D)) { player.direction_H =  1; player.Moving = true; }
            if (Input.GetKey(KeyCode.A)) { player.direction_H = -1; player.Moving = true; }
//==============================================================================================
    }

    void FixedUpdate()
    {
        if(player.Moving){player.degree_y = Mathf.Atan2(player.direction_V, player.direction_H);}//動いている時のみ向いてる方向の計算をする
        player.scalar = rb.velocity.magnitude;//速さをメンバ変数に渡す
        player.Setvelocity(rb);//速度をメンバ変数に渡す
        player.Setvector(rb);//加える力のベクトルを計算する
        player.AddGravity(Gravity);//重力を加える
        rb.AddForce(player.Getvector(), ForceMode.Acceleration);//移動、重力全ての力を加える

        //Debug.Log("働く力；"+player.Getvector());
        //Debug.Log("向き;"+player.degree_y);
        //Debug.Log("移動中;"+player.Moving);
        //Debug.Log("空中にいる;"+player.Intheair);
        //Debug.Log("重力;"+Gravity);
        player.Intheair=true;
    }
}
