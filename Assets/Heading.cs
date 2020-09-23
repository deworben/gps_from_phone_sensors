using System.Collections.Generic;
using UnityEngine.UI;

using System;
using System.Linq;

public class Heading{

    public double temp;
    public double twoDX;
    public double twoDY;
    public double twoDZ;
    public double u_projection_magnitude;
    public double orthogonal_x;
    public double orthogonal_y;
    public double orthogonal_z;

    //Remove the component of u that is orthogonal to the plane
    public double compass_needle_x;
    public double compass_needle_y;
    public double compass_needle_z;
    public double current_heading;

    public Heading(){
        //just so that the constructor has something to do
        temp=0;
    }


    public void updateHeading(double estimate_rotation_about_z, double estimate_rotation_about_x, double gravity_x, double gravity_y, double gravity_z){
        // Create the unit vector with the same orientation as defined by kalman x,y,z
        // Old method (pre 19/09/20)
        // twoDX = Math.Cos(estimate_rotation_about_z);
        // twoDY = Math.Sin(estimate_rotation_about_z);
        // twoDZ = Math.Cos(estimate_rotation_about_x);

        twoDY = Math.Cos(-estimate_rotation_about_x);
        twoDZ = twoDY * Math.Tan(-estimate_rotation_about_x);

        if(estimate_rotation_about_z < 0){
            twoDX = twoDY/( Math.Tan(-estimate_rotation_about_z) );
        }else{
            twoDX = twoDY/( Math.Tan(-estimate_rotation_about_z - (2*Math.PI) ) );
        }
        




        // Project the north vector onto a vector perpendicular to the plane (gravity)
        //kalmanNorthPosition*gravity
        u_projection_magnitude = (twoDX*gravity_x + twoDY*gravity_y + twoDZ*gravity_z)/(Math.Pow(gravity_x, 2)+Math.Pow(gravity_y, 2)+Math.Pow(gravity_z, 2));
        //multiply gravity vector by u_projection_magnitude
        orthogonal_x = u_projection_magnitude*gravity_x;
        orthogonal_y = u_projection_magnitude*gravity_y;
        orthogonal_z = u_projection_magnitude*gravity_z;

        //Remove the component of u that is orthogonal to the plane
        compass_needle_x = twoDX - orthogonal_x;
        compass_needle_y = twoDY - orthogonal_y;
        compass_needle_z = twoDZ - orthogonal_z;
        current_heading = Math.Atan2(compass_needle_x, compass_needle_y);

        if (current_heading<0){
            current_heading = current_heading + 2*Math.PI;
        }



    }
}