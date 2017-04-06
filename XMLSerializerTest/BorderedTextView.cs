using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Widget;

namespace XMLSerializerTest
{
    public class BorderedTextView : TextView
    {

        public BorderedTextView(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {

        }

        public BorderedTextView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {

        }

        public BorderedTextView(Context context) : base(context)
        {
        }

        public MPSReportFieldBorders Borders { get; set; }

        protected override void OnDraw(Android.Graphics.Canvas canvas)
        {
            base.OnDraw(canvas);

            if (Borders != null)
            {
                Android.Graphics.Paint paint = new Android.Graphics.Paint();
                paint.Color = Color.Black;
                paint.SetStyle(Android.Graphics.Paint.Style.Stroke);

                int Height = this.Height;
                int Width = this.Width;
                if (Borders.Top > 0)
                {
                    paint.StrokeWidth = Borders.Top * 2;
                    canvas.DrawLine(0, 0, Width, 0, paint);
                }
                if (Borders.Bottom > 0)
                {
                    paint.StrokeWidth = Borders.Bottom * 2;
                    canvas.DrawLine(0, Height, Width, Height, paint);
                }
                if (Borders.Left > 0)
                {
                    paint.StrokeWidth = Borders.Left * 2;
                    canvas.DrawLine(0, 0, 0, Height, paint);
                }
                if (Borders.Right > 0)
                {
                    paint.StrokeWidth = Borders.Right * 2;
                    canvas.DrawLine(Width, 0, Width, Height, paint);
                }
                paint.Dispose();
            }
        }

    }
}