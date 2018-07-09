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
            HtmlToDisplay = TransformXML(DataAccess.GetCarsXML(),1); //TODO replace with constant or enum
            NotifyOfPropertyChange(() => HtmlToDisplay);
        }


        protected string TransformXML(string xmlString, int xsltSheetId)
        {

            if (String.IsNullOrEmpty(xmlString))
                return null;

            XslCompiledTransform xslt = new XslCompiledTransform();

            //prepare the stylesheet
            string xsltString = DataAccess.GetXSLTSheet(xsltSheetId);
            if (String.IsNullOrEmpty(xsltString))
                return null;

            using (XmlReader xr = XmlReader.Create(new StringReader(xsltString)))
            {
                xslt.Load(xr);
            }

            //prepare the XML
            using (XmlReader xr = XmlReader.Create(new StringReader(xmlString)))
            {

                var sb = new StringBuilder();
                using (XmlWriter xw = XmlWriter.Create(sb))
                {
                    // Execute the transformation.
                    xslt.Transform(xr, xw);
                    return sb.ToString();
                }
            }
        }
    }
}
