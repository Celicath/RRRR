using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using SharpGL;
using SharpGL.Enumerations;
using SharpGL.SceneGraph.Assets;

namespace RRRR
{
	public partial class MainForm : Form
	{
		float size = 0.0f;

		StringFormat right = new StringFormat();
		StringFormat middle = new StringFormat();
		Font font;
		Font smallfont;

		Texture texrun;
		Texture texM, texF, texMFly, texFFly;
		Texture[] texBuilding = new Texture[4];
		Texture texFloor1, texFloor2;
		Texture texStart;
		Texture texCheck;

		float dur = 0;

		List<Walker> walkers = new List<Walker>();
		Player player;

		float chul = 0;
		int police = 0;
		float policespeed = 8.0f;

		Bitmap bpolice;
		Bitmap bcheckpoint, bmark, bgreen, bred;

		int state = 0;

		float targettime = 90000;
		float curtime = 0;
		bool timer = false;

		Bitmap logo = RRRR.Properties.Resources.logo;

		bool paused = false;

		public MainForm()
		{
			InitializeComponent();
			right.Alignment = StringAlignment.Far;
			middle.Alignment = StringAlignment.Center;

			bpolice = RRRR.Properties.Resources.Police;
			bcheckpoint = RRRR.Properties.Resources.Traffic;
			bmark = RRRR.Properties.Resources.Marker;
			bgreen = RRRR.Properties.Resources.Walk;
			bred = RRRR.Properties.Resources.Dont_Walk;
		}

