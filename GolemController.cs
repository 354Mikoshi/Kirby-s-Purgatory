using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class GolemController : MonoBehaviour
{
    /*
    WaveArmsをどこに入れるか…
    一定距離以内に一定時間以上プレイヤーがいた場合にWaveArmsを発動させる
    腕を振り回す動作にも攻撃判定を設定し、周りにまき散らす石にも攻撃判定を入れる
    */
    public GameObject spatteringStone, explosion;
    public Slider golemHPslider; //HPのスライダー
    public Text damageText;
    private PlayerController playerController;
    public static int direction_number, strength;

    private Animator animator;
    private GameObject player, canvas;
    private Rigidbody rb;
    private Vector3 playerPos, golemToPlayer, positionWhereGolemWalksAround, positionWhereExplosionOcuurs;
    private AnimatorStateInfo stateInfo;

    private int hash;
    private float walkX, walkY, walkZ;
    private float distanceBetweenGolemAndPlayer;
    private float timeWhenPlayerIsCloseToGolem;
    private bool isSwingingArmsDown, isWavingArms, isSpatteringStone;
    SpatteringStoneController spa = new SpatteringStoneController();

    private int count;
    private float tmptime;

    //諸定数
    private const float distanceGolemSwingsArmsDownAt = 1.5f, distanceGolemBeginsToRunAt = 5.0f, distanceGolemWaveArmsAt = 14.0f;
    private const float speedGolemRunsAt = 0.10f, speedGolemWalksAt = 0.40f;
    private const float radiusOfCircleGolemWalkAround = 2.0f;
    private const float timeWhenGolemStartsToWaveArms = 5.0f;
    private const int lengthOfStage = 20;

    // Start is called before the first frame update
    void Start() {
        //アニメーターの各種パラメータの初期化
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        golemHPslider.maxValue = 200f;
        golemHPslider.minValue = 0f;

        player = GameObject.Find("kirby_prefab");
        canvas = transform.Find("Canvas").gameObject;
        playerController = new PlayerController();

        playerPos = player.transform.position;
        golemToPlayer = playerPos - transform.position;
        //sizeOfCollider = AnimationUtility.get

        System.Random rnd = new System.Random();

        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        hash = stateInfo.fullPathHash;
        isSwingingArmsDown = false;
        isWavingArms = false;
        isSpatteringStone = false;

        direction_number = 0;
        strength = 200;
        positionWhereGolemWalksAround = new Vector3(
            /*rnd.Next(-lengthOfStage / 2, lengthOfStage / 2)*/10f,
            1.16f,
            /*rnd.Next(-lengthOfStage / 2, lengthOfStage / 2)*/10f
        );
        transform.position = positionWhereGolemWalksAround;

        timeWhenPlayerIsCloseToGolem = 0f;
        //Instantiate(gameObject, positionWhereGolemAppears, Quaternion.identity);

        count = 0;
        tmptime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        playerPos = player.transform.position;
        golemToPlayer = (playerPos - transform.position).normalized; //ゴーレムからプレイヤーに向かう単位ベクトル
        golemToPlayer.y = 0;
        golemHPslider.value = strength;
        golemHPslider.transform.LookAt(Camera.main.transform);

        //プレイヤーとゴーレムの間の距離を計算--------------------------------------------------------------------------------------
        distanceBetweenGolemAndPlayer = Mathf.Sqrt(Mathf.Pow(transform.position.x - playerPos.x, 2) + Mathf.Pow(transform.position.z - playerPos.z, 2));

        //ゴーレムが歩いているときの処理←if文の中の条件が甘い気がする-------------------------------------------------------------------
        if (!isSwingingArmsDown && !isWavingArms && distanceBetweenGolemAndPlayer > distanceGolemBeginsToRunAt) {
            Walk();
        }
        //ゴーレムが腕を振り回す: 終了判定を実装したい
        else if (!isSwingingArmsDown && distanceGolemBeginsToRunAt <= distanceBetweenGolemAndPlayer && distanceBetweenGolemAndPlayer < distanceGolemWaveArmsAt) {
            WaveArms();
        }
        //ゴーレムが走る
        else if (!isSwingingArmsDown && 
            distanceGolemSwingsArmsDownAt <= distanceBetweenGolemAndPlayer && distanceBetweenGolemAndPlayer < distanceGolemBeginsToRunAt) {
            Run(); 
        }
        //ゴーレムが腕を振り下ろす: 腕を振り下ろしている間はゴーレムが立ち止まっていなければならない
        //このままだとプレイヤーとゴーレムとの距離が開くと、再度ゴーレムがプレイヤーを追いかけてしまう
        //腕を振り下ろしている間、手についているコライダーの大きさが変化する
        else if (!isSwingingArmsDown && distanceBetweenGolemAndPlayer < distanceGolemSwingsArmsDownAt) {
            SwingArmsDown();
        }
        /*
        distanceGolemSwingsArmsDownAtより近い位置にプレイヤーがい続けるとSwingArmsDownのアニメーションが中断される
        */
        //--------------------------------------------------------------------------------------------------------------------

        //SpatterStones();
        if (strength <= 0) {
            //Destroy(gameObject);
        }
    }

    private IEnumerator DelayMethod(float waitTime, Action action) {
        yield return new WaitForSeconds(waitTime);
        action();
    }

    private void Walk() {
        animator.SetInteger("transitionParameter", 0);
        rb.MovePosition(new Vector3(
            positionWhereGolemWalksAround.x + radiusOfCircleGolemWalkAround * Mathf.Cos(Time.time * speedGolemWalksAt),
            positionWhereGolemWalksAround.y,
            positionWhereGolemWalksAround.z + radiusOfCircleGolemWalkAround * Mathf.Sin(Time.time * speedGolemWalksAt)
            )
        );

        //walkX = positionWhereGolemAppears.x + radiusOfCircleGolemWalkAround * Mathf.Cos(Time.time * speedGolemWalksAt);
        //walkY = transform.position.y;
        //walkZ = positionWhereGolemAppears.z + radiusOfCircleGolemWalkAround * Mathf.Sin(Time.time * speedGolemWalksAt);
        //transform.position = new Vector3(walkX, walkY, walkZ);

        Quaternion directionOfGolem = Quaternion.LookRotation(new Vector3(
            Mathf.Cos(Time.time * speedGolemWalksAt + Mathf.PI / 2),
            0,
            Mathf.Sin(Time.time * speedGolemWalksAt + Mathf.PI / 2)
            )
        );
        transform.rotation = directionOfGolem;
    }

    private void WaveArms() {
        timeWhenPlayerIsCloseToGolem += Time.deltaTime;
        //Debug.Log(timeWhenPlayerIsCloseToGolem);
        if (timeWhenPlayerIsCloseToGolem > timeWhenGolemStartsToWaveArms) {
            isWavingArms = true;
            //isSpatteringStone = false;
            positionWhereExplosionOcuurs = transform.forward + transform.position;
            positionWhereExplosionOcuurs.y = 0;
            animator.SetInteger("transitionParameter", 1);

            //if (!isSpatteringStone && count <= 4) {
            //    spa.SpatterStones(gameObject.transform.position);
            //}

            StartCoroutine(DelayMethod(1.25f, () =>
            {
                animator.SetInteger("transitionParameter", 0);
                timeWhenPlayerIsCloseToGolem = 0;
                count = 0;
            }));

            isWavingArms = false;
        }

    }

    private void Run() {
        animator.SetInteger("transitionParameter", 2);
        Vector3 aim = playerPos;
        aim.y = transform.position.y;
        transform.LookAt(aim);
        transform.position += golemToPlayer * speedGolemRunsAt;
    }

    private void SwingArmsDown() {
        isSwingingArmsDown = true;

        animator.SetInteger("transitionParameter", 3);
        positionWhereExplosionOcuurs = transform.position + 1.5f * transform.forward;
        positionWhereExplosionOcuurs.y = 0;
        StartCoroutine(DelayMethod(1.55f, () =>
        {
            Instantiate(explosion, positionWhereExplosionOcuurs, Quaternion.identity);
        }));
        StartCoroutine(DelayMethod(1.55f, () =>
        {
            animator.SetInteger("transitionParameter", 0);
            isSwingingArmsDown = false;
        }));
        positionWhereGolemWalksAround = transform.position;

    }

    //private void SpatterStones() {
    //    for (int i = 0; i < 4; i++) {
    //        direction_number = i;
    //        Vector3 direction = new Vector3(4 * Mathf.Cos(Mathf.PI / 2 * direction_number), 0f, 4 * Mathf.Sin(Mathf.PI / 2 * direction_number));
    //        Instantiate(spatteringStone, transform.position + direction, Quaternion.identity);
    //        count++;
    //    }
    //}

    void OnTriggerEnter(Collider col) {
        if (col.gameObject.tag == "NormalBlow") {
            strength -= 50;
            Text tmp = Instantiate(damageText);
            tmp.transform.parent = canvas.transform; //damageTextをCanvasの子にする
            tmp.transform.localPosition = new Vector3(0f, 7.5f, 0);
            tmp.GetComponent<Text>().text = "50";
        }
        else if (col.gameObject.tag == "ChargedBlow") {
            strength -= 200;
            Text tmp = Instantiate(damageText);
            tmp.transform.parent = canvas.transform; //damageTextをCanvasの子にする
            tmp.transform.localPosition = new Vector3(0f, 7.5f, 0);
            tmp.GetComponent<Text>().text = "200";
        }
    }
}
