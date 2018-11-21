#include<SoftwareSerial.h>
SoftwareSerial BTSerial(11, 12); //tx rx

#include "I2Cdev.h"
#include "MPU6050_6Axis_MotionApps20.h"
#if I2CDEV_IMPLEMENTATION == I2CDEV_ARDUINO_WIRE
    #include "Wire.h"
#endif

MPU6050 mpu;

#define INTERRUPT_PIN 2  // use pin 2 on Arduino Uno & most boards  
bool blinkState = false;

bool dmpReady = false;  // set true if DMP init was successful
uint8_t mpuIntStatus;   // holds actual interrupt status byte from MPU
uint8_t devStatus;      // return status after each device operation (0 = success, !0 = error)
uint16_t packetSize;    // expected DMP packet size (default is 42 bytes)
uint16_t fifoCount;     // count of all bytes currently in FIFO
uint8_t fifoBuffer[64]; // FIFO storage buffer

Quaternion q;           // [w, x, y, z]         quaternion container
VectorInt16 aa;         // [x, y, z]            accel sensor measurements
VectorInt16 aaReal;     // [x, y, z]            gravity-free accel sensor measurements
VectorInt16 aaWorld;    // [x, y, z]            world-frame accel sensor measurements
VectorFloat gravity;    // [x, y, z]            gravity vector
float euler[3];         // [psi, theta, phi]    Euler angle container
float ypr[3];           // [yaw, pitch, roll]   yaw/pitch/roll container and gravity vector

uint8_t teapotPacket[14] = { '$', 0x02, 0,0, 0,0, 0,0, 0,0, 0x00, 0x00, '\r', '\n' };

volatile bool mpuInterrupt = false;     // indicates whether MPU interrupt pin has gone high
void dmpDataReady() {
    mpuInterrupt = true;
}
void setup() {
    BTSerial.begin(38400);

    #if I2CDEV_IMPLEMENTATION == I2CDEV_ARDUINO_WIRE
        Wire.begin();
        Wire.setClock(400000); // 400kHz I2C clock. Comment this line if having compilation difficulties
    #elif I2CDEV_IMPLEMENTATION == I2CDEV_BUILTIN_FASTWIRE
        //Fastwire::setup(400, true);
    #endif

    Serial.begin(115200);
    
    mpu.initialize();
    pinMode(INTERRUPT_PIN, INPUT);
    
    devStatus = mpu.dmpInitialize();

    // supply your own gyro offsets here, scaled for min sensitivity
    mpu.setXGyroOffset(220);
    mpu.setYGyroOffset(76);
    mpu.setZGyroOffset(-85);
    mpu.setZAccelOffset(1788); 
    
    if (devStatus == 0) {
        mpu.setDMPEnabled(true);
        attachInterrupt(digitalPinToInterrupt(INTERRUPT_PIN), dmpDataReady, RISING);
        mpuIntStatus = mpu.getIntStatus();
        dmpReady = true;
        packetSize = mpu.dmpGetFIFOPacketSize();
    }
    else { }
}

void loop() {
    
    if (!dmpReady){
      Serial.println("Error");
      return;
    }
    while (!mpuInterrupt && fifoCount < packetSize) {
        if (mpuInterrupt && fifoCount < packetSize) {
          fifoCount = mpu.getFIFOCount();
        }  
    }
    mpuInterrupt = false;
    mpuIntStatus = mpu.getIntStatus();

    fifoCount = mpu.getFIFOCount();

    if ((mpuIntStatus & _BV(MPU6050_INTERRUPT_FIFO_OFLOW_BIT)) || fifoCount >= 1024) {
        mpu.resetFIFO();
        fifoCount = mpu.getFIFOCount();
    }
    else if (mpuIntStatus & _BV(MPU6050_INTERRUPT_DMP_INT_BIT)) {
      while (fifoCount < packetSize) fifoCount = mpu.getFIFOCount();
     mpu.getFIFOBytes(fifoBuffer, packetSize);
     fifoCount -= packetSize;
     
     // display quaternion values in easy matrix form: w x y z
     mpu.dmpGetQuaternion(&q, fifoBuffer);
     /*
     Serial.print(q.w);
     Serial.print(",");
     Serial.print(q.x);
     Serial.print(",");
     Serial.print(q.y);
     Serial.print(",");
     Serial.println(q.z);
*/
     mpu.dmpGetQuaternion(&q, fifoBuffer);
     mpu.dmpGetGravity(&gravity, &q);
     mpu.dmpGetYawPitchRoll(ypr, &q, &gravity);
     /*
     Serial.print(ypr[0] * 180/M_PI);
     Serial.print(",");
     Serial.print(ypr[1] * 180/M_PI);
     Serial.print(",");
     Serial.println(ypr[2] * 180/M_PI);*/

     char str_x[10]; char str_y[10]; char str_z[10];
     dtostrf(ypr[0] * 180/M_PI, 3, 2, str_x);
     dtostrf(ypr[1] * 180/M_PI, 3, 2, str_y);
     dtostrf(ypr[2] * 180/M_PI, 3, 2, str_z);

     char output[100];
     sprintf(output, "%d,%s,%s,%s\n", analogRead(A2), str_x, str_y, str_z);
     Serial.print(output);
     
     BTSerial.write(output);
     // delay(10);
    }
}
