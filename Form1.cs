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
        private Button _button1;
        private Button _button2;
        private TextBox _tb1;
        private static MMDeviceEnumerator enumer = new MMDeviceEnumerator();
        private MMDevice dev = enumer.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

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
            this._button1 = new System.Windows.Forms.Button();
            this._button2 = new System.Windows.Forms.Button();
            this._tb1 = new System.Windows.Forms.TextBox();
            this.Bknob = new KnobControl();
            this.SuspendLayout();
            // 
            // button1
            // 
            this._button1.Location = new System.Drawing.Point(12, 12);
            this._button1.Name = "_button1";
            this._button1.Size = new System.Drawing.Size(106, 23);
            this._button1.TabIndex = 4;
            this._button1.Text = "Volume Down";
            this._button1.UseVisualStyleBackColor = true;
            this._button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // button2
            // 
            this._button2.Location = new System.Drawing.Point(12, 41);
            this._button2.Name = "_button2";
            this._button2.Size = new System.Drawing.Size(106, 23);
            this._button2.TabIndex = 5;
            this._button2.Text = "Volume Up";
            this._button2.UseVisualStyleBackColor = true;
            this._button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // tb1
            // 
            this._tb1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tb1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._tb1.Location = new System.Drawing.Point(156, 25);
            this._tb1.Name = "_tb1";
            this._tb1.Size = new System.Drawing.Size(51, 23);
            this._tb1.TabIndex = 7;
            this._tb1.Text = "0";
            this._tb1.TextChanged += new System.EventHandler(this.tb1_TextChanged);
            this._tb1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tb1_KeyUp);
            // 
            // Bknob
            // 
            this.Bknob.BackColor = System.Drawing.Color.White;
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
            this.Bknob.ValueChanged += new ValueChangedEventHandler(this.Bknob_ValueChanged);
            this.Bknob.Load += new System.EventHandler(this.Bknob_Load);
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
            this.ClientSize = new System.Drawing.Size(339, 89);
            this.Controls.Add(this._tb1);
            this.Controls.Add(this._button2);
            this.Controls.Add(this._button1);
            this.Controls.Add(this.Bknob);
            this.MaximizeBox = false;
            this.Name = "Form1";
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

        private void ChangeLabelColor()
        {
	        SetVolume(CheckVolLimit(Bknob.Value));
        }

		private void Bknob_ValueChanged(object sender)
		{
			if (!Bknob.AllowDrop)
			{
				ChangeLabelColor();
			}
			else
			{
				_tb1.Text = Bknob.Value.ToString();
            }
            Bknob.AllowDrop = false;

        }

        private void Bknob_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
	        Bknob.Value = CheckVolLimit(++Bknob.Value);
            SetVolume(Bknob.Value);
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
	        Bknob.Value = CheckVolLimit(--Bknob.Value);
            SetVolume(Bknob.Value);
        }

        public void SystemVolumeConfigurator()
        {
	        _playbackDevice = _deviceEnumerator.GetDefaultAudioEndpoint((DataFlow)EDataFlow.Render, (Role)ERole.Multimedia);
        }

        public int GetVolume()
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
	        volumeLevel = CheckVolLimit(volumeLevel);
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
		        button2_Click(null, null);
	        }
	        else
	        {
		        button1_Click_1(null, null);
	        }

	        // Avoid to send MouseWheel event to the parent container
	        ((HandledMouseEventArgs)e).Handled = true;
        }

        private void tb1_TextChanged(object sender, EventArgs e)
        {
	        
        }

        private void tb1_KeyUp(object sender, KeyEventArgs e)
        {
	        if (e.KeyData == Keys.Enter)
	        {
		        SetVolume(CheckVolLimit(Convert.ToInt32(_tb1.Text)));
	        }
        }
	}
}
