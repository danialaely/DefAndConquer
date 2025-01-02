using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameToggleManager : MonoBehaviour
{
    public Toggle EasyToggle;
    public Toggle MediumToggle;
    public Toggle HardToggle;

    void Start()
    {
        SetToggles();
    }

    private void SetToggles()
    {
        EasyToggle.isOn = ToggleStateManager.EasyToggleOn;
        MediumToggle.isOn = ToggleStateManager.MediumToggleOn;
        HardToggle.isOn = ToggleStateManager.HardToggleOn;
    }
}
