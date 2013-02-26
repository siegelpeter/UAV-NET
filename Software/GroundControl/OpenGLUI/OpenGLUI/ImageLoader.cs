/*
    I grant you all the rights to do whatever you want to do with this code.
    Free for all. Credit is not necessary. ;)
*/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

using Tao.OpenGl;
using System.IO;
using Tao.Platform.Windows;
using Tao.Platform;
using Tao.FreeGlut;
class ImageContainer
{
    public int width = 0, height = 0, id = 0;
    public string title = null, filename = null;
}

class imageLoader
{
    private static ImageContainer[] images;
    private static int count = 0;
    public  imageLoader()
    {
    
        print("ImageLoader Initialized");
        images = new ImageContainer[10000]; //maximum allowed images
    }

    private void print(string txt) { Console.WriteLine(txt); }

    public void Free()
    {
        //Free place for your code :)
    }



    public void LoadFromFile(string filename, string title)
    {
        int tempID;
    
     
       if (!File.Exists(filename)) throw new Exception("Image not found");
      
        Bitmap bmp = new Bitmap(filename);
        BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

    
        //Generate ID
        Gl.glGenTextures(1, out tempID);
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, tempID);

        Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, bmp_data.Width, bmp_data.Height, 0, Gl.GL_BGRA, Gl.GL_UNSIGNED_BYTE, bmp_data.Scan0);
        Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
        Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST);
        Add(filename, title,bmp_data.Width,bmp_data.Height);
        bmp.UnlockBits(bmp_data);
      
        //Upload
        
    }

    //Get ID
    public int get(string title)
    {

        for (int i = 0; i < count; i++)
        {
            if (images[i].filename == title || images[i].title == title)
            {
                return images[i].id;
            }
        }
        return 0;
    }

    //Find Next Power of two dimensions
    public int nextPowerOfTwo(int n)
    {
        int i = 1;
        while (i < n)
        {
            i *= 2;
        }
        return i;
    }

    //Check if its power of two
    public bool powerOfTwo(int n)
    {
        return nextPowerOfTwo(n) == n;
    }

   

    private void Add(string filename, string title,int Width, int Height)
    {
        images[count] = new ImageContainer();
        images[count].height = Height;
        images[count].width = Width;
        images[count].filename = filename;
        images[count].title = title;
        images[count].id = count + 1;
        images[count + 1] = new ImageContainer();
        count++;
    }
}
