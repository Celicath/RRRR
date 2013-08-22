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
	public enum WalkerType { Player, Male, Female };
	class Walker
	{
		public float subpos;
		public const float subs = 125;

		public WalkerType type;

		protected int x;
		public float y;
		public float speed;

		protected int pieces = 4;

		protected float time = 0.0f;

		public bool die = false;

		public float falling = 0;

		public float xpos
		{
			get
			{
				return x + subpos / subs;
			}
		}

		/// <summary>
		/// 걷는 사람을 만든다.
		/// </summary>
		/// <param name="tex">텍스쳐</param>
		/// <param name="x">x 좌표</param>
		/// <param name="y">y 좌표</param>
		/// <param name="speed">속도</param>
		/// <param name="pieces">텍스쳐가 몇 개로 되어있는가</param>
		public Walker(WalkerType type, int x, float y, float speed)
		{
			this.type = type;
			this.x = x;
			this.y = y;
			this.speed = speed;
		}

		public virtual void Update(float elapsed)
		{
			y += elapsed * speed / 1000;
			time += elapsed;

			if (falling > 0)
			{
				falling = Math.Min(falling + elapsed / 9, 120);
			}
			else if (subpos > 0)
			{
				subpos += elapsed;
				if (subpos > subs)
				{
					x++;
					subpos = 0;
				}
			}
			else if (subpos < 0)
			{
				subpos -= elapsed;
				if (subpos < -subs)
				{
					x--;
					subpos = 0;
				}
			}
		}

		public virtual void Draw3D(OpenGL gl)
		{
			float drawCoord = (float)((int)(time * speed * pieces / 2000) % pieces);

			gl.Begin(BeginMode.Quads);
			{
				gl.TexCoord(drawCoord / pieces, 1);
				gl.Vertex(xpos * 0.5f - 1, y, 0);
				gl.TexCoord(drawCoord / pieces, 0);
				gl.Vertex(xpos * 0.5f - 1, y + 2 * Math.Sin(falling * Math.PI / 180), 2 * Math.Cos(falling * Math.PI / 180));
				gl.TexCoord((drawCoord + 1) / pieces, 0);
				gl.Vertex(xpos * 0.5f + 1, y + 2 * Math.Sin(falling * Math.PI / 180), 2 * Math.Cos(falling * Math.PI / 180));
				gl.TexCoord((drawCoord + 1) / pieces, 1);
				gl.Vertex(xpos * 0.5f + 1, y, 0);
			}
			gl.End();
		}
	}
}
