using System;
using System.IO.Ports;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class SerialCommunication : MonoBehaviour {

    public string port = "COM10";
    public int baudrate = 9600;
    public string readLine;
    public string state = "disconnected";

    static SerialPort _serialPort;
    Thread _thread;

	void Start () {
        Connect();
        if(state == "connected")
        {
            StartCommuncation();
        }
    }
	
    public void Connect()
    {
        try
        {
            _serialPort = new SerialPort(port, baudrate);
            _serialPort.Open();
            if (_serialPort.IsOpen)
            {
                this.state = "connected";
                Debug.Log("Serial Port Opened!");
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public void StartCommuncation()
    {
        try
        {
            //_serialPort.Write("start");
            _thread = new Thread(SerialGetLIne);
            _thread.Start();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public void StopCommunication()
    {
        _thread.Abort();
        _serialPort.Close();
        state = "disconnected";
    }
    
    void SerialGetLIne()
    {
        state = "communicating";
        while(_serialPort.IsOpen)
        {
            try
            {
                //Debug.Log(readLine);
                readLine = _serialPort.ReadLine();
            }
            catch (Exception e)
            {
                _serialPort.Close();
                state = "disconnected";
                Debug.Log(e);
                throw e;
            }
        }
    }

    void OnDestroy()
    {
        if (_serialPort != null && _serialPort.IsOpen)
        {
            _thread.Abort();
            _serialPort.Close();
            state = "disconnected";
            Debug.Log("Serial Port Closed!");
        }
    }
}
    