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
		float meter = 0.0f;
		float speed = 10.0f;

		float size = 0.0f;

		StringFormat right = new StringFormat();
		Font font;

		Texture test;
		Texture tex1;
		Texture tex2;

		float time = 0;

		float dur = 0;

		List<Walker> walkers = new List<Walker>();

		public MainForm()
		{
			InitializeComponent();
			right.Alignment = StringAlignment.Far;
		}

		public void UpdateWorld(float elapsed)
		{
			meter += elapsed / 1000 * speed;

			time += elapsed;

			dur += elapsed;
			if (dur > 1000)
			{
				dur -= 1000;
				walkers.Add(new Walker(tex1, Util.rand.Next(-10, 10), 5, (float)Util.rand.NextDouble() * 4 + 4));
				walkers.Add(new Walker(tex2, Util.rand.Next(-10, 10), 5, (float)Util.rand.NextDouble() * 4 + 4));
			}

			foreach (Walker w in walkers)
				w.Update(elapsed);
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
			gl.LookAt(0, -15, 10, 0, 0, 0, 0, 0, 1);

			for (int i = -3; i < 3; i++)
				for (int j = -10; j < 20; j++)
				{
					gl.Begin(BeginMode.Quads);
					{
						float c = ((i + j) % 2 == 0) ? 1.0f : 0.8f;
						gl.Color(c * 0.8f, c * 0.8f, c * 0.5f);
						gl.Vertex(i * 2, j * 2, 0);
						gl.Vertex(i * 2, (j + 1) * 2, 0);
						gl.Vertex((i + 1) * 2, (j + 1) * 2, 0);
						gl.Vertex((i + 1) * 2, j * 2, 0);
					}
					gl.End();
				}

			test.Bind(gl);
			gl.Enable(OpenGL.GL_BLEND);
			gl.Enable(OpenGL.GL_TEXTURE_2D);
			gl.Begin(BeginMode.Quads);
			{
				gl.Color(1.0f, 1.0f, 1.0f, 1.0f);
				gl.TexCoord(((int)(time / 50) % 10) * 0.1f, 1);
				gl.Vertex(-2, -5, 0);
				gl.TexCoord(((int)(time / 50) % 10) * 0.1f, 0);
				gl.Vertex(-2, -5, 4);
				gl.TexCoord(((int)(time / 50) % 10 + 1) * 0.1f, 0);
				gl.Vertex(2, -5, 4);
				gl.TexCoord(((int)(time / 50) % 10 + 1) * 0.1f, 1);
				gl.Vertex(2, -5, 0);
			}
			gl.End();
			gl.Disable(OpenGL.GL_TEXTURE_2D);


			foreach (Walker w in walkers)
				w.Draw3D(gl);

			gl.LoadIdentity();

			gl.Flush();
		}

		private void glcScene_GDIDraw(object sender, RenderEventArgs args)
		{
			Graphics g = args.Graphics;

			g.FillRectangle(Brushes.Black, new Rectangle(0, 0, Width, Height / 10));

			g.DrawString((int)meter + "m/ 1000m", font, Brushes.White, new PointF(size, size));

			g.DrawString((int)speed + "m/s", font, Brushes.White, new PointF(glcScene.Width - size, size), right);
		}

		private void glcScene_SizeChanged(object sender, EventArgs e)
		{
			size = glcScene.Height / 30.0f;
			font = new Font("Dotum", size);
		}

		private void glcScene_OpenGLInitialized(object sender, EventArgs e)
		{
			OpenGL gl = glcScene.OpenGL;
			test = new Texture();
			test.Create(gl, RRRR.Properties.Resources.Runner);

			tex1 = new Texture();
			tex1.Create(gl, RRRR.Properties.Resources.Walker_M);
			tex2 = new Texture();
			tex2.Create(gl, RRRR.Properties.Resources.Walker_F);
		}
	}
}
