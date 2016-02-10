using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace MyStoryMaker.Models
{
    public class StoryCollection
    {
        string paths;
        XDocument doc;
        public int id{get;set;}

        public List<Collage> CollectionList { get; set; }

        public StoryCollection()
        {
            CollectionList = new List<Collage>();
            paths = HttpContext.Current.Server.MapPath("~\\App_Data\\StoryCollection.xml");
            Fill();
        }

        private void Fill()
        {
            try
            {
                Models.Archive archive = new Archive();

                doc = XDocument.Load(paths);
                var q = from clg in doc.Elements("collages").Elements("collage") select clg;

                
                    foreach (var elem in q)
                    {
                        Collage c = new Collage();
                        c.storyId = new List<string>();
                        c.id = Int32.Parse(elem.Element("id").Value);
                        
                        c.collageName = elem.Element("collageName").Value;
                        var ids = from s in elem.Elements("storyId") select s;
                        foreach (var id in ids)
                        {
                            c.storyId.Add(id.Value.ToString());
                        }
                        CollectionList.Add(c);
                        
                    }
                }
            
            catch
            {
                return;
            }
            return;
        }

        public bool Create(Collage collage)
        {
            try
            {
                if (collage.id == 1)  //add the second collage
                {
                    doc = new XDocument();
                    XElement p = new XElement("collages");
                    XElement c = new XElement("collage");
                    XElement d = new XElement("id");
                    d.Value = "1";
                    c.Add(d);
                    XElement i = new XElement("collageName");
                    i.Value = collage.collageName;
                    c.Add(i);
                    foreach (var a in collage.storyId) { 
                    XElement s = new XElement("storyId");
                    s.Value = a.ToString();
                    c.Add(s);
                    }
                    p.Add(c);
                    doc.Add(p);
                }
                else
                {
                    doc = XDocument.Load(paths);

                    doc.Element("collages")
                        .Elements("collage")
                        .Where(items => items.Element("id").Value == (collage.id - 1).ToString()).FirstOrDefault()
                        .AddAfterSelf(new XElement("collage",
                            new XElement("id", collage.id),
                            new XElement("collageName", collage.collageName)));
                    foreach (var b in collage.storyId)
                    {
                        doc.Element("collages")
                        .Elements("collage")
                        .Where(items => items.Element("id").Value == collage.id.ToString()).FirstOrDefault()
                        .Add(new XElement("storyId", b.ToString()));
                    }
                }

                doc.Save(paths);
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
                
                    doc = XDocument.Load(paths);

                    var co = from clgs in doc.Element("collages")
                        .Elements("collage") select clgs;
                    foreach (var c in co)
                    {
                        if(c.Element("id").Value == collage.id.ToString())
                        {
                            c.Element("collageName").Value = collage.collageName;

                            c.Elements("storyId").Remove();

                            foreach (var b in collage.storyId)
                            {
                                doc.Element("collages")
                                .Elements("collage")
                                .Where(items => items.Element("id").Value == collage.id.ToString()).FirstOrDefault()
                                .Add(new XElement("storyId", b.ToString()));
                            }
                        }
                    }

                    doc.Save(paths);
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
                doc.Element("collages")
                    .Elements("collage")
                    .Where(items => items.Element("id").Value == collage.id.ToString()).FirstOrDefault()
                    .Remove();
                doc.Save(paths);
            }
            catch
            {
                return;
            }
        }
    
    }
}