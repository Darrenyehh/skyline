using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public bool trailExist = true;
    public Slider carsSlider;
    public Slider layersSlider;
    public Slider speedSlider;
    public Toggle trailsToggle;
    public Toggle trafficFlowToggle;
    public GameObject[] uamVehiclesArray;
    public BuildingTopNodeCreator btc;
    public int testingLayer = 3;
    public LayerInfoManager layInfo;


    [SerializeField] private TrafficMonitor trafficMonitor;
    public TextMeshProUGUI tm;
    public TextMeshProUGUI lay;
    public TextMeshProUGUI speedText;
    private int numberOfLayers;
    private int carTotal = 0;

    void Start()
    {
        numberOfLayers = (int)layersSlider.value;
        //InitializeUAMMovementsArray();
    }

    void Update()
    {
        // Update the traffic monitor based on UI interactions
        trafficMonitor.SetNumberOfCars((int)carsSlider.value);
        trafficMonitor.SetNumberOfLayers((int)layersSlider.value);

        //LayerInfoManager.UpdateUAMMovementsArray(uamMovements);
        //// Update stats display
        //carsText.text = "Total Cars: " + trafficMonitor.GetTotalCars();
        //speedText.text = "Average Speed: " + trafficMonitor.GetAverageSpeed();
    }

    public void summon()
    {
        if (layInfo != null)
    {
        layInfo.speedMuilti = speedSlider.value * 2;
        layInfo.checkCars();
        layInfo.UpdateInfo();
    }
    else
    {
        Debug.LogError("layInfo is null. Please assign a valid reference in the Unity Editor.");
        return;
    }

    CheckCars();
        //btc.numberOfLayers = testingLayer;
        if (numberOfLayers != (int)layersSlider.value && carTotal != (int)carsSlider.value)
        {
            Debug.Log(numberOfLayers +" : "+ (int)layersSlider.value);
            Debug.Log(carTotal +" : "+ (int)carsSlider.value);

            btc.numberOfTallestBuildings = (int)layersSlider.value;
            btc.CreateNodesAtTallestBuildings(btc.maxHeight);
            carTweak();
            Debug.Log("case 1");
            // add logic to change the number of POI

        }
        else if(numberOfLayers != (int)layersSlider.value){
            btc.numberOfTallestBuildings = (int)layersSlider.value;
            btc.CreateNodesAtTallestBuildings(btc.maxHeight);
            Debug.Log("case 2");
   
        }
        else
        {
            carTweak();
            Debug.Log("case 3");

        }
    }

    private void carTweak()
    {
        CheckCars();
        if (carTotal < (int)carsSlider.value)
        {
            btc.numberOfVehicles = (int)carsSlider.value;
            btc.InitializeUAMVehicles(carTotal);
        }
        else if (carTotal > (int)carsSlider.value)
        {
            delete(carTotal - (int)carsSlider.value);
        }
    }

    private void delete(int amountToDelete)
    {
        for (int i = 0; i < amountToDelete && i < uamVehiclesArray.Length; i++)
        {
            Destroy(uamVehiclesArray[carTotal - i]);
        }
        CheckCars();
    }


    public void CheckCars()
    {
        // check the current total of cars
        uamVehiclesArray = GameObject.FindGameObjectsWithTag("Car");
        carTotal = uamVehiclesArray.Length - 1;
    }

    public void OnToggleTrails(bool isOn)
    {
        trafficMonitor.ToggleTrails(isOn);
    }

    public void OnToggleTrafficFlow(bool isOn)
    {
        trafficMonitor.ToggleTrafficFlow(isOn);
    }

    public void showU()
    {
        int temp = (int)carsSlider.value;
        tm.text = temp + "";
    }

    public void showL()
    {
        int temp = (int)layersSlider.value;
        lay.text = temp + "";
    }
    public void showSpeed(){
        int temp = (int)speedSlider.value;
        speedText.text = temp + "mph";
    }
    public void changeTrails(){
         trailExist = trailExist ? false : true;
    }
    public GameObject[] getCars(){
        return uamVehiclesArray;
    }
}