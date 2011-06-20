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
            ReleaseData();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainerFrames = new System.Windows.Forms.SplitContainer();
            this.imageBoxFrameGrabber = new Emgu.CV.UI.ImageBox();
            this.imageBoxSkin = new Emgu.CV.UI.ImageBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cam_capability = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.cam_devices = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.statue_bar = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.screen_height = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.screen_width = new System.Windows.Forms.ToolStripLabel();
            this.notify_icon = new System.Windows.Forms.NotifyIcon(this.components);
            this.notify_popup_menu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainerFrames.Panel1.SuspendLayout();
            this.splitContainerFrames.Panel2.SuspendLayout();
            this.splitContainerFrames.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageBoxFrameGrabber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageBoxSkin)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.statue_bar.SuspendLayout();
            this.notify_popup_menu.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainerFrames);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer1.Size = new System.Drawing.Size(1022, 592);
            this.splitContainer1.SplitterDistance = 752;
            this.splitContainer1.TabIndex = 0;
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
            this.splitContainerFrames.Size = new System.Drawing.Size(752, 592);
            this.splitContainerFrames.SplitterDistance = 385;
            this.splitContainerFrames.TabIndex = 4;
            // 
            // imageBoxFrameGrabber
            // 
            this.imageBoxFrameGrabber.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.imageBoxFrameGrabber.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageBoxFrameGrabber.Location = new System.Drawing.Point(0, 0);
            this.imageBoxFrameGrabber.Name = "imageBoxFrameGrabber";
            this.imageBoxFrameGrabber.Size = new System.Drawing.Size(385, 592);
            this.imageBoxFrameGrabber.TabIndex = 4;
            this.imageBoxFrameGrabber.TabStop = false;
            // 
            // imageBoxSkin
            // 
            this.imageBoxSkin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.imageBoxSkin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageBoxSkin.Location = new System.Drawing.Point(0, 0);
            this.imageBoxSkin.Name = "imageBoxSkin";
            this.imageBoxSkin.Size = new System.Drawing.Size(363, 592);
            this.imageBoxSkin.TabIndex = 2;
            this.imageBoxSkin.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cam_capability);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.checkBox2);
            this.groupBox1.Controls.Add(this.checkBox1);
            this.groupBox1.Controls.Add(this.cam_devices);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(266, 592);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Configuration";
            // 
            // cam_capability
            // 
            this.cam_capability.FormattingEnabled = true;
            this.cam_capability.Location = new System.Drawing.Point(9, 108);
            this.cam_capability.Name = "cam_capability";
            this.cam_capability.Size = new System.Drawing.Size(145, 21);
            this.cam_capability.TabIndex = 5;
            this.cam_capability.SelectedIndexChanged += new System.EventHandler(this.cam_capability_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Camera Capability";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(9, 163);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(87, 17);
            this.checkBox1.TabIndex = 3;
            this.checkBox1.Text = "Mirror Image";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // cam_devices
            // 
            this.cam_devices.FormattingEnabled = true;
            this.cam_devices.Location = new System.Drawing.Point(9, 44);
            this.cam_devices.Name = "cam_devices";
            this.cam_devices.Size = new System.Drawing.Size(145, 21);
            this.cam_devices.TabIndex = 1;
            this.cam_devices.SelectedIndexChanged += new System.EventHandler(this.cam_devices_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Choose a camera";
            // 
            // statue_bar
            // 
            this.statue_bar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.statue_bar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.statue_bar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.screen_height,
            this.toolStripSeparator1,
            this.toolStripLabel3,
            this.screen_width});
            this.statue_bar.Location = new System.Drawing.Point(0, 567);
            this.statue_bar.Name = "statue_bar";
            this.statue_bar.Size = new System.Drawing.Size(1022, 25);
            this.statue_bar.TabIndex = 5;
            this.statue_bar.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(81, 22);
            this.toolStripLabel1.Text = "Screen Heigth";
            // 
            // screen_height
            // 
            this.screen_height.Name = "screen_height";
            this.screen_height.Size = new System.Drawing.Size(86, 22);
            this.screen_height.Text = "toolStripLabel2";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(77, 22);
            this.toolStripLabel3.Text = "Screen Width";
            // 
            // screen_width
            // 
            this.screen_width.Name = "screen_width";
            this.screen_width.Size = new System.Drawing.Size(86, 22);
            this.screen_width.Text = "toolStripLabel4";
            // 
            // notify_icon
            // 
            this.notify_icon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notify_icon.BalloonTipText = "the program has been minimzed";
            this.notify_icon.BalloonTipTitle = "Info";
            this.notify_icon.ContextMenuStrip = this.notify_popup_menu;
            this.notify_icon.Icon = ((System.Drawing.Icon)(resources.GetObject("notify_icon.Icon")));
            this.notify_icon.Text = "Hand Gestures Recognition";
            this.notify_icon.Visible = true;
            this.notify_icon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // notify_popup_menu
            // 
            this.notify_popup_menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.toolStripMenuItem4,
            this.toolStripSeparator2,
            this.toolStripMenuItem3});
            this.notify_popup_menu.Name = "contextMenuStrip1";
            this.notify_popup_menu.Size = new System.Drawing.Size(251, 98);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(250, 22);
            this.toolStripMenuItem1.Text = "Open Hand Gestures Recognition";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(250, 22);
            this.toolStripMenuItem2.Text = "Help";
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(250, 22);
            this.toolStripMenuItem4.Text = "About";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(247, 6);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(250, 22);
            this.toolStripMenuItem3.Text = "Exit";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(9, 196);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(124, 17);
            this.checkBox2.TabIndex = 3;
            this.checkBox2.Text = "Hide when minimized";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1022, 592);
            this.Controls.Add(this.statue_bar);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainerFrames.Panel1.ResumeLayout(false);
            this.splitContainerFrames.Panel2.ResumeLayout(false);
            this.splitContainerFrames.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.imageBoxFrameGrabber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageBoxSkin)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.statue_bar.ResumeLayout(false);
            this.statue_bar.PerformLayout();
            this.notify_popup_menu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainerFrames;
        private Emgu.CV.UI.ImageBox imageBoxFrameGrabber;
        private Emgu.CV.UI.ImageBox imageBoxSkin;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cam_capability;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.ComboBox cam_devices;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStrip statue_bar;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripLabel screen_height;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripLabel screen_width;
        private System.Windows.Forms.NotifyIcon notify_icon;
        private System.Windows.Forms.ContextMenuStrip notify_popup_menu;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.CheckBox checkBox2;

    }
}

