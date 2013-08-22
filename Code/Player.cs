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
		float subpos;
		const float subs = 200;

		public Player(int x, float y, float speed)
			: base(WalkerType.Player, x, y, speed)
		{
			pieces = 10;
		}

		public override void Update(float elpased)
		{
			base.Update(elpased);

			if (Keyboard.IsKeyDown(Key.Left))
			{
				subpos -= elpased;
				if (x + subpos / subs < -6)
				{
					x = -6;
					subpos = 0;
				}
			}
			if (Keyboard.IsKeyDown(Key.Right))
			{
				subpos += elpased;
				if (x + subpos / subs > 6)
				{
					x = 6;
					subpos = 0;
				}
			}
			if (subpos > subs)
			{
				subpos -= subs;
				x++;
			}
			else if (subpos < -subs)
			{
				subpos += subs;
				x--;
			}
		}

		public override void Draw3D(SharpGL.OpenGL gl)
		{
			float drawCoord = (float)((int)(time * speed * pieces / 2000) % pieces);

			gl.Enable(OpenGL.GL_TEXTURE_2D);

			gl.Begin(BeginMode.Quads);
			{
				gl.Color(1.0f, 1.0f, 1.0f, 1.0f);
				gl.TexCoord(drawCoord / pieces, 1);
				gl.Vertex(x + subpos / subs - 1.6, y, 0);
				gl.TexCoord(drawCoord / pieces, 0);
				gl.Vertex(x + subpos / subs - 1.6, y, 4);
				gl.TexCoord((drawCoord + 1) / pieces, 0);
				gl.Vertex(x + subpos / subs + 1.6, y, 4);
				gl.TexCoord((drawCoord + 1) / pieces, 1);
				gl.Vertex(x + subpos / subs + 1.6, y, 0);
			}
			gl.End();

			gl.Disable(OpenGL.GL_TEXTURE_2D);
		}
	}
}
