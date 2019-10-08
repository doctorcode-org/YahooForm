using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Softcam.Properties;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic.CompilerServices;
using System.IO;

namespace Softcam
{
    public partial class YahooForm : Form
    {
        internal bool appActive = false;
        private const byte VK_TAB = 0x9;
        private const byte KEYEVENTF_KEYUP = 0x2;
        private const byte KEYEVENTF_KEYDOWN = 0;

        private Font _CaptionFont = new Font("2  Titr", 11, FontStyle.Bold, GraphicsUnit.Point);
        private Color _CaptionTextColor = Color.White;
        private Boolean _CaptionTextShadow = true;
        private Color _CaptionTextShadowColor = Color.Navy;
        private FormSkin _FormSkin = FormSkin.Royale;

        private bool _PersianKeybordLoading = true;
        private bool _ChangeEnterToTab = true;

        private Rectangle btnRect;
        private Rectangle CloseRect;
        private Rectangle MaxRect;
        private Rectangle ResRect;
        private Rectangle IconRect;

        private bool CloseIsOver = false;
        private bool MinIsOver = false;
        private bool MaxIsOver = false;

        public YahooForm()
        {
            base.SuspendLayout();
            base.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.Opaque, false);
            base.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(231)))), ((int)(((byte)(245)))));
            base.Name = "SoftcamYahooForm";
            this.Tag = "1";
            base.ResumeLayout(false);
            UpdateCaptiobBoxRec();
            
        }

        #region PublicFields
        /// <summary>
        /// فونت متن عنوان فرم
        /// </summary>
        [Category("Appearance"), Description("تعیین فونت متن عنوان فرم")]
        public Font CaptionFont
        {
            get { return _CaptionFont; }
            set { _CaptionFont = value; this.Refresh(); }
        }
        /// <summary>
        /// رنگ متن عنوان فرم
        /// </summary>
        [Category("Appearance"), Description("تعیین رنگ متن عنوان فرم")]
        public Color CaptionTextColor
        {
            get { return _CaptionTextColor; }
            set { _CaptionTextColor = value; this.Refresh(); }
        }
        /// <summary>
        /// سایه دار بودن متن عنوان فرم
        /// </summary>
        [Category("Appearance"), Description("تعیین سایه دار بودن متن عنوان فرم"), DefaultValue(true)]
        public Boolean CaptionTextShadow
        {
            get { return _CaptionTextShadow; }
            set { _CaptionTextShadow = value; this.Refresh(); }
        }
        /// <summary>
        /// رنگ سایه متن عنوان فرم
        /// </summary>
        [Category("Appearance"), Description("تعیین رنگ سایه متن عنوان فرم")]
        public Color CaptionTextShadowColor
        {
            get { return _CaptionTextShadowColor; }
            set { _CaptionTextShadowColor = value; this.Refresh(); }
        }
        [Category("Appearance"), Description("تغییر زبان کیبورد به فارسی هنگام بارگزاری فرم"), DefaultValue(true)]
        public Boolean PersianKeybordLoading
        {
            get { return _PersianKeybordLoading; }
            set { _PersianKeybordLoading = value; this.Refresh(); }
        }
        [Category("Appearance"), Description("تغییر کلید Enter به Tab"), DefaultValue(true)]
        public Boolean ChangeEnterToTab
        {
            get { return _ChangeEnterToTab; }
            set { _ChangeEnterToTab = value; this.Refresh(); }
        }
        [Description("تعیین پوسته فرم"), Category("Appearance")]
        public FormSkin FormSkin
        {
            get { return _FormSkin; }
            set { _FormSkin = value; Invalidate(true); }
        }
        #endregion

        protected override void WndProc(ref Message m)
        {
            try
            {
                switch (m.Msg)
                {
                    case (int)NativeMethods.WindowMessages.WM_ACTIVATEAPP:
                        {
                            appActive = (m.WParam == NativeMethods.TRUE);
                            this.Invalidate();
                            break;
                        }
                    case (int)NativeMethods.WindowMessages.WM_NCACTIVATE:
                        {
                            WmNCActivate(ref m);
                            break;
                        }
                    case (int)NativeMethods.WindowMessages.WM_NCPAINT:
                        {
                            WmNCPaint(ref m);
                            break;
                        }
                    case (int)NativeMethods.WindowMessages.WM_SETTEXT:
                        {
                            WmSetText(ref m);
                            break;
                        }
                    case (int)NativeMethods.WindowMessages.WM_NCHITTEST:
                        {
                            WmNCHitTest(ref m);
                            break;
                        }
                    case (int)NativeMethods.WindowMessages.WM_NCMOUSEMOVE:
                        {
                            WmNCMouseMove(ref m);
                            break;
                        }
                    case (int)NativeMethods.WindowMessages.WM_NCMOUSELEAVE:
                        {
                            WmNCMouseLeave(ref m);
                            break;
                        }
                    case (int)NativeMethods.WindowMessages.WM_NCLBUTTONDOWN:
                        {
                            WmNCLButtonDown(ref m);
                            break;
                        }
                    case 174: // ignore magic message number
                        {
                            break;
                        }
                    default:
                        {
                            base.WndProc(ref m);
                            break;
                        }
                }
            }
            catch (Exception SoftcamException)
            {
                if (HandledException != null)
                {
                    HandledException(SoftcamException);
                }
                else
                    throw SoftcamException;
            }
        }
        protected override void OnActivated(EventArgs e)
        {
            CloseIsOver = false;
            MinIsOver = false;
            MaxIsOver = false;
            appActive = true;
            base.OnActivated(e);
        }
        protected override void OnDeactivate(EventArgs e)
        {
            CloseIsOver = false;
            MinIsOver = false;
            MaxIsOver = false;
            appActive = false;
            base.OnDeactivate(e);
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (this.PersianKeybordLoading) 
                LoadLanguage(NativeMethods.Language.LANG_PERSIAN);

        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            Brush BorderBrushes = new TextureBrush(GetImageFromSkin(), new Rectangle(20, 30, 5, 5));
            using (BorderBrushes)
            {
                e.Graphics.FillRectangle(BorderBrushes, e.ClipRectangle);
            }
            base.OnPaintBackground(e);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyData == Keys.Enter && this.ChangeEnterToTab == true)
            {
                NativeMethods.keybd_event(VK_TAB, 0, KEYEVENTF_KEYDOWN, 0);
                NativeMethods.keybd_event(VK_TAB, 0, KEYEVENTF_KEYUP, 0);
            }
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateCaptiobBoxRec();
            CloseIsOver = false;
            MinIsOver = false;
            MaxIsOver = false;
            Refresh();
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            //NativeMethods.SetWindowTheme(this.Handle, "", "");
//#if !DEBUG
//            NativeMethods.DisableProcessWindowsGhosting();
//#endif
            base.OnHandleCreated(e);
        }

        private void WmNCActivate(ref Message msg)
        {
            appActive = (msg.WParam == NativeMethods.TRUE);
            if (WindowState == FormWindowState.Minimized)
                DefWndProc(ref msg);
            else
            {
                PaintNonClientArea(msg.HWnd, (IntPtr)1);    // repaint title bar
                msg.Result = NativeMethods.TRUE;        // allow to deactivate window
            }
        }

        private void WmNCPaint(ref Message msg)
        {
            PaintNonClientArea(msg.HWnd, msg.WParam);
            msg.Result = NativeMethods.TRUE;
        }
        private void PaintNonClientArea(IntPtr hWnd, IntPtr hRgn)
        {
            NativeMethods.RECT windowRect = new NativeMethods.RECT();
            if (NativeMethods.GetWindowRect(hWnd, ref windowRect) == 0)
                return;

            Rectangle bounds = new Rectangle(0, 0,
                windowRect.right - windowRect.left,
                windowRect.bottom - windowRect.top);

            if (bounds.Width == 0 || bounds.Height == 0)
                return;

            Region clipRegion = null;
            if (hRgn != (IntPtr)1) clipRegion = System.Drawing.Region.FromHrgn(hRgn);

            IntPtr hDC = NativeMethods.GetDCEx(hWnd, hRgn,
                (int)(NativeMethods.DCX.DCX_WINDOW | NativeMethods.DCX.DCX_INTERSECTRGN
                    | NativeMethods.DCX.DCX_CACHE | NativeMethods.DCX.DCX_CLIPSIBLINGS));

            if (hDC == IntPtr.Zero) return;
            try
            {
                using (Graphics g = Graphics.FromHdc(hDC))
                {
                    g.SetClip(bounds);
                    OnNonClientAreaPaint(new PaintEventArgs(g, bounds));
                }
            }
            finally
            {
                NativeMethods.ReleaseDC(this.Handle, hDC);
            }
        }
        protected void OnNonClientAreaPaint(PaintEventArgs e)
        {

            Image ImageSkin = GetImageFromSkin();
            Rectangle Rec = e.ClipRectangle;

            // assign clip region to exclude client area
            Region clipRegion = new Region(e.ClipRectangle);
            clipRegion.Exclude(new Rectangle(Rec.Left + 5, Rec.Top + 30, Rec.Width - 10, Rec.Height - 34));
            e.Graphics.Clip = clipRegion;

            #region CaptionBar
            Rectangle CaptionRight = new Rectangle((Rec.Width - 60), 0, 60, 30);
            e.Graphics.DrawImage(ImageSkin, CaptionRight, (ImageSkin.Width - 60), 0, 60, 28, GraphicsUnit.Pixel);

            Rectangle CaptionMiddle = new Rectangle(115, 0, (Rec.Width - 175), 30);
            e.Graphics.DrawImage(ImageSkin, CaptionMiddle, 110, 0, 5, 28, GraphicsUnit.Pixel);

            Rectangle CaptionLeft = new Rectangle(0, 0, 115, 30);
            e.Graphics.DrawImage(ImageSkin, CaptionLeft, 0, 0, 110, 28, GraphicsUnit.Pixel);
            #endregion

            #region Border
            Rectangle BodyRight = new Rectangle(Rec.Width - 60, 30, 60, (Rec.Height - 33));
            e.Graphics.DrawImage(ImageSkin, BodyRight, (ImageSkin.Width - 60), 38, 60, 5, GraphicsUnit.Pixel);
            Rectangle BodyLeft = new Rectangle(0, 30, 110, (Rec.Height - 33));
            e.Graphics.DrawImage(ImageSkin, BodyLeft, 0, 28, 110, 5, GraphicsUnit.Pixel);
            #endregion

            #region Buttom
            Rectangle ButtomRight = new Rectangle((Rec.Width - 60), (Rec.Height - 5), 60, 5);
            Rectangle ButtomMiddle = new Rectangle(105, (Rec.Height - 5), (Rec.Width - 165), 5);
            Rectangle ButtomLeft = new Rectangle(0, (Rec.Height - 5), 105, 5);
            e.Graphics.DrawImage(ImageSkin, ButtomRight, (ImageSkin.Width - 60), (ImageSkin.Height - 5), 60, 5, GraphicsUnit.Pixel);
            e.Graphics.DrawImage(ImageSkin, ButtomMiddle, 130, (ImageSkin.Height - 5), 5, 5, GraphicsUnit.Pixel);
            e.Graphics.DrawImage(ImageSkin, ButtomLeft, 0, (ImageSkin.Height - 5), 105, 5, GraphicsUnit.Pixel);
            #endregion

            //_____________________________ترسیم عنوان کادر____________________________________________________________
            
            #region PaintCaptionText
            if (!String.IsNullOrEmpty(this.Text))
            {
                using (StringFormat SF = new StringFormat())
                {
                    SF.Trimming = StringTrimming.EllipsisCharacter;
                    SF.FormatFlags = StringFormatFlags.NoWrap;
                    SF.LineAlignment = StringAlignment.Center;
                    
                    Rectangle textRect = new Rectangle(24, 5, this.Width - 120, 18);
                    if (this.ShowIcon == false) textRect = new Rectangle(5, 5, this.Width - 120, 18);

                    if (this.CaptionTextShadow == true)
                    {
                        Rectangle shadowRect = textRect;
                        shadowRect.Offset(1, 1);
                        using (Brush b = new SolidBrush(this.CaptionTextShadowColor))
                        {
                            e.Graphics.DrawString(this.Text, this.CaptionFont, b, shadowRect, SF);
                        }
                    }
                    using (Brush b = new SolidBrush(this.CaptionTextColor))
                    {
                        e.Graphics.DrawString(this.Text, this.CaptionFont, b, textRect, SF);
                    }
                }
            }
            #endregion

            //____________________________ترسیم آیکون کادر ____________________________________________________________
            if (this.ShowIcon == true && this.Icon != null)
            {
                Icon ico = new Icon(this.Icon, new Size(18, 18));
                e.Graphics.DrawIcon(ico, IconRect);
            }

            //____________________________ ترسیم  دکمه های عنوان ____________________________________________________________
            if (this.ControlBox == true)
            {
                Image btnCaption = GetCaptionImage();
                DrawButton(e.Graphics, btnCaption);
            }
            GC.Collect();
        }
      
        private void WmSetText(ref Message msg)
        {


            DefWndProc(ref msg);    // allow the system to receive the new window title
            PaintNonClientArea(msg.HWnd, (IntPtr)1);    // repaint title bar
        }

        private void WmNCHitTest(ref Message m)
        {
            int x = NativeMethods.Util.LOWORD(m.LParam);
            int y = NativeMethods.Util.HIWORD(m.LParam);
            NativeMethods.POINT pt = new NativeMethods.POINT(x, y);
            NativeMethods.ScreenToClient(new HandleRef(this, base.Handle), pt);

            Point clientPoint;

            if (this.IsMdiChild && this.WindowState == FormWindowState.Minimized)
            {
                clientPoint = new Point(pt.x, pt.y);
            }
            else
            {
                clientPoint = new Point(pt.x, pt.y + 30);
            }

            if (this.ControlBox == true && IsCaptionButton(clientPoint) == true)
            {
                m.Result = new System.IntPtr(GetHitTestCode(clientPoint));
            }
            else
            {
                CloseIsOver = false;
                MinIsOver = false;
                MaxIsOver = false;

                base.WndProc(ref m);
            }
        }
        public Point PointToWindow(Point screenPoint)
        {
            return new Point(screenPoint.X - Location.X, screenPoint.Y - Location.Y);
        }

        private void WmNCLButtonDown(ref Message msg)
        {
            Point pt = this.PointToWindow(new Point(msg.LParam.ToInt32()));
            NonClientMouseEventArgs args = new NonClientMouseEventArgs(
                MouseButtons.Left, 1, pt.X, pt.Y, 0, msg.WParam.ToInt32());
            OnNonClientMouseDown(args);
            if (!args.Handled)
            {
                DefWndProc(ref msg);
            }
            msg.Result = NativeMethods.TRUE;
        }
        protected void OnNonClientMouseDown(NonClientMouseEventArgs args)
        {
            if (args.Button != MouseButtons.Left) return;

            switch (args.HitTest)
            {
                case (int)NativeMethods.NCHITTEST.HTCLOSE:
                    {
                        this.Close();
                        args.Handled = true;
                        break;
                    }
                case (int)NativeMethods.NCHITTEST.HTMINBUTTON:
                    {

                        if (this.WindowState == FormWindowState.Minimized && this.IsMdiChild)
                            this.WindowState = FormWindowState.Normal;
                        else
                            this.WindowState = FormWindowState.Minimized;

                        args.Handled = true;
                        break;
                    }
                case (int)NativeMethods.NCHITTEST.HTMAXBUTTON:
                    {
                        this.WindowState = (this.WindowState == FormWindowState.Maximized) ? FormWindowState.Normal : FormWindowState.Maximized;
                        args.Handled = true;
                        break;
                    }
            }
        }

        private void WmNCMouseMove(ref Message msg)
        {
            DrawButton();
            msg.Result = IntPtr.Zero;
        }

        private void WmNCMouseLeave(ref Message m)
        {
            DrawButton();
        }
       
        private Image GetImageFromSkin()
        {
            if (appActive == false)
                return (Image)Resources.ResourceManager.GetObject("dis" + this.FormSkin.ToString());
            else
                return (Image)Resources.ResourceManager.GetObject(this.FormSkin.ToString());
        }
        private void UpdateCaptiobBoxRec()
        {
            btnRect = new Rectangle(this.Width - 100, 4, 93, 18);
            CloseRect = new Rectangle(btnRect.X + 50, 4, 43, 18);
            MaxRect = new Rectangle(btnRect.X + 25, 4, 25, 18);
            ResRect = new Rectangle(btnRect.X, 4, 25, 18);
            IconRect = new Rectangle(5, 5, 18, 18);
        }
        private Boolean IsCaptionButton(Point pt)
        {
            return btnRect.Contains(pt);
        }
        private int GetHitTestCode(Point pt)
        {
            if (CloseRect.Contains(pt))
            {
                SetButtonState(ButtonState.Close);
                return (int)NativeMethods.NCHITTEST.HTCLOSE;
            }
            else if (MaxRect.Contains(pt) && this.MaximizeBox == true)
            {
                SetButtonState(ButtonState.Max);
                return (int)NativeMethods.NCHITTEST.HTMAXBUTTON;
            }
            else if (ResRect.Contains(pt) && this.MinimizeBox == true)
            {
                SetButtonState(ButtonState.Min);
                return (int)NativeMethods.NCHITTEST.HTMINBUTTON;
            }
            else
            {
                SetButtonState(ButtonState.None);
                return (int)NativeMethods.NCHITTEST.HTCAPTION;
            }
        }
        private Image GetCaptionImage()
        {
            Image imgResult = Resources.N;

            #region Both
            if (this.MaximizeBox == true && this.MinimizeBox == true)
            {
                if (this.WindowState == FormWindowState.Maximized)
                {
                    if (CloseIsOver) imgResult = Resources.TCO;
                    else if (MaxIsOver) imgResult = Resources.TMO;
                    else if (MinIsOver) imgResult = Resources.TRO;
                    else imgResult = Resources.TN;
                }
                else
                {
                    if (CloseIsOver) imgResult = Resources.CO;
                    else if (MaxIsOver) imgResult = Resources.MO;
                    else if (MinIsOver) imgResult = Resources.RO;
                    else imgResult = Resources.N;
                }
            }
            #endregion

            #region OnlyMax
            else if (this.MaximizeBox == true && MinimizeBox == false)
            {
                if (this.WindowState == FormWindowState.Maximized)
                {
                    if (CloseIsOver) imgResult = Resources.TRDCO;
                    else if (MaxIsOver) imgResult = Resources.TRDMO;
                    else imgResult = Resources.TRD;
                }
                else
                {
                    if (CloseIsOver) imgResult = Resources.RDCO;
                    else if (MaxIsOver) imgResult = Resources.RDMO;
                    else imgResult = Resources.RD;
                }
            }
            #endregion

            #region OnlyMin
            else if (this.MaximizeBox == false && MinimizeBox == true)
            {
                if (this.WindowState == FormWindowState.Maximized)
                {
                    if (CloseIsOver) imgResult = Resources.TMDCO;
                    else if (MinIsOver) imgResult = Resources.TMDRO;
                    else imgResult = Resources.TMD;
                }
                else
                {
                    if (CloseIsOver) imgResult = Resources.MDCO;
                    else if (MinIsOver) imgResult = Resources.MDRO;
                    else imgResult = Resources.MD;
                }
            }
            #endregion

            #region OnlyClose
            else
            {
                if (CloseIsOver)
                    imgResult = Resources.SCO;
                else
                    imgResult = Resources.SC;
            }
            #endregion

            return imgResult;
        }
        private void SetButtonState(ButtonState st)
        {
            CloseIsOver = (st == ButtonState.Close);
            MinIsOver = (st == ButtonState.Min);
            MaxIsOver = (st == ButtonState.Max);

        }
        private void DrawButton()
        {
            if (this.ControlBox == false) 
                return;
            if (IsHandleCreated)
            {
                IntPtr hDC = NativeMethods.GetDCEx(this.Handle, (IntPtr)1,
                    (int)(NativeMethods.DCX.DCX_WINDOW | NativeMethods.DCX.DCX_INTERSECTRGN
                        | NativeMethods.DCX.DCX_CACHE | NativeMethods.DCX.DCX_CLIPSIBLINGS));

                if (hDC != IntPtr.Zero)
                {
                    using (Graphics g = Graphics.FromHdc(hDC))
                    {
                        DrawButton(g, GetCaptionImage());
                    }
                }
                NativeMethods.ReleaseDC(this.Handle, hDC);
            }
        }
        private void DrawButton(Graphics g, Image img)
        {
            if (this.ControlBox == false) 
                return;

            g.DrawImage(img, btnRect);
        }

        private Boolean LoadLanguage(string strId)
        {
            try
            {
                string pwszKLID = new string('\0', 9);
                string ID = strId + "\0";

                NativeMethods. GetKeyboardLayoutName(ref pwszKLID);
                if (pwszKLID == (strId + "\0"))
                {
                    return true;
                }
                pwszKLID = new string('\0', 9);
                if (Conversions.ToString(NativeMethods.LoadKeyboardLayout(ref ID, 1)) == null)
                {
                    return false;
                }
                pwszKLID = new string('\0', 9);
                NativeMethods.GetKeyboardLayoutName(ref pwszKLID);
                return (pwszKLID == ID);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [Category("Behavior"), Description("Unhandled Exception")]
        public event ExceptionEventHandler HandledException;

       
        
    }
    
    [Serializable]
    [ComVisible(true)]
    public delegate void ExceptionEventHandler(Exception ex);

    /// <summary>
    /// پوسته فرم
    /// </summary>
    public enum FormSkin
    {
        Black,
        Graffity,
        Green,
        Purple,
        Royale,
        RubyRed,
        Silver,
        SkyBlue,
        TwinklePink,
        VioletFlame,
        Wood
    }

    public enum SoftcamCursors
    {
        aero_arrow,
        aero_busy,
        aero_link,
        aero_working
    }


    public enum ButtonState
    {
        None = 0,
        Close = 1,
        Min = 2,
        Max = 3
    }
#if !DEBUGFORM
    [DebuggerStepThrough]
#endif
    public class NonClientMouseEventArgs : MouseEventArgs
    {
        private int _hitTest;
        private bool _handled;

        public NonClientMouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta, int hitTest)
            : base(button, clicks, x, y, delta)
        {
            _hitTest = hitTest;
        }

        public int HitTest
        {
            get { return _hitTest; }
            set { _hitTest = value; }
        }

        public bool Handled
        {
            get { return _handled; }
            set { _handled = value; }
        }

    }

   
}
