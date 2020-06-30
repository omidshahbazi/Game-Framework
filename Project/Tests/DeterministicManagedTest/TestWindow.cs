using GameFramework.Deterministic;
using GameFramework.Deterministic.Physics2D;
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

		private bool doRaycast = false;
		private Raycaster.Info raycastInfo = null;
		private bool raycastHitted = false;
		private Raycaster.Result raycastResult = new Raycaster.Result();

		private Vector2 CursorPosition
		{
			get
			{
				PointF loc = editorCanvas1.ScreenToCanvas(editorCanvas1.PointToClient(MousePosition));

				return new Vector2(loc.X, loc.Y);
			}
		}

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
			groundBody.Position = new Vector2(400, 25);
			groundBody.StaticFriction = 0.5F;
			groundBody.DynamicFriction = 0.2F;
			groundBody.Restitution = 0.2F;
			groundBody.Orientation.Rotation = 60 * Math.DegreesToRadians;
			groundBody.Shape = Utilities.CreateSquareShape(new Vector2(700, 30), new Vector2(0, 0));

			Body obstacleBody = Utilities.AddBody(scene);
			obstacleBody.Position = new Vector2(400, 300);
			obstacleBody.StaticFriction = 0.5F;
			obstacleBody.DynamicFriction = 0.2F;
			obstacleBody.Restitution = 0.2F;
			obstacleBody.Shape = Utilities.CreateCircleShape(50);

			config = new Simulation.Config();
			config.StepTime = STEP_TIME;

			raycastInfo = new Raycaster.Info();

			editorCanvas1.LookAt(new PointF(400, 200));
		}

		private void Simulate()
		{
			contacts.Clear();
			Simulation.Simulate(scene, config, contacts);

			if (doRaycast)
			{
				Vector2 diff = CursorPosition - raycastInfo.Origin;

				raycastInfo.Scene = scene;
				raycastInfo.Direction = diff.Normalized;
				raycastInfo.Distance = diff.Magnitude;

				raycastHitted = Raycaster.Raycast(raycastInfo, ref raycastResult);
			}
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

			if (doRaycast)
			{
				Drawer.Draw(Device, raycastInfo);

				if (raycastHitted)
					Drawer.Draw(Device, raycastResult);
			}
		}

		private void editorCanvas1_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				Body body = Utilities.AddBody(scene);
				body.Mass = 50;
				body.Inertia = 10;
				body.Position = CursorPosition;
				body.Shape = Utilities.CreateCircleShape(25);
			}
			else if (e.Button == MouseButtons.Right)
			{
				Body body = Utilities.AddBody(scene);
				body.Mass = 20;
				body.StaticFriction = 0.4F;
				body.DynamicFriction = 0.2F;
				body.Restitution = 0.2F;
				body.Position = CursorPosition;
				body.Shape = Utilities.CreateSquareShape(new Vector2(70, 50), Vector2.Zero);
			}
		}

		private void editorCanvas1_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.S)
				doSimulation = !doSimulation;
			else if (e.KeyCode == Keys.Space)
				Simulate();
			else if (e.KeyCode == Keys.R)
			{
				doRaycast = !doRaycast;
				raycastInfo.Origin = CursorPosition;
				raycastHitted = false;
			}
		}
	}
}
