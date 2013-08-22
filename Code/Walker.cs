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

		public float falling = 0;
		public float flying = 0;

		public bool check = false;

		/// <summary>
		/// 어떤 방향으로 움직이고 있는가
		/// </summary>
		public int xmoving = 0;

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
				falling = Math.Min(falling + elapsed / 9, 120);
			else if (flying > 0)
				flying += elapsed / 70;
			else if (flying < 0)
				flying -= elapsed / 70;
			else if (xmoving > 0)
			{
				subpos += elapsed;
				if (subpos < elapsed && subpos >= 0)
				{
					subpos = 0;
					xmoving = 0;
				}
				else if (subpos > subs)
				{
					x++;
					subpos = 0;
					xmoving = 0;
				}
			}
			else if (xmoving < 0)
			{
				subpos -= elapsed;
				if (subpos < -subs)
					if (subpos > -elapsed && subpos <= 0)
					{
						subpos = 0;
						xmoving = 0;
					}
					else if (subpos < -subs)
					{
						x--;
						subpos = 0;
						xmoving = 0;
					}
			}
		}

		public virtual void Draw3D(OpenGL gl)
		{
			float drawCoord = (int)(y * 2) % pieces;

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

		public void Draw3DFlying(OpenGL gl)
		{
			double theta = (-flying * 50 + 45) * Math.PI / 180;
			float a = (float)(Math.Sqrt(2) * Math.Cos(theta));
			float b = (float)(Math.Sqrt(2) * Math.Sin(theta));

			gl.Begin(BeginMode.Quads);
			{
				gl.TexCoord(0, 1);
				gl.Vertex(xpos * 0.5f - a + flying, y, 1 - b + Math.Sqrt(Math.Abs(flying / 2)));
				gl.TexCoord(0, 0);
				gl.Vertex(xpos * 0.5f - b + flying, y, 1 + a + Math.Sqrt(Math.Abs(flying / 2)));
				gl.TexCoord(1, 0);
				gl.Vertex(xpos * 0.5f + a + flying, y, 1 + b + Math.Sqrt(Math.Abs(flying / 2)));
				gl.TexCoord(1, 1);
				gl.Vertex(xpos * 0.5f + b + flying, y, 1 - a + Math.Sqrt(Math.Abs(flying / 2)));
			}
			gl.End();
		}                        
	}
}
