using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public GameObject kyuchan, apple, xAxisFire, zAxisFire, orangeSylinder, fireCollider;
    public Text player_strength_text, elapsed_time_text, numberOfEliminatedEnemies_text, gameOver_text;
    public Text yourScoreIs, jikan, gekitsuisuu, jikan_tokuten, plus, gekitsuisuu_tokuten, equal, goukei_tokuten;
    public Image HP_red, HP_green, HP_bar, Shoot_sight;
    public AudioClip displaySound;

    private float elapsed_time;
    private float intervalWhenKyuchanAppears, interval_kyuchan;
    private float intervalWhenAppleAppears, interval_apple;
    private float intervalWhenFireCylinderAppears, interval_fireCylinder;
    private int numberOfFireCylinder;
    private AudioSource audioSource;
    public static bool isAlive;

    private float stageSize;
    private List<float> positionWhereFireOccurs = new List<float>();

    public static int kyuchanCurrentNum, EliminatedEnemyNum;
    private int kyuchanMaxNum;
    public static int sumScore;

    private void Start() {
        elapsed_time = 0f;
        interval_kyuchan = 0f; intervalWhenKyuchanAppears = 3f;
        interval_apple = 0f; intervalWhenAppleAppears = 4f;
        interval_fireCylinder = 0f; intervalWhenFireCylinderAppears = 10f;
        stageSize = 25f;

        kyuchanCurrentNum = 0; kyuchanMaxNum = 5;
        EliminatedEnemyNum = 0;
        numberOfFireCylinder = 1;

        for (int i = -20; i <= 20; i++) {
            positionWhereFireOccurs.Add(-1.3f * i);
        }

        audioSource = GetComponent<AudioSource>();
        player_strength_text.GetComponent<Text>().text = "HP: " + PlayerController.strength.ToString();
        elapsed_time_text.GetComponent<Text>().text = "Time: " + elapsed_time.ToString("F1");
        gameOver_text.enabled = false;
        yourScoreIs.enabled = false;
        jikan.enabled = false;
        gekitsuisuu.enabled = false;
        jikan_tokuten.enabled = false;
        plus.enabled = false;
        gekitsuisuu_tokuten.enabled = false;
        equal.enabled = false;
        goukei_tokuten.enabled = false;

        isAlive = true;
    }

    private void Update() {
        if (isAlive) {
            elapsed_time += Time.deltaTime;
            interval_kyuchan += Time.deltaTime;
            interval_apple += Time.deltaTime;
            interval_fireCylinder += Time.deltaTime;
        }

        player_strength_text.GetComponent<Text>().text = "HP: " + PlayerController.strength.ToString();
        elapsed_time_text.GetComponent<Text>().text = "Time: " + elapsed_time.ToString("F1");
        numberOfEliminatedEnemies_text.GetComponent<Text>().text = "撃破数: " + EliminatedEnemyNum.ToString();

        if (interval_kyuchan > intervalWhenKyuchanAppears && kyuchanCurrentNum < kyuchanMaxNum) Kyuchan();
        if (interval_apple > intervalWhenAppleAppears) Apple();
        if (interval_fireCylinder > intervalWhenFireCylinderAppears) FireCylinder();
    }

    private void Kyuchan() {
        interval_kyuchan = 0f;
        intervalWhenKyuchanAppears /= 1.1f;
        kyuchanCurrentNum++;
        Vector3 pos = new Vector3(
            UnityEngine.Random.Range(-stageSize, stageSize), UnityEngine.Random.Range(5f, 10f), UnityEngine.Random.Range(-stageSize, stageSize));
        Instantiate(kyuchan, pos, Quaternion.identity);
    }

    private void Apple() {
        interval_apple = 0f;
        Vector3 pos = new Vector3(UnityEngine.Random.Range(-stageSize, stageSize), 0.6f, UnityEngine.Random.Range(-stageSize, stageSize));
        Instantiate(apple, pos, Quaternion.identity);
    }

    private void FireCylinder() {
        interval_fireCylinder = 0f;
        System.Random rnd = new System.Random();
        List<float> tmpPositionWhereFireOccurs = new List<float>();
        List<float> x_PositionWhereFireOccurs = new List<float>();
        List<float> z_PositionWhereFireOccurs = new List<float>();
        tmpPositionWhereFireOccurs = positionWhereFireOccurs;
        int realNumOfFireCylinder = numberOfFireCylinder;
        //x軸方向に伸びた円柱の場所を決める
        while (realNumOfFireCylinder > 0) {
            int index = rnd.Next(tmpPositionWhereFireOccurs.Count);
            x_PositionWhereFireOccurs.Add(tmpPositionWhereFireOccurs[index]);
            tmpPositionWhereFireOccurs.Remove(index);
            realNumOfFireCylinder--;
        }
        tmpPositionWhereFireOccurs = positionWhereFireOccurs;
        realNumOfFireCylinder = (int)numberOfFireCylinder;
        //z軸方向に伸びた円柱の場所を決める
        while (realNumOfFireCylinder > 0) {
            int index = rnd.Next(tmpPositionWhereFireOccurs.Count);
            z_PositionWhereFireOccurs.Add(tmpPositionWhereFireOccurs[index]);
            tmpPositionWhereFireOccurs.Remove(index);
            realNumOfFireCylinder--;
        }

        //x軸方向に伸びる炎を出現させる
        for (int i = 0; i < x_PositionWhereFireOccurs.Count; i++) {
            float tmp = x_PositionWhereFireOccurs[i];
            Instantiate(orangeSylinder, new Vector3(0f, 1f, tmp), Quaternion.Euler(0f, 0f, 90f));
            StartCoroutine(DelayMethod(1.2f, () =>
            {
                Instantiate(xAxisFire, new Vector3(-80f, 0.3f, tmp), Quaternion.identity);
                Instantiate(fireCollider, new Vector3(0f, 0f, tmp), Quaternion.identity);
            }));
        }
        //z軸方向に伸びる炎を出現させる
        for (int i = 0; i < z_PositionWhereFireOccurs.Count; i++) {
            float tmp = z_PositionWhereFireOccurs[i];
            Instantiate(orangeSylinder, new Vector3(tmp, 1f, 0f), Quaternion.Euler(0f, 90f, 90f));
            StartCoroutine(DelayMethod(1.2f, () =>
            {
                Instantiate(zAxisFire, new Vector3(tmp, 0.3f, -80f), Quaternion.identity);
                Instantiate(fireCollider, new Vector3(tmp, 0f, 0f), Quaternion.Euler(0f, 90f, 0f));
            }));
        }
        if (realNumOfFireCylinder < 41) numberOfFireCylinder++ ;
    }

    public void GameOver() {
        isAlive = false;

        elapsed_time_text.enabled = false;
        HP_red.enabled = false;
        HP_green.enabled = false;
        HP_bar.enabled = false;
        player_strength_text.enabled = false;
        Shoot_sight.enabled = false;
        numberOfEliminatedEnemies_text.enabled = false;
        gameOver_text.enabled = true;

        sumScore = (int)elapsed_time + EliminatedEnemyNum * 10;
        jikan_tokuten.GetComponent<Text>().text = ((int)elapsed_time).ToString();
        gekitsuisuu_tokuten.GetComponent<Text>().text = (EliminatedEnemyNum * 10).ToString();
        goukei_tokuten.GetComponent<Text>().text = sumScore.ToString();
        StartCoroutine(DelayMethod(1.5f, () =>
        {
            yourScoreIs.enabled = true;
            //audioSource.PlayOneShot(displaySound);
        }));
        StartCoroutine(DelayMethod(3f, () =>
        {
            jikan.enabled = true;
            jikan_tokuten.enabled = true;
            //audioSource.PlayOneShot(displaySound);
        }));
        StartCoroutine(DelayMethod(4.5f, () =>
        {
            plus.enabled = true;
            gekitsuisuu.enabled = true;
            gekitsuisuu_tokuten.enabled = true;
            //audioSource.PlayOneShot(displaySound);
        }));
        StartCoroutine(DelayMethod(6f, () =>
        {
            equal.enabled = true;
            goukei_tokuten.enabled = true;
            //audioSource.PlayOneShot(displaySound);
        }));
    }

    private IEnumerator DelayMethod(float waitTime, Action action) {
        yield return new WaitForSeconds(waitTime);
        action();
    }
}
