using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Axiom.Core;
using Axiom.Overlays;
using Axiom.Math;
using Axiom.Graphics;
using InputReader = SharpInputSystem.InputManager;

namespace GroundControlUI
{
    public partial class AxiomPad : UserControl, IDisposable
    {
        protected Root engine;
        protected Camera camera;
        protected Viewport viewport;
        protected SceneManager scene;
        protected RenderWindow window;
        protected Vector3 cameraVector = Vector3.Zero;
        protected float cameraScale;
        protected bool showDebugOverlay = true;
        protected float statDelay = 0.0f;
        protected float debugTextDelay = 0.0f;
        protected string debugText = "";
        protected float keypressDelay = 0.5f;
        protected Vector3 camVelocity = Vector3.Zero;
        protected Vector3 camAccel = Vector3.Zero;
        protected float camSpeed = 2.5f;
        protected int aniso = 1;
        protected TextureFiltering filtering = TextureFiltering.Bilinear;

        private Entity uavobj;
        private float curvature = 1;
        private float tiling = 15;
        int windowCount = 0;

        public delegate InputReader ConfigureInput();
        public ConfigureInput SetupInput;

        protected SharpInputSystem.Mouse mouse;
        protected SharpInputSystem.Keyboard keyboard;

        public  void CreateScene()
        {
            // set ambient light
            scene.AmbientLight = ColorEx.Gray;

            // create a skydome
           // scene.SetSkyDome(true, "Examples/CloudySky", 5, 8);

            // create a light
            Light light = scene.CreateLight("MainLight");
            light.Position = new Vector3(20, 80, 50);

            // add a floor plane
            Plane p = new Plane();
            p.Normal = Vector3.UnitY;
            p.D = 200;
          //  MeshManager.Instance.CreatePlane("FloorPlane", ResourceGroupManager.DefaultResourceGroupName, p, 2000, 2000, 1, 1, true, 1, 5, 5, Vector3.UnitZ);

            // add the floor entity
        //    Entity floor = scene.CreateEntity("Floor", "FloorPlane");
        //    floor.MaterialName = "Examples/RustySteel";
       //     scene.RootSceneNode.CreateChildSceneNode().AttachObject(floor);

            uavobj = scene.CreateEntity("Ogre", "ogrehead.mesh");
            scene.RootSceneNode.CreateChildSceneNode().AttachObject(uavobj);
        }

        public virtual void CreateCamera()
        {
            // create a camera and initialize its position
            camera = scene.CreateCamera("MainCamera");
            camera.Position = new Vector3(0, 0, 500);
            camera.LookAt(new Vector3(0, 0, -300));

            // set the near clipping plane to be very close
            camera.Near = 5;

            camera.AutoAspectRatio = true;
        }

        public virtual void ChooseSceneManager()
        {
            // Get the SceneManager, a generic one by default
            scene = engine.CreateSceneManager("DefaultSceneManager", "TechDemoSMInstance");
            scene.ClearScene();
        }

        public virtual void CreateViewports()
        {
         //   Debug.Assert(window != null, "Attempting to use a null RenderWindow.");

            // create a new viewport and set it's background color
            viewport = window.AddViewport(camera, 0, 0, 1.0f, 1.0f, 100);
            viewport.BackgroundColor = ColorEx.Black;
        }

        public RenderWindow CreateRenderWindow(Control target)
        {
            // We cannot set up a window without an active rendersystem.
          //  Debug.Assert(Root.Instance.RenderSystem != null, "Cannot create	a RenderWindow without an active RenderSystem.");


            // Create a	new render window via the current render system	
            // The parameters are as follows:
            //	string window name	-	Name of this render window. Must be unique.
            //	int width		-	The width of the render window (for clean image)of the control. Default 100??.
            //	int height		-	The height of the render window of the control. Default 100??.
            //	int colorDepth		-	The color depth of this window. Will be either 16, 24, 32
            //	bool isFullscreen	-	boolean indicating whether or not this window should be fullscreen
            //	int left		-	The horizontal origin of the render surface in the control. Default is 0.
            //	int top			-	The vertical origin of the render surface in the control. Default is 0.
            //	bool depthBuffer	-	Boolean indicating whether or not to use a depth buffer. Default true.
            //	bool vsync		-	Boolean indicating whether or not to synch rendering to the monitor's refresh rate.
            //	Control target		-	Target control to render onto. This must be a Form or PictureBox.
            Axiom.Collections.NamedParameterList paramList = new Axiom.Collections.NamedParameterList();
            paramList["externalWindowHandle"] = target.Handle;

            engine.Initialize(false);
            
            RenderWindow rWin = engine.CreateRenderWindow(
                    "__window" + windowCount.ToString(), target.Width, target.Height, false,paramList);


            // Here, windowCount maintains a count of render windows created. If the count is zero,
            // then we want to do some setup. Doing this for every window created will cause exceptions.
            if (windowCount == 0)
            {
          //      Root.Instance.SceneManager.ShadowTechnique = ShadowTechnique.StencilModulative;


                // set default mipmap level	
//TextureManager.Instance.DefaultNumMipMaps = 5;
                // retreive and initialize the input system	
             //   input = PlatformManager.Instance.CreateInputReader();


                // The details of input handling are up to you. In this case, I'm instructing the
                // window to not capture control of the mouse.
            //    input.Initialize(rWin, true, true, false, false);
            }
            windowCount++;
            return rWin;
        } 


