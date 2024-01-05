using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TimeControl : MonoBehaviour
{ 
    [SerializeField] private InputActionProperty timeInputSource;
    [SerializeField] private float slowTimeScale = .5f;
    //[SerializeField] private float slowTimeLength = 5f;

    private void Update()
    {
        bool time = timeInputSource.action.WasPressedThisFrame();
        if (time)
        {
            SwitchTimeScale();
        }
        
    }

    private void SwitchTimeScale()
    {
        if (Time.timeScale == 1) Time.timeScale = slowTimeScale;
        else Time.timeScale = 1;
    }
}
