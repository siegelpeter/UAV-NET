using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Tao.FreeGlut;
using Tao.OpenGl;
using SalmonViewer;
namespace OpenGLUI
{
    public partial class GLEFIS : UserControl
    {




        static Model model;
        static ThreeDSFile file;

        static float[] rot = new float[] { 0, 0, 0 }; /* Amount to rotate */
        static float[] eye = new float[] { 0, 0, 0 }; /* Position of our eye or camera */
        static float[] light = new float[] { 200, 100, 40 };

        int AIOffsetX = 140;
        int AIOffsetY = 400;
        int AISizeX = 320;
        int AISizeY = 300;

        static Vector modelRotation = new Vector(0, 0, 0);
        static Vector modelCenter;
        double wnd_width = 500;
        double wnd_height = 700;

        public struct EFIS_Config
{
		// V-speed reference table
        public int v1;
        public int v2;
        public int vr;
        public int vt;
        public int mfdmode; // FMS, VOR, plan, radar, etc
	
}

        // AHRS data (heading and attitude)
        public struct AHRS_Data
{
	
        public int pitch;
		public int bank;
		public int slipskid;	
		public int heading;
	
} ;

// air data computer (airspeed, alt, etc)
 public struct ADC_Data
{
		public int ias;
		public int tas;
		public int mach;
		public int iastrend;
		public int gs;
	
		public int ialt;		// indicated altitude
		public int vspeed;
		public int altsetting;	// kollsman adjustment, could be either mb or hg
	
		public int tat;		// total air temperature
		public int sat;		// outside air temperature
		
		public int radioalt;	// not really the ADC's domain, oh fucking well
		public bool wow;		// aircraft on ground (boolean)
	
} 

// Engine data
 public struct ENG_Data
{
		public int n1l, n1r;
		public int ittl, ittr;
		public int n2l, n2r;
		
		public int ffl, ffr;	    // fuel flow
		public int oilpl, oilpr;   // oil pressure
		public int oiltl, oiltr;   // oil temp
	public 	int vibl, vibr;		// vibration
} 

// ED1 specific data
 public struct ED1_Data
{
		public int lfuel,rfuel, cfuel;	// fuel loads
	
} 

// flight control data
 public struct FCTL_Data
{
		public int flaps;  // flap setting
		
		// put landing gear under here even though not really a flight control
		public char nosegear, leftgear, rightgear; 
		
		// more later	
} 

// autopilot data
 public struct AP_Data
{
		// flight director variables
	public 	bool fdactive;	// on/off
	public 	int fdroll;
		public int fdpitch;
	
	public 	int iasbug;
		public int altbug;
	public 	int hdgbug;
} 


// navigation stuff, EFIS data
 public struct NAV_Data
{
		// direct from xplane stuff
		public int nav1crs;			// VOR/LOC courses
        public int nav2crs;
        public int nav1def;			// dot deflection
        public int nav2def;
        public int nav1vdef;			// vertical deflection (i.e. glideslope)
        public int nav2vdef;
        public char nav1tf;				// to/from
        public char nav2tf;
        public char nav1gs;			// glideslope signal received
        public char nav2gs;
        public int nav1dme;
        public int nav2dme;
		
		// simulation specific
        public int capt_source;		// source select (VOR/FMS/cross-side, etc)
        public int fo_source;
        public bool flightplan;	   // if an FMS flightplan exists
} 

        // data scaling factors (to allow non-float data types)
        private int AHRS_HEADING_SCALE = 1;
 int AHRS_PITCH_SCALE=1;
int AHRS_BANK_SCALE	=1;

int ADC_IAS_SCALE	=	1;
int ADC_GS_SCALE	=	1;
int ADC_MACH_SCALE	=	1;
int ADC_ALT_SCALE	=	1;
int ADC_ALTSETTING_SCALE=1;
int ADC_TEMP_SCALE	=	1;
int ADC_VS_SCALE	=	1;

int ENG_N1_SCALE	=	1;
int ENG_VIB_SCALE	=	1;

int FCTL_SCALE		=	1;

int NAV_DEF_SCALE	=	1;
int NAV_DME_SCALE	=	1;

        private int myheight = 0;
        private int mywidth = 0;
// NAV definitions
        const int  WHITE_NEEDLES =	0;
        const int GREEN_NEEDLES = 1;
        const int YELLOW_NEEDLES = 2;


        private SalmonViewer.Font font;

        private string TEXTURE_AI = "AITex";
        private string TEXTURE_HSI = "HSITex";

// scaling factors from incoming data
        private float auto = 1f;
        private float ALT_SCALE_FACTOR = 1f;
        private float IAS_SCALE_FACTOR = 1f;
        private int NAV_DEF_FACTOR = 32;
        private int NAV_VDEF_FACTOR = 1;
        private const int NAV_FROM = 2;
        private const int NAV_TO = 1;
        private const int NAV_NA = 0;
        public EFIS_Config efis = new EFIS_Config();
        public AHRS_Data ahrs = new AHRS_Data();
        public ADC_Data adc = new ADC_Data();
        public NAV_Data nav = new NAV_Data();
        AP_Data ap = new AP_Data();
       private float posx=0;
        private float posy = -50;
        private float scale = 1f;
        private float AI_PITCH_FACTOR=1;
        private const int SCREENS_FO_PFD = 0;
        private const int SCREENS_CPT_PFD = 1;
        private const int SCREENS_CAPT_PFD = 2;
        float heading;
        int i;

        // local nav variables
        int course;
        char tofrom;
        char gsflag;
        float deflection, vdeflection;
        float dme;


        private imageLoader imageloader = new imageLoader();
        private int type = SCREENS_CAPT_PFD;

       


        public GLEFIS()
        {
            InitializeComponent();
            this.simpleOpenGlControl1.InitializeContexts();
            ap.altbug = 500;
            ap.iasbug = 100;
            adc.ialt = 0;//150000;
            adc.ias = 100000;
            ahrs.pitch = 0;
            ahrs.bank = 10;
            efis.v1 = 125;
            efis.vr = 130;
            efis.v2 = 135;
            efis.vt = 137;
            try
            {
                Init();

                imageloader.LoadFromFile(System.IO.Path.GetFullPath("data/ai.png"), TEXTURE_AI);
                imageloader.LoadFromFile(System.IO.Path.GetFullPath("data/hsi.png"), TEXTURE_HSI);

            }
            catch (Exception ex)
            {

            }
             AIOffsetX = 140;
             AIOffsetY = 400;
             AISizeX = 320;
             AISizeY = 300;
           
            
           
        }






        /// <summary>
        /// Initalizes OpenGL parameters
        /// </summary>
        public void Init()
        {
           font = new SalmonViewer.Font();
           int[] buf = new int[32];
	
		Gl.glClearColor(0.0f, 0.0f, 0.0f, 1.0f);
	
		Gl.glDisable(Gl.GL_DEPTH_TEST);
	
		Gl.glShadeModel(Gl.GL_SMOOTH);
		Gl.glEnable(Gl.GL_BLEND);
		Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
	
		Gl.glAlphaFunc(Gl.GL_GEQUAL, 0.0625f);
  		Gl.glEnable(Gl.GL_ALPHA_TEST);
	
		Gl.glDisable(Gl.GL_CULL_FACE);
		Gl.glFrontFace(Gl.GL_CW);
	
		Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL);	
		
	    Gl.glMatrixMode(Gl.GL_PROJECTION);
    	Gl.glLoadIdentity();
    
    	Gl.glViewport(0, 0, 500, 700);      
    	Gl.glOrtho(0, 500, 700, 0, 0, 500);
    		
    	Gl.glMatrixMode(Gl.GL_MODELVIEW);
    	Gl.glLoadIdentity();
	
		Gl.glDisable(Gl.GL_POLYGON_SMOOTH);
	
		Gl.glEnable(Gl.GL_MULTISAMPLE);
				
		Gl.glGetIntegerv (Gl.GL_SAMPLE_BUFFERS_ARB, buf);
		
		Gl.glGetIntegerv (Gl.GL_SAMPLES_ARB, buf);
	
        }


