using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RRRR
{
	static class Program
	{
		static MainForm form;
		static Stopwatch sw;
		static float prev;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			//Application.SetCompatibleTextRenderingDefault(false);

			form = new MainForm();
			form.ClientSize = new Size(600, 450);

			Application.Idle += GameLoop;

			sw = Stopwatch.StartNew();

			Application.Run(form);
		}

		static void GameLoop(object sender, EventArgs e)
		{
			float current = (float)sw.Elapsed.TotalMilliseconds;
			form.UpdateWorld(current - prev);
			form.Invalidate(true);

			prev = current;
		}
	}
}
