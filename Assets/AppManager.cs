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
    public RectTransform compass_arrow;
    public Text startStopPedometer;
    private Text reset_button;
    public int initialise = 0;
    // public int temp = 0;
    private List<float> tempList;

    //butterworth filter for pedometer
    public FilterButterworth fbwX = new FilterButterworth(15, 50, FilterButterworth.PassType.Highpass, (float)1.414);
    //filters to detect the direction of gravity //0.1, and 2 both work
    public FilterButterworth findGX = new FilterButterworth((float)2 , 50, FilterButterworth.PassType.Lowpass, (float)1.414);
    public FilterButterworth findGY = new FilterButterworth((float)2, 50, FilterButterworth.PassType.Lowpass, (float)1.414);
    public FilterButterworth findGZ = new FilterButterworth((float)2, 50, FilterButterworth.PassType.Lowpass, (float)1.414);
    //Low pass filter to reduce the high frequency noise that exists in the heading estimate
    public FilterButterworth filteredHeading = new FilterButterworth((float)0.9, 50, FilterButterworth.PassType.Lowpass, (float)1.414);

    public Pedometer pedometer = new Pedometer((float)0.05);
    public GyroIntegrator gyroIntegrator = new GyroIntegrator((float)0.02);
    public AxisRotationAngles magnetoAxisRotationAngles = new AxisRotationAngles();
    public Kalman kalmanX = new Kalman();
    public Kalman kalmanY = new Kalman();
    public Kalman kalmanZ = new Kalman();
    public Heading heading = new Heading();
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
        compass_arrow = GameObject.Find("compass_arrow").GetComponent<RectTransform>();
        
    }
    private void OnApplicationQuit()
    {
        dl.CloseLog();
    }

    void FixedUpdate()
    {
        initialise += 1;
        
        // Enable the gyro and compass data
        Input.gyro.enabled = true;
        Input.compass.enabled = true;
        
        // Wait till all the sensors have warmed up before using any readings
        if ((initialise > 2/0.02) && (startStopPedometer.text == "Stop")) {

            //Graphing update values
            // tempList = pedometer.value_history.OfType<float>().ToList();
            // window.updateValues(tempList);
            // filtered_graph.updateValues(tempList);

            
            // Log the time and acceleration data
            string dl_data = System.DateTime.Now.ToString();
            string disp_data = "";
            
            this.updateAllClasses();

            // Display the data to GUI
            //  Pedometer data
            disp_data = "Currently at " + pedometer.numSteps + " steps \n"+
                // "V1: Dist = " + 0.8* pedometer.numSteps + " meters \n" + 
                // "V2: Dist = " + pedometer.distance_travelled + " meters \n" + 
                "Distance travelled:" + pedometer.distance_travelled + " meters \n";
                // "max_pulse_magnitude = " + pedometer.max_pulse_magnitude + " \n\n";
            
            // Orientation data raw from sensors
            // disp_data += string.Format("Gyro data: x: {0:F4}, y: {1:F4}, z: {2:F4}.\n", Input.gyro.rotationRate.x, Input.gyro.rotationRate.y, Input.gyro.rotationRate.z);
            // disp_data += string.Format("Magneto data: x: {0:F4}, y: {1:F4}, z: {2:F4}.\n", Input.compass.rawVector.x, Input.compass.rawVector.y, Input.compass.rawVector.z);
            // // Continuous angles data
            // disp_data += string.Format("Angle about xyz GYRO:\n x: {0:F4}, y: {1:F4}, z: {2:F4}.\n", gyroIntegrator.orientationX, gyroIntegrator.orientationY, gyroIntegrator.orientationZ);
            // disp_data += string.Format("Angle about xyz MAGNETOMETER:\n x: {0:F4}, y: {1:F4}, z: {2:F4}. \n", magnetoAxisRotationAngles.aboutX, magnetoAxisRotationAngles.aboutY, magnetoAxisRotationAngles.aboutZ);
            
            // disp_data += string.Format("Gravity data: x: {0:F4}, y: {1:F4}, z: {2:F4}.\n", findGX.outputHistory[0], findGY.outputHistory[0], findGZ.outputHistory[0]);
            // disp_data += string.Format("Kalman data: x: {0:F4}, y: {1:F4}, z: {2:F4}.\n", kalmanX.xhat, kalmanY.xhat, kalmanZ.xhat);
  
            // Heading data            
            disp_data += string.Format("Heading at: {0:F4}, degrees, filteredHeading @{1:F4}\n",(float)(heading.current_heading*(180/Math.PI)), (float)(filteredHeading.outputHistory[0]*(180/Math.PI)));
            compass_arrow.Rotate( new Vector3( 0, 0, (float)(heading.current_heading*(180/Math.PI) - compass_arrow.eulerAngles.z) ) );
            
            //update compass arrow
            // disp_data += string.Format("heading to go to: {0:F4}, heading currently at: {1:F4}, !\n",(float)(heading.current_heading*(180/Math.PI)), (float)compass_arrow.eulerAngles.z);
            
            // Update text on the screen
            stepCountViewer.GetComponent<UnityEngine.UI.Text>().text = disp_data;





            // data logging data
            // dl_data += "," + pedometer.dist_travelled_this_step + "," + heading.current_heading + ",";
            dl_data += "," + pedometer.dist_travelled_this_step + "," + filteredHeading.outputHistory[0] + ",";
            
            dl.AppendData(dl_data);
        }

    }
    public void updateAllClasses(){
        
        //Update filter for pedometer
        fbwX.Update(Input.acceleration.x);

        // Update using the filter for acceleration data related to gravity finding in X, Y, Z 
        findGX.Update(Input.acceleration.x);
        findGY.Update(Input.acceleration.y);
        findGZ.Update(Input.acceleration.z);

        //Update pedometer
        pedometer.update(fbwX.Value);
        //update our angle of rotation from the magnetometer readings
        magnetoAxisRotationAngles.update_compass(Input.compass.rawVector.x, Input.compass.rawVector.y, Input.compass.rawVector.z);
        // Update the gyro for old method just integrating gyro data
        gyroIntegrator.update_orientation(Input.gyro.rotationRate.x, Input.gyro.rotationRate.y, Input.gyro.rotationRate.z);
        // Initialise the gyro with compass values if need be
        if(!gyroIntegrator.isInitialised){
            gyroIntegrator.init_gyro_values(magnetoAxisRotationAngles.aboutX, magnetoAxisRotationAngles.aboutY, magnetoAxisRotationAngles.aboutZ);
            gyroIntegrator.isInitialised = true;
        }
        //update kalman filter in x, y, z using raw gyro and 
        kalmanX.update(Input.gyro.rotationRate.x, magnetoAxisRotationAngles.aboutX);
        kalmanY.update(Input.gyro.rotationRate.y, magnetoAxisRotationAngles.aboutY);
        kalmanZ.update(Input.gyro.rotationRate.z, magnetoAxisRotationAngles.aboutZ);
        // old method of Kalman using integrated gyro term to compare
        // kalmanZ.update(gyroIntegrator.orientationZ, magnetoAxisRotationAngles.aboutZ);
        

        //update heading
        // heading.updateHeading(magnetoAxisRotationAngles.aboutZ, magnetoAxisRotationAngles.aboutX, findGX.outputHistory[0], findGY.outputHistory[0], findGZ.outputHistory[0]);
        heading.updateHeading(kalmanZ.xhat, kalmanX.xhat, findGX.outputHistory[0], findGY.outputHistory[0], findGZ.outputHistory[0]);
        filteredHeading.Update((float)heading.current_heading);
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
