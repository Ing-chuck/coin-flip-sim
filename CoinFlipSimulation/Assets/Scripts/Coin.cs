using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour {
    public string result;
    public GameObject top;
    public GameObject bottom;
    public Vector3 topv;
    public Vector3 botv;

    void Update () {
        //if rigidbody goes to sleep we check axis the coin's child objects (axis) orientation
		if (GetComponent<Rigidbody>().IsSleeping())
        {
            if ((Mathf.Abs(top.transform.position.y) - Mathf.Abs(bottom.transform.position.y))>0.02f)
            {
                result = "Heads";
                CalculateScript.coinTop++;
            }
            else if ((Mathf.Abs(top.transform.position.y) - Mathf.Abs(bottom.transform.position.y))<-0.02f)
            {
                result = "Tails";
                CalculateScript.coinBottom++;
            }
            else
            {
                result = "Side";
                CalculateScript.coinSide++;
            }
            topv.y = (Mathf.Abs(top.transform.position.y) - Mathf.Abs(bottom.transform.position.y));
            botv = bottom.transform.position;
            this.enabled = false;   
        }
	}
}
