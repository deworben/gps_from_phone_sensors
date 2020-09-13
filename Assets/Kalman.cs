using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class Kalman {

    // Initializtion =====================================================
        public double A = 1;


        // Initialise 
        public double xhat = Math.PI/2;

        // Initialize P, I, Q, gain
        public double P = 1;
        public double I = 1;
        public double Q = 0.005;
        public double g1 = 0;
        public double g2 = 0;
        public double gyro_var = 0.0196;
        public double magneto_var = 0.0369;
        




    


    public void update(double gyro_data, double magneto_data){

        // Predict
        xhat = A * xhat;
        P         = A * P * A + Q;
        
        // Update
        //G matrix
        //G         = P  * C' / (C * P * C' + R);
        //G = [ (b*p)/(a*b + a*p + b*p), (a*p)/(a*b + a*p + b*p)]
        //       g1                    , g2
        g1 =  (magneto_var*P)/(gyro_var*magneto_var + gyro_var*P + magneto_var*P);
        g2 = (gyro_var*P)/(gyro_var*magneto_var + gyro_var*P + magneto_var*P);

        // P matrix
        //P         = (I - G * C) * P;
        //P         = -p*(g1 + g2 - 1)
        P = -P*(g1 + g2 - 1);


        //xhat value
        //xhat(:,k) = xhat(:,k) + G * (z(:,k) - C * xhat(:,k));
        //xhat = xhat + g1*(s1 - xhat) + g2*(s2 - xhat)
        xhat = xhat + g1*(gyro_data - xhat) + g2*(magneto_data - xhat);
    }
}