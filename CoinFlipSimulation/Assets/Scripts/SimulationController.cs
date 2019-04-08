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
    public Slider accelerationSlider;
    public Text accelerationText;
    public Text sampleSizeText;
    public GameObject graph;
    public GameObject linePrefab;
    public float searchStart;
    public float searchEnd;
    public float searchStep = 0.01f;
    public int searchRepeat = 1;
    public int searchMaxIters;
    public int searchIters;
    public bool simFinished = true;
    private bool isBenchmarkRunning = false;
    public bool isSearchRunning = false;
    private float coinDiameter = 20;
    private string searchFilename = "search_results.txt";
    private string benchmarkFilename = "benchmark_results.txt";

    private void Awake()
    {
        intervalSteps = (int) intervalSlider.value;
        coinPrefab.transform.localScale = new Vector3(1, thicknessSlider.value, 1);
        searchStart = coinDiameter / (2 * Mathf.Sqrt(2));
        searchEnd = coinDiameter / Mathf.Sqrt(3);
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
                Random.InitState(System.DateTime.Now.Millisecond+go.GetInstanceID());
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
        if (isSearchRunning)
            System.IO.File.AppendAllText(searchFilename, "......Simulation Terminated by user......");
        isBenchmarkRunning = false;
        isSearchRunning = false;
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
        if (!isBenchmarkRunning && !isSearchRunning)
        {
            CalculateScript.totalSimTime = 0;
            Time.timeScale = 1.0f;
            accelerationSlider.value = 1;
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

    public void OnAccelerationSliderValueChanged(float value)
    {
        accelerationText.text = "Simulation Speed: " + value + "x";
        Time.timeScale = value;
    }

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
        System.IO.File.WriteAllText(benchmarkFilename, result);
        isBenchmarkRunning = false;
    }

    public void OnSearchStartValueEnter(string value)
    {
        searchStart = float.Parse(value);
    }

    public void OnSearchEndValueEnter(string value)
    {
        searchEnd = float.Parse(value);
    }

    public void OnSearchStepValueEnter(string value)
    {
        searchStep = float.Parse(value);
    }

    public void OnSearchRepetitionsValueEnter(string value)
    {
        GameObject inField = GameObject.Find("Repetitions");
        value = inField.GetComponent<InputField>().text;
        searchRepeat = int.Parse(value);
    }

    public void StartSearch()
    {
        isSearchRunning = true;
        Transform[] allChildren = graph.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child != graph.transform && child.gameObject.name != "axisX" && child.gameObject.name != "axisY" && child.gameObject.name != "Canvas" && child.gameObject.name != "cText1" && child.gameObject.name != "cText2")
            {
                Destroy(child.gameObject);
            }
        }
        StartCoroutine(SearchLoop());
    }

    private IEnumerator SearchLoop()
    {
        float n = rows * columns;
        string result = "Sample size: " + n.ToString() + ", Coin diameter: " + coinDiameter.ToString() + System.Environment.NewLine + "Thickness, heads, tails, sides, z value" + System.Environment.NewLine;
        System.IO.File.WriteAllText(searchFilename, result);

        float currentThickness = searchStart;                           // starting thickness at knwon lower bound
        float maxThickness = searchEnd;                                 // stop at know upper bound
        float thicknessDelta = searchStep;                              // small increment each iteration

        searchMaxIters = (int)Mathf.Round((searchEnd - searchStart) / searchStep) * searchRepeat;
        searchIters = 0;

        float expected = n / 3f;                                        // expected number of sides
        float variance = Mathf.Sqrt(2 * n / 9f);                        // statistical variance
        float z;                                                        // z value of the current thickness
        float minZ = 20;                                                // samllest z value found
        float bestThickness = currentThickness;                         // thickness of with smalles z value

        int repetitions = searchRepeat;                                 // numer of throws per thickness value
        while (currentThickness < maxThickness)
        {
            int heads = 0;
            int tails = 0;
            int sides = 0;

            for (int i = 0; i < repetitions; i++)
            {
                simFinished = false;
                coinPrefab.transform.localScale = new Vector3(1, currentThickness, 1);
                CalculateScript.thickness = currentThickness;
                StartSimulation();
                //wait till every coin lands
                do
                {
                    yield return null;
                }
                while (!simFinished);

                // add results for the current thickness
                heads += CalculateScript.coinTop;
                tails += CalculateScript.coinBottom;
                sides += CalculateScript.coinSide;

                Reset();
                searchIters++;
            }

            // calculate average reults
            heads /= repetitions;
            tails /= repetitions;
            sides /= repetitions;

            // calculate z value
            z = Mathf.Abs((sides - expected) / variance);

            // store the minimum z value
            if (z < minZ)
            {
                minZ = z;
                bestThickness = currentThickness;
            }

            // save results for the current thicknes
            result = currentThickness.ToString() + ", " + heads.ToString() + ", " + tails.ToString() + ", " + sides.ToString() + ", " + z.ToString() + System.Environment.NewLine;
            System.IO.File.AppendAllText(searchFilename, result);
            currentThickness += thicknessDelta;
        }

        //write results to a file at the end of simulation
        result = "Best thickness found is: " + bestThickness.ToString() + " with a z value of: " + minZ.ToString() + System.Environment.NewLine;
        System.IO.File.AppendAllText(searchFilename, result);
        isSearchRunning = false;
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
