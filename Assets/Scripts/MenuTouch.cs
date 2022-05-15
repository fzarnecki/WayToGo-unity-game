using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTouch : MonoBehaviour
{
    void Update() {
        // for pc debug
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }

        // phone touch
        if (Input.touchCount == 1)
        {
            HandleClick();
        }
    }


    private void HandleClick() {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 200f))
        {
            if (hit.transform != null)
            {
                MenuStartButton button;
                CubeBouncer cube;

                if (button = hit.transform.GetComponentInParent<MenuStartButton>())
                {
                    button.HandleClick();
                }
                else if (cube = hit.transform.GetComponent<CubeBouncer>())
                {
                    cube.HandleTouch();
                }
            }
        }
    }
}
