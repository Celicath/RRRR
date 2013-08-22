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
			dur += elapsed;
			if (dur > 1000)
			{
				dur -= 1000;
				walkers.Add(new Walker(WalkerType.Male, Util.rand.Next(-5, 6), player.y + 50, (float)Util.rand.NextDouble() * 4 + 4));
				walkers.Add(new Walker(WalkerType.Female, Util.rand.Next(-5, 6), player.y + 50, (float)Util.rand.NextDouble() * 3 + 4));
			}

			foreach (Walker w in walkers)
				w.Update(elapsed);

			walkers.RemoveAll(new Predicate<Walker>(a => a.y > player.y + 80 || a.y < player.y - 40));

			walkers.Sort(new Comparison<Walker>((a, b) => { return b.y.CompareTo(a.y); }));
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
			gl.LookAt(0, player.y - 6, 10, 0, player.y + 6, 0, 0, 0, 1);

			for (int i = -6; i < 6; i+=2)
				for (int j = (int)(player.y / 2) * 2 - 20; j < (int)(player.y / 2) * 2 + 80; j += 2)
				{
					gl.Begin(BeginMode.Quads);
					{
						float c = (((i + j) / 2) % 2 == 0) ? 1.0f : 0.8f;
						gl.Color(c * 0.8f, c * 0.8f, c * 0.5f);
						gl.Vertex(i, j, 0);
						gl.Vertex(i, j + 2, 0);
						gl.Vertex(i + 2, j + 2, 0);
						gl.Vertex(i + 2, j, 0);
					}
					gl.End();
				}

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

			g.DrawString((int)player.speed + "m/s", font, Brushes.White, new PointF(glcScene.Width - size, size), right);
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
		}
	}
}
