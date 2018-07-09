using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Corron.Cars.ViewModels
{
    class ReportViewModel : Screen
    {

        public delegate void ScreenStateChanged(bool canChangeScreen);
        private ScreenStateChanged _screenStateChanged;

        public string HtmlToDisplay{get; set;}
        public string XmlBox { get; set; }

        public ReportViewModel(ScreenStateChanged screenStateChanged)
        {
            _screenStateChanged = screenStateChanged;
        }

        public void CarsReport()
        {
            XmlBox = FormatXml(DataAccess.GetCarsXML());
            NotifyOfPropertyChange(() => XmlBox);
        }

        protected string FormatXml(string xmlString)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlString);
            StringBuilder sb = new StringBuilder();
            System.IO.TextWriter tr = new System.IO.StringWriter(sb);
            XmlTextWriter wr = new XmlTextWriter(tr);
            wr.Formatting = Formatting.Indented;
            doc.Save(wr);
            wr.Close();
            return sb.ToString();
        }

        protected void TransformXML(string xmlString)
        {
            // Load the style sheet.
            XslCompiledTransform xslt = new XslCompiledTransform();
            xslt.Load("output.xsl");

            // Create the FileStream.
            using (FileStream fs = new FileStream(@"c:\data\output.xml", FileMode.Create))
            {
                // Execute the transformation.
                xslt.Transform(new XPathDocument("books.xml"), null, fs);
            }
        }
    }
}
