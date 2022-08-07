using System;
using System.Drawing;
using System.Windows.Forms;
using NAudio.CoreAudioApi;
using MMDevice = NAudio.CoreAudioApi.MMDevice;
using MMDeviceEnumerator = NAudio.CoreAudioApi.MMDeviceEnumerator;

namespace VolumeControl
{
    /// <summary>
    /// Summary description for Form1.
    /// </summary>
    public class Form1 : Form
    {
        private KnobControl _bknob;
        private static MMDeviceEnumerator _enumer = new MMDeviceEnumerator();
        private readonly MMDevice _audioEndpoint = _enumer.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
        private Label _tb1;
        private readonly System.ComponentModel.Container _components = null;

        private Form1()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //


        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _audioEndpoint.Dispose();
                _enumer.Dispose();
                if (_components != null)
                {
                    _components.Dispose();
                }
            }

            base.Dispose(disposing);
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            _bknob.Value = GetVolume();
            _bknob.AllowDrop = false;
            _audioEndpoint.AudioEndpointVolume.OnVolumeNotification += AudioEndpointVolume_OnVolumeNotification;
            
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.Run(new Form1());
            //dev.AudioEndpointVolume.OnVolumeNotification += VolumeNotification(null, null);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this._tb1 = new System.Windows.Forms.Label();
            this._bknob = new VolumeControl.KnobControl();
            this.SuspendLayout();
            // 
            // _tb1
            // 
            this._tb1.BackColor = System.Drawing.SystemColors.Control;
            this._tb1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._tb1.Location = new System.Drawing.Point(0, 99);
            this._tb1.Name = "_tb1";
            this._tb1.Size = new System.Drawing.Size(125, 19);
            this._tb1.TabIndex = 0;
            this._tb1.Text = "0";
            this._tb1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this._tb1.Click += new System.EventHandler(this._tb1_Click);
            // 
            // _bknob
            // 
            this._bknob.BackColor = System.Drawing.SystemColors.Control;
            this._bknob.Dock = System.Windows.Forms.DockStyle.Right;
            this._bknob.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._bknob.ForeColor = System.Drawing.Color.Black;
            this._bknob.ImeMode = System.Windows.Forms.ImeMode.On;
            this._bknob.LargeChange = 10;
            this._bknob.Location = new System.Drawing.Point(12, 0);
            this._bknob.Maximum = 100;
            this._bknob.Minimum = 0;
            this._bknob.Name = "_bknob";
            this._bknob.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._bknob.ShowLargeScale = true;
            this._bknob.ShowSmallScale = false;
            this._bknob.Size = new System.Drawing.Size(113, 99);
            this._bknob.SmallChange = 1;
            this._bknob.TabIndex = 3;
            this._bknob.Value = 0;
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(125, 118);
            this.ControlBox = false;
            this.Controls.Add(this._bknob);
            this.Controls.Add(this._tb1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.ImeMode = System.Windows.Forms.ImeMode.On;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(100, 100);
            this.Name = "Form1";
            this.Text = "Craver VOLMAX";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        #region Audio Control
        private int GetVolume()
        {
            return (int)(_audioEndpoint.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
        }

        private void SetVolume(int volumeLevel)
        {
            _audioEndpoint.AudioEndpointVolume.MasterVolumeLevelScalar = volumeLevel / 100.0f;
            //_tb1.Text = _audioEndpoint.AudioEndpointVolume.Mute
            //    ? "MUTE"
            //    : volumeLevel.ToString();
        }
        #endregion

        #region Audio Stats


        #endregion

        #region Audio Tools
        private int CheckVolLimit(int volumeLevel)
        {
            return volumeLevel < 0
                ? 0
                : volumeLevel > 100
                    ? 100
                    : volumeLevel;
        }

        #endregion

        #region Misc Events
        void AudioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
        {
            SetVolLabel(CheckVolLimit((int)(data.MasterVolume * 100)).ToString(), true);
            SetVolText(data.Muted
                ? "MUTE"
                : GetVolume().ToString());

            _tb1.ForeColor = data.Muted
                ? Color.Red
                : Color.DarkGray;

        }

        private void Bknob_ValueChanged(object sender)
        {
            if (!_bknob.AllowDrop)
            {
                SetVolume(CheckVolLimit(_bknob.Value));
            }
            else
            {
                _bknob.AllowDrop = false;
            }

            //_tb1.Text = _bknob.Value.ToString();
            //_tb1.Text = _audioEndpoint.AudioEndpointVolume.Mute
            //    ? "MUTE"
            //    : _bknob.Value.ToString();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            // the Delta value is always 120, as explained in MSDN
            int v = (e.Delta / 120) * (100 - 0) / 5;
            if (v > 0)
            {
                //btnVolUp_Click(null, null);
                _audioEndpoint.AudioEndpointVolume.VolumeStepUp();
            }
            else
            {
                //btnVolDown_Click(null, null);
                _audioEndpoint.AudioEndpointVolume.VolumeStepDown();
            }

            // Avoid sending the MouseWheel event to the parent container
            ((HandledMouseEventArgs)e).Handled = true;
        }

        private void _bknob_MouseClick(object sender, MouseEventArgs e)
        {
            //var knob = new KnobControl();
            //// get current position of pointer             
            //Point arrow = knob.GetKnobPosition();
            
            //arrow.X = arrow.X - 3;
            //arrow.Y = arrow.Y - 3;

            ((HandledMouseEventArgs)e).Handled = true;
        }

        private void _bknob_MouseDown(object sender, MouseEventArgs e)
        {
            this.Cursor = new Cursor(Cursor.Current.Handle);
            Cursor.Position = new Point(Cursor.Position.X - 50, Cursor.Position.Y - 50);
            Cursor.Clip = new Rectangle(this.Location, this.Size);
        }
        #endregion

        #region Thread related
        private void SetVolText(string value) // Thread-safe
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string> (SetVolText), value);
                return;
            }

            _tb1.Text = value;
        }

        private void SetVolLabel(string value, bool cb) // Thread-safe
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string, bool>(SetVolLabel), value, cb);
                return;
            }

            _bknob.AllowDrop = cb;
            _bknob.Value = Convert.ToInt32(value);
        }

        #endregion

        private void _tb1_Click(object sender, EventArgs e)
        {
            _audioEndpoint.AudioEndpointVolume.Mute = !_audioEndpoint.AudioEndpointVolume.Mute;
            //_tb1.ForeColor = _audioEndpoint.AudioEndpointVolume.Mute
            //    ? Color.Red
            //    : Color.DarkGray;
            //if (_audioEndpoint.AudioEndpointVolume.Mute == true)
            //{
            //    SetVolText("MUTE");
            //}
            //else
            //{
            //    SetVolText(CheckVolLimit(_bknob.Value).ToString());
            //}
        }
    }
}

