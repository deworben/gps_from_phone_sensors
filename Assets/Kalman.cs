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
        public double A = 1;
        public double P = 1;
        public double I = 1;
        public double C = 1;
        public double G = 0;
        public double Q = 0.005;
        public double gyro_var = 0.0196;
        public double magneto_var = 0.0369;
        public double delT = 0.02;
        




    


    public void update(double gyro_data, double magneto_data){

        // Predict
        xhat = A * xhat + gyro_data*delT;
        P         = A * P * A + Q;
        
        // Update
        //G matrix
        //G         = P  * C' / (C * P * C' + R);
        G = P * C / (C * P * C + gyro_var);
        

        // P matrix
        //P         = (I - G * C) * P;
        //P         = -p*(g1 + g2 - 1)
        P = (I - G * C) * P;


        //xhat value
        //xhat(:,k) = xhat(:,k) + G * (z(:,k) - C * xhat(:,k));
        //xhat = xhat + g1*(s1 - xhat) + g2*(s2 - xhat)
        xhat = xhat + G*(magneto_data - C*xhat);
    }
}