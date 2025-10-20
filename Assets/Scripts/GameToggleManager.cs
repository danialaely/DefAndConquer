using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameToggleManager : MonoBehaviour
{
    public Toggle EasyToggle;
    public Toggle MediumToggle;
    public Toggle HardToggle;

    public Toggle BlueToggle;
    public Toggle OrangeToggle;
    public Toggle GreyToggle;

    void Start()
    {
        SetToggles();
    }

    private void SetToggles()
    {
        EasyToggle.isOn = ToggleStateManager.EasyToggleOn;
        MediumToggle.isOn = ToggleStateManager.MediumToggleOn;
        HardToggle.isOn = ToggleStateManager.HardToggleOn;

        BlueToggle.isOn = ToggleStateManager.BoardOneToggleOn;
        OrangeToggle.isOn = ToggleStateManager.BoardTwoToggleOn;
        GreyToggle.isOn = ToggleStateManager.BoardThreeToggleOn;
    }
}
