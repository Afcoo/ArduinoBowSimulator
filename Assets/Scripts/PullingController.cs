using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullingController : MonoBehaviour {

    public GameObject createArrow;
    public GameObject player;
    public GameObject Aim_Outline;
    public SerialCommunication serial;

    public string SerialLine;

    Quaternion Bow_Rotation;
    public float rotation_x;
    public float rotation_y;
    public float rotation_z;

    public int PullingValue;
    public int PrevPullingValue;
    public int IdlePullingValue = 375;
    public int MaxPullingValueGap = 25;
    public int MaxAimOutlinePixel = 128;
    public int MinAimOUtlinePixel = 32;

    float PrevTime;
    public float SpeedCheckInterval = 0.1f;
    public float PullOutSpeed;
    public float MinShottingPullOutSpeed = 20f; // 화살을 날릴 수 있는 
    public float MaxShottingPullOutSpeed = 140f;
    

    void Start () {
        PrevPullingValue = IdlePullingValue;

        PrevTime = Time.time;
        //GameObject arrow = createArrow.GetComponent<CreateArrow>().CreateNewArrow();
        //GameObject Fletchings = arrow.transform.FindChild("Fletchings").gameObject;
        //Fletchings.GetComponent<Rigidbody>().AddForce(player.transform.forward * 140 * 200f);
    }
	
	// Update is called once per frame
	void Update ()
    {
        //Debug.Log("FPS : " + 1 / Time.deltaTime);
        if (serial.state == "communicating")
        {
            try
            {
                //Debug.Log(serial.readLine);
                SerialLine = serial.readLine;
                string[] parced = SerialLine.Split(',');
                //Debug.Log(parced[0] + ' ' + parced[1] + ' ' + parced[2] + ' ' + parced[3]);

                PullingValue = int.Parse(parced[0]);
                rotation_y = float.Parse(parced[1]);
                rotation_x = float.Parse(parced[2]) * -1;
                rotation_z = float.Parse(parced[3]) * -1;
                //rotation_z = 0f;

                if (Mathf.Abs(PullingValue - PrevPullingValue) < 5)
                {
                    Bow_Rotation.eulerAngles = new Vector3(rotation_x, rotation_y, rotation_z);
                    Vector2 sizeVector = new Vector2(1, 1) * (MaxAimOutlinePixel - (MaxAimOutlinePixel - MinAimOUtlinePixel) * (IdlePullingValue - PullingValue) / MaxPullingValueGap);
                    Aim_Outline.GetComponent<RectTransform>().sizeDelta = sizeVector;
                }
                

                player.transform.rotation = Bow_Rotation;

                if(Time.time - PrevTime > SpeedCheckInterval)
                {
                    PullOutSpeed = (PullingValue - PrevPullingValue) / (Time.time - PrevTime);
                    PrevPullingValue = PullingValue;
                    PrevTime = Time.time;

                    if(PullOutSpeed > MinShottingPullOutSpeed)
                    {
                        ShotArrow(PullOutSpeed);
                        PrevTime += 0.5f;
                    }
                }
                else if(Mathf.Abs(PullingValue - PrevPullingValue) < 5)
                {
                    PrevPullingValue = PullingValue;
                    PrevTime = Time.time;

                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
	}

    void ShotArrow(float speed)
    {
        Debug.Log(speed);
        GameObject arrow = createArrow.GetComponent<CreateArrow>().CreateNewArrow();
        GameObject Fletchings = arrow.transform.FindChild("Fletchings").gameObject;
        Fletchings.GetComponent<Rigidbody>().AddForce(player.transform.forward * speed * 200f);
    }
}
