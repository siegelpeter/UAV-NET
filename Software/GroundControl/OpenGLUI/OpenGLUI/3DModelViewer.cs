using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using SalmonViewer;
using Tao.FreeGlut;
using Tao.OpenGl;

namespace OpenGLUI
{
    public partial class _3DModelViewer : UserControl
    {


           // angle to radian
                const double ATR = .01745;

        
                
    
                
                static int pace = 1;
        private int i = 0;
        static Model model;
        static ThreeDSFile file;

        static float[] rot = new float[] { 0, 0, 0 }; /* Amount to rotate */
        static float[] eye = new float[] { 0, 0, 0 }; /* Position of our eye or camera */
        static float[] light = new float[] { 200, 100, 40 };

        private bool demomode = false;

        public Vector modelRotation = new Vector(0, 0, 0);
        public Vector modelCenter;

        public void SetRotation(int x, int y,int z)
        {
            modelRotation.X = x;
            modelRotation.Y = y;
            modelRotation.Z = z;
        }

        public void UpdateDemo(bool demo)
        {
            this.demomode = demo;
            if (demo){
             //  rot[1]++;
                i++;
                if (i >= 180) i = -180;
                modelRotation.Y = i;
                modelRotation.X = i;
                modelRotation.Z = i;
            }
            simpleOpenGlControl1.Invalidate();
        }

        public _3DModelViewer()
        {
            InitializeComponent();
            this.simpleOpenGlControl1.InitializeContexts();
            Init();
            try
            {
                file = new ThreeDSFile("./models/Airplane.3ds");
                model = file.Model;
                modelCenter = new Vector((file.MaxX - file.MinX) / 2 + file.MinX,
                                         (file.MaxY - file.MinY) / 2 + file.MinY,
                                         (file.MaxZ - file.MinZ) / 2 + file.MinZ);
                
                // move eye so model is entirely visible at startup
                modelRotation = new Vector(0, 0, 0);
                // center x/y at model's center x/y
                double width = file.MaxX - file.MinX;
                double height = file.MaxY - file.MinY;
                eye[0] = Convert.ToSingle(file.MinX + width / 2);
                eye[1] = Convert.ToSingle(file.MinY + height / 2);

                // use trigonometry to calculate the z value that exposes the model
                eye[2] = Convert.ToSingle(file.MaxZ + (width > height ? width : height / 2) /
                                          Math.Tan((Math.PI / 180) * 90 / 2));
            }
            catch (Exception ex)
            {

            }
        }






        /// <summary>
        /// Initalizes OpenGL parameters
        /// </summary>
        public void Init()
        {
            // enable depth
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            // set background to white
      //      Gl.glClearColor(0.7f, 1.0f, 1.0f, 1.0f);
            // enable fill
            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL);
            //Gl.glPolygonMode ( Gl.GL_FRONT_AND_BACK, Gl.GL_LINE );

            // set background to white
      //      Gl.glClearColor(1.0f, 1.0f, 1.0f, 1.0f);

            // smooth shading
            Gl.glShadeModel(Gl.GL_SMOOTH);

            // enable culling (improves performance i think)
            Gl.glEnable(Gl.GL_CULL_FACE);
            Gl.glCullFace(Gl.GL_BACK);

            //Gl.glBlendFunc(Gl.GL_SRC_ALPHA_SATURATE, Gl.GL_ONE)

            //// enable lighting
            //Gl.glEnable(Gl.GL_LIGHT0);
            //Gl.glEnable(Gl.GL_LIGHTING);
            //Gl.glLightModeli(Gl.GL_LIGHT_MODEL_TWO_SIDE, Gl.GL_TRUE);

            Gl.glEnable(Gl.GL_TEXTURE_2D);


            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            //Glu.gluPerspective(30.0, (float) w / (float) h, 1.0, 20.0);
            Glu.gluPerspective(90, this.Width / this.Height, 1, 999999);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
        }

        public override void Refresh()
        {
            base.Refresh();
            simpleOpenGlControl1.Invalidate();
        }
        void simpleOpenGlControl1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {

            // clear out the current OpenGL context
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
           

            //Gl.glDisable(Gl.GL_BLEND);
            //Gl.glDisable(Gl.GL_POLYGON_SMOOTH);

            // create our view matrix and load into OpenGL
            float[] matrix = Matrix.Transform(rot[0], rot[1], rot[2], eye[0], eye[1], eye[2]);
            matrix = Matrix.Inverse(matrix);
            Gl.glLoadMatrixf(matrix);

            // rotate object as needed
          Gl.glTranslated(modelCenter.X, modelCenter.Y, modelCenter.Z);
            Gl.glRotated(modelRotation.X, 1, 0, 0);
            Gl.glRotated(modelRotation.Y, 0, 1, 0);
            Gl.glRotated(modelRotation.Z, 0, 0, 1);
            Gl.glTranslated(-modelCenter.X, -modelCenter.Y, -modelCenter.Z);

            //Gl.glPopMatrix();

            // set the lighting
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, light);

            //draw floor

            //for (int ii = 50; ii > -50; ii -= 5)
            //{
            //    Gl.glBegin(Gl.GL_TRIANGLE_STRIP);
            //    Gl.glNormal3f(0.0f, 1.0f, 0.0f);

            //    for (int jj = -50; jj < 50; jj += 5)
            //    {
            //        Gl.glColor3f(0.4f, 0.6f, 0.8f);
            //        Gl.glVertex3f(jj, -10, ii);
            //        Gl.glVertex3f(jj, -10, ii + 5);
            //    }

            //    Gl.glEnd();
            //}

            // render our model
                if (model != null) model.Render();

                // flush out what in the OpenGL context
                Gl.glFlush();

                // we are double buffering
             

            }

        private void simpleOpenGlControl1_Resize(object sender, EventArgs e)
        {
            Gl.glViewport(0, 0, this.Width, this.Height);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
          
            //Glu.gluPerspective(30.0, (float) w / (float) h, 1.0, 20.0);
            Glu.gluPerspective(90, Convert.ToDouble(this.Width) / Convert.ToDouble(this.Height), 1, 99999);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
        }
        
    }
}

