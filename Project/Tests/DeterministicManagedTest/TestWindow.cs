using GameFramework.Common.Extensions;
using GameFramework.Deterministic;
using GameFramework.Deterministic.Physics;
using GameFramework.GDIRenderer;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DeterministicTest
{
	public partial class TestWindow : Form
	{
		private const float STEP_TIME = 0.016F;

		private Scene scene = null;
		private ContactList contacts = null;
		private Simulation.Config config = null;

		public TestWindow()
		{
			InitializeComponent();

			Timer timer = new Timer();
			timer.Interval = (int)(STEP_TIME * 1000);
			timer.Tick += Timer_Tick;
			timer.Start();

			scene = new Scene();
			contacts = new ContactList();

			Body groundBody = new Body();
			ArrayUtilities.Add(ref scene.Bodies, groundBody);
			groundBody.Mass = 0;
			groundBody.Position = new Vector3(400, 25, 0);
			groundBody.Orientation = Matrix3.Identity;
			groundBody.Shape = new PolygonShape()
			{
				Vertices = new Vector3[] { new Vector3(100, 0, 0), new Vector3(100, 20, 0), new Vector3(700, -10, 0), new Vector3(700, -30, 0) },
				Normals = new Vector3[] { -Vector3.Up, Vector3.Up, Vector3.Up, -Vector3.Up }
			};

			config = new Simulation.Config();
			config.StepTime = STEP_TIME;

			editorCanvas1.LookAt(new PointF(800, 200));
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			contacts.Clear();
			Simulation.Simulate(scene, config, contacts);

			editorCanvas1.Refresh();
		}

		private void editorCanvas1_DrawCanvas(IDevice Device)
		{
			Drawer.Draw(Device, scene);

			Drawer.Draw(Device, contacts);
		}

		private void editorCanvas1_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
				return;

			PointF loc = editorCanvas1.ScreenToCanvas(e.Location);

			Body body = new Body();
			ArrayUtilities.Add(ref scene.Bodies, body);
			body.Mass = 80;
			body.Position = new Vector3(loc.X, loc.Y, 0);
			body.Orientation = Matrix3.Identity;
			body.Shape = new SphereShape() { Radius = 20 };
		}
	}
}
