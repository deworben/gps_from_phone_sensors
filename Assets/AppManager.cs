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
    private Text reset_button;
    public int initialise = 0;
    // public int temp = 0;
    private List<float> tempList;


    public FilterButterworth fbwX = new FilterButterworth(15, 50, FilterButterworth.PassType.Highpass, (float)1.414);
    public FilterButterworth findGX = new FilterButterworth(0, 2, FilterButterworth.PassType.Lowpass, (float)1.414);
    public FilterButterworth findGY = new FilterButterworth(0, 1, FilterButterworth.PassType.Lowpass, (float)1.414);
    public FilterButterworth findGZ = new FilterButterworth((float)0.2, 3, FilterButterworth.PassType.Lowpass, (float)1.414);

    public Pedometer pedometer = new Pedometer((float)0.05);
    public Gyro_first_attempt gyro_first_attempt = new Gyro_first_attempt((float)0.02);
    public Compass compass = new Compass();
    public Kalman kalman = new Kalman();
    public DataLogger dl = new DataLogger();


    // Live graphs
    // public Window_Graph window = new Window_Graph();
    // public Window_Graph window;
    // public Window_Graph filtered_graph;

    public void Start()
    { 
        dl.InitialiseLogger();
        // window = GameObject.Find("window_graph").GetComponent<Window_Graph>();
        // filtered_graph = GameObject.Find("filtered_graph").GetComponent<Window_Graph>();
        startStopPedometer = GameObject.Find("startStopPedometer").GetComponent<Text>();
        reset_button = GameObject.Find("reset").GetComponent<Text>();
        
    }
    private void OnApplicationQuit()
    {
        dl.CloseLog();
    }

    void FixedUpdate()
    {
        initialise += 1;
        // double aboutX;
        // double aboutY;
        // double aboutZ;

        // Enable the gyro and compass data
        Input.gyro.enabled = true;
        Input.compass.enabled = true;
        
        if ((initialise > 2/0.02) && (startStopPedometer.text == "Stop")) {

            //Graphing update values
            // tempList = pedometer.value_history.OfType<float>().ToList();
            // window.updateValues(tempList);
            // filtered_graph.updateValues(tempList);

            
            // Log the time and acceleration data
            string data = System.DateTime.Now.ToString();
            string disp_data = "";
            

            //Update filter
            fbwX.Update(Input.acceleration.x);

            // Find g
            findGX.Update(Input.acceleration.x);
            findGY.Update(Input.acceleration.y);
            findGZ.Update(Input.acceleration.z);

            //Update pedometer
            pedometer.update(fbwX.Value);
            //update the compass
            compass.update_compass(Input.compass.rawVector.x, Input.compass.rawVector.y, Input.compass.rawVector.z);
            // Update the gyro
            gyro_first_attempt.update_orientation(Input.gyro.rotationRate.x, Input.gyro.rotationRate.y, Input.gyro.rotationRate.z);
            // Initialise the gyro with compass values if need be
            if(!gyro_first_attempt.isInitialised){
                gyro_first_attempt.init_gyro_values(compass.aboutX, compass.aboutY, compass.aboutZ);
                gyro_first_attempt.isInitialised = true;
            }
            //update kalman
            kalman.update(gyro_first_attempt.orientationY, compass.aboutY);
            

            // Display the data to GUI
            //  Pedometer data
            disp_data = "Currently at " + pedometer.numSteps + " steps \n" +
                "V1: Dist = " + 0.8* pedometer.numSteps + " meters \n" + 
                "V2: Dist = " + pedometer.distance_travelled + " meters \n" + 
                "max_pulse_magnitude = " + pedometer.max_pulse_magnitude + " \n" + 
                "kalman at = " + kalman.xhat + " \n" + 
                "";
            disp_data += string.Format("Gyro data: x: {0:F4}, y: {1:F4}, z: {2:F4}.\n", Input.gyro.rotationRate.x, Input.gyro.rotationRate.y, Input.gyro.rotationRate.z);
            disp_data += string.Format("Compass data: x: {0:F4}, y: {1:F4}, z: {2:F4}.\n\n", Input.compass.rawVector.x, Input.compass.rawVector.y, Input.compass.rawVector.z);


            //   Continuous angles data
            disp_data += string.Format("Gyro angles: x: {0:F4}, y: {1:F4}, z: {2:F4}.\n", gyro_first_attempt.orientationX, gyro_first_attempt.orientationY, gyro_first_attempt.orientationZ);
            disp_data += string.Format("Compass angles: x: {0:F4}, y: {1:F4}, z: {2:F4}. \n", compass.aboutX, compass.aboutY, compass.aboutZ);
            
            // Update text on the screen
            stepCountViewer.GetComponent<UnityEngine.UI.Text>().text = disp_data;





            // In step to 1, 0 converter
            // if (pedometer.inStep == false) {
            //     temp = 0;
            // }
            // else {
            //     temp = 1;
            // }
            
            data += "," + findGX.outputHistory[0]  + "," + findGY.outputHistory[0]  + "," + findGZ.outputHistory[0];
            // data += "," + gyro_first_attempt.orientationX + "," + gyro_first_attempt.orientationY + "," + gyro_first_attempt.orientationZ + "," + compass.aboutX + "," + compass.aboutY + "," + compass.aboutZ + ",";
            // data += "," + Input.acceleration.x + "," + fbwX.outputHistory[0] + "," + temp + ",";
            dl.AppendData(data);
        }

    }
    public void startAndStopPedometer(){
        if(startStopPedometer.text == "Start"){
            startStopPedometer.text = "Stop";
        }else{
            startStopPedometer.text = "Start";
        }
    }
    public void reset_pedometer(){
        this.pedometer.reset_step_count();
    }

}
