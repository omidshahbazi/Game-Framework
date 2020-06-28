using GameFramework.Common.Extensions;
using GameFramework.Common.Timing;
using GameFramework.Deterministic;
using GameFramework.Deterministic.Physics;
using GameFramework.GDIRenderer;
using System.Drawing;
using System.Windows.Forms;

namespace DeterministicTest
{
	public partial class TestWindow : Form
	{
		private const float STEP_TIME = 0.016F;

		private Timer timer = null;
		private Scene scene = null;
		private ContactList contacts = null;
		private Simulation.Config config = null;

		private bool doSimulation = true;

		public TestWindow()
		{
			InitializeComponent();

			timer = new Timer();
			timer.Interval = (int)(STEP_TIME * 1000);
			timer.Tick += Timer_Tick;
			timer.Start();

			scene = new Scene();
			contacts = new ContactList();

			Body groundBody = Utilities.AddBody(scene);
			groundBody.Position = new Vector3(400, 25, 0);
			groundBody.StaticFriction = 0.5F;
			groundBody.DynamicFriction = 0.2F;
			groundBody.Restitution = 0.2F;
			groundBody.Shape = Utilities.CreateSquareShape(new Vector2(700, 20), new Vector2(0, 0));
			groundBody.Shape = Utilities.CreatePolygonShape(new Vector3[] { new Vector3(-350, -10, 0), new Vector3(-350, 10, 0), new Vector3(350, 500, 0), new Vector3(350, -10, 0) });

			Body obstacleBody = Utilities.AddBody(scene);
			obstacleBody.Position = new Vector3(400, 300, 0);
			obstacleBody.StaticFriction = 0.5F;
			obstacleBody.DynamicFriction = 0.2F;
			obstacleBody.Restitution = 0.2F;
			obstacleBody.Shape = Utilities.CreateSphereShape(50);

			config = new Simulation.Config();
			config.StepTime = STEP_TIME;
			config.Gravity = new Vector3(0, -980, 0);

			editorCanvas1.LookAt(new PointF(400, 200));
		}

		private void Simulate()
		{
			contacts.Clear();
			Simulation.Simulate(scene, config, contacts);
		}

		private void Timer_Tick(object sender, System.EventArgs e)
		{
			if (doSimulation)
				Simulate();

			editorCanvas1.Refresh();
		}

		private void editorCanvas1_DrawCanvas(IDevice Device)
		{
			Drawer.Draw(Device, scene);

			Drawer.Draw(Device, contacts);
		}

		private void editorCanvas1_MouseUp(object sender, MouseEventArgs e)
		{
			PointF loc = editorCanvas1.ScreenToCanvas(e.Location);

			if (e.Button == MouseButtons.Left)
			{
				Body body = Utilities.AddBody(scene);
				body.Mass = 80;
				body.Inertia = 50;
				body.AngularVelocity = new Vector3(0, 0, 23);
				body.Position = new Vector3(loc.X, loc.Y, 0);
				body.Shape = Utilities.CreateSphereShape(20);
			}
			else if (e.Button == MouseButtons.Right)
			{
				Body body = Utilities.AddBody(scene);
				body.Mass = 20;
				body.StaticFriction = 0.4F;
				body.DynamicFriction = 0.2F;
				body.Restitution = 0.2F;
				body.Position = new Vector3(loc.X, loc.Y, 0);
				body.Shape = Utilities.CreateSquareShape(new Vector2(70, 50), Vector2.Zero);
			}
		}

		private void editorCanvas1_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == 's')
				doSimulation = !doSimulation;
			else if (e.KeyChar == ' ')
				Simulate();
		}
	}
}