		public void UpdateWorld(float elapsed)
		{
			if (paused) return;
//			if (elapsed > 0.1f)
//				elapsed = 0.01f;
			switch (state)
			{
				case 0:
					#region 0 : 타이틀
					if (Keyboard.IsKeyDown(Key.Enter))
						state = 1;
					#endregion
					break;
				case 1:
					#region 1 : 게임
					player.sprint += elapsed * player.speed / 20000;
					if (player.fever || player.speed > 13)
					{
						player.sprint -= elapsed / 30;
						if (player.sprint < 0) player.sprint = 0;
					}

					if (player.speed > 3.0f)
						dur += elapsed * player.speed;
					float ratio = 200000 / (float)Math.Sqrt(player.y + 1000);
					if (dur > ratio)
					{
						dur -= ratio;
						ratio = 10 + (float)Math.Sqrt(player.y);
						if (ratio > 75) ratio = 75;
						walkers.Add(new Walker(Util.rand.Next(100) > ratio ? WalkerType.Male : WalkerType.Female, Util.rand.Next(-5, 6), player.y + 24, (float)Util.rand.NextDouble() * 6 + 1));
					}

					foreach (Walker w in walkers)
						w.Update(elapsed);

					walkers.RemoveAll(new Predicate<Walker>(a => a.y > player.y + 50 || a.y < player.y - 20));

					walkers.Sort(new Comparison<Walker>((a, b) => a.y.CompareTo(b.y)));

					for (int i = 0; i < walkers.Count - 1; i++)
					{
						if (walkers[i].xmoving != 0 || walkers[i].type == WalkerType.Player) continue;
						if (walkers[i].y < player.y + 3 && walkers[i].y > player.y - 0.3f) continue;
						for (int j = i + 1; j < walkers.Count; j++)
						{
							if (walkers[i].y < walkers[j].y - 2.0f)
								break;
							if (Math.Abs(walkers[i].xpos - walkers[j].xpos) < 1.2f && walkers[i].speed > walkers[j].speed)
							{
								if (walkers[i].y > walkers[j].y - 0.75f)
								{
									walkers[i].speed = walkers[j].speed - 1.0f;
									if (walkers[i].speed < walkers[j].speed / 2)
										walkers[i].speed = walkers[j].speed / 2;
									break;
								}
								else
								{
									if (walkers[i].xpos > walkers[j].xpos)
										walkers[i].xmoving = 1;
									else if (walkers[i].xpos < walkers[j].xpos)
										walkers[i].xmoving = -1;
									else walkers[i].xmoving = Util.rand.Next(0, 2) * 2 - 1;

									if (walkers[i].xpos >= 3)
										walkers[i].xmoving = -1;
									else if (walkers[i].xpos <= -3)
										walkers[i].xmoving = 1;

									break;
								}
							}
						}
					}

					foreach (Walker w in walkers)
					{
						if (w.type == WalkerType.Player) continue;
						if (w.flying != 0 || w.falling != 0) continue;

						if (player.speed > 16)
						{
							if (Math.Abs(w.xpos - player.xpos) <= 2.0f && w.y > player.y - 0.5f && w.y < player.y + 1.2f)
							{
								w.speed = Util.rand.Next(20, 48);
								if (w.speed >= 30) w.speed += 1;
								w.flying = (float)(Util.rand.NextDouble() * 2 - 1);
							}
						}
						else if (w.y > player.y - 0.2f && w.y < player.y + 0.5f)
						{
							float dist = Math.Abs(w.xpos - player.xpos);
							if (dist < 0.98f)
							{
								player.speed = 0;
								w.speed = 0;
								w.falling = 1;

								if (w.type == WalkerType.Female)
									chul += 10;
							}
							else if (dist <= 1.1f && !w.check)
							{
								w.check = true;
								player.sprint += 4f;
								if (w.type == WalkerType.Female)
									chul += 5;
							}
						}
					}
					if (player.sprint > 100) player.sprint = 100;
					if (chul >= 100)
					{
						chul = 100;
						police = 10000;
						policespeed = (float)Math.Sqrt(player.y) / 5 + 2;
						if (policespeed > 12.0f) policespeed = 12.0f;
					}
					if (police > 0)
					{
						chul -= elapsed / 160;
						police += (int)((policespeed - player.speed) * elapsed);
						if (police < 1)
							police = 1;
						else if (police > 50000)
							gameover();

						if (policespeed < 10.0f)
							policespeed += elapsed / 1500;
						else if (policespeed < 15.0f)
							policespeed += elapsed / 3000;
						else if (policespeed < 20.0f)
							policespeed += elapsed / 6000;
						else policespeed = 20.0f;

						if (chul <= 0)
						{
							police = 0;
							chul = 0;
						}
					}

					if (targettime < 20000 || player.y % 500 > 400)
						timer = true;

					if (timer)
					{
						curtime += elapsed;
						if ((player.y % 500) < 100)
						{
							targettime = 90000 - 10000 * (int)(player.y / 500);
							if (targettime < 40000) targettime = 40000;
							curtime = 0;
							timer = false;
						}
					}
					else targettime -= elapsed;

					if (targettime < curtime)
						state = 3;

					#endregion
					break;
				case 2: case 3:
					#region 2 : 철컹철컹 게임 오버 / 3 : 시간초과 게임 오버
					#endregion
					break;
			}
		}

		private void gameover()
		{
			state = 2;
		}

