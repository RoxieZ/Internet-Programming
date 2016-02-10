using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace MyStoryMaker.Models
{
    public class Collages
    {
        string path;
        XDocument doc;

        public List<Collage> collageList { get; set; }

        public Collages()
        {
            collageList = new List<Collage>();
            path = HttpContext.Current.Server.MapPath("~\\App_Data\\Collages.xml");
            Fill_Collages();
        }


        public void Fill_Collages()
        {
            try
            {
                doc = XDocument.Load(path);
                var q = from clg in doc.Elements("collages").Elements("collage") select clg;
                
                foreach (var elem in q)
                {
                    Collage c = new Collage();
                    c.id = Int32.Parse(elem.Element("id").Value);
                    c.collageName = elem.Element("collageName").Value;
                    
                    collageList.Add(c);
                }
            }
            catch
            {
                return;
            }
            return;
        }

        public bool Create( Collage collage)
        {
            try
            {
                if (collage.id == 0)  //add the very first collage
                {
                    doc = new XDocument();
                    XElement p = new XElement("collages");
                    XElement c = new XElement("collage");
                    XElement d = new XElement("id");
                    d.Value = "0";
                    c.Add(d);
                    XElement i = new XElement("collageName");
                    i.Value = collage.collageName;
                    c.Add(i);
                    p.Add(c);
                    doc.Add(p);
                }
                else {
                    doc = XDocument.Load(path);
                   
                    doc.Element("collages")
                        .Elements("collage")
                        .Where(items => items.Element("id").Value == (collage.id-1).ToString()).FirstOrDefault()
                        .AddAfterSelf(new XElement("collage",
                            new XElement("id", collage.id),
                            new XElement("collageName", collage.collageName)));
                }
                doc.Save(path);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public void Edit(Collage collage)
        {
            try
            {
                doc = XDocument.Load(path);
                var co = from clgs in doc.Element("collages").Elements("collage") select clgs;
                foreach(var clg in co)
                {
                    if(clg.Element("id").Value == collage.id.ToString())
                    {
                        clg.Element("collageName").Value = collage.collageName;
                    }
                }
                
                doc.Save(path);
            }
            catch
            {
                int i = collage.id;
                
                return;
            }
        }

        public void Delete(Collage collage)
        {
            try
            {
                var stys = from co in doc.Element("collages")
                    .Elements("collage")
                    .Where(items => items.Element("id").Value == collage.id.ToString()).FirstOrDefault()
                    .Elements("story") select co;
                foreach (var sty in stys)
                {
                    string i = sty.Element("id").Value;
                    string imgPath = HttpContext.Current.Server.MapPath(@"~/Img/" + i);
                    if (Directory.Exists(imgPath)) { 
                        Directory.Delete(imgPath, true);
                    }
                }

                doc.Element("collages")
                    .Elements("collage")
                    .Where(items => items.Element("id").Value == collage.id.ToString()).FirstOrDefault()
                    .Remove();
                doc.Save(path);
            }
            catch
            {
                return;
            }
        }
    }
}