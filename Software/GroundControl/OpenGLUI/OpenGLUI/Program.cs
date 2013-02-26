// Title:	Program.cs
// Author: 	Scott Ellington <scott.ellington@gmail.com>
//
// Copyright (C) 2006-2007 Scott Ellington and authors
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Tao.FreeGlut;
using Tao.OpenGl;
using System.IO;
using System.Windows.Forms;
namespace SalmonViewer 
{
	/// <summary>
	/// Main class
	/// </summary>
    public sealed class Program 
	{
		
#region Constants		
		
		// angle to radian
		const double ATR = .01745;

#endregion		
		
#region Vars		
		
		static int pace = 1;

		static int winW = 400;
		static int winH = 400;

		static Model model;
		static ThreeDSFile file;

		static float[] rot = new float[] {0,0,0}; /* Amount to rotate */
		static float[] eye = new float[] {0,0,0}; /* Position of our eye or camera */
		static float[] light = new float[] {200, 100, 40}; 

		static bool[] keydown = new bool [256];
		
		static Vector modelRotation = new Vector(0,0,0);
		static Vector modelCenter;
		
#endregion
		
#region Main method		
		
		/// <summary>
		/// Main method
		/// </summary>
		/// <param name="argv">
		/// A <see cref="System.String"/>
		/// </param>
		public static void Main (string[] argv) 
		{

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TestForm());
		}
		
#endregion
		
#region Helper methods

		/// <summary>
		/// Prints Viewer instructions to Console
		/// </summary>
		static void PrintInstructions ()
		{
			string text = "Salmon Viewer commands: \n" +
				" 'w','s' move forward and backward \n" +
				" 'a','d' turn left and right\n" +
				" 'z','x' move up and down\n" +
				" '-','=' move light source along x-axis\n" +
				" '[',']' rotate object on x-axis\n" +
				" ';',''' rotate object on y-axis\n" +
				" '.','/' rotate object on z-axis\n" +
				" click and drag the mouse to change direction";
			Console.WriteLine ( text );
		}
		
		/// <summary>
		/// Initalizes OpenGL parameters
		/// </summary>
		private static void Init() 
		{
			// enable depth
			Gl.glEnable ( Gl.GL_DEPTH_TEST );
			
			// enable fill
			Gl.glPolygonMode ( Gl.GL_FRONT_AND_BACK, Gl.GL_FILL );	
			//Gl.glPolygonMode ( Gl.GL_FRONT_AND_BACK, Gl.GL_LINE );

			// set background to white
			Gl.glClearColor( 1.0f, 1.0f, 1.0f, 1.0f );
			
			// smooth shading
			Gl.glShadeModel ( Gl.GL_SMOOTH );
			
			// enable culling (improves performance i think)
			Gl.glEnable(Gl.GL_CULL_FACE);
			Gl.glCullFace(Gl.GL_BACK);
			
			//Gl.glBlendFunc(Gl.GL_SRC_ALPHA_SATURATE, Gl.GL_ONE)

			// enable lighting
			Gl.glEnable( Gl.GL_LIGHT0 );
			Gl.glEnable( Gl.GL_LIGHTING );
			Gl.glLightModeli( Gl.GL_LIGHT_MODEL_TWO_SIDE, Gl.GL_TRUE);

			//Gl.glEnable( Gl.GL_TEXTURE_2D );
		}

		/// <summary>
		/// draws the scene
		/// </summary>
		private static void Display() 
		{
			// clear out the current OpenGL context
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
			//Gl.glDisable(Gl.GL_BLEND);
			//Gl.glDisable(Gl.GL_POLYGON_SMOOTH);
			
			// create our view matrix and load into OpenGL
			float[] matrix = Matrix.Transform ( rot[0], rot[1], rot[2], eye[0], eye[1], eye[2] );
			matrix = Matrix.Inverse ( matrix );
			Gl.glLoadMatrixf ( matrix );

			// rotate object as needed
			Gl.glTranslated(modelCenter.X, modelCenter.Y, modelCenter.Z);
			Gl.glRotated(modelRotation.X,1,0,0); 
			Gl.glRotated(modelRotation.Y,0,1,0);
			Gl.glRotated(modelRotation.Z,0,0,1);
			Gl.glTranslated(-modelCenter.X, -modelCenter.Y, -modelCenter.Z);
			
			//Gl.glPopMatrix();
			
			// set the lighting
			Gl.glLightfv( Gl.GL_LIGHT0, Gl.GL_POSITION, light);
			
			//draw floor
			/*
			for (int ii = 50; ii > -50; ii-=5)
			{
				Gl.glBegin (Gl.GL_TRIANGLE_STRIP);
				Gl.glNormal3f(0.0f, 1.0f, 0.0f) ;

				for (int jj = -50; jj < 50; jj+=5)
				{
					Gl.glColor3f (0.4f,0.6f,0.8f);
					Gl.glVertex3f (jj,-10,ii);	
					Gl.glVertex3f (jj,-10,ii+5);	
				}

				Gl.glEnd ();
			}*/
			
			// render our model
			model.Render ();
			
			// flush out what in the OpenGL context
			Gl.glFlush ();
			
			// we are double buffering
			Glut.glutSwapBuffers ();
		}