		private void glcScene_OpenGLDraw(object sender, RenderEventArgs args)
		{
			switch (state)
			{
				case 0:
					#region 0 : 타이틀
					#endregion
					break;
				case 1: case 2: case 3:
					#region 1 : 게임 / 2,3 : 게임 오버
					OpenGL gl = glcScene.OpenGL;

					gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
					gl.ClearColor(0.8f, 0.8f, 0.8f, 1.0f);
					gl.BlendFunc(BlendingSourceFactor.SourceAlpha, BlendingDestinationFactor.OneMinusSourceAlpha);

					gl.LoadIdentity();

					gl.MatrixMode(MatrixMode.Projection);
					gl.Perspective(60.0, (double)this.Width / this.Height, 0.01, 1000.0);
					gl.LookAt(0, player.y - 4, 5, 0, player.y + 3, 0, 0, 0, 1);

					gl.Enable(OpenGL.GL_TEXTURE_2D);
					for (int i = -3; i < 3; i++)
						for (int j = (int)player.y - 10; j < (int)player.y + 50; j++)
						{
							if ((i + 2 * j) % 4 == 0)
								texFloor2.Bind(gl);
							else texFloor1.Bind(gl);
							gl.Begin(BeginMode.Quads);
							{
								gl.TexCoord(0, 0);
								gl.Vertex(i, j, 0);
								gl.TexCoord(0, 1);
								gl.Vertex(i, j + 1, 0);
								gl.TexCoord(1, 1);
								gl.Vertex(i + 1, j + 1, 0);
								gl.TexCoord(1, 0);
								gl.Vertex(i + 1, j, 0);
							}
							gl.End();
						}

					if (player.y < 20)
					{
						texStart.Bind(gl);
						gl.Begin(BeginMode.Quads);
						{
							gl.TexCoord(0, 1);
							gl.Vertex(-2, 3, 3);
							gl.TexCoord(0, 0);
							gl.Vertex(-2, 3, 4);
							gl.TexCoord(1, 0);
							gl.Vertex(2, 3, 4);
							gl.TexCoord(1, 1);
							gl.Vertex(2, 3, 3);
						}
						gl.End();
					}
					else if(player.y % 500 > 450 || player.y % 500 < 50)
					{
						int yy = (int)((player.y + 250) / 500) * 500;
						texCheck.Bind(gl);
						gl.Begin(BeginMode.Quads);
						{
							gl.TexCoord(0, 1);
							gl.Vertex(-2, yy, 3);
							gl.TexCoord(0, 0);
							gl.Vertex(-2, yy, 4);
							gl.TexCoord(1, 0);
							gl.Vertex(2, yy, 4);
							gl.TexCoord(1, 1);
							gl.Vertex(2, yy, 3);
						}
						gl.End();
					}
					gl.Disable(OpenGL.GL_TEXTURE_2D);

					/*
					gl.Color(0.0f, 0.0f, 0.0f, 0.5f);

					gl.Enable(OpenGL.GL_BLEND);
					foreach (Walker w in walkers)
					{
						gl.Begin(BeginMode.Polygon);
						{
							for (int i = 0; i < 16; i++)
							{
								gl.Vertex(w.xpos * 0.5f + w.flying + 0.35 * Math.Cos(i * Math.PI / 8), w.y + 0.35 * Math.Sin(i * Math.PI / 8), 0.1);
							}
						}
						gl.End();
					}
					gl.Disable(OpenGL.GL_BLEND);
					*/

					gl.Enable(OpenGL.GL_TEXTURE_2D);

					int wi = 18, hi = 4;
					for (int j = (int)((player.y - 40) / wi) * wi; j <= (int)((player.y + 20) / wi) * wi; j += wi)
					{
						texBuilding[(j / wi + 100) % 4].Bind(gl);
						if ((j / wi - 3) % 11 == 10)
							gl.Color(0.5f, 0.5f, 0.5f, 1.0f);
						else gl.Color(1.0f, 1.0f, 1.0f, 1.0f);
						gl.Begin(BeginMode.Quads);
						{
							gl.TexCoord(0, 1);
							gl.Vertex(-3, j, 0);
							gl.TexCoord(1, 1);
							gl.Vertex(-3, j + wi, 0);
							gl.TexCoord(1, 0);
							gl.Vertex(-3, j + wi, hi);
							gl.TexCoord(0, 0);
							gl.Vertex(-3, j, hi);
						}
						gl.End();
						texBuilding[(j / wi + 102) % 4].Bind(gl);
						gl.Begin(BeginMode.Quads);
						{
							gl.TexCoord(1, 1);
							gl.Vertex(3, j, 0);
							gl.TexCoord(0, 1);
							gl.Vertex(3, j + wi, 0);
							gl.TexCoord(0, 0);
							gl.Vertex(3, j + wi, hi);
							gl.TexCoord(1, 0);
							gl.Vertex(3, j, hi);
						}
						gl.End();
					}

					gl.Color(1.0f, 1.0f, 1.0f, 1.0f);

					gl.Enable(OpenGL.GL_BLEND);
					gl.Enable(OpenGL.GL_ALPHA_TEST);
					gl.AlphaFunc(AlphaTestFunction.Great, 0.1f);

					texF.Bind(gl);
					foreach (Walker w in walkers)
						if (w.type == WalkerType.Female && w.flying == 0)
							w.Draw3D(gl);
					texM.Bind(gl);
					foreach (Walker w in walkers)
						if (w.type == WalkerType.Male && w.flying == 0)
							w.Draw3D(gl);
					texFFly.Bind(gl);
					foreach (Walker w in walkers)
						if (w.type == WalkerType.Female && w.flying != 0)
							w.Draw3DFlying(gl);
					texMFly.Bind(gl);
					foreach (Walker w in walkers)
						if (w.type == WalkerType.Male && w.flying != 0)
							w.Draw3DFlying(gl);

					texrun.Bind(gl);
					player.Draw3D(gl);

					gl.Disable(OpenGL.GL_TEXTURE_2D);

					gl.LoadIdentity();

					gl.Disable(OpenGL.GL_ALPHA_TEST);
					gl.Disable(OpenGL.GL_BLEND);
					gl.Flush();
					#endregion
					break;
			}
		}

