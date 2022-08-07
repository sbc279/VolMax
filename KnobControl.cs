using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using KnobControl;

namespace VolumeControl
{

    // A delegate type for hooking up ValueChanged notifications. 
    public delegate void ValueChangedEventHandler(object sender);

    /// <summary>
    /// Summary description for KnobControl.
    /// </summary>
    public class KnobControl : UserControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Container _components = null;

        private int _minimum = 0;
        private int _maximum = 25;
        private int _largeChange = 5;
        private int _smallChange = 1;
        private bool _showSmallScale = false;
        private bool _showLargeScale = true;
        private bool _isFocused = false;


        private int _value = 0;
        private bool _isKnobRotating = false;
        private Rectangle _rKnob;
        private Point _pKnob;
        private Rectangle _rScale;
        private Pen _dottedPen;

        Brush _bKnob;
        Brush _bKnobPoint;

        //-------------------------------------------------------
        // declare Off screen image and Offscreen graphics       
        //-------------------------------------------------------
        private Image _offScreenImage;
        private Graphics _gOffScreen;


        //-------------------------------------------------------
        // An event that clients can use to be notified whenever 
        // the Value is Changed.                                 
        //-------------------------------------------------------
        public event ValueChangedEventHandler ValueChanged;

        //-------------------------------------------------------
        // Invoke the ValueChanged event; called  when value     
        // is changed                                            
        //-------------------------------------------------------
        protected virtual void OnValueChanged(object sender)
        {
            if (ValueChanged != null)
                ValueChanged(sender);
        }

        /// <summary>
        /// Shows Small Scale marking.
        /// </summary>
        public bool ShowSmallScale
        {
            get { return _showSmallScale; }
            set
            {
                _showSmallScale = value;
                // need to redraw 
                Refresh();
            }
        }

        /// <summary>
        /// Shows Large Scale marking
        /// </summary>
        public bool ShowLargeScale
        {
            get { return _showLargeScale; }
            set
            {
                _showLargeScale = value;
                // need to redraw
                Refresh();
            }
        }

        /// <summary>
        /// Minimum Value for knob Control
        /// </summary>
        /// 
        public int Minimum
        {
            get { return _minimum; }
            set { _minimum = value; }
        }

        /// <summary>
        /// Maximum value for knob control
        /// </summary>
        public int Maximum
        {
            get { return _maximum; }
            set { _maximum = value; }
        }

        /// <summary>
        /// value set for large change
        /// </summary>
        public int LargeChange
        {
            get { return _largeChange; }
            set
            {
                _largeChange = value;
                Refresh();
            }
        }

        /// <summary>
        /// value set for small change.
        /// </summary>
        public int SmallChange
        {
            get { return _smallChange; }
            set
            {
                _smallChange = value;
                Refresh();
            }
        }

        /// <summary>
        /// Current Value of knob control
        /// </summary>
        public int Value
        {
            get { return _value; }
            set
            {

                _value = value;
                // need to redraw 
                Refresh();
                // call delegate  
                OnValueChanged(this);
            }
        }

        public KnobControl()
        {

            // This call is required by the Windows.Forms Form Designer.
            _dottedPen = new Pen(Utility.GetDarkColor(Color.DarkGray, 40));
            _dottedPen.DashStyle = DashStyle.Dash;
            _dottedPen.DashCap = DashCap.Flat;

            InitializeComponent();
            SetDimensions();


            // TODO: Add any initialization after the InitForm call

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            // Set background color of Image...            
            _gOffScreen.Clear(BackColor);
            // Fill knob Background to give knob effect....
            _gOffScreen.FillEllipse(_bKnob, _rKnob);
            // Set antialias effect on                     
            _gOffScreen.SmoothingMode = SmoothingMode.AntiAlias;
            // Draw border of knob                         
            _gOffScreen.DrawEllipse(new Pen(BackColor), _rKnob);

            //if control is focused 
            if (_isFocused)
            {
                _gOffScreen.DrawEllipse(_dottedPen, _rKnob);
            }

            // get current position of pointer             
            Point arrow = GetKnobPosition();

            // Draw pointer arrow that shows knob position 
            Utility.DrawInsetCircle(ref _gOffScreen, new Rectangle(arrow.X - 3, arrow.Y - 3, 6, 6), new Pen(Color.DarkGray));

            //---------------------------------------------
            // darw small and large scale                  
            //---------------------------------------------
            if (_showSmallScale)
            {
                for (int i = Minimum; i <= Maximum; i += _smallChange)
                {
                    _gOffScreen.DrawLine(new Pen(ForeColor), GetMarkerPoint(0, i), GetMarkerPoint(3, i));
                }
            }
            if (_showLargeScale)
            {
                for (int i = Minimum; i <= Maximum; i += _largeChange)
                {
                    _gOffScreen.DrawLine(new Pen(ForeColor), GetMarkerPoint(0, i), GetMarkerPoint(5, i));
                }
            }

            // Drawimage on screen                    
            g.DrawImage(_offScreenImage, 0, 0);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Empty To avoid Flickring due do background Drawing.
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (Utility.IsPointinRectangle(new Point(e.X, e.Y), _rKnob))
            {
                // Start Rotation of knob         
                _isKnobRotating = true;
            }

        }

