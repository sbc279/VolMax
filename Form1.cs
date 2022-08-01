using System;
using System.Windows.Forms;
using NAudio.CoreAudioApi;
using Ozeki.Media.Audio.CoreAudio;
using MMDevice = NAudio.CoreAudioApi.MMDevice;
using MMDeviceEnumerator = NAudio.CoreAudioApi.MMDeviceEnumerator;

namespace knobControl
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private KnobControl.KnobControl Bknob;
        private Button button1;
        private Button button2;
        private TextBox tb1;

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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.Bknob = new KnobControl.KnobControl();
            this.tb1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(106, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Volume Down";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 41);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(106, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "Volume Up";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
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
            this.Bknob.ValueChanged += new KnobControl.ValueChangedEventHandler(this.Bknob_ValueChanged);
            this.Bknob.Load += new System.EventHandler(this.Bknob_Load);
            // 
            // tb1
            // 
            this.tb1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tb1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb1.Location = new System.Drawing.Point(156, 25);
            this.tb1.Name = "tb1";
            this.tb1.Size = new System.Drawing.Size(51, 23);
            this.tb1.TabIndex = 7;
            this.tb1.Text = "0";
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
            this.ClientSize = new System.Drawing.Size(339, 89);
            this.Controls.Add(this.tb1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.Bknob);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Craver Systems VOLMAX";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
        #endregion


        private void Form1_Load(object sender, EventArgs e)
        {
	        SystemVolumeConfigurator();

	        this.Bknob.Value = GetVolume();
	        
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.Run(new Form1());
		}

        private void Rknob_ValueChanged(object Sender)
		{
			//ChangeLabelColor();
		}
		private void ChangeLabelColor()
		{
			; //i.ToString(); //this.Bknob.Value.ToString();
			SetVolume(this.Bknob.Value);
            //i = this.Bknob.Value;
            //this.tb1.Text = i.ToString(); //this.Bknob.Value.ToString();
        }


        private void Gknob_ValueChanged(object Sender)
		{
			ChangeLabelColor();
		}

		private void Bknob_ValueChanged(object Sender)
		{
			ChangeLabelColor();
		}

        private void Bknob_Load(object sender, EventArgs e)
        {

        }
		//=========================================================================


        //public int i = 0;


        private void button2_Click(object sender, EventArgs e)
        {
	        this.Bknob.Value = CheckVolLimit(++this.Bknob.Value);
            SetVolume(this.Bknob.Value);
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
	        this.Bknob.Value = CheckVolLimit(--this.Bknob.Value);
            SetVolume(this.Bknob.Value);
        }

        private readonly MMDeviceEnumerator _deviceEnumerator = new MMDeviceEnumerator();
        private MMDevice _playbackDevice;



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
	        this.tb1.Text = volumeLevel.ToString();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            //if (isFocused && isKnobRotating &&
            //    Utility.isPointinRectangle(new Point(e.X, e.Y), rKnob))
            {
                // the Delta value is always 120, as explained in MSDN
                int v = (e.Delta / 120) * (100 - 0) / 5;
                if(v>=0) this.Bknob.Value = v;

                // Avoid to send MouseWheel event to the parent container
                ((HandledMouseEventArgs)e).Handled = true;
            }
        }
    }
}
