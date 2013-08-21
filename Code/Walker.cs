using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGL;
using SharpGL.Enumerations;
using SharpGL.SceneGraph.Assets;

namespace RRRR
{
	class Walker
	{
		Texture tex;

		public int x;
		public float y;
		public float speed;

		float time = 0.0f;

		/// <summary> 3D 상에서 이동 속도 </summary>
		public Vector3 vel = Vector3.Zero;

		public Walker(Texture tex, int x, float y, float speed)
		{
			this.tex = tex;
			this.x = x;
			this.y = y;
			this.speed = speed;
		}

		public void Update(float elpased)
		{
			y += elpased * speed/1000;
			time += elpased;
		}

		public void Draw3D(OpenGL gl)
		{
			tex.Bind(gl);
			gl.Enable(OpenGL.GL_TEXTURE_2D);

			gl.Begin(BeginMode.Quads);
			{
				gl.Color(1.0f, 1.0f, 1.0f, 1.0f);
				gl.TexCoord(((int)(time / speed) % 4) * 0.25f, 1);
				gl.Vertex(x - 2, y, 0);
				gl.TexCoord(((int)(time / speed) % 4) * 0.25f, 0);
				gl.Vertex(x - 2, y, 4);
				gl.TexCoord(((int)(time / speed) % 4 + 1) * 0.25f, 0);
				gl.Vertex(x + 2, y, 4);
				gl.TexCoord(((int)(time / speed) % 4 + 1) * 0.25f, 1);
				gl.Vertex(x + 2, y, 0);
			}
			gl.End();

			gl.Disable(OpenGL.GL_TEXTURE_2D);
		}
	}
}
