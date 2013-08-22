using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpGL;
using SharpGL.Enumerations;
using SharpGL.SceneGraph.Assets;

namespace RRRR
{
	public partial class MainForm : Form
	{
		float size = 0.0f;

		StringFormat right = new StringFormat();
		Font font;

		Texture texrun;
		Texture texM;
		Texture texF;

		float dur = 0;

		List<Walker> walkers = new List<Walker>();
		Player player;

		public MainForm()
		{
			InitializeComponent();
			right.Alignment = StringAlignment.Far;
		}

		public void UpdateWorld(float elapsed)
		{
			if(player.speed > 3.0f)
				dur += elapsed * player.speed;
			float ratio = 10000 / (player.y / 500 + 1);
			if (dur > ratio)
			{
				dur -= ratio;
				ratio = 10 + (float)Math.Sqrt(player.y);
				if (ratio > 75) ratio = 75;
				walkers.Add(new Walker(Util.rand.Next(100) > ratio ? WalkerType.Male : WalkerType.Female, Util.rand.Next(-5, 6), player.y + 24, (float)Util.rand.NextDouble() * 6 + 1));
			}

			foreach (Walker w in walkers)
				w.Update(elapsed);

			walkers.RemoveAll(new Predicate<Walker>(a => a.y > player.y + 50 || a.y < player.y - 20));

			walkers.Sort(new Comparison<Walker>((a, b) => a.y.CompareTo(b.y)));

			for (int i = 0; i < walkers.Count - 1; i++)
			{
				if (walkers[i].subpos != 0 || walkers[i].type == WalkerType.Player) continue;
				if (walkers[i].y < player.y + 3) continue;
				for (int j = i + 1; j < walkers.Count; j++)
				{
					if (Math.Abs(walkers[i].xpos - walkers[j].xpos) < 1.2f && walkers[i].speed > walkers[j].speed)
					{
						if (walkers[i].y > walkers[j].y - 0.8f)
						{
							walkers[i].speed = walkers[j].speed - 1.0f;
							if (walkers[i].speed < walkers[j].speed / 2)
								walkers[i].speed = walkers[j].speed / 2;
							break;
						}
						else if (walkers[i].y > walkers[j].y - 2.0f)
						{
							if (walkers[i].xpos > walkers[j].xpos)
								walkers[i].subpos = 0.01f;
							else if (walkers[i].xpos < walkers[j].xpos)
								walkers[i].subpos = -0.01f;
							else walkers[i].subpos = (float)(Util.rand.NextDouble() * 0.02 - 0.01);

							if (walkers[j].xpos + 1 > 3)
								walkers[i].subpos = -0.01f;
							else if (walkers[j].xpos - 1 < -3)
								walkers[i].subpos = +0.01f;

							break;
						}
					}
				}
			}

			foreach (Walker w in walkers)
			{
				if (w.type == WalkerType.Player) continue;
				if (w.xpos > player.xpos - 0.8f && w.xpos < player.xpos + 0.8f && w.y > player.y - 0.1f && w.y < player.y + 0.4f && w.falling == 0)
				{
					player.speed = 0;
					w.speed = 0;
					w.falling = 1;
				}
			}
		}

		private void glcScene_OpenGLDraw(object sender, SharpGL.RenderEventArgs args)
		{
			OpenGL gl = glcScene.OpenGL;

			gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
			gl.ClearColor(0.25f, 0.75f, 1.0f, 1.0f);
			gl.BlendFunc(BlendingSourceFactor.SourceAlpha, BlendingDestinationFactor.OneMinusSourceAlpha);

			gl.LoadIdentity();

			gl.MatrixMode(MatrixMode.Projection);
			gl.Perspective(60.0, (double)this.Width / this.Height, 0.01, 1000.0);
			gl.LookAt(0, player.y - 4, 5, 0, player.y + 3, 0, 0, 0, 1);

			for (int i = -3; i < 3; i++)
				for (int j = (int)player.y - 10; j < (int)player.y + 50; j++)
				{
					gl.Begin(BeginMode.Quads);
					{
						float c = ((i + j) % 2 == 0) ? 1.0f : 0.8f;
						gl.Color(c * 0.8f, c * 0.8f, c * 0.5f);
						gl.Vertex(i, j, 0);
						gl.Vertex(i, j + 1, 0);
						gl.Vertex(i + 1, j + 1, 0);
						gl.Vertex(i + 1, j, 0);
					}
					gl.End();
				}

			gl.Color(1.0f, 1.0f, 1.0f, 1.0f);

			gl.Enable(OpenGL.GL_BLEND);
			gl.Enable(OpenGL.GL_ALPHA_TEST);
			gl.AlphaFunc(AlphaTestFunction.Great, 0.1f);

			gl.Enable(OpenGL.GL_TEXTURE_2D);

			texF.Bind(gl);
			foreach (Walker w in walkers)
				if(w.type == WalkerType.Female)
					w.Draw3D(gl);
			texM.Bind(gl);
			foreach (Walker w in walkers)
				if (w.type == WalkerType.Male)
					w.Draw3D(gl);
			texrun.Bind(gl);
			player.Draw3D(gl);

			gl.Disable(OpenGL.GL_TEXTURE_2D);

			gl.LoadIdentity();

			gl.Disable(OpenGL.GL_ALPHA_TEST);
			gl.Disable(OpenGL.GL_BLEND);
			gl.Flush();
		}

		private void glcScene_GDIDraw(object sender, RenderEventArgs args)
		{
			Graphics g = args.Graphics;

			g.FillRectangle(Brushes.Black, new Rectangle(0, 0, Width, Height / 10));

			g.DrawString((int)player.y + "m", font, Brushes.White, new PointF(size, size));

			g.DrawString(walkers.Count.ToString(), font, Brushes.White, new PointF(size + glcScene.Width * 0.48f, size), right);

			g.DrawString(player.speed.ToString("0.0") + "m/s", font, Brushes.White, new PointF(glcScene.Width - size, size), right);
		}

		private void glcScene_SizeChanged(object sender, EventArgs e)
		{
			size = glcScene.Height / 30.0f;
			font = new Font("Dotum", size);
		}

		private void glcScene_OpenGLInitialized(object sender, EventArgs e)
		{
			OpenGL gl = glcScene.OpenGL;
			texrun = new Texture();
			texrun.Create(gl, RRRR.Properties.Resources.Runner);

			texM = new Texture();
			texM.Create(gl, RRRR.Properties.Resources.Walker_M);
			texF = new Texture();
			texF.Create(gl, RRRR.Properties.Resources.Walker_F);

			player = new Player(0, 0, 10);
			walkers.Add(player);
			player.y = 0;
		}
	}
}
