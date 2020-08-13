using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.IO;
using System.Text;
using System;
using System.Linq;

public class IMU : MonoBehaviour
{
    SerialPort sp;

    public static float Euler_Roll;
    public static float Euler_Pitch;
    public static float Euler_Yaw;
    public static float Gyro_Roll;
    public static float Gyro_Pitch;
    public static float Gyro_Yaw;
    public static float Accel_x;
    public static float Accel_y;
    public static float Accel_z;

    public IMU()
    {
        string port = connect();
        sp = new SerialPort(port, 9600);
        

    }

    // Start is called before the first frame update
    void Start()
    {
        //sp.Open();
        //sp.ReadTimeout = 2500; //milliseconds before time-out when read operation does not finish
        
    }

    // Update is called once per frame
    void Update()
    {
        while (false)
        {  //never happens cuz update bad ************************************************************************************
            /*try
            {
                string new_line = sp.ReadLine();//read from the port that the arduino is accessing

                List<string> new_list = new_line.Split().ToList();
                if (new_list.Count >= 11)
                {
                    float temp_Euler_Roll = Convert.ToSingle(new_list[2]);
                    if (temp_Euler_Roll >= 0)
                    {
                        Euler_Roll = temp_Euler_Roll;
                    }
                    else
                    {
                        Euler_Roll = 360 + temp_Euler_Roll;
                    }

                    float temp_Euler_Pitch = Convert.ToSingle(new_list[3]);
                    if (temp_Euler_Pitch >= 0)
                    {
                        Euler_Pitch = temp_Euler_Pitch;
                    }
                    else
                    {
                        Euler_Pitch = 360 + temp_Euler_Pitch;
                    }

                    float temp_Euler_Yaw = Convert.ToSingle(new_list[4]);
                    if (temp_Euler_Yaw >= 0)
                    {
                        Euler_Yaw = temp_Euler_Yaw;
                    }
                    else
                    {
                        Euler_Yaw = 360 + temp_Euler_Yaw;
                    }

                    Gyro_Roll = Convert.ToSingle(new_list[5]);
                    Gyro_Pitch = Convert.ToSingle(new_list[6]);
                    Gyro_Yaw = Convert.ToSingle(new_list[7]);
                    Accel_x = Convert.ToSingle(new_list[8]);
                    Accel_y = Convert.ToSingle(new_list[9]);
                    Accel_z = Convert.ToSingle(new_list[10]);
                }
            }
            catch (Exception e)
            {
                Gyro_Roll = 0f;
                Gyro_Pitch = 0f;
                Gyro_Yaw = 0f;
                Accel_x = 0f;
                Accel_y = 0f;
                Accel_z = 0f;

                // if an exception is thrown(no connection to a port) a reconnection is made here
                string port = connect();
                if (port.Length > 1)
                {
                    sp = new SerialPort(port, 9600);
                    sp.Open();
                    sp.ReadTimeout = 2500;
                }
            }*/
        }
    }

    string connect()
    {//Scan for all available ports, then find the one that receives data starting with "IMU"
        /*
        // Get a list of serial port names.
        string[] ports = SerialPort.GetPortNames();

        Debug.Log("The following serial ports were found:");

        // Display each port name to the console.
        foreach (string port in ports)
        {
            Debug.Log(port);
            
            
        }

        //for (int i = 0; i < ports.Length; i++)
        //{
        //    if (ports[i].Length > 4)
        //    {//All ports with number >= 10 mush have the prefix "\\\\.\\"
        //        string prefix = "\\\\\\\\.\\\\";
        //        ports[i] = prefix + ports[i];
        //    }
        //}

        //string line_test = "";

        foreach (string port in ports)
        {//Try to read data sent by each available ports and determine if they start with "IMU"
            
            Debug.Log(port);

            try 
            {
                SerialPort sp_test = new SerialPort(port, 9600);
                sp_test.ReadTimeout = 500;
                sp_test.Open();
                for (int i = 0; i < 15; i++)
                {
                    try
                    {
                        String line_test = sp_test.ReadLine();
                        Debug.Log("Testing:" + line_test);

                        if (line_test.Length > 4 && line_test[1] == 'I' && line_test[2] == 'M' && line_test[3] == 'U')
                        {
                            sp_test.Close();
                            return port;
                        }
                    }
                    catch (Exception e)
                    {
                        
                    }
                }
                sp_test.Close();
                
            }
            catch (Exception e)
            {
                
            }
        }
        */
        return ("");
    }
}
