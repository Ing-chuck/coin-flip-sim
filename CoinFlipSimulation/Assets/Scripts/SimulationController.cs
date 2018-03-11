using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SimulationController : MonoBehaviour {

    public GameObject coinPrefab;
    public int rows;
    public int columns;
    public GameObject enableonstart;
    public GameObject startbutton;
    public GameObject disableonstart;
    public Slider thicknessSlider;
    public Slider intervalSlider;
    private int intervalSteps;
    public Text intervalText;
    public Text sampleSizeText;

    private void Awake()
    {
        intervalSteps = (int) intervalSlider.value;
        coinPrefab.transform.localScale = new Vector3(1, thicknessSlider.value, 1);
    }


    public void StartSimulation()
    {
        enableonstart.SetActive(enabled);
        startbutton.SetActive(false);
        disableonstart.SetActive(false);
        StartCoroutine(Generate());
    }

    public IEnumerator Generate()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                GameObject go = Instantiate(coinPrefab, this.transform);
                go.transform.position = new Vector3(i, 10f, j);
                Random.seed = System.DateTime.Now.Millisecond+go.GetInstanceID();
                Vector3 hitForce = new Vector3(Random.Range(-500.0f, 500.0f), Random.Range(100.0f, 500.0f), Random.Range(-500.0f, 500.0f));
                Vector3 hitTorque = new Vector3(Random.Range(-5000.0f, 5000.0f), Random.Range(-5000.0f, 5000.0f), Random.Range(-5000.0f, 5000.0f));
                go.GetComponent<Rigidbody>().AddForce(hitForce);
                go.GetComponent<Rigidbody>().AddTorque(hitTorque);
            }
        }
        yield return null;
    }

    public void OnResetButtonClick()
    {
        isBenchmarkRunning = false;
        Reset();
    }

    public void Reset()
    {
        CalculateScript.coinBottom = 0;
        CalculateScript.coinTop = 0;
        CalculateScript.coinSide = 0;
        Transform[] allChildren = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child != this.transform)
            {

                Destroy(child.gameObject);
            }
        }
        enableonstart.SetActive(false);
        startbutton.SetActive(true);
        disableonstart.SetActive(true);
        simFinished = true;
        if (!isBenchmarkRunning)
        {
            graph.SetActive(false);
            StopAllCoroutines();
        }
    }
 
    public void OnThicknessSliderValueChanged(float value)
    {
        coinPrefab.transform.localScale = new Vector3(1,value,1);
    }

    public void OnInterValSliderChanged(float value)
    {
        
        intervalSteps = (int)value;
        intervalText.text = "Split simulation interval to " + intervalSteps + " pieces";
    }

    public GameObject graph;
    public GameObject linePrefab;
    public bool simFinished = true;
    private bool isBenchmarkRunning = false;
    public void StartBenchmark()
    {
        isBenchmarkRunning = true;
        Transform[] allChildren = graph.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child != graph.transform && child.gameObject.name != "axisX" &&  child.gameObject.name != "axisY" && child.gameObject.name != "Canvas" && child.gameObject.name != "cText1" && child.gameObject.name != "cText2")
            {
                Destroy(child.gameObject);
            }
        }
        StartCoroutine(Benchloop());

    }

    private IEnumerator Benchloop()
    {
        string result = "Sample size" + System.Environment.NewLine + (rows*columns).ToString() + System.Environment.NewLine + "Landed on side:" + System.Environment.NewLine;

        float currentThickness= thicknessSlider.maxValue / intervalSteps;
        for (int i = 0; i < intervalSteps; i++)
        {
            simFinished = false;

            coinPrefab.transform.localScale = new Vector3(1, currentThickness, 1);
            thicknessSlider.value = currentThickness;
            StartSimulation();
            //wait till every coin lands
            do
            {
                yield return null;
            }
            while (!simFinished);
            //draw the line to the diagram
            GameObject line = Instantiate(linePrefab, graph.transform);
            line.GetComponent<LineRenderer>().SetPosition(0, new Vector3(((float)i / (float)intervalSteps)*2.0f, 0, 0));
            line.GetComponent<LineRenderer>().SetPosition(1, new Vector3(((float)i / (float)intervalSteps)*2.0f, (float)CalculateScript.coinSide/ (float)(rows * columns)*10, 0));
            result += CalculateScript.coinSide.ToString() + System.Environment.NewLine;
            if (i < intervalSteps-1)
            {
                Reset();
                currentThickness += thicknessSlider.maxValue / intervalSteps;
                graph.SetActive(true);
            }
            
        }
        //write results to a file at the end of simulation
        System.IO.File.WriteAllText("results.txt", result);
        isBenchmarkRunning = false;
    }

    public void OnColsValueChanged(string value)
    {
        int val=50;
        int.TryParse(value, out val);
        columns = val;
        sampleSizeText.text = "Sample size: " + rows * columns;
       CalculateScript.maxCoins = rows * columns;
    }
    public void OnRowsValueChanged(string value)
    {
        int val = 50;
        int.TryParse(value, out val);
        rows = val;
        sampleSizeText.text = "Sample size: " + rows * columns;
        CalculateScript.maxCoins = rows * columns;
    }
}
