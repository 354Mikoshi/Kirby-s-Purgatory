using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{
    public AudioClip pushButtonSound;
    public Text sumScoreText;
    private AudioSource audioSource;

    private static int maxScore = 0;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        maxScore = Math.Max(maxScore, GameManager.sumScore);
        sumScoreText.GetComponent<Text>().text = "あなたの最高スコア: " + maxScore.ToString(); ;
    }

    private void Update()
    {
    }

    public void GameStartButton() {
        StartCoroutine(DelayMethod(1.5f, () =>
        {
            SceneManager.LoadScene("SampleScene");
        }));
        audioSource.PlayOneShot(pushButtonSound);
    }

    private IEnumerator DelayMethod(float waitTime, Action action) {
        yield return new WaitForSeconds(waitTime);
        action();
    }
}
