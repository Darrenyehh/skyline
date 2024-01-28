using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menuControl : MonoBehaviour
{
    protected bool menu = false;
    private GameObject togglemenu;
    private GameObject statMenu;

    void Start()
    {
    
        togglemenu = GameObject.FindGameObjectWithTag("ui");
        statMenu = GameObject.FindGameObjectWithTag("iu");
        if (statMenu == null)
        {
            Debug.LogError("No GameObject with tag 'iu' found.");
        }
        else
        {
        
            statMenu.SetActive(menu);
        }

        if (togglemenu == null)
        {
            Debug.LogError("No GameObject with tag 'ui' found.");
        }
        else
        {
        
            togglemenu.SetActive(menu);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Debug.Log("esc pressed");
            // Toggle the menu state
            menu = !menu;
            if (togglemenu != null)
            {   

                togglemenu.SetActive(menu);
                statMenu.SetActive(!menu);
            }
        }
    }
}