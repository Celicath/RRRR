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
		public WalkerType type;

		public int x;
		public float y;
		public float speed;

		protected int pieces = 4;

		protected float time = 0.0f;

		/// <summary> 3D 상에서 이동 속도 </summary>
		public Vector3 vel = Vector3.Zero;

		public bool die = false;

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

		public virtual void Update(float elpased)
		{
			y += elpased * speed/1000;
			time += elpased;
		}

		public virtual void Draw3D(OpenGL gl)
		{
			float drawCoord = (float)((int)(time * speed * pieces / 2000) % pieces);

			gl.Begin(BeginMode.Quads);
			{
				gl.Color(1.0f, 1.0f, 1.0f, 1.0f);
				gl.TexCoord(drawCoord / pieces, 1);
				gl.Vertex(x - 2, y, 0);
				gl.TexCoord(drawCoord / pieces, 0);
				gl.Vertex(x - 2, y, 4);
				gl.TexCoord((drawCoord + 1) / pieces, 0);
				gl.Vertex(x + 2, y, 4);
				gl.TexCoord((drawCoord + 1) / pieces, 1);
				gl.Vertex(x + 2, y, 0);
			}
			gl.End();
		}
	}
}
