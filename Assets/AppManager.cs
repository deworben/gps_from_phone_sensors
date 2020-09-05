/*
 * MCEN90032 Sensor Systems.
 * Benjamin De Worsop 913844
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class AppManager : MonoBehaviour

{
    public GameObject stepCountViewer;
    public Text startStopPedometer;
    public int initialise = 0;
    public int temp = 0;
    private List<float> tempList;


    public FilterButterworth fbwX = new FilterButterworth(15, 50, FilterButterworth.PassType.Highpass, (float)1.414);
    public Pedometer pedometer = new Pedometer((float)0.05);

    public DataLogger dl = new DataLogger();
    // public Window_Graph window = new Window_Graph();
    public Window_Graph window;

    public void Start()
    { 
        dl.InitialiseLogger();
        window = GameObject.Find("window_graph").GetComponent<Window_Graph>();
        startStopPedometer = GameObject.Find("startStopPedometer").GetComponent<Text>();
        
    }
    private void OnApplicationQuit()
    {
        dl.CloseLog();
    }

    void FixedUpdate()
    {
        initialise += 1;
        
        if ((initialise > 2 / 0.02) && (startStopPedometer.text == "stop")) {

            //Graph update values
            tempList = pedometer.value_history.OfType<float>().ToList();
            window.updateValues(tempList);

            // Log the time and acceleration data
            string data = System.DateTime.Now.ToString();
            
            fbwX.Update(Input.acceleration.x);
            pedometer.update(fbwX.Value);
            //data = data + "   " + pedometer.numSteps;
            //data = data + "   " + Input.acceleration.x;
            //stepCountViewer.GetComponent<UnityEngine.UI.Text>().text = data;

            stepCountViewer.GetComponent<UnityEngine.UI.Text>().text = "Currently at " + pedometer.numSteps + " steps \n" +
                "totally travelled " + 0.65* pedometer.numSteps + " meters";

            if (pedometer.inStep == false) {
                temp = 0;
            }
            else {
                temp = 1;
            }
            //data += "," + Input.acceleration.x + "," + Input.acceleration.y + "," + Input.acceleration.z + "," + fbwX.Value + "," + fbwY.Value + "," + fbwZ.Value + ",";
            data += "," + Input.acceleration.x + "," + fbwX.outputHistory[0] + "," + temp + ",";
            // print(Input.acceleration.x);
            dl.AppendData(data);
        }

    }
    public void startAndStopPedometer(){
        if(startStopPedometer.text == "start"){
            startStopPedometer.text = "stop";
        }else{
            startStopPedometer.text = "start";
        }
    }

}
