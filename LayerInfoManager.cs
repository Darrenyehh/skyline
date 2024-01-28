// LayerInfoManager.cs
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public class LayerInfoManager : MonoBehaviour
{
    public int totNodesHit;
    public UIManager uiMan;
    public List<UAMMovement> uamMovements = new List<UAMMovement>();
    public TextMeshProUGUI averageSpeedText;
    public TextMeshProUGUI totalCarsText;
    private GameObject[] uamVehiclesArray;
    public float speedMuilti = 150f;

    void Start()
    {
        uamVehiclesArray = GameObject.FindGameObjectsWithTag("Car");
        InvokeRepeating("UpdateInfo", 0f, 3f);
    }


 private void CalculateAndDisplayLayerInfo()
    {
        uamVehiclesArray = GameObject.FindGameObjectsWithTag("Car");

        if (uamMovements.Count == 0)
            return;

        float totalSpeed = 0;

        foreach (var uamMovement in uamMovements)
        {
            if (uamMovement != null){
            
            float speed = uamMovement.GetCurrentSpeed();
            totalSpeed += speed;
            uamMovement.setSpeedMuilti(speedMuilti);
            }
        }

        // Calculate overall average speed

        float overallAverageSpeed = totalSpeed / uamMovements.Count * speedMuilti / 2.2f;
        int displaySpeed = (int)overallAverageSpeed;

        // Display overall average speed
        averageSpeedText.text = $"Average Speed: {displaySpeed}";
        totalCarsText.text = $"Total Nodes Hit: {totNodesHit}";

    }

    public void SetUAMVehiclesArray(GameObject[] uamVehiclesArray)
    {
        this.uamVehiclesArray = uamVehiclesArray;
        UpdateInfo();
    }

    public void UpdateInfo()
    {
        uamMovements.Clear();
        Debug.Log("info updating");

        foreach (var uamVehicle in uamVehiclesArray)
        {
            // Check if the GameObject is not null and is not destroyed
            if (uamVehicle != null && uamVehicle.activeSelf)
            {
                var movementComponent = uamVehicle.GetComponent<UAMMovement>();
                if (movementComponent != null)
                {
                    uamMovements.Add(movementComponent);
                }
            }
        }

        CalculateAndDisplayLayerInfo();
    }
    public void checkCars()
    {
        uamVehiclesArray = new GameObject[0]; // Clear the array
        uamVehiclesArray = GameObject.FindGameObjectsWithTag("Car");
    }
}