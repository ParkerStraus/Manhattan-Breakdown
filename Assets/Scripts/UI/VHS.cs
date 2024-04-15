using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VHS : MonoBehaviour
{
    public GameObject VHSOBJ;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayVHS()
    {
        VHSOBJ.SetActive(true);
    }

    public void HideVHS()
    {

        VHSOBJ.SetActive(false);
    }
}
