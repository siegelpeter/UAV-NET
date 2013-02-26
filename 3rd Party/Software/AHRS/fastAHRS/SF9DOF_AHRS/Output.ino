
void printdata(void)
{    
     // Serial.print("!");

     int test1 = ToDeg(roll)*100;
      int test2 = ToDeg(pitch)*100;
      int test3 = ToDeg(yaw)*100;
      int itest1 = AN[sensors[0]];
      int itest2 = AN[sensors[1]];
      int itest3 = AN[sensors[2]];
      int atest1 = ACC[0];
      int atest2 = ACC[1];
      int atest3 = ACC[2];
      int mtest1 = magnetom_x;
      int mtest2= magnetom_y;
      int mtest3 = magnetom_z;
      
       sprintf(com_out_stream, "!ANG:%i,%i,%i,AN:%i,%i,%i,%i,%i,%i,%i,%i,%i\r\n", test1,test2,test3,itest1,itest2,itest3,atest1,atest2,atest3,mtest1,mtest2,mtest3);

      int f = 0;
      while (com_out_stream[f] != '\0')
      {Serial.write(com_out_stream[f]);f++;} 
    
      /*#if PRINT_DCM == 1
      Serial.print (",DCM:");
      Serial.print(convert_to_dec(DCM_Matrix[0][0]));
      Serial.print (",");
      Serial.print(convert_to_dec(DCM_Matrix[0][1]));
      Serial.print (",");
      Serial.print(convert_to_dec(DCM_Matrix[0][2]));
      Serial.print (",");
      Serial.print(convert_to_dec(DCM_Matrix[1][0]));
      Serial.print (",");
      Serial.print(convert_to_dec(DCM_Matrix[1][1]));
      Serial.print (",");
      Serial.print(convert_to_dec(DCM_Matrix[1][2]));
      Serial.print (",");
      Serial.print(convert_to_dec(DCM_Matrix[2][0]));
      Serial.print (",");
      Serial.print(convert_to_dec(DCM_Matrix[2][1]));
      Serial.print (",");
      Serial.print(convert_to_dec(DCM_Matrix[2][2]));
      #endif*/
     //Serial.println();    
      
}

long convert_to_dec(float x)
{
  return x*10000000;
}


