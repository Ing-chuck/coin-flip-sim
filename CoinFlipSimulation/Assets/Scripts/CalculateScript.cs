using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalculateScript : MonoBehaviour {

    public Text uitext;
    public SimulationController sim;
    public static Text resultText;
    public static int coinTop = 0;
    public static int coinBottom = 0;
    public static int coinSide = 0;
    public static int maxCoins;

    void Awake () {
        resultText = uitext;
        maxCoins = sim.rows * sim.columns;
    }

	void Update () {
        resultText.text = "Tails: " + coinTop + " Heads: " + coinBottom + " Side: " + coinSide + "\n\rStatus:" + (float)(coinTop+coinBottom+coinSide)/(float)maxCoins*100 + "%";
        if ((coinTop + coinBottom + coinSide) == maxCoins)
        {
            sim.simFinished=true;
            resultText.text = "Tails: " + coinTop + " Heads: " + coinBottom + " Side: " + coinSide + "\n\rStatus: COMPLETED";
        }
	}
}
