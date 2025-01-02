using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Slider slider2;
    public Slider slider3;
    public Slider slider4;

    public void SetHealth(int health) 
    {
        slider.value = health;
    }

    public void SetHealth2(int health2) 
    {
        slider2.value = health2;
    }

    public void SetTurnTime(int timer) 
    {
        slider3.value = timer;
    }

    public void SetTurnTime2(int timer)
    {
        slider4.value = timer;
    }
}
