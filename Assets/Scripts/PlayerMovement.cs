using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private MonumentInput inputActions;

    void Start()
    {
        inputActions = new MonumentInput();
        inputActions.Player.Click.performed += ctx => OnClick();
        inputActions.Enable();
    }

    private void OnDisable()
    {
        if(inputActions != null) inputActions.Disable();
    }

    private void OnEnable()
    {
        if (inputActions != null) inputActions.Enable();
    }

    private void OnClick() 
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject.GetComponent<Walkable>()) 
            {
                Debug.Log("Pinchaste un cubito");
            }
        }
    }

    private void MovePlayer(Walkable target) 
    {
        //See if there's any path until target
    }

}
