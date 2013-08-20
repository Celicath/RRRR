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

namespace RRRR
{
	public partial class MainForm : Form
	{
		float meter = 0.0f;
		float speed = 10.0f;

		float size = 0.0f;

		public MainForm()
		{
			InitializeComponent();
		}

		public void UpdateWorld(float elapsed)
		{
			meter += elapsed / 1000 * speed;
		}

		private void glcScene_OpenGLDraw(object sender, SharpGL.RenderEventArgs args)
		{
			OpenGL gl = glcScene.OpenGL;

			gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
			gl.ClearColor(0.25f, 0.75f, 1.0f, 1.0f);


		}

		private void glcScene_GDIDraw(object sender, RenderEventArgs args)
		{
			Graphics g = args.Graphics;

			g.DrawString((int)meter + "m/ 1000m", new Font("Dotum", size), Brushes.White, new PointF(size, size));

			StringFormat f = new StringFormat();
			f.Alignment = StringAlignment.Far;

			g.DrawString((int)speed + "m/s", new Font("Dotum", size), Brushes.White, new PointF(glcScene.Width - size, size), f);
		}

		private void glcScene_SizeChanged(object sender, EventArgs e)
		{
			size = glcScene.Height / 30.0f;
		}
	}
}
