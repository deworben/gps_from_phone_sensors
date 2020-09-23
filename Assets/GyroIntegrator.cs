using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class GyroIntegrator
{
    public float orientationX;
    public float orientationY;
    public float orientationZ;
    public bool isInitialised = false;
    private float sample_rate;
    
    public GyroIntegrator(float sr) {
        sample_rate = sr;
        orientationX = 0;
        orientationY = 0;
        orientationZ = 0;
    }

    public void update_orientation(float omegaX, float omegaY, float omegaZ){
        if(isInitialised){
            this.orientationX = this.orientationX + this.sample_rate*omegaX; 
            this.orientationY = this.orientationY + this.sample_rate*omegaY; 
            this.orientationZ = this.orientationZ + this.sample_rate*omegaZ;

            this.orientationX = this.orientationX % (float)(2* Math.PI);
            this.orientationY = this.orientationY % (float)(2* Math.PI);
            this.orientationZ = this.orientationZ % (float)(2* Math.PI);
        }else{

        } 
    }

    public void init_gyro_values(double omegaX, double omegaY, double omegaZ){
        orientationX = (float)omegaX;
        orientationY = (float)omegaY;
        orientationZ = (float)omegaZ;
    }
}
