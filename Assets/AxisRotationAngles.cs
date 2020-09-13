using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class AxisRotationAngles
{
    public double aboutX;
    public double aboutY;
    public double aboutZ;
    public AxisRotationAngles(){
        aboutX = 0;
        aboutY = 0;
        aboutZ = 0;  
    }

    public void update_compass(double x, double y, double z){
        aboutX = -Math.Atan2(z, y);
        aboutY = -Math.Atan2(z, -x);

        //Z angle
        if (Math.Atan2(y, x) >= 0) {
            aboutZ = -Math.Atan2(y, x); // % Math.PI
        }
        else {
            aboutZ = -(Math.Atan2(y, x) + 2 * Math.PI);
        }
            // aboutZ = Math.Atan2(y, x);
    }
    // public void update
}