		/// <summary>
		/// Handles flagging keys as down.  
		/// </summary>
		/// <param name="key">
		/// A <see cref="System.Byte"/>
		/// </param>
		/// <param name="x">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="y">
		/// A <see cref="System.Int32"/>
		/// </param>
		private static void Keyboard(byte key, int x, int y) { keydown [ key ] = true; }
		
		/// <summary>
		/// Handles unflagging keys
		/// </summary>
		/// <param name="key">
		/// A <see cref="System.Byte"/>
		/// </param>
		/// <param name="x">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="y">
		/// A <see cref="System.Int32"/>
		/// </param>
		private static void KeyboardUp(byte key, int x, int y) { keydown [ key ] = false; }

		/// <summary>
		/// Idle handler.  Occurs as often as the cpu will allow.
		/// </summary>
		static void Idle ()
		{
			// go through the keys and deal with user input
			for ( int ii=0; ii < keydown.Length ; ii++ )
			{
				if ( keydown [ii] )
				{
					switch ( (char) ii) 
					{
						case 'x':
							eye[1]++;
							break;
						case 'z':
							eye[1]--;
							break;
						case 'w':
							eye[0] -= (float) Math.Sin (rot[1]*ATR) * pace;
							eye[2] -= (float) Math.Cos (rot[1]*ATR) * pace;
							break;
						case 's':
							eye[0] += (float) Math.Sin(rot[1]*ATR) * pace;
							eye[2] += (float) Math.Cos(rot[1]*ATR) * pace;
							break;
						case 'a':
							rot[1] += pace;
							break;
						case 'd':
							rot[1] -= pace;
							break;
						case '=':
							light[0]++;
							break;
						case '-':
							light[0]--;
							break;
						case '[':
							modelRotation.X--;
							break;
						case ']':
							modelRotation.X++;
							break;
						case ';':
							modelRotation.Y--;
							break;
						case '\'':
							modelRotation.Y++;
							break;
						case '/':
							modelRotation.Z++;
							break;
						case '.':
							modelRotation.Z--;
							break;
						case (char) 27:
							Environment.Exit(0);
							break;
						default:
							break;
					}
				}
			}
			
			// render the model
			Glut.glutPostRedisplay();	
		}

		/// <summary>
		/// Handles a window resize
		/// </summary>
		/// <param name="w">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="h">
		/// A <see cref="System.Int32"/>
		/// </param>
		private static void Reshape(int w, int h) 
		{
			Gl.glViewport(0, 0, w, h);
			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glLoadIdentity();
			//Glu.gluPerspective(30.0, (float) w / (float) h, 1.0, 20.0);
			Glu.gluPerspective (90, Convert.ToDouble(winW) / Convert.ToDouble(winH), 1, 9999);
			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glLoadIdentity();
		}

		static bool started = false;
		static int lastX, lastY;

		/// <summary>
		/// handles mouse motion
		/// </summary>
		/// <param name="x">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="y">
		/// A <see cref="System.Int32"/>
		/// </param>
		private static void Motion ( int x, int y )
		{
			// changes rotation
			if (started && x > 0 && y > 0 && x < winW && y < winH)
			{
				rot[1] -= (float) (x-lastX)/3;
				rot[0] += (float) (y-lastY)/3;
			}
			else started = true;

			// track last val
			lastY = y;
			lastX = x;
			
			// keep rotation values sane
			Clamp ();

			// redraw
			Glut.glutPostRedisplay();	
		}

		/// <summary>
		/// ensures rotation parameters stay within 360
		/// </summary>
		static void Clamp ()
		{
			for (int i = 0; i < 3; i ++)
				if (rot[i] >= 360 || rot[i] < -360)
					rot[i] = 0;
		}
		
#endregion

	}
}
