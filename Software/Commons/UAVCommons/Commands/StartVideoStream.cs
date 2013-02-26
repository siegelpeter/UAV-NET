using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace UAVCommons.Commands
{
    /// <summary>
    /// Starts the remote encoding Process and the Local player and puts it in foreground
    /// </summary>
    [Serializable]
    public class StartVideoStream : BaseCommand
    {
        public string targetip = "";
        public Int32 targetPort = 4444;
        public int fps = 15;
        public int width = 640;
        public int height = 480;
        public int pid = 0;
        [NonSerialized]
        private UAVBase local;
	
		public StartVideoStream(){


		}
           /// <summary>
           /// Creates a new Start Video Command
           /// </summary>
           /// <param name="ip"></param>
           /// <param name="targetport"></param>
           /// <param name="fps"></param>
           /// <param name="width"></param>
           /// <param name="height"></param>
       public StartVideoStream(string ip,int targetport,int fps,int width,int height)
       {
           this.targetPort = targetport;
           targetip = ip;
           this.fps = fps;
           this.height = height;
           this.width = width;
       }

        public override void RemoteExecute(UAVBase core)
        {
            base.RemoteExecute(core);
           ///Start Encoder
            Console.WriteLine("Execute ffmpeg ");
           /// ffmpeg -an -f video4linux2 -s 640x480 -r 15 -i /dev/video0 -vcodec libx264 -preset ultrafast -tune zerolatency -x264opts "vbv-maxrate=2000:vbv-bufsize=200:slice-max-size=1500" -f h264 udp://localhost:1234
            Process p = Process.Start("/usr/bin/ffmpeg"," -an -f video4linux2 -s "+width+"x"+height+" -r "+fps+" -i /dev/video0 -vcodec libx264 -preset ultrafast -tune zerolatency -x264opts \"vbv-maxrate=2000:vbv-bufsize=200:slice-max-size=1500\" -f h264 udp://"+targetip+":"+targetPort);
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.Exited += new EventHandler(p_Exited);
            p.Start();
          
          
   			pid = p.Id;
            Console.WriteLine("Video Stream Started");

        }

        void p_Exited(object sender, EventArgs e)
        {
            Console.WriteLine("Video Transmission Stopped");
        }

        public override void ProcessResults(UAVBase core)
        {
            base.ProcessResults(core);
            local = core;
            Process p = Process.Start("\\FFMpeg\\ffplay"," -i udp://"+targetip+"?localport="+targetPort);
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            p.Exited += new EventHandler(Playerexited);
            p.Start();
        }

        void Playerexited(object sender, EventArgs e)
        {
            local.SendCommandAsync(new UAVCommons.Commands.KillProcess(pid));
        }
    }
}