		private void glcScene_GDIDraw(object sender, RenderEventArgs args)
		{
			Graphics g = args.Graphics;
			switch (state)
			{
				case 0:
					#region 0 : 타이틀
					int s = Math.Min(glcScene.Width, glcScene.Height);
					g.DrawImage(logo, new Rectangle((glcScene.Width - s) / 2, (glcScene.Height - s) / 2, s, s));
					g.DrawString("Press [Enter] to start!", new Font("Arial", 12), Brushes.White, new PointF(glcScene.Width / 2, glcScene.Height - 21), middle);
					#endregion
					break;
				case 1:
					#region 1 : 게임

					g.FillRectangle(Brushes.Black, new Rectangle(0, 0, Width, Height / 10));

					g.DrawString((int)player.y + "m", font, Brushes.White, new PointF(size, size));

					g.DrawString(player.speed.ToString("0.0") + "m/s", font, Brushes.White, new PointF(glcScene.Width - size, size), right);

					g.FillRectangle(Brushes.Orange, new Rectangle(glcScene.Width * 2 / 5, glcScene.Height / 32, (int)(glcScene.Width * player.sprint / 500), glcScene.Height / 70));
					g.DrawString(((int)player.sprint).ToString(), smallfont, Brushes.White, new PointF(glcScene.Width / 2, glcScene.Height / 32 - size * 0.35f), middle);

					g.FillRectangle(Brushes.Red, new Rectangle(glcScene.Width * 2 / 5, glcScene.Height / 16, (int)(glcScene.Width * chul / 500), glcScene.Height / 70));
					g.DrawString(((int)chul).ToString(), smallfont, Brushes.Gray, new PointF(glcScene.Width / 2, glcScene.Height / 16 - size * 0.35f), middle);

					if (police > 0)
					{
						float he = glcScene.Height * 0.2f * police / 10000;
						if (police < 2500) he = glcScene.Height * 0.05f;

						g.DrawImage(bpolice, new RectangleF(0, glcScene.Height - he, he * bpolice.Width / bpolice.Height, he));
					}
					if (timer)
					{
						float he = glcScene.Width * 0.25f;
						g.DrawImage(bcheckpoint, new RectangleF(glcScene.Width - he, 0, he, he));
						g.DrawImage(bgreen, new RectangleF(glcScene.Width - he * 0.45f, he * 0.05f, he * 0.4f, he * 0.4f));
						for(int i = 0; i < (targettime - curtime) / targettime * 7; i++)
							g.DrawImage(bmark, new RectangleF(glcScene.Width - he * 0.12f, he * (0.4f - 0.06f * i), he * 0.06f, he * 0.06f));
					}
					#endregion
					break;
				case 2:
					#region 2 : 철컹철컹 게임 오버

					g.FillRectangle(new SolidBrush(Color.FromArgb(128, 0, 0, 0)), new Rectangle(0, 0, Width, Height));

					g.FillRectangle(Brushes.White, new Rectangle(glcScene.Width / 4, glcScene.Height / 4, glcScene.Width / 2, glcScene.Height / 2));
					g.DrawString("출근을 하지 못하고\r\n\r\n철컹철컹 당했다.\r\n\r\n움직인 거리 : " + player.y, font, Brushes.Black, new PointF(glcScene.Width / 2, glcScene.Height * 0.37f), middle);

					#endregion
					break;
				case 3:
					#region 3 : 시간 초과 게임 오버

					float heaa = glcScene.Width * 0.25f;
					g.DrawImage(bcheckpoint, new RectangleF(glcScene.Width - heaa, 0, heaa, heaa));
					g.DrawImage(bred, new RectangleF(glcScene.Width - heaa * 0.45f, heaa * 0.05f, heaa * 0.4f, heaa * 0.4f));

					g.FillRectangle(new SolidBrush(Color.FromArgb(128, 0, 0, 0)), new Rectangle(0, 0, Width, Height));

					g.FillRectangle(Brushes.White, new Rectangle(glcScene.Width / 4, glcScene.Height / 4, glcScene.Width / 2, glcScene.Height / 2));
					g.DrawString("신호등을 건너지 못해\r\n\r\n회사를 가지 못했다.\r\n\r\n움직인 거리 : " + player.y, font, Brushes.Black, new PointF(glcScene.Width / 2, glcScene.Height * 0.37f), middle);

					#endregion
					break;
			}
			if (paused)
				g.DrawString("PAUSED", font, Brushes.Blue, new PointF(glcScene.Width / 2, glcScene.Height * 0.45f), middle);
		}

