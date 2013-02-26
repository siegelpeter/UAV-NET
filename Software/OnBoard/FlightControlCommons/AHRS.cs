using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UAVCommons;
using System.Threading;
using UAVCommons;

namespace FlightControlCommons
{
    /// <summary>
    /// An AHRS Sensor Object connected via USB
    /// </summary>
        [Serializable]
    public class AHRS : UAVCommons.UAVStructure, IHardwareConnectable
    {
        /// <summary>
        /// Communications port of the AHRS Razor IMU
        /// e.g. /dev/ttyUSB1
        /// </summary>
        string port;
        [NonSerialized]
        SerialPort port1 = null;
        [NonSerialized]		// Serialisieren heist abspeichern in einen String  Markiert die Nachfolgende Variable als nicht speicherbar
        Thread myworker = null;
        private int urate = 50;// Hilfvariable damit die UpdateRate der UAVParameter phi bis gyroalphastich unten auf einmal geändert werden kann 
		
	        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">name of the Device e.g. AHRS</param>
        /// <param name="port">Port used to connect to the device e.g. Com2 / /dev/ttyUSB1</param>
        public AHRS(string name, string port)
            : base(name, "")
        {
            this.port = port;
            values.Add(new UAVParameter("phi"  , 0, -180, 180, urate)); //Maximale Übertragungsrate zum Boden = urate 
            values.Add(new UAVParameter("theta", 0,  -90,  90, urate));
            values.Add(new UAVParameter("psi"  , 0, -180, 180, urate));

            values.Add(new UAVParameter("accx", 0, -1000, 1000, urate));
            values.Add(new UAVParameter("accy", 0, -1000, 1000, urate));
            values.Add(new UAVParameter("accz", 0, -1000, 1000, urate));

            values.Add(new UAVParameter("magx", 0, -1000, 1000, urate));
            values.Add(new UAVParameter("magy", 0, -1000, 1000, urate));
            values.Add(new UAVParameter("magz", 0, -1000, 1000, urate));

            values.Add(new UAVParameter("gyrogammastrich", 0, -100, 100, urate));	//values.Add("altitude", new UAVParameter("altitude", 0));
            values.Add(new UAVParameter("gyrobetastrich",  0, -100, 100, urate));
            values.Add(new UAVParameter("gyroalphastrich", 0, -100, 100, urate));

            values.Add(new UAVParameter("oldsettings", "", null, null, urate));

            values.Add(new UAVParameter("cursettings", "", null, null, urate));

          
        }

        /// <summary>
        /// Connect to the Hardware and start the Backgroundjob which reads the Values
        /// </summary>
        public void ConnectHardware()
        {
            port1 = new SerialPort(port, 115200);
            port1.NewData += new SerialPort.NewDataRowHandler(port1_NewData);
            port1.currentsettings = new List<string>();
            if (values["cursettings"].Value.ToString().Split (',').Length == 6)
            {
				Console.WriteLine("Setting Razor Settings");
                port1.currentsettings.AddRange(values["cursettings"].Value.ToString().Split(','));
            }
            myworker = new Thread(new ThreadStart(ReadValues));
            myworker.Start();
        }

        /// <summary>
        /// Backgroundjob which reads the values from the Com Port
        /// </summary>
        private void ReadValues() {
            port1.Start();
        }

