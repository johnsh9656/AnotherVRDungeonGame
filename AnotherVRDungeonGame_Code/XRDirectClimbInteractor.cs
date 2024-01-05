using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.XR.Interaction.Toolkit;

public class XRDirectClimbInteractor : XRDirectInteractor
{
    public static event Action<string> ClimbHandActivated;
    public static event Action<string> ClimbHandDeactivated;

    private string _controllerName;

    protected override void Start()
    {
        base.Start();
        _controllerName = gameObject.name;
    }
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        
        if (args.interactableObject.transform.gameObject.tag == "Climbable")
        {
            ClimbHandActivated?.Invoke(_controllerName);
            args.interactableObject.transform.gameObject.layer = 7;
        }
        /*else if (args.interactableObject.transform.gameObject.layer == 8) // interactable
        {
            args.interactableObject.transform.gameObject.layer = 9; // become weapon
        }*/


    }
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        ClimbHandDeactivated?.Invoke(_controllerName);
        if (args.interactableObject.transform.gameObject.tag == "Climbable")
        {
            args.interactableObject.transform.gameObject.layer = 6;
        }
        /*else if (args.interactableObject.transform.gameObject.layer == 9) // weapon
        {
            args.interactableObject.transform.gameObject.layer = 8; // become interactable
        }*/
    }
    
    public void CancelClimb()
    {
        StartCoroutine(ClimbCancelWait());
    }

    private IEnumerator ClimbCancelWait()
    {
        base.allowSelect = false;
        yield return new WaitForSeconds(0.4f);
        base.allowSelect = true;
    }
}