        void simpleOpenGlControl1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {

            // for scissor windows
         

    
          
            
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();

            // translate and scale entire PFD
            Gl.glTranslatef(posx, posy, 0.0f);
            Gl.glScalef(scale, scale, 1.0f);

         

            Gl.glEnable(Gl.GL_TEXTURE_2D);
            DrawAI();
            DrawAlt();
            DrawVSI();
            DrawIAS();
            DrawHSI();
            // flush out what in the OpenGL context
            Gl.glFlush();
            //if (active)
            //    ActiveDraw();

            }


        
/******
ATTITUDE INDICATOR CODE
******/

void DrawAI()
{
    float pitch, bank, ypitch, coordinate;
	
	pitch = (float) ahrs.pitch / AHRS_PITCH_SCALE;
	bank = (float) ahrs.bank / AHRS_BANK_SCALE;
  
     // set up scissor window for AI
    Gl.glScissor((int)(posx + (AIOffsetX * scale)), (int)(693 - (posy + (AIOffsetY * scale))), (int)(AISizeX * scale), (int)(AISizeY * scale));
    Gl.glEnable(Gl.GL_SCISSOR_TEST);
    Gl.glEnable(Gl.GL_TEXTURE_2D);
	
    // draw AI       
    Gl.glPushMatrix();
    Gl.glBindTexture(Gl.GL_TEXTURE_2D, imageloader.get(TEXTURE_AI));
	
    // draw land and sky
    Gl.glTranslatef(300.0f, 250.0f, 0.0f);
    Gl.glRotatef(bank, 0.0f, 0.0f, 1.0f);
    
    ypitch = -(pitch * AI_PITCH_FACTOR); 
   
    Gl.glColor3f(0.16f, 0.55f, 1.0f);
    Gl.glDisable(Gl.GL_TEXTURE_2D);
    Gl.glBegin(Gl.GL_QUADS);   
        Gl.glVertex2f(-300, ypitch);    
        Gl.glVertex2f(300, ypitch);
        Gl.glVertex2f(300, -300);    
        Gl.glVertex2f(-300, -300);                      
    Gl.glEnd();
    
    
    Gl.glColor3f(0.61f, 0.40f, 0.15f);   // brown        
   
    Gl.glBegin(Gl.GL_QUADS);
        Gl.glVertex2f(-300, ypitch);
        Gl.glVertex2f(-300, 300);
        Gl.glVertex2f(300, 300);
        Gl.glVertex2f(300, ypitch);    
    Gl.glEnd();

	Gl.glColor3f( 1.0f, 1.0f, 1.0f);
	
    Gl.glEnable(Gl.GL_TEXTURE_2D);
	
	Gl.glBegin(Gl.GL_QUADS);
		Gl.glTexCoord2f(0.605f, 0.080f);
		Gl.glVertex2i(-300, (int)(ypitch - 20));
		Gl.glTexCoord2f(0.742f, 0.080f);
		Gl.glVertex2i(300, (int)(ypitch - 20));
		Gl.glTexCoord2f(0.742f, 0.232f);
		Gl.glVertex2i(300, (int)(ypitch + 20));
		Gl.glTexCoord2f(0.605f, 0.232f);
		Gl.glVertex2i(-300, (int)(ypitch + 20));
	Gl.glEnd();
	
    // draw pitch lines
    Gl.glColor3f( 1.0f, 1.0f, 1.0f);
    
	Gl.glBegin(Gl.GL_QUADS);
		// horizon line
		Gl.glTexCoord2f(0.016f, 0.981f);
		Gl.glVertex2f(-125, ypitch - 2);
		Gl.glTexCoord2f(0.755f, 0.981f);
		Gl.glVertex2f(125, ypitch - 2);
		Gl.glTexCoord2f(0.755f, 0.991f);
		Gl.glVertex2f(125, ypitch + 2);
		Gl.glTexCoord2f(0.016f, 0.991f);
		Gl.glVertex2f(-125, ypitch + 2);
	
		// 2.5 degree lines
		Gl.glTexCoord2f(0.016f, 0.922f);
		Gl.glVertex2f(-5, (ypitch - (2.5f * AI_PITCH_FACTOR)) - 2);
		Gl.glTexCoord2f(0.051f, 0.922f);
		Gl.glVertex2f(5, (ypitch - (2.5f * AI_PITCH_FACTOR)) - 2);
		Gl.glTexCoord2f(0.051f, 0.932f);
		Gl.glVertex2f(5, (ypitch - (2.5f * AI_PITCH_FACTOR)) + 2);
		Gl.glTexCoord2f(0.016f, 0.932f);
		Gl.glVertex2f(-5, (ypitch - (2.5f * AI_PITCH_FACTOR)) + 2);
					 
		Gl.glTexCoord2f(0.016f, 0.922f);
		Gl.glVertex2f(-5, (ypitch + (2.5f * AI_PITCH_FACTOR)) - 2);
		Gl.glTexCoord2f(0.051f, 0.922f);
		Gl.glVertex2f(5, (ypitch + (2.5f * AI_PITCH_FACTOR)) - 2);
		Gl.glTexCoord2f(0.051f, 0.932f);
		Gl.glVertex2f(5, (ypitch + (2.5f * AI_PITCH_FACTOR)) + 2);
		Gl.glTexCoord2f(0.016f, 0.932f);
		Gl.glVertex2f(-5, (ypitch + (2.5f * AI_PITCH_FACTOR)) + 2);

		// 5 degree lines
		Gl.glTexCoord2f(0.016f, 0.942f);
		Gl.glVertex2f(-13, (ypitch + (5.0f * AI_PITCH_FACTOR)) - 2);
		Gl.glTexCoord2f(0.100f, 0.942f);
		Gl.glVertex2f(13, (ypitch + (5.0f * AI_PITCH_FACTOR)) - 2);
		Gl.glTexCoord2f(0.100f, 0.952f);
		Gl.glVertex2f(13, (ypitch + (5.0f * AI_PITCH_FACTOR)) + 2);
		Gl.glTexCoord2f(0.016f, 0.952f);
		Gl.glVertex2f(-13, (ypitch + (5.0f * AI_PITCH_FACTOR)) + 2);
	
		Gl.glTexCoord2f(0.016f, 0.942f);
		Gl.glVertex2f(-13, (ypitch - (5.0f * AI_PITCH_FACTOR)) - 2);
		Gl.glTexCoord2f(0.100f, 0.942f);
		Gl.glVertex2f(13, (ypitch - (5.0f * AI_PITCH_FACTOR)) - 2);
		Gl.glTexCoord2f(0.100f, 0.952f);
		Gl.glVertex2f(13, (ypitch - (5.0f * AI_PITCH_FACTOR)) + 2);
		Gl.glTexCoord2f(0.016f, 0.952f);
		Gl.glVertex2f(-13, (ypitch - (5.0f * AI_PITCH_FACTOR)) + 2);
		
		// 7.5 degree lines
		Gl.glTexCoord2f(0.016f, 0.922f);
		Gl.glVertex2f(-5, (ypitch - (7.5f * AI_PITCH_FACTOR)) - 2);
		Gl.glTexCoord2f(0.051f, 0.922f);
		Gl.glVertex2f(5, (ypitch - (7.5f * AI_PITCH_FACTOR)) - 2);
		Gl.glTexCoord2f(0.051f, 0.932f);
		Gl.glVertex2f(5, (ypitch - (7.5f * AI_PITCH_FACTOR)) + 2);
		Gl.glTexCoord2f(0.016f, 0.932f);
		Gl.glVertex2f(-5, (ypitch - (7.5f * AI_PITCH_FACTOR)) + 2);
					 
		Gl.glTexCoord2f(0.016f, 0.922f);
		Gl.glVertex2f(-5, (ypitch + (7.5f * AI_PITCH_FACTOR)) - 2);
		Gl.glTexCoord2f(0.051f, 0.922f);
		Gl.glVertex2f(5, (ypitch + (7.5f * AI_PITCH_FACTOR)) - 2);
		Gl.glTexCoord2f(0.051f, 0.932f);
		Gl.glVertex2f(5, (ypitch + (7.5f * AI_PITCH_FACTOR)) + 2);
		Gl.glTexCoord2f(0.016f, 0.932f);
		Gl.glVertex2f(-5, (ypitch + (7.5f * AI_PITCH_FACTOR)) + 2);

		// 12.5 degree lines
		Gl.glTexCoord2f(0.016f, 0.922f);
		Gl.glVertex2f(-5, (ypitch - (12.5f * AI_PITCH_FACTOR)) - 2);
		Gl.glTexCoord2f(0.051f, 0.922f);
		Gl.glVertex2f(5, (ypitch - (12.5f * AI_PITCH_FACTOR)) - 2);
		Gl.glTexCoord2f(0.051f, 0.932f);
		Gl.glVertex2f(5, (ypitch - (12.5f * AI_PITCH_FACTOR)) + 2);
		Gl.glTexCoord2f(0.016f, 0.932f);
		Gl.glVertex2f(-5, (ypitch - (12.5f * AI_PITCH_FACTOR)) + 2);
					 
		Gl.glTexCoord2f(0.016f, 0.922f);
		Gl.glVertex2f(-5, (ypitch + (12.5f * AI_PITCH_FACTOR)) - 2);
		Gl.glTexCoord2f(0.051f, 0.922f);
		Gl.glVertex2f(5, (ypitch + (12.5f * AI_PITCH_FACTOR)) - 2);
		Gl.glTexCoord2f(0.051f, 0.932f);
		Gl.glVertex2f(5, (ypitch + (12.5f * AI_PITCH_FACTOR)) + 2);
		Gl.glTexCoord2f(0.016f, 0.932f);
		Gl.glVertex2f(-5, (ypitch + (12.5f * AI_PITCH_FACTOR)) + 2);
	
	Gl.glEnd();
	// 10 degree lines
	for(int i = 10; i <= 90; i+=10)
	{
		Gl.glBegin(Gl.GL_QUADS);
			Gl.glTexCoord2f(0.016f, 0.961f);
			Gl.glVertex2f(-25, (ypitch + (i * AI_PITCH_FACTOR)) - 2);
			Gl.glTexCoord2f(0.169f, 0.961f);
			Gl.glVertex2f(25, (ypitch + (i * AI_PITCH_FACTOR)) - 2);
			Gl.glTexCoord2f(0.169f, 0.971f);
			Gl.glVertex2f(25, (ypitch + (i * AI_PITCH_FACTOR)) + 2);
			Gl.glTexCoord2f(0.016f, 0.971f);
			Gl.glVertex2f(-25, (ypitch + (i * AI_PITCH_FACTOR)) + 2);
					   
			Gl.glTexCoord2f(0.016f, 0.961f);
			Gl.glVertex2f(-25, (ypitch - (i * AI_PITCH_FACTOR)) - 2);
			Gl.glTexCoord2f(0.169f, 0.961f);
			Gl.glVertex2f(25, (ypitch - (i * AI_PITCH_FACTOR)) - 2);
			Gl.glTexCoord2f(0.169f, 0.971f);
			Gl.glVertex2f(25, (ypitch - (i * AI_PITCH_FACTOR)) + 2);
			Gl.glTexCoord2f(0.016f, 0.971f);
			Gl.glVertex2f(-25, (ypitch - (i * AI_PITCH_FACTOR)) + 2);
		Gl.glEnd();
	} 
    
    // 10 degree labels
    for(int j = 10; j <= 90; j+=10)
    {
        Gl.glPushMatrix();
        Gl.glTranslatef(27, ypitch + (j * AI_PITCH_FACTOR), 0.0f);
        font.glPrint(0.75f, Convert.ToString(j / 10) + "0");//"%dO"
        Gl.glPopMatrix();
		
        Gl.glPushMatrix();
        Gl.glTranslatef(27, ypitch - (j * AI_PITCH_FACTOR), 0.0f);
        font.glPrint(0.75f, Convert.ToString(j / 10)+"0");//"%dO"
        Gl.glPopMatrix();
    }
 
    // draw coordinator and bank indicator
	Gl.glBindTexture(Gl.GL_TEXTURE_2D, imageloader.get(TEXTURE_AI));
	
    Gl.glBegin(Gl.GL_TRIANGLES);
		Gl.glTexCoord2f(0.5f, 0.398f);
        Gl.glVertex2i(0, -122);
		Gl.glTexCoord2f(0.546f, 0.324f);
        Gl.glVertex2i(13, -102);
		Gl.glTexCoord2f(0.454f, 0.324f);
        Gl.glVertex2i(-13, -102);
    Gl.glEnd();            
	
	Gl.glBegin(Gl.GL_QUADS);
		Gl.glTexCoord2f(0.0816f, 0.823f);
        Gl.glVertex2i(12 + (ahrs.slipskid / 4),-105);
		Gl.glTexCoord2f(0.0816f, 0.858f);
	    Gl.glVertex2i(12 + (ahrs.slipskid / 4),-96);
		Gl.glTexCoord2f(0.0071f, 0.858f);
        Gl.glVertex2i(-12 + (ahrs.slipskid / 4), -96);
		Gl.glTexCoord2f(0.0071f, 0.823f);
		Gl.glVertex2i(-12 + (ahrs.slipskid / 4), -105);
	Gl.glEnd();
	
  
    //  restore PFD default position
    Gl.glPopMatrix();
       
    if(ap.fdactive)
    {
        Gl.glPushMatrix();
        Gl.glTranslatef(300.0f, 250.0f, 0.0f);
              
          // draw flight director
        Gl.glRotatef((bank - ap.fdroll), 0.0f, 0.0f, 1.0f);
        Gl.glTranslatef(0.0f, -(pitch - ap.fdpitch) * AI_PITCH_FACTOR, 0.0f);	

		Gl.glBegin(Gl.GL_QUADS);
			Gl.glTexCoord2f(0.002f, 0.175f);
			Gl.glVertex2i(-87, 32);
			Gl.glTexCoord2f(0.002f, 0.012f);
			Gl.glVertex2i(-87, -3);
			Gl.glTexCoord2f(0.37f, 0.012f);
			Gl.glVertex2i(0, -3);
			Gl.glTexCoord2f(0.37f, 0.175f);
			Gl.glVertex2i(0, 32);
		
			Gl.glTexCoord2f(0.37f, 0.012f);
			Gl.glVertex2i(0, -3);
			Gl.glTexCoord2f(0.002f, 0.012f);
			Gl.glVertex2i(87, -3);
			Gl.glTexCoord2f(0.002f, 0.175f);
			Gl.glVertex2i(87, 32);
			Gl.glTexCoord2f(0.37f, 0.175f);
			Gl.glVertex2i(0, 32);
		Gl.glEnd();
        Gl.glPopMatrix();
    }
    
    Gl.glPushMatrix();
    Gl.glTranslatef(300.0f, 250.0f, 0.0f);
	
	// draw symbology
	Gl.glBindTexture(Gl.GL_TEXTURE_2D, imageloader.get(TEXTURE_AI));
	Gl.glBegin(Gl.GL_QUADS);
		Gl.glTexCoord2f(0.04f, 0.180f);
		Gl.glVertex2i(-75, -2);
		Gl.glTexCoord2f(0.37f, 0.180f);
		Gl.glVertex2i(0, -2);
		Gl.glTexCoord2f(0.37f, 0.31f);
		Gl.glVertex2i(0, 26);
		Gl.glTexCoord2f(0.04f, 0.31f);
		Gl.glVertex2i(-75, 26);
	
		// other half
		Gl.glTexCoord2f(0.37f, 0.180f);
		Gl.glVertex2i(0, -2);
		Gl.glTexCoord2f(0.04f, 0.180f);
		Gl.glVertex2i(75, -2);
		Gl.glTexCoord2f(0.04f, 0.31f);
		Gl.glVertex2i(75, 26);
		Gl.glTexCoord2f(0.37f, 0.31f);
		Gl.glVertex2i(0, 26);
	Gl.glEnd();
   
    // draw altitude reference line
    Gl.glBegin(Gl.GL_QUADS);
		Gl.glTexCoord2f(0.343f, 0.961f);
        Gl.glVertex2i(145,-2);
		Gl.glTexCoord2f(0.437f, 0.961f);
		Gl.glVertex2i(160, -2);
		Gl.glTexCoord2f(0.437f, 0.972f);
		Gl.glVertex2i(160, 2);
		Gl.glTexCoord2f(0.343f, 0.972f);
		Gl.glVertex2i(145, 2);
    Gl.glEnd();
    
	// draw bank anGl.gle symbology
	Gl.glBegin(Gl.GL_QUADS);
		Gl.glTexCoord2f(0.00f, 0.32f);
		Gl.glVertex2i(-140, -140);
		Gl.glTexCoord2f(0.54f, 0.32f);
		Gl.glVertex2i(11, -140);
		Gl.glTexCoord2f(0.54f, 0.86f);
		Gl.glVertex2i(11, 5);
		Gl.glTexCoord2f(0.00f, 0.86f);
		Gl.glVertex2i(-140, 5);
	
		// other half
		Gl.glTexCoord2f(0.459f, 0.32f);
		Gl.glVertex2i(12, -140);
		Gl.glTexCoord2f(0.00f, 0.32f);
		Gl.glVertex2i(140, -140);
		Gl.glTexCoord2f(0.00f, 0.86f);
		Gl.glVertex2i(140, 5);
		Gl.glTexCoord2f(0.459f, 0.86f);
		Gl.glVertex2i(12, 5);
	Gl.glEnd();
	
    Gl.glPopMatrix();  // original PFD translation
    
    Gl.glDisable(Gl.GL_SCISSOR_TEST);
       
    
}

/******
ALTIMETER CODE
******/
void DrawAlt()
{
    float ialt;
    float tens, hundreds, thousands, tthousands;
    float tmp;
    float offset;
    int i, label;
       
     // set up scissor window for altimeter
    Gl.glScissor((int)(posx + (450 * scale)), (int)(myheight - (posy + (405 * scale))), (int)(150 * scale), (int)(305 * scale));
    Gl.glEnable(Gl.GL_SCISSOR_TEST);
    
    Gl.glColor3f( 1.0f, 1.0f, 1.0f);
	
	ialt = (float)(adc.ialt / ADC_ALT_SCALE);
	
    // find altitude in tens column
    tmp = ialt / 100;
    tmp = ((float)Math.Floor(tmp) * 100);
    tens = ialt - tmp;
    
    // find altitude in hundreds column
    tmp = ialt / 1000;
    tmp = ((float)Math.Floor(tmp) * 1000);
    hundreds = ialt - tmp;
        
    // find altitude in thousands
    tmp = ialt / 10000;
    tmp = ((float)Math.Floor(tmp) * 10000);
    tmp = ialt - tmp;
    thousands = (float)Math.Floor(tmp / 1000);
    
    // find altitude in tens of thousands
    tthousands = (float)Math.Floor(ialt / 10000);
    
    Gl.glPushMatrix();
    // translate to center
    Gl.glTranslatef(450.0f, 250.0f, 0.0f);
    
    // determine where next higher 20 foot mark begins
    if(tens > 0)
    {
        if(tens > 20)
        {
            if(tens > 40)
            {
                if(tens > 60)
                {
                    if(tens > 80)
                    {
                        offset = (100 - tens) * ALT_SCALE_FACTOR;
                    } else offset = (80 - tens) * ALT_SCALE_FACTOR;
                } else offset = (60 - tens) * ALT_SCALE_FACTOR;
            } else offset = (40 - tens) * ALT_SCALE_FACTOR;
        } else offset = (20 - tens) * ALT_SCALE_FACTOR;
    } else offset = 0.0f;
	
    // draw 20 foot marks
    for(i = 0; i < 13; i++)
    {
        tmp = (-(i * 20) * ALT_SCALE_FACTOR) - offset;
        Gl.glBegin(Gl.GL_QUADS);
			Gl.glTexCoord2f(0.120f, 0.920f);
			Gl.glVertex2f(64, tmp - 2);
			Gl.glTexCoord2f(0.152f, 0.920f);
			Gl.glVertex2f(74, tmp - 2);
			Gl.glTexCoord2f(0.152f, 0.934f);
			Gl.glVertex2f(74, tmp + 2);
			Gl.glTexCoord2f(0.120f, 0.934f);
			Gl.glVertex2f(64, tmp + 2);
		
        tmp = ((i * 20) * ALT_SCALE_FACTOR) - offset;
		
			Gl.glTexCoord2f(0.120f, 0.920f);
			Gl.glVertex2f(64, tmp - 2);
			Gl.glTexCoord2f(0.152f, 0.920f);
			Gl.glVertex2f(74, tmp - 2);
			Gl.glTexCoord2f(0.152f, 0.934f);
			Gl.glVertex2f(74, tmp + 2);
			Gl.glTexCoord2f(0.120f, 0.934f);
			Gl.glVertex2f(64, tmp + 2);
		 
		Gl.glEnd();
	
    }       
 	
    // draw hundreds
    if(hundreds > 0)
    {
        if(hundreds > 100)
        {
            if(hundreds > 200)
            {
                if(hundreds > 300)
                {
                    if(hundreds > 400)
                    {
                        if(hundreds > 500)
                        {
                            if(hundreds > 600)
                            {
                                if(hundreds > 700)
                                {
                                    if(hundreds > 800)
                                    {
                                        if(hundreds > 900)
                                        {
                                            offset = (1000 - hundreds) * ALT_SCALE_FACTOR;
                                            label = 0;
                                        }
                                        else {
                                            offset = (900 - hundreds) * ALT_SCALE_FACTOR;
                                            label = 900;
                                        }
                                    }
                                    else {
                                        offset = (800 - hundreds) * ALT_SCALE_FACTOR;
                                        label = 800;
                                    }
                                }
                                else {
                                    offset = (700 - hundreds) * ALT_SCALE_FACTOR;
                                    label = 700;
                                }
                            } 
                            else {
                                offset = (600 - hundreds) * ALT_SCALE_FACTOR;
                                label = 600;
                            }
                        } 
                        else {
                            offset = (500 - hundreds) * ALT_SCALE_FACTOR;
                            label = 500;
                        }
                    } 
                    else {
                        offset = (400 - hundreds) * ALT_SCALE_FACTOR;
                        label = 400;
                    }
                } 
                else {
                    offset = (300 - hundreds) * ALT_SCALE_FACTOR;
                    label = 300;
                }
            } 
            else {
                offset = (200 - hundreds) * ALT_SCALE_FACTOR;
                label = 200;
            }
        } 
        else {
            offset = (100 - hundreds) * ALT_SCALE_FACTOR;
            label = 100;
        }            
    } 
    else { 
        offset = 0.0f;
        label = 0;
    }
  
    Gl.glPushMatrix();
    Gl.glTranslatef(75.0f, -offset, 0.0f);
    for(i = 0; i < 4; i++)
    {
        Gl.glPushMatrix();
        Gl.glTranslatef(0.0f, -((i * 100) * ALT_SCALE_FACTOR), 0.0f);
        if((label + (i * 100)) == 1000)
        {
			Gl.glBindTexture(Gl.GL_TEXTURE_2D, imageloader.get(TEXTURE_AI));
			Gl.glBegin(Gl.GL_QUADS);
				Gl.glTexCoord2f(0.180f, 0.959f);
				Gl.glVertex2i(0, 17);
				Gl.glTexCoord2f(0.338f, 0.959f);
				Gl.glVertex2i(50, 17);
				Gl.glTexCoord2f(0.338f, 0.974f);
				Gl.glVertex2i(50, 21);
				Gl.glTexCoord2f(0.180f, 0.974f);
				Gl.glVertex2i(0, 21);
			Gl.glEnd();
			Gl.glBegin(Gl.GL_QUADS);
				Gl.glTexCoord2f(0.180f, 0.959f);
				Gl.glVertex2i(0, -17);
				Gl.glTexCoord2f(0.338f, 0.959f);
				Gl.glVertex2i(50, -17);
				Gl.glTexCoord2f(0.338f, 0.974f);
				Gl.glVertex2i(50, -21);
				Gl.glTexCoord2f(0.180f, 0.974f);
				Gl.glVertex2i(0, -21);
			Gl.glEnd();
        }
        if((label + (i * 100)) == 1000)
            font.glPrint(0.75f, "000");
        else if((label + (i * 100)) > 1000)
           font.glPrint(0.75f, "100");
        else font.glPrint(0.75f,  Convert.ToString(label + (i * 100)));
        Gl.glPopMatrix();
        Gl.glPushMatrix();
        Gl.glTranslatef(0.0f, ((i * 100) * ALT_SCALE_FACTOR), 0.0f);
        if((label - (i * 100)) == 0)
        {
			Gl.glBindTexture(Gl.GL_TEXTURE_2D, imageloader.get(TEXTURE_AI));
			Gl.glBegin(Gl.GL_QUADS);
				Gl.glTexCoord2f(0.180f, 0.959f);
				Gl.glVertex2i(0, 17);
				Gl.glTexCoord2f(0.338f, 0.959f);
				Gl.glVertex2i(50, 17);
				Gl.glTexCoord2f(0.338f, 0.974f);
				Gl.glVertex2i(50, 21);
				Gl.glTexCoord2f(0.180f, 0.974f);
				Gl.glVertex2i(0, 21);
			Gl.glEnd();
			Gl.glBegin(Gl.GL_QUADS);
				Gl.glTexCoord2f(0.180f, 0.959f);
				Gl.glVertex2i(0, -17);
				Gl.glTexCoord2f(0.338f, 0.959f);
				Gl.glVertex2i(50, -17);
				Gl.glTexCoord2f(0.338f, 0.974f);
				Gl.glVertex2i(50, -21);
				Gl.glTexCoord2f(0.180f, 0.974f);
				Gl.glVertex2i(0, -21);
			Gl.glEnd();
        }
        if((label - (i * 100)) == 0) 
            font.glPrint(0.75f, "000");
        else if((label - (i * 100)) < 0)
            font.glPrint(0.75f,  Convert.ToString( 1000 + (label - (i * 100))));          
        else font.glPrint(0.75f,  Convert.ToString(label - (i * 100)));
        Gl.glPopMatrix();
    } 
    
    Gl.glPopMatrix();
   
// draw altitude bug on hundreds scale
    tmp = -((ap.altbug - ialt) * ALT_SCALE_FACTOR);
 	
    Gl.glColor3f(1.0f, 0.2f, 1.0f);
	Gl.glBindTexture(Gl.GL_TEXTURE_2D, imageloader.get(TEXTURE_AI));
	Gl.glBegin(Gl.GL_QUADS);
		Gl.glTexCoord2f(0.180f, 0.959f);
		Gl.glVertex2f(75, tmp + 21);
		Gl.glTexCoord2f(0.337f, 0.959f);
		Gl.glVertex2f(125, tmp + 21);
		Gl.glTexCoord2f(0.337f, 0.974f);
		Gl.glVertex2f(125, tmp + 17);
		Gl.glTexCoord2f(0.180f, 0.974f);
		Gl.glVertex2f(75, tmp + 17);
	
		Gl.glTexCoord2f(0.180f, 0.959f);
		Gl.glVertex2f(75, tmp - 21);
		Gl.glTexCoord2f(0.337f, 0.959f);
		Gl.glVertex2f(125, tmp - 21);
		Gl.glTexCoord2f(0.337f, 0.974f);
		Gl.glVertex2f(125, tmp - 17);
		Gl.glTexCoord2f(0.180f, 0.974f);
		Gl.glVertex2f(75, tmp - 17);
	
		Gl.glTexCoord2f(0.180f, 0.959f);
		Gl.glVertex2f(75, tmp - 27);
		Gl.glTexCoord2f(0.337f, 0.959f);
		Gl.glVertex2f(125, tmp - 27);
		Gl.glTexCoord2f(0.337f, 0.974f);
		Gl.glVertex2f(125, tmp - 23);
		Gl.glTexCoord2f(0.180f, 0.974f);
		Gl.glVertex2f(75, tmp - 23);
	
		Gl.glTexCoord2f(0.180f, 0.959f);
		Gl.glVertex2f(75, tmp + 27);
		Gl.glTexCoord2f(0.337f, 0.959f);
		Gl.glVertex2f(125, tmp + 27);
		Gl.glTexCoord2f(0.337f, 0.974f);
		Gl.glVertex2f(125, tmp + 23);
		Gl.glTexCoord2f(0.180f, 0.974f);
		Gl.glVertex2f(75, tmp + 23);

	Gl.glEnd();

//////// Draw radio altimeter
    if(adc.radioalt < 2500)
    {
        offset = adc.radioalt * ALT_SCALE_FACTOR;
        Gl.glPushMatrix();
		
		Gl.glBindTexture(Gl.GL_TEXTURE_2D, imageloader.get(TEXTURE_AI));
		Gl.glColor3f( 1.0f, 1.0f, 1.0f);
		Gl.glBegin(Gl.GL_QUADS);
			Gl.glTexCoord2f(1.0f, 0.001f);
			Gl.glVertex2f(8, offset);
			Gl.glTexCoord2f(1.0f, 0.064f);
			Gl.glVertex2f(34, offset);
			Gl.glTexCoord2f(0.375f, 0.064f);
			Gl.glVertex2f(34, 200);
			Gl.glTexCoord2f(0.375f, 0.001f);
			Gl.glVertex2f(8, 200);
		Gl.glEnd();
  
        // draw green alt marks and text
        Gl.glColor3f(0.25f, 1.0f, 0.25f);
        Gl.glBegin(Gl.GL_QUADS);
        for(i = 1; i < 21; i++)
        {
			Gl.glTexCoord2f(0.120f, 0.920f);
			Gl.glVertex2f(12, (-(i * (50 * ALT_SCALE_FACTOR)) + offset) - 2);
			Gl.glTexCoord2f(0.152f, 0.920f);
			Gl.glVertex2f(18, (-(i * (50 * ALT_SCALE_FACTOR)) + offset) - 2);
			Gl.glTexCoord2f(0.152f, 0.934f);
			Gl.glVertex2f(18, (-(i * (50 * ALT_SCALE_FACTOR)) + offset) + 2);
			Gl.glTexCoord2f(0.120f, 0.934f);
			Gl.glVertex2f(12, (-(i * (50 * ALT_SCALE_FACTOR)) + offset) + 2);
        }
        Gl.glEnd();
        
        Gl.glTranslatef(20, offset - (100 * ALT_SCALE_FACTOR), 0.0f);
        for(i = 1; i < 11; i++)
        {
            Gl.glPushMatrix();
            if(i > 9)
                font.glPrint(0.65f, "0");
            else font.glPrint(0.65f,  i.ToString());
            Gl.glPopMatrix();
            Gl.glTranslatef(0.0f, -(100 * ALT_SCALE_FACTOR), 0.0f);
        }
        Gl.glPopMatrix();
        
        // Draw radio altimeter display
        Gl.glDisable(Gl.GL_SCISSOR_TEST);
		
        Gl.glColor3f(0.25f, 1.0f, 0.25f);
        
        Gl.glPushMatrix();
        Gl.glTranslatef(-85.0f, 169.0f, 0.0f);
        font.glPrint(0.8f, Convert.ToString( adc.radioalt));

        Gl.glTranslatef(68.0f, 2.0f, 0.0f);
        font.glPrint(0.65f, "FT");
        Gl.glPopMatrix();
        
        Gl.glEnable(Gl.GL_SCISSOR_TEST);
        
    }
    
/////// draw 500 and 1000 foot altitude markers
    Gl.glPushMatrix();
    Gl.glColor3f( 1.0f, 1.0f, 1.0f);
    Gl.glBindTexture(Gl.GL_TEXTURE_2D, imageloader.get(TEXTURE_AI));

    offset = Convert.ToSingle( hundreds) * .1355f;
    Gl.glTranslatef(0.0f, offset, 0.0f);  
    
    for(i = 0; i < 6; i++)
    {
        tmp = (i * 67.75f);
        
        if(i % 2 != 0) // odd number
        {
			Gl.glBegin(Gl.GL_QUADS);
				Gl.glTexCoord2f(0.181f, 0.913f);
				Gl.glVertex2f(51, tmp - 3.5f);
				Gl.glTexCoord2f(0.219f, 0.913f);
				Gl.glVertex2f(61, tmp - 3.5f);
				Gl.glTexCoord2f(0.219f, 0.942f);
				Gl.glVertex2f(61, tmp + 3.5f);
				Gl.glTexCoord2f(0.181f, 0.942f);
				Gl.glVertex2f(51, tmp + 3.5f);
			
				Gl.glTexCoord2f(0.181f, 0.913f);
				Gl.glVertex2f(51, -(tmp - 3.5f));
				Gl.glTexCoord2f(0.219f, 0.913f);
				Gl.glVertex2f(61, -(tmp - 3.5f));
				Gl.glTexCoord2f(0.219f, 0.942f);
				Gl.glVertex2f(61, -(tmp + 3.5f));
				Gl.glTexCoord2f(0.181f, 0.942f);
				Gl.glVertex2f(51, -(tmp + 3.5f));
			Gl.glEnd();
        }
        else 
        {
			Gl.glBegin(Gl.GL_QUADS);
				Gl.glTexCoord2f(0.229f, 0.913f);
				Gl.glVertex2f(39, tmp - 3.5f);
				Gl.glTexCoord2f(0.317f, 0.913f);
				Gl.glVertex2f(61, tmp - 3.5f);
				Gl.glTexCoord2f(0.317f, 0.942f);
				Gl.glVertex2f(61, tmp + 3.5f);
				Gl.glTexCoord2f(0.229f, 0.942f);
				Gl.glVertex2f(39, tmp + 3.5f);
			
				Gl.glTexCoord2f(0.229f, 0.913f);
				Gl.glVertex2f(39, -(tmp - 3.5f));
				Gl.glTexCoord2f(0.317f, 0.913f);
				Gl.glVertex2f(61, -(tmp - 3.5f));
				Gl.glTexCoord2f(0.317f, 0.942f);
				Gl.glVertex2f(61, -(tmp + 3.5f));
				Gl.glTexCoord2f(0.229f, 0.942f);
				Gl.glVertex2f(39, -(tmp + 3.5f));
			Gl.glEnd();
        }
    }
    
////// draw rough scale alt bug
    tmp = ((ialt - ap.altbug) * 0.1355f) - offset;
    
    Gl.glColor3f(1.0f, 0.2f, 1.0f);
	Gl.glBegin(Gl.GL_QUADS);
		Gl.glTexCoord2f(0.345f, 0.959f);
		Gl.glVertex2f(37, (tmp + 8) - 2);
		Gl.glTexCoord2f(0.435f, 0.959f);
		Gl.glVertex2f(62, (tmp + 8) - 2);
		Gl.glTexCoord2f(0.435f, 0.974f);
		Gl.glVertex2f(62, (tmp + 8) + 2);
		Gl.glTexCoord2f(0.345f, 0.974f);
		Gl.glVertex2f(37, (tmp + 8) + 2);
	
		Gl.glTexCoord2f(0.345f, 0.959f);
		Gl.glVertex2f(37, (tmp - 8) - 2);
		Gl.glTexCoord2f(0.435f, 0.959f);
		Gl.glVertex2f(62, (tmp - 8) - 2);
		Gl.glTexCoord2f(0.435f, 0.974f);
		Gl.glVertex2f(62, (tmp - 8) + 2);
		Gl.glTexCoord2f(0.345f, 0.974f);
		Gl.glVertex2f(37, (tmp - 8) + 2);
	
		Gl.glTexCoord2f(0.345f, 0.959f);
		Gl.glVertex2f(37, (tmp + 13) - 2);
		Gl.glTexCoord2f(0.435f, 0.959f);
		Gl.glVertex2f(62, (tmp + 13) - 2);
		Gl.glTexCoord2f(0.435f, 0.974f);
		Gl.glVertex2f(62, (tmp + 13) + 2);
		Gl.glTexCoord2f(0.345f, 0.974f);
		Gl.glVertex2f(37, (tmp + 13) + 2);
	
		Gl.glTexCoord2f(0.345f, 0.959f);
		Gl.glVertex2f(37, (tmp - 13) - 2);
		Gl.glTexCoord2f(0.435f, 0.959f);
		Gl.glVertex2f(62, (tmp - 13) - 2);
		Gl.glTexCoord2f(0.435f, 0.974f);
		Gl.glVertex2f(62, (tmp - 13) + 2);
		Gl.glTexCoord2f(0.345f, 0.974f);
		Gl.glVertex2f(37, (tmp - 13) + 2);
	
	Gl.glEnd();
    
    Gl.glPopMatrix();
    
	Gl.glDisable(Gl.GL_TEXTURE_2D);
    Gl.glColor3f(0.0f, 0.0f, 0.0f);
    Gl.glBegin(Gl.GL_POLYGON);
        Gl.glVertex2i(35, 32);
        Gl.glVertex2i(55, 32);
        Gl.glVertex2i(65, 20);
        Gl.glVertex2i(65, -20);
        Gl.glVertex2i(55, -32);
        Gl.glVertex2i(35, -32); 
    Gl.glEnd();
    Gl.glEnable(Gl.GL_TEXTURE_2D);
	
////// Draw thousands and ten thousands
    // setup scissor window
    Gl.glPopMatrix();  // original
    
    Gl.glScissor((int)(posx + (460 * scale)), (int)(myheight - (posy + (270 * scale))), (int)(50 * scale), (int)(40 * scale));
    Gl.glPushMatrix();
    Gl.glTranslatef(450.0f, 250.0f, 0.0f);      
    
    Gl.glColor3f( 1.0f, 1.0f, 1.0f);
    
    if(hundreds > 950) 
    {
        offset = -(1000 - hundreds) * ALT_SCALE_FACTOR;
        
        Gl.glPushMatrix();
        Gl.glTranslatef(40.0f, offset, 0.0f);
        font.glPrint(1.2f, Convert.ToString(thousands + 1 == 10 ? 0 : thousands + 1));     // "%.0f", 
        Gl.glPopMatrix();
           
        Gl.glPushMatrix();
        Gl.glTranslatef(40.0f, offset + 35.0f, 0.0f);
        font.glPrint(1.2f, Convert.ToString(thousands));     
        Gl.glPopMatrix();
        
        Gl.glPushMatrix();
        Gl.glTranslatef(40.0f, offset - 35.0f, 0.0f);
        font.glPrint(1.2f,  Convert.ToString(thousands));     
        Gl.glPopMatrix();
    }    
        
    else 
    {
        Gl.glPushMatrix();
        Gl.glTranslatef(40.0f, 0.0f, 0.0f);
        font.glPrint(1.2f,  Convert.ToString(thousands));     
        Gl.glPopMatrix();
    }
    
    
    // ten thousands
    if(thousands == 9 && hundreds > 950) 
    {
        offset = -(1000 - hundreds) * ALT_SCALE_FACTOR;
        
        Gl.glPushMatrix();
        Gl.glTranslatef(20.0f, offset, 0.0f);
        font.glPrint(1.2f, Convert.ToString( (tthousands + 1 == 10) ? 0 : tthousands + 1));     
        Gl.glPopMatrix();
        
        if(tthousands!=0)
        {   
            Gl.glPushMatrix();
            Gl.glTranslatef(20.0f, offset + 35.0f, 0.0f);
            font.glPrint(1.2f, Convert.ToString(tthousands));     
            Gl.glPopMatrix();
        }
        
        Gl.glPushMatrix();
        Gl.glTranslatef(20.0f, offset - 35.0f, 0.0f);
        font.glPrint(1.2f,Convert.ToString(thousands));     
        Gl.glPopMatrix();
    }

    else if (tthousands != 0)
    {
        Gl.glPushMatrix();
		Gl.glTranslatef(20.0f, 0.0f, 0.0f);
        font.glPrint(1.2f,  Convert.ToString(tthousands));     
        Gl.glPopMatrix();
    }

    Gl.glDisable(Gl.GL_SCISSOR_TEST);
     
/////// Draw indicator lines
    Gl.glBindTexture(Gl.GL_TEXTURE_2D, imageloader.get(TEXTURE_AI));
	if(adc.radioalt < 1000)
    {
        Gl.glBegin(Gl.GL_QUADS);
			Gl.glTexCoord2f(0.764f, 0.160f);
			Gl.glVertex2i(15, 32);
			Gl.glTexCoord2f(0.998f, 0.160f);
			Gl.glVertex2i(130, 32);
			Gl.glTexCoord2f(0.998f, 0.127f);
			Gl.glVertex2i(130, 20);
			Gl.glTexCoord2f(0.764f, 0.127f);
			Gl.glVertex2i(15, 20);
		
			Gl.glTexCoord2f(0.764f, 0.160f);
			Gl.glVertex2i(15, -32);
			Gl.glTexCoord2f(0.998f, 0.160f);
			Gl.glVertex2i(130, -32);
			Gl.glTexCoord2f(0.998f, 0.127f);
			Gl.glVertex2i(130, -20);
			Gl.glTexCoord2f(0.764f, 0.127f);
			Gl.glVertex2i(15, -20);
		Gl.glEnd(); 

    }        
    else
    {
		Gl.glBegin(Gl.GL_QUADS);
			Gl.glTexCoord2f(0.764f, 0.111f);
			Gl.glVertex2i(15, 32);
			Gl.glTexCoord2f(0.998f, 0.111f);
			Gl.glVertex2i(130, 32);
			Gl.glTexCoord2f(0.998f, 0.078f);
			Gl.glVertex2i(130, 20);
			Gl.glTexCoord2f(0.764f, 0.078f);
			Gl.glVertex2i(15, 20);
		
			Gl.glTexCoord2f(0.764f, 0.111f);
			Gl.glVertex2i(15, -32);
			Gl.glTexCoord2f(0.998f, 0.111f);
			Gl.glVertex2i(130, -32);
			Gl.glTexCoord2f(0.998f, 0.078f);
			Gl.glVertex2i(130, -20);
			Gl.glTexCoord2f(0.764f, 0.078f);
			Gl.glVertex2i(15, -20);
		Gl.glEnd(); 
		 
    }
    Gl.glDisable(Gl.GL_SCISSOR_TEST);

////// Kollsman display  
    Gl.glPushMatrix();
    Gl.glColor3f(0.16f, 0.55f, 1.0f);
    Gl.glTranslatef(20.0f, 171.0f, 0.0f);
    font.glPrint(0.75f,  Convert.ToString((float)adc.altsetting / ADC_ALTSETTING_SCALE));  

    Gl.glPopMatrix();
    Gl.glPushMatrix();
    Gl.glTranslatef(84.0f, 172.0f, 0.0f);
    font.glPrint(0.65f, "IN");
    Gl.glPopMatrix();
    
///// Altitude bug readout display
    Gl.glPushMatrix();
    Gl.glColor3f(1.0f, 0.2f, 1.0f);
    if((ap.altbug / 1000) >= 10)
        Gl.glTranslatef(52.0f, -200.0f, 0.0f);
    else
        Gl.glTranslatef(70.0f, -202.0f, 0.0f);
    if((ap.altbug / 1000) > 0)
        font.glPrint(1.5f,Convert.ToString(ap.altbug / 1000));
    else font.glPrint(1.0f, "0");
    Gl.glPopMatrix();

    // find altitude bug in hundreds column
    tmp = ap.altbug / 1000;
    tmp = ((float)Math.Floor(tmp) * 1000);
    tmp = (int)(ap.altbug - tmp);

    Gl.glPushMatrix();
    Gl.glTranslatef(90.0f, -200.0f, 0.0f);
    if (tmp != 0)
        font.glPrint(0.75f, Convert.ToString((int)tmp));
    else
        font.glPrint(0.75f, "000");
    Gl.glPopMatrix();

                  
    Gl.glPopMatrix();
    
}

/******
HSI CODE
******/
void DrawHSI()
{  
    float heading = 0;
    int temp = 0;
    int i =0;

    // local nav variables
    int course = 0;
    int tofrom = 0;
    char gsflag;
    float deflection = 0;
    float vdeflection =0;
    float dme = 0;
	
	heading = (float)ahrs.heading / AHRS_HEADING_SCALE;
	
	// figure out what we are and adjust variables accordingly
	switch(type)
	{
		
		case SCREENS_CAPT_PFD:
			
			switch(nav.capt_source)
			{   
				
				case GREEN_NEEDLES:
					course = nav.nav1crs;
					deflection = nav.nav1def;
					vdeflection = nav.nav1vdef;
					tofrom = nav.nav1tf;
					gsflag = nav.nav1gs;
					dme = nav.nav1dme;
					temp = GREEN_NEEDLES;
					break;
					
				case  WHITE_NEEDLES:		// TODO when we have a fucking FMS
					break;
					
				case  YELLOW_NEEDLES:
					course = nav.nav2crs;
					deflection = nav.nav2def;
					vdeflection = nav.nav2vdef;
					tofrom = nav.nav2tf;
					gsflag = nav.nav2gs;
					dme = nav.nav2dme;
					temp =  YELLOW_NEEDLES;
					break;
			}
			break;
			
		case SCREENS_FO_PFD:
		
			switch(nav.fo_source)
			{
				
				case GREEN_NEEDLES:
					course = nav.nav2crs;
					deflection = nav.nav2def;
					vdeflection = nav.nav2vdef;
					tofrom = nav.nav2tf;
					gsflag = nav.nav2gs;
					dme = nav.nav2dme;
					temp = GREEN_NEEDLES;
					break;
					
				case  WHITE_NEEDLES:		// TODO when we have a fucking FMS
					break;
					
				case  YELLOW_NEEDLES:
					course = nav.nav1crs;
					deflection = nav.nav1def;
					vdeflection = nav.nav1vdef;
					tofrom = nav.nav1tf;
					gsflag = nav.nav1gs;
					dme = nav.nav1dme;
					temp = GREEN_NEEDLES;
					break;
			}
			break;
			
		default: Console.WriteLine("Unhandled exception");
			break;
	}

	deflection = (deflection / NAV_DEF_SCALE) * NAV_DEF_FACTOR;
	vdeflection = (deflection / NAV_DEF_SCALE) * NAV_VDEF_FACTOR;
	dme = (dme / NAV_DME_SCALE);
	
    Gl.glScissor((int)posx, (int)(myheight - (posy + (700 * scale))), (int)(600 * scale), (int)(300 * scale));
    Gl.glEnable(Gl.GL_SCISSOR_TEST);
    

    Gl.glPushMatrix();
    Gl.glTranslatef(300.0f, 600.0f, 0.0f);     
    
	Gl.glBindTexture(Gl.GL_TEXTURE_2D, imageloader.get(TEXTURE_HSI));
	Gl.glColor3f( 1.0f, 1.0f, 1.0f);
	
    // draw HSI compass rose
    Gl.glPushMatrix();
	Gl.glRotatef(-heading, 0.0f, 0.0f, 1.0f);
	Gl.glBegin(Gl.GL_QUADS);
		Gl.glTexCoord2f(0.0f, 0.0f);
		Gl.glVertex2i(-160, -160);
		Gl.glTexCoord2f(0.856f, 0.0f);
		Gl.glVertex2i(160, -160);
		Gl.glTexCoord2f(0.856f, 0.856f);
		Gl.glVertex2i(160, 160);
		Gl.glTexCoord2f(0.0f, 0.856f);
		Gl.glVertex2i(-160, 160);
	Gl.glEnd();

	Gl.glPopMatrix();
   
    // draw course needles
    Gl.glPushMatrix();
	
	Gl.glRotatef((course - heading) + 180, 0.0f, 0.0f, 1.0f);
	
    switch(temp)
    {
        case  WHITE_NEEDLES: Gl.glColor3f( 1.0f, 1.0f, 1.0f);
            break;
        case GREEN_NEEDLES: Gl.glColor3f(0.25f, 1.0f, 0.25f);
            break;
        case  YELLOW_NEEDLES: Gl.glColor3f( 1.0f, 1.0f, 0.0f);
            break;
    }
    
	Gl.glBegin(Gl.GL_QUADS);
		Gl.glTexCoord2f(0.864f, 0.009f);
		Gl.glVertex2i(-10, 140);
		Gl.glTexCoord2f(0.921f, 0.009f);
		Gl.glVertex2i(10, 140);
		Gl.glTexCoord2f(0.921f, 0.748f);
		Gl.glVertex2i(10, -155);
		Gl.glTexCoord2f(0.864f, 0.748f);
		Gl.glVertex2i(-10, -155);
	Gl.glEnd();
	
	Gl.glPushMatrix();
	Gl.glTranslatef(deflection, 0.0f, 0.0f);
	Gl.glBegin(Gl.GL_QUADS);
		Gl.glTexCoord2f(0.931f, 0.010f);
		Gl.glVertex2i(-2, 80);
		Gl.glTexCoord2f(0.943f, 0.010f);
		Gl.glVertex2i(2, 80);
		Gl.glTexCoord2f(0.943f, 0.417f);
		Gl.glVertex2i(2, -80);
		Gl.glTexCoord2f(0.931f, 0.417f);
		Gl.glVertex2i(-2, -80);
	Gl.glEnd();
	Gl.glPopMatrix();
        	
	switch(tofrom)
    {
        case NAV_NA:
            break;
            
        case NAV_TO:
            Gl.glBegin(Gl.GL_QUADS);
				Gl.glTexCoord2f(0.864f, 0.009f);
				Gl.glVertex2i(-10, 80);
				Gl.glTexCoord2f(0.921f, 0.009f);
				Gl.glVertex2i(10, 80);
				Gl.glTexCoord2f(0.921f, 0.0665f);
				Gl.glVertex2i(10, 60);
				Gl.glTexCoord2f(0.864f, 0.0665f);
				Gl.glVertex2i(-10, 60);
			Gl.glEnd();
            break;
            
        case NAV_FROM:
			Gl.glBegin(Gl.GL_QUADS);
				Gl.glTexCoord2f(0.864f, 0.009f);
				Gl.glVertex2i(-10, -80);
				Gl.glTexCoord2f(0.921f, 0.009f);
				Gl.glVertex2i(10, -80);
				Gl.glTexCoord2f(0.921f, 0.0665f);
				Gl.glVertex2i(10, -60);
				Gl.glTexCoord2f(0.864f, 0.0665f);
				Gl.glVertex2i(-10, -60);
			Gl.glEnd();
            break;
    }
  
	Gl.glTranslatef(-64.0f, 0.0f, 0.0f);
	
	for(i = 1; i < 6; i++)
	{
		if(i == 3) 
		{
			Gl.glTranslatef(32.0f, 0.0f, 0.0f);
			continue;
		}
		Gl.glBegin(Gl.GL_QUADS);
			Gl.glTexCoord2f(0.925f, 0.479f);
			Gl.glVertex2i(-4, -4);
			Gl.glTexCoord2f(0.952f, 0.479f);
			Gl.glVertex2i(4, -4);
			Gl.glTexCoord2f(0.952f, 0.505f);
			Gl.glVertex2i(4, 4);
			Gl.glTexCoord2f(0.925f, 0.505f);
			Gl.glVertex2i(-4, 4);
		Gl.glEnd();
		Gl.glTranslatef(32.0f, 0.0f, 0.0f);
	}
		  
    Gl.glPopMatrix();
    
    Gl.glPopMatrix();  // back to what we had coming in
    
    
    
    // draw heading markers
    Gl.glPushMatrix();
    Gl.glColor3f( 1.0f, 1.0f, 1.0f);    
    // translate to center of HSI
    Gl.glTranslatef(300.0f, 600.0f, 0.0f);
	
	Gl.glBegin(Gl.GL_QUADS);
		Gl.glTexCoord2f(0.921f, 0.561f);
		Gl.glVertex2i(-14, -178);
		Gl.glTexCoord2f(0.993f, 0.561f);
		Gl.glVertex2i(14, -178);
		Gl.glTexCoord2f(0.993f, 0.613f);
		Gl.glVertex2i(14, -157);
		Gl.glTexCoord2f(0.921f, 0.613f);
		Gl.glVertex2i(-14, -157);
	Gl.glEnd();
	      
    Gl.glRotatef(-45.0f, 0.0f, 0.0f, 1.0f);
    Gl.glBegin(Gl.GL_QUADS);
		Gl.glTexCoord2f(0.921f, 0.561f);
		Gl.glVertex2i(-7, -174);
		Gl.glTexCoord2f(0.993f, 0.561f);
		Gl.glVertex2i(7, -174);
		Gl.glTexCoord2f(0.993f, 0.613f);
		Gl.glVertex2i(7, -164);
		Gl.glTexCoord2f(0.921f, 0.613f);
		Gl.glVertex2i(-7, -164);
	Gl.glEnd();
	
    Gl.glRotatef(90.0f, 0.0f, 0.0f, 1.0f);
	Gl.glBegin(Gl.GL_QUADS);
		Gl.glTexCoord2f(0.921f, 0.561f);
		Gl.glVertex2i(-7, -174);
		Gl.glTexCoord2f(0.993f, 0.561f);
		Gl.glVertex2i(7, -174);
		Gl.glTexCoord2f(0.993f, 0.613f);
		Gl.glVertex2i(7, -164);
		Gl.glTexCoord2f(0.921f, 0.613f);
		Gl.glVertex2i(-7, -164);
	Gl.glEnd();
            
    Gl.glRotatef(-45.0f, 0.0f, 0.0f, 1.0f);
    Gl.glBegin(Gl.GL_QUADS);
		Gl.glTexCoord2f(0.922f, 0.641f);
        Gl.glVertex2i(160, -2);
		Gl.glTexCoord2f(0.989f, 0.641f);
		Gl.glVertex2i(180, -2);
		Gl.glTexCoord2f(0.989f, 0.655f);
		Gl.glVertex2i(180, 2);
		Gl.glTexCoord2f(0.922f, 0.655f);
		Gl.glVertex2i(160, 2);
	
		Gl.glTexCoord2f(0.922f, 0.641f);
		Gl.glVertex2i(-180, -2);
		Gl.glTexCoord2f(0.989f, 0.641f);
		Gl.glVertex2i(-160, -2);
		Gl.glTexCoord2f(0.989f, 0.655f);
		Gl.glVertex2i(-160, 2);
		Gl.glTexCoord2f(0.922f, 0.655f);
		Gl.glVertex2i(-180, 2);
    Gl.glEnd();
    
    Gl.glPopMatrix();

////// draw heading bug
    Gl.glPushMatrix();
    Gl.glTranslatef(300.0f, 600.0f, 0.0f);
    Gl.glColor3f(1.0f, 0.2f, 1.0f);
       
    Gl.glRotatef((ap.hdgbug - heading), 0.0f, 0.0f, 1.0f);
    
	Gl.glBegin(Gl.GL_QUADS);
		Gl.glTexCoord2f(0.887f, 0.786f);
		Gl.glVertex2i(-15, -170);
		Gl.glTexCoord2f(0.979f, 0.786f);
		Gl.glVertex2i(15, -170);
		Gl.glTexCoord2f(0.979f, 0.830f);
		Gl.glVertex2i(15, -156);
		Gl.glTexCoord2f(0.887f, 0.830f);
		Gl.glVertex2i(-15, -156);
	Gl.glEnd();
	    
    // if heading bug is offscreen draw line instead
    if((ap.hdgbug - heading) > 120 && (ap.hdgbug - heading) < 240)
    {
		Gl.glBegin(Gl.GL_QUADS);
			Gl.glTexCoord2f(0.974f, 0.011f);
			Gl.glVertex2i(-2, -157);
			Gl.glTexCoord2f(0.982f, 0.011f);
			Gl.glVertex2i(2, -157);
			Gl.glTexCoord2f(0.982f, 0.417f);
			Gl.glVertex2i(2, -15);
			Gl.glTexCoord2f(0.974f, 0.417f);
			Gl.glVertex2i(-2, -15);
		Gl.glEnd();
    }    
    Gl.glPopMatrix();
    
    Gl.glColor3f( 1.0f, 1.0f, 1.0f);    
    // draw little fucking airplane
    Gl.glPushMatrix();
    Gl.glTranslatef(300.0f, 600.0f, 0.0f);
    
    Gl.glBegin(Gl.GL_QUADS);
		Gl.glTexCoord2f(0.867f, 0.871f);
		Gl.glVertex2i(-20, -10);
		Gl.glTexCoord2f(0.996f, 0.871f);
		Gl.glVertex2i(20, -10);
		Gl.glTexCoord2f(0.996f, 0.989f);
		Gl.glVertex2i(20, 24);
		Gl.glTexCoord2f(0.867f, 0.989f);
		Gl.glVertex2i(-20, 24);
	
    Gl.glEnd();
    Gl.glPopMatrix();
    
    Gl.glDisable(Gl.GL_SCISSOR_TEST);
    
//// navigation info next to HSI
    Gl.glPushMatrix();
    Gl.glTranslatef(20.0f, 500.0f, 0.0f);
    Gl.glColor3f(0.25f, 1.0f, 0.25f);
     
    // nav source    
    Gl.glPushMatrix();
    font.glPrint(0.75f, "VOR 1");     
    Gl.glPopMatrix();
    
    // nav course
    Gl.glPushMatrix();
    Gl.glTranslatef(0.0f, 30.0f, 0.0f);
	font.glPrint(0.75f, "CRS "+  course.ToString());     
    Gl.glPopMatrix();

    Gl.glPushMatrix();
    Gl.glTranslatef(0.0f, 60.0f, 0.0f);
    font.glPrint(0.75f, Math.Round(dme,1).ToString() +" NM");
    Gl.glPopMatrix();

    Gl.glPushMatrix();
    Gl.glTranslatef(0.0f, 90.0f, 0.0f);
//		font.glPrint(0.75f, "%s", nav.capt_fix); 
	font.glPrint(0.75f, "IFUK");
    Gl.glPopMatrix();

    Gl.glPopMatrix();
}

/******
AIRSPEED INDICATOR CODE
******/
void DrawIAS()
{
    float ias;
    float offset, tens, tmp;
    int i;

	ias = (float)adc.ias / ADC_IAS_SCALE;
	
    if(ias < 40) ias = 40;  // minimum displayed airspeed
    if(ap.iasbug < 40) ap.iasbug = 40;        // minimum bug speed
    
    offset = (ias - 40) * IAS_SCALE_FACTOR;
    
    tens = Convert.ToSingle(Math.Floor(ias / 10) * 10);
       
    // setup scissor window for airspeed tape
    Gl.glScissor((int)(posx + (20 * scale)), (int)(myheight - (posy + (400 * scale))), (int)(120 * scale), (int)(300 * scale));
    Gl.glEnable(Gl.GL_SCISSOR_TEST);
    Gl.glPushMatrix();
    
    Gl.glTranslatef(20.0f, 250.0f, 0.0f);
    
    Gl.glColor3f( 1.0f, 1.0f, 1.0f);
    Gl.glLineWidth(2.5f * scale);
    Gl.glPointSize(2.5f * scale);
    
    Gl.glPushMatrix();
    
    // draw airspeed labels and scale
    for(i = (int)(tens - 60); i < (int)(tens + 60); i += 5)
    {
		Gl.glBindTexture(Gl.GL_TEXTURE_2D, imageloader.get(TEXTURE_AI));
        if (i < 40) {
            i = 35;
            continue;
        }
        if (i % 2 != 0)
        {
            Gl.glBegin(Gl.GL_QUADS);
				Gl.glTexCoord2f(0.120f, 0.920f);
				Gl.glVertex2f(76.0f, (-((i - 40) * IAS_SCALE_FACTOR) + offset) - 2);
				Gl.glTexCoord2f(0.152f, 0.920f);
				Gl.glVertex2f(82.0f, (-((i - 40) * IAS_SCALE_FACTOR) + offset) - 2);
				Gl.glTexCoord2f(0.152f, 0.934f);
				Gl.glVertex2f(82.0f, (-((i - 40) * IAS_SCALE_FACTOR) + offset) + 2);
				Gl.glTexCoord2f(0.120f, 0.934f);
				Gl.glVertex2f(76.0f, (-((i - 40) * IAS_SCALE_FACTOR) + offset) + 2);			
            Gl.glEnd();
            continue;
        }
        if ((i / 10) % 2 != 0) 
        {
			Gl.glBegin(Gl.GL_QUADS);
				Gl.glTexCoord2f(0.120f, 0.920f);
				Gl.glVertex2f(76.0f, (-((i - 40) * IAS_SCALE_FACTOR) + offset) - 2);
				Gl.glTexCoord2f(0.152f, 0.920f);
				Gl.glVertex2f(82.0f, (-((i - 40) * IAS_SCALE_FACTOR) + offset) - 2);
				Gl.glTexCoord2f(0.152f, 0.934f);
				Gl.glVertex2f(82.0f, (-((i - 40) * IAS_SCALE_FACTOR) + offset) + 2);
				Gl.glTexCoord2f(0.120f, 0.934f);
				Gl.glVertex2f(76.0f, (-((i - 40) * IAS_SCALE_FACTOR) + offset) + 2);			
            Gl.glEnd();
            continue;
        }
        Gl.glBegin(Gl.GL_QUADS);
				Gl.glTexCoord2f(0.120f, 0.920f);
				Gl.glVertex2f(76.0f, (-((i - 40) * IAS_SCALE_FACTOR) + offset) - 2);
				Gl.glTexCoord2f(0.152f, 0.920f);
				Gl.glVertex2f(82.0f, (-((i - 40) * IAS_SCALE_FACTOR) + offset) - 2);
				Gl.glTexCoord2f(0.152f, 0.934f);
				Gl.glVertex2f(82.0f, (-((i - 40) * IAS_SCALE_FACTOR) + offset) + 2);
				Gl.glTexCoord2f(0.120f, 0.934f);
				Gl.glVertex2f(76.0f, (-((i - 40) * IAS_SCALE_FACTOR) + offset) + 2);			
        Gl.glEnd();
        Gl.glPushMatrix();
        if (i < 100)
            Gl.glTranslatef(38.0f, -((i - 40) * IAS_SCALE_FACTOR) + offset, 0.0f);
        else
            Gl.glTranslatef(24.0f, -((i - 40) * IAS_SCALE_FACTOR) + offset, 0.0f);
        font.glPrint(0.75f,  i.ToString());
        Gl.glPopMatrix();
    }
        
    // right side line    
	Gl.glBegin(Gl.GL_QUADS);
    if(ias < 100)
    {
		Gl.glTexCoord2f(0.016f, 0.981f);
		Gl.glVertex2f(80.0f, offset + 2);
		Gl.glTexCoord2f(0.755f, 0.981f);
		Gl.glVertex2f(80.0f, offset - 250);
		Gl.glTexCoord2f(0.755f, 0.991f);
		Gl.glVertex2f(84.0f, offset - 250);
		Gl.glTexCoord2f(0.016f, 0.991f);
		Gl.glVertex2f(84.0f, offset + 2);
		
		Gl.glTexCoord2f(0.016f, 0.981f);
		Gl.glVertex2f(80.0f, offset - 248);
		Gl.glTexCoord2f(0.755f, 0.981f);
		Gl.glVertex2f(80.0f, offset - 500);
		Gl.glTexCoord2f(0.755f, 0.991f);
		Gl.glVertex2f(84.0f, offset - 500);
		Gl.glTexCoord2f(0.016f, 0.991f);
		Gl.glVertex2f(84.0f, offset - 248);
    }
    else
    {
		Gl.glTexCoord2f(0.016f, 0.981f);
		Gl.glVertex2f(80.0f, 300);
		Gl.glTexCoord2f(0.755f, 0.981f);
		Gl.glVertex2f(80.0f, -2);
		Gl.glTexCoord2f(0.755f, 0.991f);
		Gl.glVertex2f(84.0f, -2);
		Gl.glTexCoord2f(0.016f, 0.991f);
		Gl.glVertex2f(84.0f, 300);

		Gl.glTexCoord2f(0.016f, 0.981f);
		Gl.glVertex2f(80.0f, 2);
		Gl.glTexCoord2f(0.755f, 0.981f);
		Gl.glVertex2f(80.0f, -300);
		Gl.glTexCoord2f(0.755f, 0.991f);
		Gl.glVertex2f(84.0f, -300);
		Gl.glTexCoord2f(0.016f, 0.991f);
		Gl.glVertex2f(84.0f, 2);
    }
    Gl.glEnd();
        
    Gl.glPopMatrix();   

    // draw trend line
    if(!adc.wow || adc.iastrend == 0)
    {
        Gl.glColor3f(1.0f, 0.2f, 1.0f);
        Gl.glLineWidth(2.5f * scale);
        Gl.glPointSize(2.5f * scale);
    
        Gl.glBegin(Gl.GL_LINES);
            Gl.glVertex2f(107.0f, 0.0f);
            Gl.glVertex2f(107.0f, -adc.iastrend * IAS_SCALE_FACTOR);
            Gl.glVertex2f(112.0f, -adc.iastrend * IAS_SCALE_FACTOR);
            Gl.glVertex2f(102, -adc.iastrend * IAS_SCALE_FACTOR);
        Gl.glEnd();
    }
    
    Gl.glColor3f( 1.0f, 1.0f, 1.0f);
	Gl.glEnable(Gl.GL_TEXTURE_2D);
	Gl.glBindTexture(Gl.GL_TEXTURE_2D, imageloader.get(TEXTURE_AI));
	Gl.glBegin(Gl.GL_QUADS);
		Gl.glTexCoord2f(0.476f, 0.873f);
		Gl.glVertex2i(84, -12);
		Gl.glTexCoord2f(0.604f, 0.873f);
		Gl.glVertex2i(113, -12);
		Gl.glTexCoord2f(0.604f, 0.971f);
		Gl.glVertex2i(113, 12);
		Gl.glTexCoord2f(0.476f, 0.971f);
		Gl.glVertex2i(84, 12);
	Gl.glEnd();
	
    Gl.glLineWidth(3.0f * scale);
    Gl.glPointSize(3.0f * scale); 

/// draw speed reference lines
    Gl.glColor3f( 0.0f, 1.0f, 1.0f);
   

    // v1
	Gl.glBegin(Gl.GL_QUADS);
		Gl.glTexCoord2f(0.016f, 0.941f);
		Gl.glVertex2f(30.0f, (-((efis.v1 - 40) * IAS_SCALE_FACTOR) + offset) - 2);
		Gl.glTexCoord2f(0.100f, 0.941f);
		Gl.glVertex2f(105.0f, (-((efis.v1 - 40) * IAS_SCALE_FACTOR) + offset) - 2);
		Gl.glTexCoord2f(0.100f, 0.953f);
		Gl.glVertex2f(105.0f, (-((efis.v1 - 40) * IAS_SCALE_FACTOR) + offset) + 2);
		Gl.glTexCoord2f(0.016f, 0.953f);
		Gl.glVertex2f(30.0f, (-((efis.v1 - 40) * IAS_SCALE_FACTOR) + offset) + 2);
	Gl.glEnd();
	
	Gl.glPushMatrix();
    Gl.glTranslatef(107.0f, -((efis.v1 - 40) * IAS_SCALE_FACTOR) + offset, 0.0f);
    font.glPrint(0.5f, "1");     
    Gl.glPopMatrix();

    // v2
	Gl.glBindTexture(Gl.GL_TEXTURE_2D, imageloader.get(TEXTURE_AI));
	Gl.glBegin(Gl.GL_QUADS);
		Gl.glTexCoord2f(0.016f, 0.941f);
		Gl.glVertex2f(30.0f, (-((efis.v2 - 40) * IAS_SCALE_FACTOR) + offset) - 2);
		Gl.glTexCoord2f(0.100f, 0.941f);
		Gl.glVertex2f(105.0f, (-((efis.v2 - 40) * IAS_SCALE_FACTOR) + offset) - 2);
		Gl.glTexCoord2f(0.100f, 0.953f);
		Gl.glVertex2f(105.0f, (-((efis.v2 - 40) * IAS_SCALE_FACTOR) + offset) + 2);
		Gl.glTexCoord2f(0.016f, 0.953f);
		Gl.glVertex2f(30.0f, (-((efis.v2 - 40) * IAS_SCALE_FACTOR) + offset) + 2);
	Gl.glEnd();
	
    Gl.glPushMatrix();
    Gl.glTranslatef(107.0f, -((efis.v2 - 40) * IAS_SCALE_FACTOR) + offset, 0.0f);
    font.glPrint(0.5f, "2");     
    Gl.glPopMatrix();

    // vt
	Gl.glBindTexture(Gl.GL_TEXTURE_2D, imageloader.get(TEXTURE_AI));
	Gl.glBegin(Gl.GL_QUADS);
		Gl.glTexCoord2f(0.016f, 0.941f);
		Gl.glVertex2f(30.0f, (-((efis.vt - 40) * IAS_SCALE_FACTOR) + offset) - 2);
		Gl.glTexCoord2f(0.100f, 0.941f);
		Gl.glVertex2f(90.0f, (-((efis.vt - 40) * IAS_SCALE_FACTOR) + offset) - 2);
		Gl.glTexCoord2f(0.100f, 0.953f);
		Gl.glVertex2f(90.0f, (-((efis.vt - 40) * IAS_SCALE_FACTOR) + offset) + 2);
		Gl.glTexCoord2f(0.016f, 0.953f);
		Gl.glVertex2f(30.0f, (-((efis.vt - 40) * IAS_SCALE_FACTOR) + offset) + 2);
	Gl.glEnd();
   
    Gl.glPushMatrix();
    Gl.glTranslatef(92.0f, -((efis.vt - 40) * IAS_SCALE_FACTOR) + offset, 0.0f);
    font.glPrint(0.5f, "T");     
    Gl.glPopMatrix();

    // vr
	Gl.glBindTexture(Gl.GL_TEXTURE_2D, imageloader.get(TEXTURE_AI));
    Gl.glBegin(Gl.GL_QUADS);
		Gl.glTexCoord2f(0.016f, 0.941f);
		Gl.glVertex2f(30.0f, (-((efis.vr - 40) * IAS_SCALE_FACTOR) + offset) - 2);
		Gl.glTexCoord2f(0.100f, 0.941f);
		Gl.glVertex2f(90.0f, (-((efis.vr - 40) * IAS_SCALE_FACTOR) + offset) - 2);
		Gl.glTexCoord2f(0.100f, 0.953f);
		Gl.glVertex2f(90.0f, (-((efis.vr - 40) * IAS_SCALE_FACTOR) + offset) + 2);
		Gl.glTexCoord2f(0.016f, 0.953f);
		Gl.glVertex2f(30.0f, (-((efis.vr - 40) * IAS_SCALE_FACTOR) + offset) + 2);
	Gl.glEnd();
    Gl.glPushMatrix();
    Gl.glTranslatef(92.0f, -((efis.vr - 40) * IAS_SCALE_FACTOR) + offset, 0.0f);
    font.glPrint(0.5f, "R");     
    Gl.glPopMatrix();

//////  draw airspeed bug
    tmp = -((ap.iasbug - 40) * IAS_SCALE_FACTOR) + offset;
    
    
    Gl.glLineWidth(3.0f * scale);
    Gl.glColor3f(1.0f, 0.2f, 1.0f);
	Gl.glBindTexture(Gl.GL_TEXTURE_2D, imageloader.get(TEXTURE_AI));
	Gl.glBegin(Gl.GL_QUADS);
		Gl.glTexCoord2f(0.463f, 0.122f);
		Gl.glVertex2f(76.0f, tmp - 17);
		Gl.glTexCoord2f(0.463f, 0.079f);
		Gl.glVertex2f(90.0f, tmp - 17);
		Gl.glTexCoord2f(0.554f, 0.079f);
		Gl.glVertex2f(90.0f, tmp + 17);
		Gl.glTexCoord2f(0.554f, 0.122f);
		Gl.glVertex2f(76.0f, tmp + 17);
	Gl.glEnd();

    Gl.glDisable(Gl.GL_SCISSOR_TEST);
    
    Gl.glPopMatrix();
    Gl.glPushMatrix();
    Gl.glTranslatef(42.0f, 420.0f, 0.0f);
	
///// draw airspeed bug reference
    Gl.glColor3f(1.0f, 0.2f, 1.0f);
	Gl.glBindTexture(Gl.GL_TEXTURE_2D, imageloader.get(TEXTURE_AI));
	Gl.glBegin(Gl.GL_QUADS);
		Gl.glTexCoord2f(0.463f, 0.123f);
		Gl.glVertex2f(-2.0f, -17);
		Gl.glTexCoord2f(0.463f, 0.078f);
		Gl.glVertex2f(12.0f, -17);
		Gl.glTexCoord2f(0.554f, 0.078f);
		Gl.glVertex2f(12.0f, 17);
		Gl.glTexCoord2f(0.554f, 0.123f);
		Gl.glVertex2f(-2.0f, 17);
	Gl.glEnd();
	
    Gl.glPushMatrix();
    Gl.glTranslatef(28.0f, 0.0f, 0.0f);
    font.glPrint(0.75f,  ap.iasbug.ToString());     
    Gl.glPopMatrix();
    
    Gl.glPopMatrix();

    
}

/******
VSI CODE
******/

void DrawVSI()
{
    float vspeed;
	
	vspeed = (float)(adc.vspeed / ADC_VS_SCALE);
	
	Gl.glPushMatrix();
    
    Gl.glTranslatef(560.0f, 575.0f, 0.0f);

	Gl.glPushMatrix();
    
    // draw VSI needle
	Gl.glColor3f( 1.0f, 1.0f, 1.0f);
    Gl.glPushMatrix();
    
    if(vspeed > 0)
    {
        if(vspeed > 1000.0f)
        {
            if(vspeed > 2000.0f)
            {
                if(vspeed >= 4000.0f)
                    Gl.glRotatef(90.0f, 0.0f, 0.0f, 1.0f);
                else    // vspeed is between 2000 and 4000
                    Gl.glRotatef(75.0f + ((vspeed - 2000.0f) * .0075f), 0.0f, 0.0f, 1.0f);
            }
            else // vspeed is between 1000 and 2000
                Gl.glRotatef(60.0f + ((vspeed - 1000.0f) * 0.015f), 0.0f, 0.0f, 1.0f);
        }
        else // vspeed is between 0 and 1000    
            Gl.glRotatef(6.0f * (vspeed/100.0f), 0.0f, 0.0f, 1.0f);
    }
    else if(vspeed < 0)    // vspeed < 0
    {
        if(vspeed < -1000.0f)
        {
            if(vspeed < -2000.0f)
            {
                if(vspeed <= -4000.0f)
                    Gl.glRotatef(-90.0f, 0.0f, 0.0f, 1.0f);
                else    // vspeed is between -2000 and -4000
                    Gl.glRotatef(-75.0f + ((vspeed + 2000.0f) * .0075f), 0.0f, 0.0f, 1.0f);
                    
            }
            else    // vspeed is between -1000 and -2000
                Gl.glRotatef(-60.0f + ((vspeed + 1000.0f) * 0.015f), 0.0f, 0.0f, 1.0f);
        }
        else    // vspeed is between 0 and -1000
            Gl.glRotatef(6.0f * (vspeed/100.0f), 0.0f, 0.0f, 1.0f);
    }


	Gl.glBindTexture(Gl.GL_TEXTURE_2D, imageloader.get(TEXTURE_AI));
	Gl.glBegin(Gl.GL_QUADS);
		Gl.glTexCoord2f(0.787f, 0.892f);
		Gl.glVertex2i(-80, 6);
		Gl.glTexCoord2f(0.99f, 0.892f);
		Gl.glVertex2i(-33, 6);
		Gl.glTexCoord2f(0.99f, 0.949f);
		Gl.glVertex2i(-33, -6);
		Gl.glTexCoord2f(0.787f, 0.949f);
		Gl.glVertex2i(-80, -6);
	Gl.glEnd();
	    
    Gl.glPopMatrix();      
    
	
	
	// draw tick marks
	Gl.glBegin(Gl.GL_QUADS);
		Gl.glTexCoord2f(0.636f, 0.253f);
		Gl.glVertex2i(-80, -80);
		Gl.glTexCoord2f(0.951f, 0.253f);
		Gl.glVertex2i(1, -80);
		Gl.glTexCoord2f(0.951f, 0.871f);
		Gl.glVertex2i(1, 80);
		Gl.glTexCoord2f(0.636f, 0.871f);
		Gl.glVertex2i(-80, 80);
	Gl.glEnd();
	
    Gl.glPopMatrix();
    
    // draw numbers
    Gl.glPushMatrix();
    Gl.glTranslatef(-6.0f, 92.0f, 0.0f);
    font.glPrint(0.6f, "4");
    Gl.glPopMatrix();
    
    Gl.glPushMatrix();
    Gl.glTranslatef(-6.0f, -92.0f, 0.0f);
    font.glPrint(0.6f, "4");
    Gl.glPopMatrix();
    
    Gl.glPushMatrix();
    Gl.glTranslatef(-28.0f, -88.0f, 0.0f);
    font.glPrint(0.6f, "2");
    Gl.glPopMatrix();
    
    Gl.glPushMatrix();
    Gl.glTranslatef(-28.0f, 88.0f, 0.0f);
    font.glPrint(0.6f, "2");
    Gl.glPopMatrix();
    
    Gl.glPushMatrix();
    Gl.glTranslatef(-50.0f, -78.0f, 0.0f);
    font.glPrint(0.6f, "1");
    Gl.glPopMatrix();

    Gl.glPushMatrix();
    Gl.glTranslatef(-50.0f, 78.0f, 0.0f);
    font.glPrint(0.6f, "1");
    Gl.glPopMatrix();    
               
    // draw speed numbers
    Gl.glPushMatrix();
    Gl.glColor3f(0.25f, 1.0f, 0.25f);
    Gl.glTranslatef(-28.0f, 0.0f, 0.0f);
    font.glPrint(0.7f,  (vspeed/1000).ToString());
    
	Gl.glPopMatrix();
	Gl.glPopMatrix();
}

        private void simpleOpenGlControl1_Resize(object sender, EventArgs e)
        {
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();

            Gl.glViewport(0, 0, 757, 693);
            Gl.glOrtho(0, 757, 693, 0, 0, 1000);

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            mywidth = 660;
            myheight = 700;
            wnd_width = 500;
            wnd_height = 500;
            
            //var scaleheight = (Convert.ToSingle(Height) / 300);
            //var scalewidth = (Convert.ToSingle(Width) / 300);
            //scale = scaleheight;
            //if (scalewidth < scaleheight) scale = scalewidth;
            //posx = -150*scale;
            //posy = -100 * scale;
        }
        
    }
}

