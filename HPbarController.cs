using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPbarController : MonoBehaviour
{
    private GameObject player;

    public Image lifeGreenGage;
    public Image lifeRedGage;

    void Start() {
        player = GameObject.Find("kirby_prefab");
        this.initParameter();
    }

    void Update() {
        lifeGreenGage.fillAmount = PlayerController.strength / 100f;
        
        //iTween.ValueTo(lifeRedGage)
    }

    private void initParameter() {
        lifeGreenGage = GameObject.Find("HP_green").GetComponent<Image>();
        lifeGreenGage.fillAmount = 1;

        lifeRedGage = GameObject.Find("HP_red").GetComponent<Image>();
        lifeRedGage.fillAmount = 1;
    }
}
