namespace RRRR
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.glcScene = new SharpGL.OpenGLControl();
			((System.ComponentModel.ISupportInitialize)(this.glcScene)).BeginInit();
			this.SuspendLayout();
			// 
			// glcScene
			// 
			this.glcScene.Dock = System.Windows.Forms.DockStyle.Fill;
			this.glcScene.DrawFPS = false;
			this.glcScene.Location = new System.Drawing.Point(0, 0);
			this.glcScene.Name = "glcScene";
			this.glcScene.RenderContextType = SharpGL.RenderContextType.DIBSection;
			this.glcScene.RenderTrigger = SharpGL.RenderTrigger.Manual;
			this.glcScene.Size = new System.Drawing.Size(284, 262);
			this.glcScene.TabIndex = 0;
			this.glcScene.OpenGLDraw += new SharpGL.RenderEventHandler(this.glcScene_OpenGLDraw);
			this.glcScene.GDIDraw += new SharpGL.RenderEventHandler(this.glcScene_GDIDraw);
			this.glcScene.SizeChanged += new System.EventHandler(this.glcScene_SizeChanged);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 262);
			this.Controls.Add(this.glcScene);
			this.Name = "MainForm";
			this.Text = "Rush Hour! Run! Run! Run!";
			((System.ComponentModel.ISupportInitialize)(this.glcScene)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private SharpGL.OpenGLControl glcScene;
	}
}

