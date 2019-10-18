using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GolemSliderController : MonoBehaviour
{
    public Slider golemHP_Slider;

    // Start is called before the first frame update
    void Start()
    {
        golemHP_Slider = GetComponent<Slider>();
        golemHP_Slider.maxValue = 200f;
        golemHP_Slider.minValue = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        golemHP_Slider.value = golemController.strength / 200f;
    }
}