        /// <summary>
        /// Event Handler which is fired then new Data arrives from the Device
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void port1_NewData(object sender, List<UAVParameter> e)
        {

            if ((e != null)&&(e.Count == 12))  // wo ist der Zähler versteckt ????
				{
				//Console.WriteLine(e[0].Value+"|"+e[0].DoubleValue);
                values["phi"].Value 	= e[0].DoubleValue / 100; //X Längsachse Roll phi
                values["theta"].Value = -e[1].DoubleValue / 100; //Y Querachse Pitch theta
                values["psi"].Value = -e[2].DoubleValue / 100; //Z Hochachse Heading psi

                values["gyrogammastrich"].Value	= (int)Tools.Limit(((double)e[5].Value-372)/372*100,values["gyrogammastrich"].MinDoubleValue,values["gyrogammastrich"].MaxDoubleValue);
                values["gyrobetastrich"].Value 	= (int)Tools.Limit(((double)e[4].Value-381)/381*100,values["gyrobetastrich"].MinDoubleValue , values["gyrobetastrich"].MaxDoubleValue );	
                values["gyroalphastrich"].Value = (int)Tools.Limit(((double)e[3].Value-378)/378*100,values["gyroalphastrich"].MinDoubleValue, values["gyroalphastrich"].MaxDoubleValue);
        
                values["accx"].Value = e[6].Value;
                values["accy"].Value = e[7].Value;
                values["accz"].Value = e[8].Value;

                values["magx"].Value = e[9].Value;
                values["magy"].Value = e[10].Value;
                values["magz"].Value = e[11].Value;
                FireDataRecievedEvent(this,false);
            	}
			else{
                if (e == null) {
                    values["oldsettings"].updateRate = 0;
                    values["oldsettings"].Value = String.Join(",", port1.list);
                    return;
                }
                Console.WriteLine("Found " + e.Count + " AHRS-channels -> Wrong Firmware Version!");}
        }



//#define GRAVITY
////C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
////ORIGINAL LINE: #define Accel_Scale(x) x*(GRAVITY/9.81)
//#define Accel_Scale
////C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
////ORIGINAL LINE: #define ToRad(x) (x*0.01745329252)
//#define ToRad
////C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
////ORIGINAL LINE: #define ToDeg(x) (x*57.2957795131)
//#define ToDeg
//#define Gyro_Gain_X
//#define Gyro_Gain_Y
//#define Gyro_Gain_Z
////C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
////ORIGINAL LINE: #define Gyro_Scaled_X(x) x*(Gyro_Gain_X*0.01745329252)
//#define Gyro_Scaled_X
////C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
////ORIGINAL LINE: #define Gyro_Scaled_Y(x) x*(Gyro_Gain_Y*0.01745329252)
//#define Gyro_Scaled_Y
////C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
////ORIGINAL LINE: #define Gyro_Scaled_Z(x) x*(Gyro_Gain_Z*0.01745329252)
//#define Gyro_Scaled_Z
//#define Kp_ROLLPITCH
//#define Ki_ROLLPITCH
//#define Kp_YAW
//#define Ki_YAW
//#define OUTPUTMODE
//#define PRINT_ANALOGS
//#define PRINT_EULER
//#define ADC_WARM_CYCLES
//#define STATUS_LED

//using System;

//// ADXL345 Sensitivity(from datasheet) => 4mg/LSB   1G => 1000mg/4mg = 256 steps
//// Tested value : 248


//// LPR530 & LY530 Sensitivity (from datasheet) => (3.3mv at 3v)at 3.3v: 3mV/º/s, 3.22mV/ADC step => 0.93
//// Tested values : 0.92


///*For debugging purposes*/
////OUTPUTMODE=1 will print the corrected data, 
////OUTPUTMODE=0 will print uncorrected data of the gyros (with drift)

////#define PRINT_DCM 0     //Will print the whole direction cosine matrix


//private string sensors = {1,2,0}; // Map the ADC channels gyro_x, gyro_y, gyro_z
//private int[] SENSOR_SIGN = {-1,1,-1,1,1,1,-1,-1,-1}; //Correct directions x,y,z - gyros, accels, magnetormeter

//private float G_Dt = 0.02F; // Integration time (DCM algorithm)  We will run the integration loop at 50Hz if possible

//private int timer = 0; //general purpuse timer
//private int timer_old;
//private int timer24 = 0; //Second timer used to print values
//private int[] AN = new int[6]; //array that store the 3 ADC filtered data (gyros)
//private int[] AN_OFFSET = {0,0,0,0,0,0}; //Array that stores the Offset of the sensors
//private int[] ACC = new int[3]; //array that store the accelerometers data

//private int accel_x;
//private int accel_y;
//private int accel_z;
//private int magnetom_x;
//private int magnetom_y;
//private int magnetom_z;
//private float MAG_Heading;

//private float[] Accel_Vector = {0,0,0}; //Store the acceleration in a vector
//private float[] Gyro_Vector = {0,0,0}; //Store the gyros turn rate in a vector
//private float[] Omega_Vector = {0,0,0}; //Corrected Gyro_Vector data
//private float[] Omega_P = {0,0,0}; //Omega Proportional correction
//private float[] Omega_I = {0,0,0}; //Omega Integrator
//private float[] Omega = {0,0,0};

//// Euler angles
//private float roll;
//private float pitch;
//private float yaw;

//private float[] errorRollPitch = {0,0,0};
//private float[] errorYaw = {0,0,0};

//private uint counter = 0;
//private byte gyro_sat = 0;

//private float[,] DCM_Matrix = {{1,0,0},{0,1,0},{0,0,1}};
//private float[,] Update_Matrix = {{0,1,2},{3,4,5},{6,7,8}}; //Gyros here


//private float[,] Temporary_Matrix = {{0,0,0},{0,0,0},{0,0,0}};


//            /**************************************************/
////Multiply two 3x3 matrixs. This function developed by Jordi can be easily adapted to multiple n*n matrix's. (Pero me da flojera!). 
//private void Matrix_Multiply(float[,] a, float[,] b, float[,] mat)
//{
//  float[] op = new float[3];
//  for (int x = 0; x < 3; x++)
//  {
//    for (int y = 0; y < 3; y++)
//    {
//      for (int w = 0; w < 3; w++)
//      {
//       op[w] = a[x, w] * b[w, y];
//      }
//      mat[x, y] = 0F;
//      mat[x, y] = op[0] + op[1] + op[2];

//      float test = mat[x, y];
//    }
//  }
//}

////Computes the dot product of two vectors
//private float Vector_Dot_Product(float[] vector1, float[] vector2)
//{
//  float op = 0F;

//  for (int c = 0; c < 3; c++)
//  {
//  op += vector1[c] * vector2[c];
//  }

//  return op;
//}

////Computes the cross product of two vectors
//private void Vector_Cross_Product(float[] vectorOut, float[] v1, float[] v2)
//{
//  vectorOut[0] = (v1[1] * v2[2]) - (v1[2] * v2[1]);
//  vectorOut[1] = (v1[2] * v2[0]) - (v1[0] * v2[2]);
//  vectorOut[2] = (v1[0] * v2[1]) - (v1[1] * v2[0]);
//}

////Multiply the vector by a scalar. 
//private void Vector_Scale(float[] vectorOut, float[] vectorIn, float scale2)
//{
//  for (int c = 0; c < 3; c++)
//  {
//   vectorOut[c] = vectorIn[c] * scale2;
//  }
//}

//private void Vector_Add(float[] vectorOut, float[] vectorIn1, float[] vectorIn2)
//{
//  for (int c = 0; c < 3; c++)
//  {
//     vectorOut[c] = vectorIn1[c] + vectorIn2[c];
//  }
//}



///**************************************************/
//private void Normalize()
//{
//  float error = 0F;
//  float[,] temporary = new float[3, 3];
//  float renorm = 0F;

//  error = -Vector_Dot_Product(DCM_Matrix[0, 0], DCM_Matrix[1, 0]) * .5; //eq.19

//  Vector_Scale(temporary[0, 0], DCM_Matrix[1, 0], error); //eq.19
//  Vector_Scale(temporary[1, 0], DCM_Matrix[0, 0], error); //eq.19

//  Vector_Add(temporary[0, 0], temporary[0, 0], DCM_Matrix[0, 0]); //eq.19
//  Vector_Add(temporary[1, 0], temporary[1, 0], DCM_Matrix[1, 0]); //eq.19

//  Vector_Cross_Product(temporary[2, 0], temporary[0, 0], temporary[1, 0]); // c= a x b //eq.20

//  renorm = .5 * (3 - Vector_Dot_Product(temporary[0, 0], temporary[0, 0])); //eq.21
//  Vector_Scale(DCM_Matrix[0, 0], temporary[0, 0], renorm);

//  renorm = .5 * (3 - Vector_Dot_Product(temporary[1, 0], temporary[1, 0])); //eq.21
//  Vector_Scale(DCM_Matrix[1, 0], temporary[1, 0], renorm);

//  renorm = .5 * (3 - Vector_Dot_Product(temporary[2, 0], temporary[2, 0])); //eq.21
//  Vector_Scale(DCM_Matrix[2, 0], temporary[2, 0], renorm);
//}
////C++ TO C# CONVERTER NOTE: This was formerly a static local variable declaration (not allowed in C#):
//private float[] Drift_correction_Scaled_Omega_P = new float[3];
////C++ TO C# CONVERTER NOTE: This was formerly a static local variable declaration (not allowed in C#):
//private float[] Drift_correction_Scaled_Omega_I = new float[3];

///**************************************************/
//private void Drift_correction()
//{
//  float mag_heading_x;
//  float mag_heading_y;
//  float errorCourse;
//  //Compensation the Roll, Pitch and Yaw drift. 
////C++ TO C# CONVERTER NOTE: This static local variable declaration (not allowed in C#) has been moved just prior to the method:
////  static float Scaled_Omega_P[3];
////C++ TO C# CONVERTER NOTE: This static local variable declaration (not allowed in C#) has been moved just prior to the method:
////  static float Scaled_Omega_I[3];
//  float Accel_magnitude;
//  float Accel_weight;


//  //*****Roll and Pitch***************

//  // Calculate the magnitude of the accelerometer vector
//  Accel_magnitude = Math.Sqrt(Accel_Vector[0] * Accel_Vector[0] + Accel_Vector[1] * Accel_Vector[1] + Accel_Vector[2] * Accel_Vector[2]);
//  Accel_magnitude = Accel_magnitude / DefineConstants.GRAVITY; // Scale to gravity.
//  // Dynamic weighting of accelerometer info (reliability filter)
//  // Weight for accelerometer info (<0.5G = 0.0, 1G = 1.0 , >1.5G = 0.0)
//  Accel_weight = constrain(1 - 2 * Math.Abs(1 - Accel_magnitude),0,1);

//  Vector_Cross_Product(errorRollPitch[0], Accel_Vector[0], DCM_Matrix[2, 0]); //adjust the ground of reference
//  Vector_Scale(Omega_P[0], errorRollPitch[0], DefineConstants.Kp_ROLLPITCH * Accel_weight);

//  Vector_Scale(Drift_correction_Scaled_Omega_I[0], errorRollPitch[0], DefineConstants.Ki_ROLLPITCH * Accel_weight);
//  Vector_Add(Omega_I, Omega_I, Drift_correction_Scaled_Omega_I);

//  //*****YAW***************
//  // We make the gyro YAW drift correction based on compass magnetic heading

//  mag_heading_x = Math.Cos(MAG_Heading);
//  mag_heading_y = Math.Sin(MAG_Heading);
//  errorCourse = (DCM_Matrix[0, 0] * mag_heading_y) - (DCM_Matrix[1, 0] * mag_heading_x); //Calculating YAW error
//  Vector_Scale(errorYaw, DCM_Matrix[2, 0], errorCourse); //Applys the yaw correction to the XYZ rotation of the aircraft, depeding the position.

//  Vector_Scale(Drift_correction_Scaled_Omega_P[0], errorYaw[0], DefineConstants.Kp_YAW); //.01proportional of YAW.
//  Vector_Add(Omega_P, Omega_P, Drift_correction_Scaled_Omega_P); //Adding  Proportional.

//  Vector_Scale(Drift_correction_Scaled_Omega_I[0], errorYaw[0], DefineConstants.Ki_YAW); //.00001Integrator
//  Vector_Add(Omega_I, Omega_I, Drift_correction_Scaled_Omega_I); //adding integrator to the Omega_I
//}
///**************************************************/
///*
//void Accel_adjust(void)
//{
// Accel_Vector[1] += Accel_Scale(speed_3d*Omega[2]);  // Centrifugal force on Acc_y = GPS_speed*GyroZ
// Accel_Vector[2] -= Accel_Scale(speed_3d*Omega[1]);  // Centrifugal force on Acc_z = GPS_speed*GyroY 
//}
//*/
///**************************************************/

//private void Matrix_update()
//{
//  Gyro_Vector[0] = read_adc(0) * (DefineConstants.Gyro_Gain_X * 0.01745329252);
//  Gyro_Vector[1] = read_adc(1) * (DefineConstants.Gyro_Gain_Y * 0.01745329252);
//  Gyro_Vector[2] = read_adc(2) * (DefineConstants.Gyro_Gain_Z * 0.01745329252);

//  Accel_Vector[0] = accel_x;
//  Accel_Vector[1] = accel_y;
//  Accel_Vector[2] = accel_z;

//  Vector_Add(Omega[0], Gyro_Vector[0], Omega_I[0]); //adding proportional term
//  Vector_Add(Omega_Vector[0], Omega[0], Omega_P[0]); //adding Integrator term

//  //Accel_adjust();    //Remove centrifugal acceleration.   We are not using this function in this version - we have no speed measurement

////C++ TO C# CONVERTER TODO TASK: C# does not allow setting or comparing #define constants:
// #if OUTPUTMODE==1
//  Update_Matrix[0, 0] = 0F;
//  Update_Matrix[0, 1] = -G_Dt * Omega_Vector[2]; //-z
//  Update_Matrix[0, 2] = G_Dt * Omega_Vector[1]; //y
//  Update_Matrix[1, 0] = G_Dt * Omega_Vector[2]; //z
//  Update_Matrix[1, 1] = 0F;
//  Update_Matrix[1, 2] = -G_Dt * Omega_Vector[0]; //-x
//  Update_Matrix[2, 0] = -G_Dt * Omega_Vector[1]; //-y
//  Update_Matrix[2, 1] = G_Dt * Omega_Vector[0]; //x
//  Update_Matrix[2, 2] = 0F;
// #else
//  Update_Matrix[0, 0] = 0F;
//  Update_Matrix[0, 1] = -G_Dt * Gyro_Vector[2]; //-z
//  Update_Matrix[0, 2] = G_Dt * Gyro_Vector[1]; //y
//  Update_Matrix[1, 0] = G_Dt * Gyro_Vector[2]; //z
//  Update_Matrix[1, 1] = 0F;
//  Update_Matrix[1, 2] = -G_Dt * Gyro_Vector[0];
//  Update_Matrix[2, 0] = -G_Dt * Gyro_Vector[1];
//  Update_Matrix[2, 1] = G_Dt * Gyro_Vector[0];
//  Update_Matrix[2, 2] = 0F;
// #endif

//  Matrix_Multiply(DCM_Matrix, Update_Matrix, Temporary_Matrix); //a*b=c

//  for (int x = 0; x < 3; x++) //Matrix Addition (update)
//  {
//    for (int y = 0; y < 3; y++)
//    {
//      DCM_Matrix[x, y] += Temporary_Matrix[x, y];
//    }
//  }
//}

//private void Euler_angles()
//{
//  pitch = -Math.Asin(DCM_Matrix[2, 0]);
//  roll = Math.Atan2(DCM_Matrix[2, 1],DCM_Matrix[2, 2]);
//  yaw = Math.Atan2(DCM_Matrix[1, 0],DCM_Matrix[0, 0]);
//}


//internal static partial class DefineConstants
//{
//    public const int GRAVITY = 248;
//    public const double Gyro_Gain_X = 0.92;
//    public const double Gyro_Gain_Y = 0.92;
//    public const double Gyro_Gain_Z = 0.92;
//    public const double Kp_ROLLPITCH = 0.02;
//    public const double Ki_ROLLPITCH = 0.00002;
//    public const double Kp_YAW = 1.2;
//    public const double Ki_YAW = 0.00002;
//    public const int OUTPUTMODE = 1;
//    public const int PRINT_ANALOGS = 1;
//    public const int PRINT_EULER = 1;
//    public const int ADC_WARM_CYCLES = 50;
//    public const int STATUS_LED = 13;
//}





    }
}
