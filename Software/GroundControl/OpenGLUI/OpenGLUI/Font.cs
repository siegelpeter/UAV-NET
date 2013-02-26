using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using Tao.OpenGl;
using Tao.FreeGlut;
using GLuint = System.Int32;
namespace SalmonViewer
{





    public class Font
    {

     


        imageLoader loader = new imageLoader();

        private static float FontScale =0.5f;
float cx, cy;
int count = 0;
    GLuint basenumber;
        private string texture ="font";
        public Font(){
            loader.LoadFromFile(System.IO.Path.GetFullPath("data/font.png"), texture);
      
	basenumber = Gl.glGenLists(256);								
	
	Gl.glBindTexture(Gl.GL_TEXTURE_2D, loader.get(texture));
	
	for(int loop = 0; loop < 10; loop++)
	{
		for(int loop2 = 0; loop2 < 16; loop2++)
		{
			cx = (float)(loop2 * 0.0625f);
			cy = (float)(loop * 0.09375f);
			if(count == 14)	// period
			{
				Gl.glNewList(basenumber + count, Gl.GL_COMPILE);
				Gl.glBegin(Gl.GL_TRIANGLE_STRIP);
					Gl.glTexCoord2f(cx + 0.02553f, cy + 0.09375f);
					Gl.glVertex2i(8, 12);
					Gl.glTexCoord2f(cx+0.04897f,cy + 0.09375f);
					Gl.glVertex2i(0, 12);							
					Gl.glTexCoord2f(cx+0.04897f, cy);
					Gl.glVertex2i(0, -12);	
					Gl.glTexCoord2f(cx + 0.02553f,cy);
					Gl.glVertex2i(8, -12);
				Gl.glEnd();	
				Gl.glTranslated(8, 0, 0);
				Gl.glEndList();
			}
			else {
				Gl.glNewList(basenumber + count, Gl.GL_COMPILE);	
					Gl.glBegin(Gl.GL_QUADS);
						Gl.glTexCoord2f(cx,cy + 0.09375f);				
						Gl.glVertex2i(0, 12);							
						Gl.glTexCoord2f(cx+0.0625f,cy + 0.09375f);
						Gl.glVertex2i(16, 12);							
						Gl.glTexCoord2f(cx+0.0625f, cy);
						Gl.glVertex2i(16, -12);	
						Gl.glTexCoord2f(cx,cy);
						Gl.glVertex2i(0, -12);
					Gl.glEnd();	
					Gl.glTranslated(18, 0, 0);
				Gl.glEndList();
			}
			count++;
		}
	}		
} 
    

//public ~Font()
//{
//    Gl.glDeleteLists(basenumber, 256);
//    TEXSERVER.FreeTexture(texture);
//}


        public void glPrint(float scale, string fmt)
        {
            string text;								// Holds Our String

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, loader.get(texture));

      
            //va_start(ap, fmt);									// Parses The String For Variables
            //   vsprintf(text, fmt, ap);						// And Converts Symbols To Actual Numbers
            //va_end(ap);											// Results Are Stored In Text
            text = fmt;



            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glPushMatrix();
            Gl.glScalef(scale * FontScale, scale * FontScale, 1.0f);
            Gl.glListBase(basenumber - 32);
            Gl.glCallLists(text.Length, Gl.GL_UNSIGNED_BYTE, text);
            Gl.glPopMatrix();
        }

      

public void glLine(float x1, float y1, float x2, float y2, float scale)
{

    Gl.glBindTexture(Gl.GL_TEXTURE_2D, loader.get(texture));
	
	Gl.glEnable(Gl.GL_TEXTURE_2D);
	if(Math.Abs((int)(x2 - x1)) > 100)
	{
		Gl.glBegin(Gl.GL_QUADS);
			Gl.glTexCoord2f(0.2148f, 0.958f);				
			Gl.glVertex2f(x1, y1 + scale);							
			Gl.glTexCoord2f(0.6054f, 0.958f);
			Gl.glVertex2f(x2, y1 + scale);							
			Gl.glTexCoord2f(0.6054f, 0.982f);
			Gl.glVertex2f(x2, y1 - scale);	
			Gl.glTexCoord2f(0.2148f,0.982f);
			Gl.glVertex2f(x1, y1 - scale);
		Gl.glEnd();
	}
	else {
	Gl.glBegin(Gl.GL_QUADS);
		Gl.glTexCoord2f(0.00976f, 0.958f);				
		Gl.glVertex2f(x1, y1 + scale);							
		Gl.glTexCoord2f(0.205f, 0.958f);
		Gl.glVertex2f(x2, y1 + scale);							
		Gl.glTexCoord2f(0.205f, 0.982f);
		Gl.glVertex2f(x2, y1 - scale);	
		Gl.glTexCoord2f(0.00976f,0.982f);
		Gl.glVertex2f(x1, y1 - scale);
	Gl.glEnd();
	}
	Gl.glDisable(Gl.GL_TEXTURE_2D);
}

    }
}
