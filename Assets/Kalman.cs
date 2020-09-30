using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class Kalman {

    // Initializtion =====================================================


        // Initialise 
        public double xhat = Math.PI/2;

        // Initialize P, I, Q, gain
        public double A = 1; //phi in discrete domain
        public double I = 1;
        public double C = 1;
        public double G = 0;
        
        public double gyro_var = 0.00007; //process noise
        public double P = 0.00007*10;
        public double magneto_var = 0.00001; //sensor noise
        public double delT = 0.02;

        public bool isInitialised = false;
        

    public void update(double gyro_data, double magneto_data){

        if(isInitialised == false){
            return;
        }

        // Predict
        xhat = A * xhat + delT*gyro_data;
        P         = A * P * A + gyro_var;
        
        // Update
        //G matrix
        //G         = P  * C' / (C * P * C' + R);
        G = (P * C) / (C * P * C + magneto_var);
        

        //xhat value
        //xhat(:,k) = xhat(:,k) + G * (z(:,k) - C * xhat(:,k));
        //xhat = xhat + g1*(s1 - xhat) + g2*(s2 - xhat)
        xhat = xhat + G*(magneto_data - C*xhat);


        // Update our error as P matrix
        //P         = (I - G * C) * P;
        //P         = -p*(g1 + g2 - 1)
        P = (I - G * C) * P;
    }

    public void init_kalman_values(double initialisedState){
        this.xhat = initialisedState;
    }
}