using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
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
		Texture texM, texF, texMFly, texFFly;
		Texture[] texBuilding = new Texture[4];

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
			float ratio = 200000 / (float)Math.Sqrt(player.y + 100);
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
				if (walkers[i].xmoving != 0 || walkers[i].type == WalkerType.Player) continue;
				if (walkers[i].y < player.y + 3 && walkers[i].y > player.y) continue;
				for (int j = i + 1; j < walkers.Count; j++)
				{
					if (walkers[i].y < walkers[j].y - 2.0f)
						break;
					if (Math.Abs(walkers[i].xpos - walkers[j].xpos) < 1.2f && walkers[i].speed > walkers[j].speed)
					{
						if (walkers[i].y > walkers[j].y - 0.75f)
						{
							walkers[i].speed = walkers[j].speed - 1.0f;
							if (walkers[i].speed < walkers[j].speed / 2)
								walkers[i].speed = walkers[j].speed / 2;
							break;
						}
						else
						{
							if (walkers[i].xpos > walkers[j].xpos)
								walkers[i].xmoving = 1;
							else if (walkers[i].xpos < walkers[j].xpos)
								walkers[i].xmoving = -1;
							else walkers[i].xmoving = Util.rand.Next(0, 2) * 2 - 1;

							if (walkers[i].xpos >= 3)
								walkers[i].xmoving = -1;
							else if (walkers[i].xpos <= -3)
								walkers[i].xmoving = 1;

							break;
						}
					}
				}
			}

			foreach (Walker w in walkers)
			{
				if (w.type == WalkerType.Player) continue;
				if (w.flying != 0 || w.falling != 0) continue;

				if (player.speed > 16)
				{
					if (Math.Abs(w.xpos - player.xpos) <= 2.0f && w.y > player.y - 0.5f && w.y < player.y + 1.2f)
					{
						w.speed = Util.rand.Next(20, 48);
						if (w.speed >= 30) w.speed += 1;
						w.flying = (float)(Util.rand.NextDouble() * 2 - 1);
					}
				}
				else
				{
					if (Math.Abs(w.xpos - player.xpos) < 0.98f && w.y > player.y - 0.2f && w.y < player.y + 0.5f)
					{
						player.speed = 0;
						w.speed = 0;
						w.falling = 1;
					}
				}
			}
		}

		private void glcScene_OpenGLDraw(object sender, RenderEventArgs args)
		{
			OpenGL gl = glcScene.OpenGL;

			gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
			gl.ClearColor(0.8f, 0.8f, 0.8f, 1.0f);
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

			gl.Enable(OpenGL.GL_TEXTURE_2D);

			gl.Color(1.0f, 1.0f, 1.0f, 1.0f);

			int wi = 18, hi = 4;
			for (int j = (int)((player.y - 40) / wi) * wi; j <= (int)((player.y + 20) / wi) * wi; j += wi)
			{
				texBuilding[(j / wi + 100) % 4].Bind(gl);
				gl.Begin(BeginMode.Quads);
				{
					gl.TexCoord(0, 1);
					gl.Vertex(-3, j, 0);
					gl.TexCoord(1, 1);
					gl.Vertex(-3, j + wi, 0);
					gl.TexCoord(1, 0);
					gl.Vertex(-3, j + wi, hi);
					gl.TexCoord(0, 0);
					gl.Vertex(-3, j, hi);
				}
				gl.End();
				texBuilding[(j / wi + 102) % 4].Bind(gl);
				gl.Begin(BeginMode.Quads);
				{
					gl.TexCoord(1, 1);
					gl.Vertex(3, j, 0);
					gl.TexCoord(0, 1);
					gl.Vertex(3, j + wi, 0);
					gl.TexCoord(0, 0);
					gl.Vertex(3, j + wi, hi);
					gl.TexCoord(1, 0);
					gl.Vertex(3, j, hi);
				}
				gl.End();
			}

			gl.Enable(OpenGL.GL_BLEND);
			gl.Enable(OpenGL.GL_ALPHA_TEST);
			gl.AlphaFunc(AlphaTestFunction.Great, 0.1f);

			texF.Bind(gl);
			foreach (Walker w in walkers)
				if (w.type == WalkerType.Female && w.flying == 0)
					w.Draw3D(gl);
			texM.Bind(gl);
			foreach (Walker w in walkers)
				if (w.type == WalkerType.Male && w.flying == 0)
					w.Draw3D(gl);
			texFFly.Bind(gl);
			foreach (Walker w in walkers)
				if (w.type == WalkerType.Female && w.flying != 0)
					w.Draw3DFlying(gl);
			texMFly.Bind(gl);
			foreach (Walker w in walkers)
				if (w.type == WalkerType.Male && w.flying != 0)
					w.Draw3DFlying(gl);

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

			texMFly = new Texture();
			texMFly.Create(gl, RRRR.Properties.Resources.Walker_M_Flew);
			texFFly = new Texture();
			texFFly.Create(gl, RRRR.Properties.Resources.Walker_F_Flew);

			player = new Player(0, 0, 0);
			walkers.Add(player);
			player.y = 0;

			texBuilding[0] = new Texture();
			texBuilding[0].Create(gl, RRRR.Properties.Resources.apart_1);
			texBuilding[1] = new Texture();
			texBuilding[1].Create(gl, RRRR.Properties.Resources.apart_2);
			texBuilding[2] = new Texture();
			texBuilding[2].Create(gl, RRRR.Properties.Resources.apart_3);
			texBuilding[3] = new Texture();
			texBuilding[3].Create(gl, RRRR.Properties.Resources.apart_4);
		}
	}
}
