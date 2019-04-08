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
    public static float thickness;
    public static float totalSimTime = 0;

    static float simTime = 0;
    static bool timerStarted = false;
    float t1, t2;

    void Awake () {
        resultText = uitext;
        maxCoins = sim.rows * sim.columns;
    }

	void Update () {

        if(!sim.simFinished && !timerStarted)
        {
            timerStarted = true;
            t1 = Time.realtimeSinceStartup;
        }

        resultText.text = (sim.isSearchRunning)? ("Searching from " + sim.searchStart + "mm to " + sim.searchEnd + "mm in increments of " + sim.searchStep + "mm\nCurrent Thickness: " + thickness + "mm\n") : "";

        if ((coinTop + coinBottom + coinSide) == maxCoins)
        {
            t2 = Time.realtimeSinceStartup;
            simTime = t2 - t1;
            totalSimTime += simTime;

            sim.simFinished = true;
            timerStarted = false;
            resultText.text += "Tails: " + coinTop + " Heads: " + coinBottom + " Side: " + coinSide + "\n\rStatus: COMPLETED";
        }
        else
            resultText.text += "Tails: " + coinTop + " Heads: " + coinBottom + " Side: " + coinSide + "\n\rStatus:" + (float)(coinTop + coinBottom + coinSide) / (float)maxCoins * 100 + "%";
        if (sim.isSearchRunning)
        {
            int remaining = sim.searchMaxIters - sim.searchIters;
            string eta = formatTime(simTime / sim.searchIters * remaining);
            //string seconds = (eta == 0) ? "~" : eta.ToString();
            resultText.text += "\n\rOverall Status: " + (float)sim.searchIters / (float)sim.searchMaxIters * 100 + "%\n\rETA: " + eta;
        }
    }

    string formatTime(float time)
    {
        if (time == float.NaN)
            return "~";

        time /= 3600f;
        int hours = (int)time;
        time = (time - hours) * 60f;
        int minutes = (int)time;
        time = (time - minutes) * 60f;
        int seconds = (int)time;

        string t = "";

        if (!(hours <= 0)) t += hours.ToString() + " hours ";
        if (!(minutes != 0)) t += minutes.ToString() + " minutes ";
        if (!(seconds != 0)) t += seconds.ToString() + " seconds";
        if (t == "") t = "~";

        return t;
    }
}