        protected override bool IsInputKey(Keys key)
        {
            //switch (key)
            //{
            //    case Keys.Up:
            //    case Keys.Down:
            //    case Keys.Right:
            //    case Keys.Left:
            //        return true;
            //}
            return base.IsInputKey(key);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            //// Stop rotation                   
            _isKnobRotating = false;
            if (Utility.IsPointinRectangle(new Point(e.X, e.Y), _rKnob))
            {
                // get value                   
                Value = GetValueFromPosition(new Point(e.X, e.Y));
            }
            Cursor = Cursors.Default;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            //--------------------------------------
            //  Following Handles Knob Rotating     
            //--------------------------------------
            if (_isKnobRotating == true)
            {
                Cursor = Cursors.Hand;
                Point p = new Point(e.X, e.Y);
                int posVal = GetValueFromPosition(p);
                Value = posVal;
            }

        }

        protected override void OnEnter(EventArgs e)
        {
            _isFocused = true;
            Refresh();
            base.OnEnter(new EventArgs());
        }

        protected override void OnLeave(EventArgs e)
        {
            _isFocused = false;
            Refresh();
            base.OnLeave(new EventArgs());
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {

            //--------------------------------------------------------
            // Handles knob rotation with up,down,left and right keys 
            //--------------------------------------------------------
            //if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Right)
            //{
            //    if (_value < Maximum) Value = _value + 1;
            //    Refresh();
            //}
            //else if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Left)
            //{
            //    if (_value > Minimum) Value = _value - 1;
            //    Refresh();
            //}
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_components != null)
                {
                    _components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code
        private void InitializeComponent()
        {
            // 
            // KnobControl
            // 
            this.ImeMode = System.Windows.Forms.ImeMode.On;
            this.Name = "KnobControl";
            this.Resize += new System.EventHandler(this.KnobControl_Resize);

        }
        #endregion

        private void SetDimensions()
        {
            // get smaller from height and width
            int size = Width;
            if (Width > Height)
            {
                size = Height;
            }
            // allow 10% gap on all side to determine size of knob    
            _rKnob = new Rectangle((int)(size * 0.10), (int)(size * 0.10), (int)(size * 0.80), (int)(size * 0.80));

            _rScale = new Rectangle(2, 2, size - 4, size - 4);

            _pKnob = new Point(_rKnob.X + _rKnob.Width / 2, _rKnob.Y + _rKnob.Height / 2);
            // create offscreen image                                 
            _offScreenImage = new Bitmap(Width, Height);
            // create offscreen graphics                              
            _gOffScreen = Graphics.FromImage(_offScreenImage);

            // create LinearGradientBrush for creating knob            
            _bKnob = new LinearGradientBrush(
                _rKnob, Utility.GetLightColor(Color.LightSteelBlue, 55), Utility.GetDarkColor(Color.LightGray, 55), LinearGradientMode.ForwardDiagonal);
            // create LinearGradientBrush for knobPoint                
            _bKnobPoint = new LinearGradientBrush(
                _rKnob, Utility.GetLightColor(BackColor, 55), Utility.GetDarkColor(BackColor, 55), LinearGradientMode.ForwardDiagonal);
        }

        private void KnobControl_Resize(object sender, EventArgs e)
        {
            SetDimensions();
            Refresh();
        }

        /// <summary>
        /// gets knob position that is to be drawn on control.
        /// </summary>
        /// <returns>Point that describes current knob position</returns>
        public Point GetKnobPosition()
        {
            double degree = 270 * Value / (Maximum - Minimum);
            degree = (degree + 135) * Math.PI / 180;

            Point pos = new Point(0, 0);
            pos.X = (int)(Math.Cos(degree) * (_rKnob.Width / 2 - 10) + _rKnob.X + _rKnob.Width / 2);
            pos.Y = (int)(Math.Sin(degree) * (_rKnob.Width / 2 - 10) + _rKnob.Y + _rKnob.Height / 2);
            return pos;
        }

        /// <summary>
        /// gets marker point required to draw scale marker.
        /// </summary>
        /// <param name="length">distance from center</param>
        /// <param name="value">value that is to be marked</param>
        /// <returns>Point that describes marker position</returns>
        public Point GetMarkerPoint(int length, int value)
        {
            double degree = 270 * value / (Maximum - Minimum);
            degree = (degree + 135) * Math.PI / 180;

            Point pos = new Point(0, 0);
            pos.X = (int)(Math.Cos(degree) * (_rKnob.Width / 2 - length + 7) + _rKnob.X + _rKnob.Width / 2);
            pos.Y = (int)(Math.Sin(degree) * (_rKnob.Width / 2 - length + 7) + _rKnob.Y + _rKnob.Height / 2);
            return pos;
        }

        /// <summary>
        /// converts geomatrical position in to value..
        /// </summary>
        /// <param name="p">Point that is to be converted</param>
        /// <returns>Value derived from position</returns>
        public int GetValueFromPosition(Point p)
        {
            double degree = 0.0;
            int v = 0;
            if (p.X <= _pKnob.X)
            {
                degree = (double)(_pKnob.Y - p.Y) / (double)(_pKnob.X - p.X);
                degree = Math.Atan(degree);
                degree = (degree) * (180 / Math.PI) + 45;
                v = (int)(degree * (Maximum - Minimum) / 270);

            }
            else if (p.X > _pKnob.X)
            {
                degree = (double)(p.Y - _pKnob.Y) / (double)(p.X - _pKnob.X);
                degree = Math.Atan(degree);
                degree = 225 + (degree) * (180 / Math.PI);
                v = (int)(degree * (Maximum - Minimum) / 270);

            }
            if (v > Maximum) v = Maximum;
            if (v < Minimum) v = Minimum;

            return v;

        }
        
    }
}
