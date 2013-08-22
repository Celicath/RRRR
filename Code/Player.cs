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
		public Player(int x, float y, float speed)
			: base(WalkerType.Player, x, y, speed)
		{
			pieces = 10;
		}

		public override void Update(float elapsed)
		{
			float acc = Math.Min(10 - speed, 3);
			speed += elapsed * acc / 1000;
			y += elapsed * speed / 1000;
			time += elapsed;

			if (Keyboard.IsKeyDown(Key.Left))
			{
				subpos -= elapsed;
				if (x + subpos / subs < -5)
				{
					x = -5;
					subpos = 0;
				}
			}
			if (Keyboard.IsKeyDown(Key.Right))
			{
				subpos += elapsed;
				if (x + subpos / subs > 5)
				{
					x = 5;
					subpos = 0;
				}
			}
			if (subpos > subs)
			{
				subpos = 0;
				x++;
			}
			else if (subpos < -subs)
			{
				subpos = 0;
				x--;
			}
		}

		public override void Draw3D(OpenGL gl)
		{
			float drawCoord = (float)((int)(time * speed * pieces / 20000) % pieces);

			gl.Begin(BeginMode.Quads);
			{
				gl.TexCoord(drawCoord / pieces, 1);
				gl.Vertex(xpos * 0.5f - 0.8f, y, 0);
				gl.TexCoord(drawCoord / pieces, 0);
				gl.Vertex(xpos * 0.5f - 0.8f, y, 2);
				gl.TexCoord((drawCoord + 1) / pieces, 0);
				gl.Vertex(xpos * 0.5f + 0.8f, y, 2);
				gl.TexCoord((drawCoord + 1) / pieces, 1);
				gl.Vertex(xpos * 0.5f + 0.8f, y, 0);
			}
			gl.End();
		}
	}
}
