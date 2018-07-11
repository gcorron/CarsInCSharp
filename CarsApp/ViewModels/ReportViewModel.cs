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

        private ShellViewModel.ScreenStateChanged _screenStateChanged;
        private ShellViewModel.ErrorHandler _notifyError;

        public string HtmlToDisplay{get; set;}
        public string XmlBox { get; set; }

        public ReportViewModel(ShellViewModel.ScreenStateChanged screenStateChanged,ShellViewModel.ErrorHandler notifyError)
        {
            _screenStateChanged = screenStateChanged;
            _notifyError = notifyError;
        }

        public void CarsReport()
        {
            try
            {
                HtmlToDisplay = TransformXML(DataAccess.GetCarsXML(), 1); //TODO replace with constant or enum
            }
            catch (Exception e)
            {
                _notifyError(e);
            }
            NotifyOfPropertyChange(() => HtmlToDisplay);
        }


        protected string TransformXML(string xmlString, int xsltSheetId)
        {

            if (String.IsNullOrEmpty(xmlString))
                return null;

            XslCompiledTransform xslt = new XslCompiledTransform();

            //prepare the stylesheet


            try
            {
                string xsltString = DataAccess.GetXSLTSheet(xsltSheetId);
                if (String.IsNullOrEmpty(xsltString))
                    return null;

                using (XmlReader xr = XmlReader.Create(new StringReader(xsltString)))
                {
                    xslt.Load(xr);
                }
            }
            catch (Exception e)
            {
                _notifyError(e);
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
