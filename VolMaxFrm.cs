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
    public class VolMaxFrm : Form
    {
        private VolMax _bknob;
        private Button _btnVolDown;
        private Button _btnVolUp; 
        private static MMDeviceEnumerator _enumer = new MMDeviceEnumerator();
        private readonly MMDevice _audioEndpoint = _enumer.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
        private CheckBox _checkBox1;
        private Label _tb1;
        private readonly System.ComponentModel.Container _components = null;

        private VolMaxFrm()
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
            Application.Run(new VolMaxFrm());
            //dev.AudioEndpointVolume.OnVolumeNotification += VolumeNotification(null, null);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VolMaxFrm));
            this._btnVolDown = new System.Windows.Forms.Button();
            this._btnVolUp = new System.Windows.Forms.Button();
            this._checkBox1 = new System.Windows.Forms.CheckBox();
            this._tb1 = new System.Windows.Forms.Label();
            this._bknob = new VolumeControl.VolMax();
            this.SuspendLayout();
            // 
            // _btnVolDown
            // 
            this._btnVolDown.Location = new System.Drawing.Point(12, 41);
            this._btnVolDown.Name = "_btnVolDown";
            this._btnVolDown.Size = new System.Drawing.Size(74, 23);
            this._btnVolDown.TabIndex = 4;
            this._btnVolDown.TabStop = false;
            this._btnVolDown.Text = "Volume -";
            this._btnVolDown.UseVisualStyleBackColor = true;
            this._btnVolDown.Click += new System.EventHandler(this.btnVolDown_Click);
            // 
            // _btnVolUp
            // 
            this._btnVolUp.Location = new System.Drawing.Point(12, 12);
            this._btnVolUp.Name = "_btnVolUp";
            this._btnVolUp.Size = new System.Drawing.Size(74, 23);
            this._btnVolUp.TabIndex = 5;
            this._btnVolUp.TabStop = false;
            this._btnVolUp.Text = "Volume +";
            this._btnVolUp.UseVisualStyleBackColor = true;
            this._btnVolUp.Click += new System.EventHandler(this.btnVolUp_Click);
            // 
            // _checkBox1
            // 
            this._checkBox1.AutoSize = true;
            this._checkBox1.CheckAlign = System.Drawing.ContentAlignment.BottomRight;
            this._checkBox1.Location = new System.Drawing.Point(26, 72);
            this._checkBox1.Name = "_checkBox1";
            this._checkBox1.Size = new System.Drawing.Size(58, 20);
            this._checkBox1.TabIndex = 8;
            this._checkBox1.TabStop = false;
            this._checkBox1.Text = "Mute";
            this._checkBox1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this._checkBox1.UseVisualStyleBackColor = true;
            this._checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // _tb1
            // 
            this._tb1.BackColor = System.Drawing.SystemColors.Control;
            this._tb1.Location = new System.Drawing.Point(134, 79);
            this._tb1.Name = "_tb1";
            this._tb1.Size = new System.Drawing.Size(43, 19);
            this._tb1.TabIndex = 0;
            this._tb1.Text = "0";
            this._tb1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _bknob
            // 
            this._bknob.BackColor = System.Drawing.SystemColors.Control;
            this._bknob.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._bknob.ForeColor = System.Drawing.Color.Black;
            this._bknob.ImeMode = System.Windows.Forms.ImeMode.On;
            this._bknob.LargeChange = 10;
            this._bknob.Location = new System.Drawing.Point(115, 5);
            this._bknob.Maximum = 100;
            this._bknob.Minimum = 0;
            this._bknob.Name = "_bknob";
            this._bknob.ShowLargeScale = true;
            this._bknob.ShowSmallScale = false;
            this._bknob.Size = new System.Drawing.Size(83, 83);
            this._bknob.SmallChange = 1;
            this._bknob.TabIndex = 2;
            this._bknob.Value = 0;
            this._bknob.ValueChanged += new VolumeControl.ValueChangedEventHandler(this.Bknob_ValueChanged);
            this._bknob.MouseClick += new System.Windows.Forms.MouseEventHandler(this._bknob_MouseClick);
            this._bknob.MouseDown += new System.Windows.Forms.MouseEventHandler(this._bknob_MouseDown);
            // 
            // VolMaxFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
            this.AutoSize = true;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(219, 101);
            this.Controls.Add(this._checkBox1);
            this.Controls.Add(this._btnVolUp);
            this.Controls.Add(this._btnVolDown);
            this.Controls.Add(this._tb1);
            this.Controls.Add(this._bknob);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VolMaxFrm";
            this.ShowIcon = false;
            this.Text = "Craver Systems VOLMAX";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region Buttons

        private void btnVolUp_Click(object sender, EventArgs e)
        {
            _audioEndpoint.AudioEndpointVolume.VolumeStepUp();
        }

        private void btnVolDown_Click(object sender, EventArgs e)
        {
            _audioEndpoint.AudioEndpointVolume.VolumeStepDown();
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
            _tb1.Text = volumeLevel.ToString();
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

            _tb1.Text = _bknob.Value.ToString();

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
            _audioEndpoint.AudioEndpointVolume.Mute = this._checkBox1.Checked;
        }

        private void _bknob_MouseClick(object sender, MouseEventArgs e)
        {
            var knob = new VolMax();
            // get current position of pointer             
            Point arrow = knob.GetKnobPosition();


            arrow.X = arrow.X - 3;
            arrow.Y = arrow.Y - 3;

            this.Cursor = new Cursor(Cursor.Current.Handle);

            var f = knob.GetValueFromPosition(Cursor.Position);

            //arrow.Offset(Cursor.Position);
            //Cursor.Position.Offset(arrow);
            //Cursor.Position = arrow;//Point(Cursor.Position.X - GetVolume()/100, Cursor.Position.Y - 50);

            //e.Location.X = left to right
            //e.Location.X = e.Location.X - GetVolume(); //top to bottom
            
            
        }

        private void _bknob_MouseDown(object sender, MouseEventArgs e)
        {
            this.Cursor = new Cursor(Cursor.Current.Handle);
            Cursor.Position = new Point(Cursor.Position.X - 50, Cursor.Position.Y - 50);
            Cursor.Clip = new Rectangle(this.Location, this.Size);
        }
        #endregion

        #region Thread related
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

    }
}