		private void glcScene_SizeChanged(object sender, EventArgs e)
		{
			size = glcScene.Height / 30.0f;
			if (size < 1.0f) size = 1.0f;
			font = new Font("Dotum", size);
			smallfont = new Font("Dotum", size * 0.7f);
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

			texMFly = new Texture();
			texMFly.Create(gl, RRRR.Properties.Resources.Walker_M_Flew);
			texFFly = new Texture();
			texFFly.Create(gl, RRRR.Properties.Resources.Walker_F_Flew);

			player = new Player(0, 0, 0);
			walkers.Add(player);
			player.y = 400;

			texBuilding[0] = new Texture();
			texBuilding[0].Create(gl, RRRR.Properties.Resources.apart_1);
			texBuilding[1] = new Texture();
			texBuilding[1].Create(gl, RRRR.Properties.Resources.apart_2);
			texBuilding[2] = new Texture();
			texBuilding[2].Create(gl, RRRR.Properties.Resources.apart_3);
			texBuilding[3] = new Texture();
			texBuilding[3].Create(gl, RRRR.Properties.Resources.apart_4);

			texFloor1 = new Texture();
			texFloor1.Create(gl, RRRR.Properties.Resources.block_0);
			texFloor2 = new Texture();
			texFloor2.Create(gl, RRRR.Properties.Resources.block_1);

			texStart = new Texture();
			texStart.Create(gl, RRRR.Properties.Resources.START);
			texCheck = new Texture();
			texCheck.Create(gl, RRRR.Properties.Resources.checkpoint);
		}

		private void glcScene_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
			{
				paused = !paused;
			}
		}
	}
}
