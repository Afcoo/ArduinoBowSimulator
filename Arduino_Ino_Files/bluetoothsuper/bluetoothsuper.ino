#include<SoftwareSerial.h>
SoftwareSerial BTSerial(2, 3); //tx rx
int flex;
void setup() {
  Serial.begin(115200);
  BTSerial.begin(38400);
}
int tf = 0;
void loop() {
  if(Serial.available()){
    tf = 1;
    Serial.write(Serial.read());
  }
  if(tf == 0)return;
  //Serial.write("!");
  
  if(BTSerial.available()){
    Serial.write(BTSerial.read());
  }
  /*
  if(Serial.available())
    BTSerial.write(Serial.read());*/
}

