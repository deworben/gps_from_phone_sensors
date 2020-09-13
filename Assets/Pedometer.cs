using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Pedometer
{
    public float threshold;
    public float[] value_history = new float[13];
    public float[] step_flag_memory = new float[7];
    public bool inStep = false;
    public int numSteps = 0;
    public float distance_travelled = 0;
    public float max_pulse_magnitude = 0;

    public Pedometer(float threshold) {
        this.threshold = threshold;
        step_flag_memory[0] = 0;
        step_flag_memory[1] = 0;
        step_flag_memory[2] = 0;
        step_flag_memory[3] = 0;
        step_flag_memory[4] = 0;
        step_flag_memory[5] = 0;
        step_flag_memory[6] = 0;


    }

    public void update(float value) {
        
        value_history[12] = value_history[11];
        value_history[11] = value_history[10];
        value_history[10] = value_history[9];
        value_history[9] = value_history[8];
        value_history[8] = value_history[7];
        value_history[7] = value_history[6];
        value_history[6] = value_history[5];
        value_history[5] = value_history[4];
        value_history[4] = value_history[3];
        value_history[3] = value_history[2];
        value_history[2] = value_history[1];
        value_history[1] = value_history[0];
        value_history[0] = value;


        // if (inStep == true){

        // }


        //if the current value is bigger in magnitude than threshold, 
        //this timestep has value=1 for step_flag_memory.Move everything else back one spot
        if (value > this.threshold || -value > this.threshold)
        {
            update_step_flag_memory(1);
            if(Math.Abs(value)>this.max_pulse_magnitude){
                this.max_pulse_magnitude = Math.Abs(value);
            }
        }
        else {
            update_step_flag_memory(0);
        }

        // add up step_flag_memory to see if were in step
        float sum = 0;
        Array.ForEach(this.step_flag_memory, i => sum += i);
        //Debug.Log(sum);

        //if sum>2.9 (there's at least 1 threshold breach in the last 3 timesteps,
        //then we're mid-step. 
        if (sum > 4.9)
        {
            //Increment step counter on a rising edge
            if (this.inStep == false)
            {
                this.numSteps++;
                //Debug.Log("Stepped!:\n");
                //Debug.Log("Number of steps = " + this.numSteps);
                //Debug.Log("\n----------------------------\n"); 2.1...0.05
                this.distance_travelled = (float)(this.distance_travelled + (1.5)*this.max_pulse_magnitude + 0.35);
                max_pulse_magnitude = 0;
            }
            this.inStep = true;
        }
        else {
            this.inStep = false;
        }

    }

    private void update_step_flag_memory(int stepFlag)
    {
        step_flag_memory[6] = step_flag_memory[5];
        step_flag_memory[5] = step_flag_memory[4];
        step_flag_memory[4] = step_flag_memory[3];
        step_flag_memory[3] = step_flag_memory[2];
        step_flag_memory[2] = step_flag_memory[1];
        step_flag_memory[1] = step_flag_memory[0];
        step_flag_memory[0] = stepFlag;
    }

    public bool Value
    {
        get { return this.inStep; }
    }
    public void reset_step_count()
    {
        this.numSteps = 0;
        this.inStep = false;
    }


}
