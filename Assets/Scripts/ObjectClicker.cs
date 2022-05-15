using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectClicker : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    
    /***/

    void Update()
    {
        if (gameManager.IsGameLost() || gameManager.IsGameWon())
            return;

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


    private void HandleClick()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 100f))
        {
            if (hit.transform != null)
            {
                Cube cube;
                if (cube = hit.transform.GetComponent<Cube>())
                {
                    cube.HandleCubeClick();
                }
            }
        }
    }
}
