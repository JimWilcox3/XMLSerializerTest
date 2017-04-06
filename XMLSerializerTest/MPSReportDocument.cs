using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;

namespace XMLSerializerTest
{
    [Serializable]
    public class Margins
    {
        public Margins()
        {
        }

        public Margins(int Bottom, int Left, int Right, int Top)
        {
            this.Bottom = Bottom;
            this.Left = Left;
            this.Right = Right;
            this.Top = Top;
        }
        public int Bottom { get; set; }
        public int Left { get; set; }
        public int Right { get; set; }
        public int Top { get; set; }
    }

    [Android.Runtime.Preserve(AllMembers = true)]
    [Serializable]
    public class MPSReportDocument : INotifyPropertyChanged, IDisposable
    {

        #region " -- IDisposable -- "
        [XmlIgnore()]
        public bool Disposed = false;        // To detect redundant calls

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!this.Disposed)
            {
                if (disposing)
                {
                    try
                    {
                        foreach (MPSReportPage curr in this.Pages)
                            curr.Dispose();

                        this.Pages.Clear();

                        foreach (MPSReportDocument curr in this.SubReports)
                            curr.Dispose();

                        this.SubReports.Clear();

                        //if (this.DefaultFont != null )
                        //{
                        //    this.DefaultFont.Dispose();
                        //    this.DefaultFont = null;
                        //}
                    }
                    catch (Exception ex)
                    {
                        Debug.Print("Dispose Document:");
                        Debug.Print(ex.ToString());
                    }
                }

                // TODO: free your own state (unmanaged objects).
                // TODO: set large fields to null.
                this.Disposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~MPSReportDocument()
        {
            Dispose(false);
        }

        #endregion

        public MPSReportDocument()
        {
            this.Pages = new List<MPSReportPage>();
            this.SubReports = new List<MPSReportDocument>();
        }

        private static XmlSerializer ReportSerializer = null;

        public static void NewSerializer()
        {
            Type[] extraTypes = new Type[7];
            extraTypes[0] = typeof(MPSReportPage);
            extraTypes[1] = typeof(MPSReportField);
            extraTypes[2] = typeof(MPSTextField);
            extraTypes[3] = typeof(MPSImageField);
            extraTypes[4] = typeof(Margins);
            extraTypes[5] = typeof(Size);
            extraTypes[6] = typeof(System.Drawing.Point);

            try
            {
                ReportSerializer = new XmlSerializer(typeof(MPSReportDocument), extraTypes);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if ((this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        private string _Name;
        public string Name
        {
            get
            {
                return _Name;

            }
            set
            {
                _Name = value;
                OnPropertyChanged("Name");
            }
        }

        public List<MPSReportPage> Pages { get; set; }

        public List<MPSReportDocument> SubReports { get; set; }

        public string SMTPServer { get; set; }

        public string EmailFrom { get; set; }

        private string _Font;

        public string DefaultFont
        {
            get
            {
                return _Font;
            }
            set
            {
                _Font = value;
                OnPropertyChanged("DefaultFont");
            }
        }

        public static MPSReportDocument Read(System.IO.Stream ipf)
        {

            if (ReportSerializer == null)
                NewSerializer();

            MPSReportDocument Document = (MPSReportDocument)ReportSerializer.Deserialize(ipf);

            return Document;
        }

        string ReportSearchName = "";

        public MPSReportDocument GetSubReport(string ReportName)
        {

            ReportSearchName = ReportName;

            return this.SubReports.Find(ReportSearchPredicate);

        }

        private bool ReportSearchPredicate(MPSReportDocument Doc)
        {
            if (String.Compare(Doc.Name, ReportSearchName, true) == 0)
                return true;
            else
                return false;
        }

        public static SizeF MeasureString(string Text, string FontString, int Width, float ps = 1)
        {

            SizeF Result;

            TextView view = new TextView(global::Android.App.Application.Context);

            view.SetWidth(Width);

            string[] Font = FontString.Split(',');
            float fontSize = float.Parse(Font[1].Replace("pt", "").Trim());
            fontSize *= ps;
            view.SetTextSize(Android.Util.ComplexUnitType.Sp, fontSize);

            view.Text = Text;

            int widthMeasureSpec = Android.Views.View.MeasureSpec.MakeMeasureSpec(
              Width, MeasureSpecMode.AtMost);
            int heightMeasureSpec = Android.Views.View.MeasureSpec.MakeMeasureSpec(
                0, MeasureSpecMode.Unspecified);

            view.Measure(widthMeasureSpec, heightMeasureSpec);

            Result = new SizeF(view.MeasuredWidth, view.MeasuredHeight);

            view.Dispose();

            return Result;

        }
    }

    [Serializable()]
    public class MPSReportPage : INotifyPropertyChanged, ICloneable, IDisposable
    {

        public MPSReportPage()
        {
            Fields = new List<MPSReportField>();
        }

        #region " -- IDisposable -- "

        [XmlIgnore()]
        public bool Disposed = false;        // To detect redundant calls

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!this.Disposed)
            {
                if (disposing)
                {
                    try
                    {
                        // TODO: free other state (managed objects).
                        foreach (MPSReportField curr in this.Fields)
                            curr.Dispose();

                        this.Fields.Clear();
                    }
                    catch (Exception)
                    {
                        //Debug.Print("Dispose Page:")
                        //Debug.Print(ex.ToString)
                    }
                }

                // TODO: free your own state (unmanaged objects).
                // TODO: set large fields to null.
                this.Disposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~MPSReportPage()
        {
            Dispose(false);
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if ((this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                OnPropertyChanged("Name");
            }
        }

        public object Clone()
        {
            MPSReportPage Page = (MPSReportPage)this.MemberwiseClone();
            Page.Fields = new List<MPSReportField>();
            foreach (MPSReportField Field in this.Fields)
                Page.Fields.Add((MPSReportField)Field.Clone());
            return Page;
        }

        private RelativeLayout _Layout = null;
        [XmlIgnore()]
        public RelativeLayout Layout
        {
            get
            {
                return _Layout;
            }
            set
            {
                _Layout = value;
                OnPropertyChanged("Layout");
            }
        }

        private Size _Size = new Size(850, 1100);
        public Size Size
        {
            get
            {
                return _Size;
            }
            set
            {
                _Size = value;
                OnPropertyChanged("Size");
            }
        }

        private Margins _Margins = new Margins(50, 50, 50, 50);
        public Margins Margins
        {
            get
            {
                return _Margins;
            }
            set
            {
                _Margins = value;
                OnPropertyChanged("Margins");
            }
        }

        public List<MPSReportField> Fields;

        public void Paint(LinearLayout Parent, Context context, float ps = 1)
        {
            if (this.Layout == null)
            {
                this.Layout = new RelativeLayout(context);
                Layout.SetMinimumHeight((int)(Size.Height * ps));
                Layout.SetMinimumWidth((int)(Size.Width * ps));
                Layout.SetBackgroundColor(Android.Graphics.Color.White);
                Parent.AddView(Layout);
            }

            foreach (MPSReportField Curr in this.Fields)
            {
                Curr.Paint(Layout, context, ps);
            }
        }

        private string SearchFieldName;

        public void SetTextField(string FieldName, string Text)
        {
            foreach (MPSReportField field in this.Fields.Where(f => f.Name == FieldName))
            {

                if (field.GetType() == typeof(MPSTextField))
                {
                    MPSTextField tfield = (MPSTextField)field;
                    tfield.Text = Text;
                }
            }
        }

        public MPSReportField GetField(string FieldName)
        {
            SearchFieldName = FieldName;

            return this.Fields.Find(FieldSearchPredicate);

        }

        private bool FieldSearchPredicate(MPSReportField Field)
        {
            if (String.Compare(Field.Name, SearchFieldName, true) == 0)
                return true;
            else
                return false;
        }

    }

    [Serializable()]
    public class MPSReportFieldBorders : INotifyPropertyChanged, IDisposable
    {

        #region " -- IDisposable -- "

        [XmlIgnore()]
        public bool Disposed = false;        // To detect redundant calls

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!this.Disposed)
            {
                if (disposing)
                {
                    // TODO: free other state (managed objects).
                }

                // TODO: free your own state (unmanaged objects).
                // TODO: set large fields to null.
                this.Disposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~MPSReportFieldBorders()
        {
            Dispose(false);
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if ((this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public MPSReportFieldBorders()
        {
        }

        public MPSReportFieldBorders(int Top, int Bottom, int Left, int Right)
        {
            this.Top = Top;
            this.Bottom = Bottom;
            this.Left = Left;
            this.Right = Right;
        }

        public MPSReportFieldBorders Clone()
        {
            return (MPSReportFieldBorders)this.MemberwiseClone();
        }

        private int _Top;
        public int Top
        {
            get
            {
                return _Top;
            }
            set
            {
                _Top = value;
                OnPropertyChanged("Top");
            }
        }

        private int _Bottom;
        public int Bottom
        {
            get
            {
                return _Bottom;
            }
            set
            {
                _Bottom = value;
                OnPropertyChanged("Bottom");
            }
        }

        private int _Left;
        public int Left
        {
            get
            {
                return _Left;
            }
            set
            {
                _Left = value;
                OnPropertyChanged("Left");
            }
        }

        private int _Right;
        public int Right
        {
            get
            {
                return _Right;
            }
            set
            {
                _Right = value;
                OnPropertyChanged("Right");
            }
        }

    }

    [Serializable()]
    public class MPSReportFieldPadding : INotifyPropertyChanged, IDisposable
    {

        #region " -- IDisposable -- "

        [XmlIgnore()]
        public bool Disposed = false;        // To detect redundant calls

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!this.Disposed)
            {
                if (disposing)
                {
                    // TODO: free other state (managed objects).
                }

                // TODO: free your own state (unmanaged objects).
                // TODO: set large fields to null.
                this.Disposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~MPSReportFieldPadding()
        {
            Dispose(false);
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if ((this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public MPSReportFieldPadding()
        {
        }

        public MPSReportFieldPadding(int Top, int Bottom, int Left, int Right)
        {
            this.Top = Top;
            this.Bottom = Bottom;
            this.Left = Left;
            this.Right = Right;
        }

        public MPSReportFieldPadding Clone()
        {
            return (MPSReportFieldPadding)this.MemberwiseClone();
        }

        private int _Top;
        public int Top
        {
            get
            {
                return _Top;
            }
            set
            {
                _Top = value;
                OnPropertyChanged("Top");
            }
        }

        private int _Bottom;
        public int Bottom
        {
            get
            {
                return _Bottom;
            }
            set
            {
                _Bottom = value;
                OnPropertyChanged("Bottom");
            }
        }

        private int _Left;
        public int Left
        {
            get
            {
                return _Left;
            }
            set
            {
                _Left = value;
                OnPropertyChanged("Left");
            }
        }

        private int _Right;
        public int Right
        {
            get
            {
                return _Right;
            }
            set
            {
                _Right = value;
                OnPropertyChanged("Right");
            }
        }


    }

    [Serializable()]
    public class MPSReportField : INotifyPropertyChanged, IDisposable
    {

        #region " -- IDisposable -- "

        [XmlIgnore()]
        public bool Disposed = false;        // To detect redundant calls

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!this.Disposed)
            {
                if (disposing)
                {
                    // TODO: free other state (managed objects).
                    if (View != null)
                        View.Dispose();
                }

                // TODO: free your own state (unmanaged objects).
                // TODO: set large fields to null.
                this.Disposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~MPSReportField()
        {
            Dispose(false);
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if ((this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public MPSReportField()
        {
        }

        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }

        private View _View;
        [XmlIgnore()]
        public View View
        {
            get
            {
                return _View;
            }
            set
            {
                _View = value;
                OnPropertyChanged("View");
            }
        }

        private string _Alignment;
        public string Alignment
        {
            get
            {
                return _Alignment;
            }
            set
            {
                _Alignment = value;
                OnPropertyChanged("Alignment");
            }
        }

        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                OnPropertyChanged("Name");
            }
        }

        private string _LineAlignment;
        public string LineAlignment
        {
            get
            {
                return _LineAlignment;
            }
            set
            {
                _LineAlignment = value;
                OnPropertyChanged("LineAlignment");
            }
        }

        private string _BackColor = "Transparent";
        public string BackColor
        {
            get
            {
                return _BackColor;
            }
            set
            {
                _BackColor = value;
                OnPropertyChanged("BackColor");
            }
        }

        public virtual void Paint(RelativeLayout Parent, Context Context, Single ps = 1) //Implements IMPSReportField.Paint
        {
            if (!this.Visible)
                return;
        }

        private Size _Size = new Size(50, 20);
        public Size Size
        {
            get
            {
                return _Size;
            }
            set
            {
                bool DoEvent = true;
                if (value.Height == _Size.Height && value.Width == _Size.Width)
                    DoEvent = false;
                _Size = value;
                if (DoEvent)
                    OnPropertyChanged("Size");
            }
        }

        private bool _Visible = true;
        public bool Visible
        {
            get
            {
                return _Visible;
            }
            set
            {
                _Visible = value;
                OnPropertyChanged("Visible");
            }
        }

        private System.Drawing.Point _Location;
        public System.Drawing.Point Location
        {
            get
            {
                return _Location;
            }
            set
            {
                _Location = value;
                OnPropertyChanged("Location");
            }
        }

        MPSReportFieldBorders _Borders = new MPSReportFieldBorders(0, 0, 0, 0);
        [TypeConverter(typeof(MPSReportFieldBorderConverter))]
        public MPSReportFieldBorders Borders
        {
            get
            {
                return _Borders;
            }
            set
            {
                if (_Borders != null)
                    _Borders.Dispose();
                _Borders = value;
                OnPropertyChanged("Borders");
            }
        }

        MPSReportFieldPadding _Padding = new MPSReportFieldPadding(0, 0, 0, 0);
        [TypeConverter(typeof(MPSReportFieldPaddingConverter))]
        public MPSReportFieldPadding Padding
        {
            get
            {
                return _Padding;
            }
            set
            {
                if (_Padding != null)
                    _Padding.Dispose();
                _Padding = value;
                OnPropertyChanged("Padding");
            }
        }

        public virtual Rectangle getRegion()  //Implements IMPSReportField.getRegion
        {
            return new Rectangle(this.Location.X, this.Location.Y, this.Size.Width, this.Size.Height);
        }

        public class ReportFieldComparer : IComparer<MPSReportField>
        {
            public int Compare(MPSReportField x, MPSReportField y)
            {
                if (x.Location.Y < y.Location.Y)
                    return -1;
                else if (x.Location.Y > y.Location.Y)
                    return 1;
                else
                {
                    if (x.Location.X < y.Location.X)
                        return -1;
                    else if (x.Location.X > y.Location.X)
                        return 1;
                }
                return 0;
            }
        }

        public class MPSReportFieldBorderConverter : ExpandableObjectConverter
        {
            public override bool CanConvertTo(
                                  ITypeDescriptorContext context,
                                  Type destinationType)
            {
                if ((destinationType.GetType() == typeof(MPSReportFieldBorders)))
                {
                    return true;
                }
                return base.CanConvertFrom(context, destinationType);
            }

            public override object ConvertTo(
                                  ITypeDescriptorContext context,
                                  CultureInfo culture,
                                   Object value,
                                  Type destinationType)
            {
                if (destinationType.GetType() == typeof(System.String)
                    && value.GetType() == typeof(MPSReportFieldBorders))
                {
                    MPSReportFieldBorders so = (MPSReportFieldBorders)value;

                    return so.Bottom + ", " + so.Left + ", " + so.Right + ", " + so.Top;
                }
                return base.ConvertTo(context, culture, value, destinationType);
            }

            public override bool CanConvertFrom(
                               ITypeDescriptorContext context,
                                System.Type sourceType)
            {
                if ((sourceType.GetType() == typeof(String)))
                {
                    return true;
                }
                return base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(
                                  ITypeDescriptorContext context,
                                  CultureInfo culture,
                                  object value)
            {

                if (value.GetType() == typeof(String))
                {
                    try
                    {
                        string[] s = value.ToString().Split(',');

                        if ((s.GetUpperBound(0) > 2))
                        {
                            string bottom = s[0];
                            string left = s[1];
                            string right = s[2];
                            string top = s[3];

                            MPSReportFieldBorders so = new MPSReportFieldBorders();
                            so.Top = int.Parse(top);
                            so.Bottom = int.Parse(bottom);
                            so.Left = int.Parse(left);
                            so.Right = int.Parse(right);

                            return so;
                        }
                    }
                    catch
                    {
                        throw new ArgumentException(
                            "Can not convert '" + value.ToString() + "' to type MPRReportFieldBorders");

                    }
                }
                return base.ConvertFrom(context, culture, value);
            }

        }

        public class MPSReportFieldPaddingConverter : ExpandableObjectConverter
        {
            public override bool CanConvertTo(
                                  ITypeDescriptorContext context,
                                  Type destinationType)
            {
                if ((destinationType.GetType() == typeof(MPSReportFieldPadding)))
                {
                    return true;
                }
                return base.CanConvertFrom(context, destinationType);
            }

            public override object ConvertTo(
                                  ITypeDescriptorContext context,
                                  CultureInfo culture,
                                  object value,
                                  Type destinationType)
            {
                if ((destinationType.GetType() == typeof(System.String)
                    && value.GetType() == typeof(MPSReportFieldPadding)))
                {
                    MPSReportFieldPadding so = (MPSReportFieldPadding)value;

                    return so.Bottom + ", " + so.Left + ", " + so.Right + ", " + so.Top;
                }
                return base.ConvertTo(context, culture, value, destinationType);
            }

            public override bool CanConvertFrom(
                               ITypeDescriptorContext context,
                               Type sourceType)
            {
                if ((sourceType.GetType() == typeof(String)))
                {
                    return true;
                }
                return base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(
                                  ITypeDescriptorContext context,
                                   CultureInfo culture,
                                  object value)
            {

                if (value.GetType() == typeof(string))
                {
                    try
                    {
                        string[] s = value.ToString().Split(',');

                        if ((s.GetUpperBound(0) > 2))
                        {
                            string bottom = s[0];
                            string left = s[1];
                            string right = s[2];
                            string top = s[3];

                            MPSReportFieldPadding so = new MPSReportFieldPadding();
                            so.Top = int.Parse(top);
                            so.Bottom = int.Parse(bottom);
                            so.Left = int.Parse(left);
                            so.Right = int.Parse(right);

                            return so;
                        }
                    }
                    catch
                    {
                        throw new ArgumentException("Can not convert '" + value.ToString() + "' to type MPRReportFieldPadding");

                    }
                }
                return base.ConvertFrom(context, culture, value);
            }

        }

    }

    [Serializable()]
    public class MPSTextField : MPSReportField
    {
        protected override void Dispose(bool disposing)
        {
            if (!this.Disposed)
            {
                if (disposing)
                {
                }
                base.Dispose(disposing);
            }
        }


        public MPSTextField(string Font)
        {
            this.Font = Font;
        }

        public MPSTextField()
        {
            //this.Font = New Font("Tahoma", 10, FontStyle.Regular)
        }

        public override object Clone()
        {
            MPSTextField ret = (MPSTextField)this.MemberwiseClone();
            ret.Borders = this.Borders.Clone();
            ret.Padding = this.Padding.Clone();
            return ret;
        }

        public override void Paint(RelativeLayout Parent, Context Context, Single ps = 1)
        {
            if (this.View == null)
            {
                this.View = new BorderedTextView(Context);
                Parent.AddView(View);
            }

            base.Paint(Parent, Context, ps);

            BorderedTextView textView = (BorderedTextView)View;

            textView.Text = Text;
            textView.SetHeight((int)(Size.Height * ps));
            textView.SetWidth((int)(Size.Width * ps));
            textView.SetX((int)(Location.X * ps));
            textView.SetY((int)(Location.Y * ps));

            string[] Font = this.Font.Split(',');

            float fontSize = float.Parse(Font[1].Replace("pt", "").Trim());
            textView.SetTextSize(Android.Util.ComplexUnitType.Sp, fontSize);
            textView.SetTextColor(Android.Graphics.Color.ParseColor(ForeColor));

            Android.Views.GravityFlags alignment = Android.Views.GravityFlags.NoGravity;

            switch (Alignment)
            {
                case "Near":
                    alignment = Android.Views.GravityFlags.Left;
                    break;
                case "Center":
                    alignment = Android.Views.GravityFlags.CenterHorizontal;
                    break;
                case "Far":
                    alignment = Android.Views.GravityFlags.Right;
                    break;
            }

            switch (LineAlignment)
            {
                case "Near":
                    alignment |= Android.Views.GravityFlags.Top;
                    break;
                case "Center":
                    alignment |= Android.Views.GravityFlags.CenterVertical;
                    break;
                case "Far":
                    alignment |= Android.Views.GravityFlags.Bottom;
                    break;
            }

            textView.Gravity = alignment;

            if (BackColor != "Transparent")
            {
                GradientDrawable shape = new GradientDrawable();
                shape.SetShape(ShapeType.Rectangle);
                shape.SetColor(Android.Graphics.Color.ParseColor(BackColor));
                textView.Background = shape;
            }

            textView.Borders = Borders;
            textView.SetPadding(Padding.Left, Padding.Top, Padding.Right, Padding.Bottom);

        }

        private string _Text = "";
        public string Text
        {
            get
            {
                return _Text;
            }
            set
            {
                _Text = value;
                OnPropertyChanged("Text");
            }
        }

        private bool _AutoHeight = true;
        public bool AutoHeight
        {
            get
            {
                return _AutoHeight;
            }
            set
            {
                _AutoHeight = value;
                OnPropertyChanged("AutoHeight");
            }
        }

        private string _Font;
        public string Font
        {
            get
            {
                return _Font;
            }
            set
            {
                _Font = value;
                OnPropertyChanged("Font");
            }
        }

        private string _ForeColor = "Black";
        public string ForeColor
        {
            get
            {
                return _ForeColor;
            }
            set
            {
                _ForeColor = value;
                OnPropertyChanged("ForeColor");
            }
        }
    }

    [Serializable()]
    public class MPSImageField : MPSReportField
    {

        #region " -- IDisposable -- "

        protected override void Dispose(bool disposing)
        {
            if (!this.Disposed)
            {
                if (disposing)
                {
                    try
                    {
                        // TODO: free other state (managed objects).
                        if (Image != null)
                        {
                            Image.Dispose();
                            Image = null;
                        }
                    }
                    catch (Exception)
                    {
                        //Debug.Print("Dispose Image:");
                        //Debug.Print(ex.ToString);
                    }
                }
                // TODO: free your own state (unmanaged objects).
                // TODO: set large fields to null.
                base.Dispose(disposing);
            }
        }


        #endregion

        public override object Clone()
        {
            return this.MemberwiseClone();
        }

        private string _ImageFile;
        public string ImageFile
        {
            get
            {
                return _ImageFile;
            }
            set
            {
                _ImageFile = value;
                OnPropertyChanged("ImageFile");
            }
        }

        private Bitmap _Image = null;
        [XmlIgnore()]
        public Bitmap Image
        {
            get
            {
                return _Image;
            }
            set
            {
                _Image = value;
                OnPropertyChanged("Image");
            }
        }

        public override void Paint(RelativeLayout Parent, Context Context, Single ps = 1)
        {
            base.Paint(Parent, Context, ps);

            if (!this.Visible)
            {
                return;
            }

            if (Image == null)
            {
                Image = Bitmap.CreateBitmap(Size.Width, Size.Height, Bitmap.Config.Argb8888);
                Canvas c = new Canvas(Image);
                Paint p = new Paint();
                p.Color = Android.Graphics.Color.Red;
                p.SetStyle(Android.Graphics.Paint.Style.Stroke);
                c.DrawLine(0, 0, Size.Width, Size.Height, p);
                c.DrawLine(0, Size.Height, Size.Width, 0, p);
                p.Dispose();
                c.Dispose();
            }

            float Height = (float)Size.Height * ps;

            float Width = (float)Image.Width * (Height / (float)Image.Height);

            int X = 0;

            int Y = 0;

            switch (Alignment)
            {
                case "Center":
                    X = (int)((this.Location.X * ps) - (Width / 2));
                    break;
                case "Near":
                    X = (int)((this.Location.X * ps));
                    break;
                case "Far":
                    X = (int)((this.Location.X * ps) - Width);
                    break;
            }

            switch (LineAlignment)
            {
                case "Center":
                    Y = (int)((this.Location.Y * ps) - (Height / 2));
                    break;
                case "Near":
                    Y = (int)((this.Location.Y * ps));
                    break;
                case "Far":
                    Y = (int)((this.Location.Y * ps) - Height);
                    break;
            }

            if (View == null)
            {
                View = new ImageView(Context);
                View.SetY( Y);
                View.SetX( X);
                Parent.AddView(View);
            }

            Bitmap bmp = Bitmap.CreateScaledBitmap(Image, (int)Width, (int)Height, true);
            ImageView vw = (ImageView)View;
            vw.SetImageBitmap(bmp);
        }

        public override Rectangle getRegion()
        {

            int X = this.Location.X;
            int Y = this.Location.Y;

            switch (this.Alignment)
            {
                case "Center":
                    X = this.Location.X - (this.Size.Width / 2);
                    break;
                case "Near":
                    X = this.Location.X;
                    break;
                case "Far":
                    X = this.Location.X - this.Size.Width;
                    break;
            }

            switch (this.LineAlignment)
            {
                case "Center":
                    Y = this.Location.Y - (this.Size.Height / 2);
                    break;
                case "Near":
                    Y = this.Location.Y;
                    break;
                case "Far":
                    Y = this.Location.Y - this.Size.Height;
                    break;
            }

            return new Rectangle(X, Y, this.Size.Width, this.Size.Height);

        }
    }
}
