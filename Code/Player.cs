using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SharpGL;
using SharpGL.Enumerations;
using SharpGL.SceneGraph.Assets;

namespace RRRR
{
	class Player : Walker
	{
		public bool fever = false;

		public float sprint = 0;

		public Player(int x, float y, float speed)
			: base(WalkerType.Player, x, y, speed)
		{
			pieces = 10;
		}

		public override void Update(float elapsed)
		{
			float acc = 0;
			if (fever)
				acc = 60 - speed * 2;
			else
			{
				if (speed < 10)
					acc = Math.Min(5 - speed / 2, 3);
				else acc = 20 - speed * 2;
			}
			speed += elapsed * acc / 1000;

			if (Keyboard.IsKeyDown(Key.Left))
			{
				if (x + subpos / subs > -5)
					xmoving = -1;
			}
			else if (Keyboard.IsKeyDown(Key.Right))
			{
				if (x + subpos / subs < 5)
					xmoving = 1;
			}

			if (Keyboard.IsKeyDown(Key.Space) && (sprint > 25 || sprint > 10 && fever))
				fever = true;
			else fever = false;

			base.Update(elapsed);
		}

		public override void Draw3D(OpenGL gl)
		{
			float drawCoord = (int)(y * 2) % pieces;

			float size = 2;
			double theta = 0;
			if (fever)
			{
				size = 3f;
				if (xpos < -4)
					theta = (xpos + 4) * Math.PI / 4;
				else if (xpos > 4)
					theta = (xpos - 4) * Math.PI / 4;
			}

			gl.Begin(BeginMode.Quads);
			{
				if (fever)
					gl.Color(1.0f, 0.5f, 0.5f);
				gl.TexCoord(drawCoord / pieces, 1);
				gl.Vertex(xpos * 0.5f - (0.4f * size) * Math.Cos(theta), y - (0.4f * size) * Math.Sin(theta), 0);
				gl.TexCoord(drawCoord / pieces, 0);
				gl.Vertex(xpos * 0.5f - (0.4f * size) * Math.Cos(theta), y - (0.4f * size) * Math.Sin(theta), size);
				gl.TexCoord((drawCoord + 1) / pieces, 0);
				gl.Vertex(xpos * 0.5f + (0.4f * size) * Math.Cos(theta), y + (0.4f * size) * Math.Sin(theta), size);
				gl.TexCoord((drawCoord + 1) / pieces, 1);
				gl.Vertex(xpos * 0.5f + (0.4f * size) * Math.Cos(theta), y + (0.4f * size) * Math.Sin(theta), 0);
				gl.Color(1.0f, 1.0f, 1.0f);
			}
			gl.End();
		}
	}
}
