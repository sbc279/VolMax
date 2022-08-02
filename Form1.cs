using System;
using System.Windows.Forms;
using NAudio.CoreAudioApi;
using Ozeki.Media.Audio.CoreAudio;
using MMDevice = NAudio.CoreAudioApi.MMDevice;
using MMDeviceEnumerator = NAudio.CoreAudioApi.MMDeviceEnumerator;

namespace VolumeControl
{
    /// <summary>
    /// Summary description for Form1.
    /// </summary>
    public class Form1 : Form
	{
		private KnobControl Bknob;
        private Button btnVolDown;
        private Button btnVolUp;
        private TextBox _tb1;
        private static MMDeviceEnumerator enumer = new MMDeviceEnumerator();
        private MMDevice dev = enumer.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
        private CheckBox checkBox1;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btnVolDown = new System.Windows.Forms.Button();
            this.btnVolUp = new System.Windows.Forms.Button();
            this._tb1 = new System.Windows.Forms.TextBox();
            this.Bknob = new VolumeControl.KnobControl();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnVolDown
            // 
            this.btnVolDown.Location = new System.Drawing.Point(12, 40);
            this.btnVolDown.Name = "btnVolDown";
            this.btnVolDown.Size = new System.Drawing.Size(106, 23);
            this.btnVolDown.TabIndex = 4;
            this.btnVolDown.Text = "Volume Down";
            this.btnVolDown.UseVisualStyleBackColor = true;
            this.btnVolDown.Click += new System.EventHandler(this.btnVolDown_Click);
            // 
            // btnVolUp
            // 
            this.btnVolUp.Location = new System.Drawing.Point(12, 11);
            this.btnVolUp.Name = "btnVolUp";
            this.btnVolUp.Size = new System.Drawing.Size(106, 23);
            this.btnVolUp.TabIndex = 5;
            this.btnVolUp.Text = "Volume Up";
            this.btnVolUp.UseVisualStyleBackColor = true;
            this.btnVolUp.Click += new System.EventHandler(this.btnVolUp_Click);
            // 
            // _tb1
            // 
            this._tb1.BackColor = System.Drawing.SystemColors.Control;
            this._tb1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tb1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._tb1.Location = new System.Drawing.Point(154, 13);
            this._tb1.Name = "_tb1";
            this._tb1.Size = new System.Drawing.Size(51, 23);
            this._tb1.TabIndex = 7;
            this._tb1.Text = "0";
            //this._tb1.TextChanged += new System.EventHandler(this.tb1_TextChanged);
            this._tb1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tb1_KeyUp);
            // 
            // Bknob
            // 
            this.Bknob.BackColor = System.Drawing.SystemColors.Control;
            this.Bknob.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Bknob.ForeColor = System.Drawing.Color.Black;
            this.Bknob.ImeMode = System.Windows.Forms.ImeMode.On;
            this.Bknob.LargeChange = 10;
            this.Bknob.Location = new System.Drawing.Point(246, 2);
            this.Bknob.Maximum = 100;
            this.Bknob.Minimum = 0;
            this.Bknob.Name = "Bknob";
            this.Bknob.ShowLargeScale = true;
            this.Bknob.ShowSmallScale = false;
            this.Bknob.Size = new System.Drawing.Size(81, 75);
            this.Bknob.SmallChange = 1;
            this.Bknob.TabIndex = 2;
            this.Bknob.Value = 0;
            this.Bknob.ValueChanged += new VolumeControl.ValueChangedEventHandler(this.Bknob_ValueChanged);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(152, 43);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(58, 20);
            this.checkBox1.TabIndex = 8;
            this.checkBox1.Text = "Mute";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
            this.ClientSize = new System.Drawing.Size(335, 85);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this._tb1);
            this.Controls.Add(this.btnVolUp);
            this.Controls.Add(this.btnVolDown);
            this.Controls.Add(this.Bknob);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.ShowIcon = false;
            this.Text = "Craver Systems VOLMAX";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
        #endregion

        private readonly MMDeviceEnumerator _deviceEnumerator = new MMDeviceEnumerator();
        private MMDevice _playbackDevice;

        private void Form1_Load(object sender, EventArgs e)
        {
	        SystemVolumeConfigurator();

	        Bknob.Value = GetVolume();
            Bknob.AllowDrop = false;

            dev.AudioEndpointVolume.OnVolumeNotification += AudioEndpointVolume_OnVolumeNotification;

        }


        void AudioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
        {
            // This shows data.MasterVolume, you can do whatever you want here
            //this.Bknob.Value = CheckVolLimit((int)(data.MasterVolume * 100));
            SetVolLabel(CheckVolLimit((int)(data.MasterVolume * 100)).ToString(), true);
        }

        private void SetVolLabel(string value, bool cb) // Thread-safe
        {
	        if (InvokeRequired)
	        {
		        Invoke(new Action<string, bool>(SetVolLabel), value, cb);
		        return;
	        }
            Bknob.AllowDrop = cb;
            Bknob.Value = Convert.ToInt32(value);
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.Run(new Form1());
		}

		private void Bknob_ValueChanged(object sender)
		{
            if (!Bknob.AllowDrop)
            {
                SetVolume(CheckVolLimit(Bknob.Value));
            }
            else
            {
                Bknob.AllowDrop = false;
            }

            _tb1.Text = Bknob.Value.ToString();

		}

        private void btnVolUp_Click(object sender, EventArgs e)
        {
	        _playbackDevice.AudioEndpointVolume.VolumeStepUp();
        }

        private void btnVolDown_Click(object sender, EventArgs e)
        {
	        _playbackDevice.AudioEndpointVolume.VolumeStepDown();
        }

        private void SystemVolumeConfigurator()
        {
	        _playbackDevice = _deviceEnumerator.GetDefaultAudioEndpoint((DataFlow)EDataFlow.Render, (Role)ERole.Multimedia);
        }

        private int GetVolume()
        {
	        return (int)(_playbackDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
        }

        private int CheckVolLimit(int volumeLevel)
        {
            return volumeLevel < 0
	            ? 0
	            : volumeLevel > 100
		            ? 100
		            : volumeLevel;
        }

        private void SetVolume(int volumeLevel)
        {
	        _playbackDevice.AudioEndpointVolume.MasterVolumeLevelScalar = volumeLevel / 100.0f;
	        _tb1.Text = volumeLevel.ToString();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
	        base.OnMouseWheel(e);

	        // the Delta value is always 120, as explained in MSDN
	        int v = (e.Delta / 120) * (100 - 0) / 5;
	        if (v > 0)
	        {
		        btnVolUp_Click(null, null);
	        }
	        else
	        {
		        btnVolDown_Click(null, null);
	        }

	        // Avoid sending the MouseWheel event to the parent container
	        ((HandledMouseEventArgs)e).Handled = true;
        }

        private void tb1_KeyUp(object sender, KeyEventArgs e)
        {
            // If enter is pressed on the text box
	        if (e.KeyData == Keys.Enter)
	        {
		        if (int.TryParse(this._tb1.Text, out var value))
		        {
			        SetVolume(CheckVolLimit(value));
		        }
	        }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
	        _playbackDevice.AudioEndpointVolume.Mute = this.checkBox1.Checked;
        }
	}
}