        public virtual bool Setup()
        {

          
             


            // instantiate the Root singleton
            engine = new Root( "AxiomEngine.log" );
          //  engine = Root.Instance;
            ResourceGroupManager.Instance.AddResourceLocation("./media/", "Folder", "General", false, true);
            Axiom.Core.ResourceGroupManager.Instance.InitializeAllResourceGroups();

            engine.RenderSystem = engine.RenderSystems.First().Value;

         //   window = Root.Instance.Initialize(false);
            window = CreateRenderWindow(this.pictureEdit1);
            // add event handlers for frame events
            engine.FrameStarted += OnFrameStarted;
            //   engine.FrameRenderingQueued += OnFrameRenderingQueued;
            // engine.FrameEnded += OnFrameEnded;
            
           // TechDemoListener rwl = new TechDemoListener(window);
          //  WindowEventMonitor.Instance.RegisterListener(window, rwl);

            ChooseSceneManager();

            CreateCamera();

            CreateViewports();

            // set default mipmap level
            TextureManager.Instance.DefaultMipmapCount = 5;

            // Create any resource listeners (for loading screens)
           // this.CreateResourceListener();
            // Load resources

            ResourceGroupManager.Instance.InitializeAllResourceGroups();
	
            //ShowDebugOverlay(showDebugOverlay);

            //CreateGUI();


         //   input = SetupInput();

            // call the overridden CreateScene method

            CreateScene();
            return true;
        }


        public virtual void Dispose()
        {
            if (engine != null)
            {
                // remove event handlers
                engine.FrameStarted -= OnFrameStarted;
                //	engine.FrameEnded -= OnFrameEnded;
            }
            if (scene != null)
                scene.RemoveAllCameras();
            camera = null;
            if (Root.Instance != null)
                Root.Instance.RenderSystem.DetachRenderTarget(window);
            if (window != null)
                window.Dispose();
            if (engine != null)
                engine.Dispose();
        }



        protected void OnFrameStarted(Object source, FrameEventArgs evt)
        {
            if (evt.StopRendering)
                return;



       //     scene.SetSkyDome(true, "Examples/CloudySky", curvature, tiling);
        }



        public AxiomPad()
        {
            InitializeComponent();
         
        }

        protected InputReader _setupInput()
        {
            InputReader ir = null;
			SharpInputSystem.ParameterList pl = new SharpInputSystem.ParameterList();
			pl.Add( new SharpInputSystem.Parameter( "WINDOW", this.Handle ) );

			//Default mode is foreground exclusive..but, we want to show mouse - so nonexclusive
			pl.Add( new SharpInputSystem.Parameter( "w32_mouse", "CLF_BACKGROUND" ) );
			pl.Add( new SharpInputSystem.Parameter( "w32_mouse", "CLF_NONEXCLUSIVE" ) );

			//This never returns null.. it will raise an exception on errors
			ir = SharpInputSystem.InputManager.CreateInputSystem( pl );
			mouse = ir.CreateInputObject<SharpInputSystem.Mouse>( true, "" );
			keyboard = ir.CreateInputObject<SharpInputSystem.Keyboard>( true, "" );
            return ir;
        }

        private void button1_Click()
        {
            SetupInput = new ConfigureInput(_setupInput);

            if (Setup())
            {
                // start the engines rendering loop
                engine.StartRendering();
            }

        }

        private void _3DPad_MouseDown(object sender, MouseEventArgs e)
        {
            button1_Click();
        }

        private void _3DPad_Load(object sender, EventArgs e)
        {
            button1_Click();
        }

    }
}
