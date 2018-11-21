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
            _serialPort = new SerialPort();
            _serialPort.PortName = port;
            _serialPort.BaudRate = baudrate;
            _serialPort.NewLine = "\n";
            //_serialPort.ReadTimeout = 50;
            //_serialPort.DataReceived += new SerialDataReceivedEventHandler(Port_DataReceived);

            _serialPort.Open();
            if (_serialPort.IsOpen)
            {
                this.state = "connected";
                Debug.Log("Serial Port Opened!");
            }
        }
        catch (Exception e)
        {
            Debug.Log("Error : " + e);
        }
    }

    //private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
    //{
    //    // Show all the incoming data in the port's buffer in the output window
    //    Debug.Log("data : " + _serialPort.ReadExisting());
    //}

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
        if (_serialPort.IsOpen)
        {
            try
            {
                _serialPort.WriteLine("start");
                Debug.Log("Send start message");
                string line = _serialPort.ReadExisting();
                while (line == null || line == string.Empty)
                {
                    _serialPort.WriteLine("start");
                    Debug.Log("waiting for response....");
                    line = _serialPort.ReadExisting();
                }
                //readLine = line;
                state = "communicating";
                Debug.Log("communicate start");

                while (true)
                {
                    readLine = _serialPort.ReadLine();
                }
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
            //_thread.Abort();
            _serialPort.Close();
            state = "disconnected";
            Debug.Log("Serial Port Closed!");
        }
    }
}
    