using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace MyStoryMaker.Models
{
    public class Archive
    {
        string paths;
        XDocument doc;
        public int id{get;set;}

        public List<int> archiveList { get; set; }

        public Archive()
        {
            archiveList = new List<int>();
            paths = HttpContext.Current.Server.MapPath("~\\App_Data\\Archive.xml");
            Fill();
        }

        private void Fill()
        {
            try
            {
                doc = XDocument.Load(paths);
                var q = from clg in doc.Element("archive").Elements("collageId") select clg;

                foreach (var elem in q)
                {
                    int clgid = Int32.Parse(elem.Value);
                    archiveList.Add(clgid);
                }
            }
            catch
            {
                return;
            }
            return;
        }
        public void Do_archive(int collageId)
        {
            doc.Element("archive").Add(new XElement("collageId", collageId));
            doc.Save(paths);
        }

        public void Unarchive(int collageId)
        {
            doc.Element("archive")
                .Elements("collageId")
                .Where(items => items.Value == collageId.ToString())
                .Remove();
            doc.Save(paths);
        }
    }
}