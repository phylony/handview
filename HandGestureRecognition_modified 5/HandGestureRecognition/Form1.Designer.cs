namespace HandGestureRecognition
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.splitContainerFrames = new System.Windows.Forms.SplitContainer();
            this.imageBoxSkin = new Emgu.CV.UI.ImageBox();
            this.imageBoxFrameGrabber = new Emgu.CV.UI.ImageBox();
            this.splitContainerFrames.Panel1.SuspendLayout();
            this.splitContainerFrames.Panel2.SuspendLayout();
            this.splitContainerFrames.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageBoxSkin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageBoxFrameGrabber)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainerFrames
            // 
            this.splitContainerFrames.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerFrames.Location = new System.Drawing.Point(0, 0);
            this.splitContainerFrames.Name = "splitContainerFrames";
            // 
            // splitContainerFrames.Panel1
            // 
            this.splitContainerFrames.Panel1.Controls.Add(this.imageBoxFrameGrabber);
            // 
            // splitContainerFrames.Panel2
            // 
            this.splitContainerFrames.Panel2.Controls.Add(this.imageBoxSkin);
            this.splitContainerFrames.Size = new System.Drawing.Size(1022, 592);
            this.splitContainerFrames.SplitterDistance = 524;
            this.splitContainerFrames.TabIndex = 3;
            // 
            // imageBoxSkin
            // 
            this.imageBoxSkin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.imageBoxSkin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageBoxSkin.Location = new System.Drawing.Point(0, 0);
            this.imageBoxSkin.Name = "imageBoxSkin";
            this.imageBoxSkin.Size = new System.Drawing.Size(494, 592);
            this.imageBoxSkin.TabIndex = 2;
            this.imageBoxSkin.TabStop = false;
            // 
            // imageBoxFrameGrabber
            // 
            this.imageBoxFrameGrabber.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.imageBoxFrameGrabber.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageBoxFrameGrabber.Location = new System.Drawing.Point(0, 0);
            this.imageBoxFrameGrabber.Name = "imageBoxFrameGrabber";
            this.imageBoxFrameGrabber.Size = new System.Drawing.Size(524, 592);
            this.imageBoxFrameGrabber.TabIndex = 4;
            this.imageBoxFrameGrabber.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1022, 592);
            this.Controls.Add(this.splitContainerFrames);
            this.KeyPreview = true;
            this.Name = "Form1";
            this.Text = "Form1";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.splitContainerFrames.Panel1.ResumeLayout(false);
            this.splitContainerFrames.Panel2.ResumeLayout(false);
            this.splitContainerFrames.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.imageBoxSkin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageBoxFrameGrabber)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerFrames;
        private Emgu.CV.UI.ImageBox imageBoxSkin;
        private Emgu.CV.UI.ImageBox imageBoxFrameGrabber;
    }
}

