using Android.App;
using Android.Widget;
using Android.OS;
using System;
using System.IO;
using Android.Graphics;

namespace XMLSerializerTest
{
    [Activity(Label = "XMLSerializerTest", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {

        MPSReportDocument doc;

        float zoom = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            zoom *= Resources.GetDimension(Resource.Dimension.onepixel);

            CreateContentView();

        }


        private void CreateContentView()
        {
            HorizontalScrollView root = new HorizontalScrollView(this);

            ScrollView scroll = new ScrollView(this);

            root.AddView(scroll);

            LinearLayout layout = new LinearLayout(this);
            layout.Orientation = Orientation.Vertical;

            scroll.AddView(layout);


            doc = FormatReport();

            foreach (MPSReportPage page in doc.Pages)
            {
                page.Paint(layout, this, zoom);
            }

            SetContentView(root);
        }

        private MPSReportDocument FormatReport()
        {

            Stream input = Assets.Open("TestReport.xml");

            MPSReportDocument rpt = MPSReportDocument.Read(input);

            input.Close();

            MPSReportPage Page = rpt.Pages[0];

            FormatMainPage(Page);

            for (int i = 0; i < rpt.Pages.Count; i++)
            {
                rpt.Pages[i].SetTextField("Page", (i + 1).ToString());
                rpt.Pages[i].SetTextField("Pages", rpt.Pages.Count.ToString());
            }

            return rpt;

        }

        private void FormatMainPage(MPSReportPage Page)
        {
            Page.SetTextField("TextField2", "My Really Nice Report");
            MPSImageField SigField = (MPSImageField)Page.GetField("ImageField1");

            SigField.Image = BitmapFactory.DecodeResource(Resources, Resource.Drawable.Xamarin);

            SigField = (MPSImageField)Page.GetField("TSPSig");
        }



    }
}

