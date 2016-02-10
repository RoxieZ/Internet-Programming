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
    public class Collage
    {
        string path;
        private XDocument doc;

        public int id { get; set; }

        [DisplayName("Collage Name")]
        [Required(ErrorMessage = "Enter collage name")]
        public string collageName { get; set; }

        [DisplayName("CollageDuration")]
        [Required(ErrorMessage = "Enter a duration time for this collage")]
        public string collageDuration { get; set; }

        public List<string> storyId { get; set; }

        public List<Story> storyList { get; set; }

        public Collage()
        {
            storyList = new List<Story>();
            path = HttpContext.Current.Server.MapPath("~\\App_Data\\Collages.xml");
            Fill_collage();
        }

        private void Fill_collage()
        {
            try
            {
                Models.Archive archive = new Archive();

                List<int> styId = new List<int>();
                foreach (int a in archive.archiveList)
                {
                    string collection = HttpContext.Current.Server.MapPath("~\\App_Data\\StoryCollection.xml");
                    XDocument docu = XDocument.Load(collection);

                    var cos = from s in docu.Element("collages").Elements("collage") select s;
                    foreach (var co in cos)
                    {
                        if (co.Element("id").Value == a.ToString())
                        {
                            var ids = from s in co.Elements("storyId") select s;
                            foreach (var id in ids)
                            {
                                styId.Add(Int32.Parse(id.Value));
                            }
                        }
                    }
                }

                    doc = XDocument.Load(path);
                    var q = from sty in doc.Elements("collages").Elements("collage").Elements("story") select sty;
                    foreach (var elem in q)
                    {
                        Story c = new Story();
                        c.id = Int32.Parse(elem.Element("id").Value);
                        c.storyCaption = elem.Element("storyCaption").Value;
                        c.storyTime = elem.Element("storyTime").Value;
                        c.storyOrder = elem.Element("storyOrder").Value;

                        storyList.Add(c);
                        foreach (int i in styId)
                        {
                            if (i == c.id)
                            {
                                storyList.Remove(c);
                            }
                        }
                    }
                    
            }
            catch
            {
                return;
            }
            return;
        }

        public bool Create(int id, Story story)
        {
            try
            {
              doc = XDocument.Load(path);

               doc.Element("collages")
                  .Elements("collage")
                  .Where(items => items.Element("id").Value == id.ToString()).FirstOrDefault()
                  .Add(new XElement("story",
                new XElement("id", story.id),
                new XElement("storyCaption", story.storyCaption),
                new XElement("storyTime", story.storyTime),
                new XElement("storyOrder", story.storyOrder)));
                
                doc.Save(path);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public void Edit(int id, Story story)
        {
            try
            {
                doc = XDocument.Load(path);
                var s = from stys in doc.Element("collages").Elements("collage").Elements("story") select stys;
                foreach (var sty in s)
                {
                    if (sty.Element("id").Value == story.id.ToString())
                    {
                        sty.Element("storyCaption").Value = story.storyCaption;
                        sty.Element("storyTime").Value = story.storyTime;
                        sty.Element("storyOrder").Value = story.storyOrder;
                    }
                }

                doc.Save(path);

            }
            catch
            {
                return;
            }
            return;
            
        }

        public void Delete(Story story)
        {
            try { 
                doc = XDocument.Load(path);

                doc.Element("collages")
                   .Elements("collage")
                   .Elements("story")
                   .Where(items => items.Element("id").Value == story.id.ToString()).FirstOrDefault()
                   .Remove();
                doc.Save(path);
                string imgPath = HttpContext.Current.Server.MapPath(@"~/Img/" + story.id);
                Directory.Delete(imgPath, true);
             }
            catch
            {
                return;
            }
            return;
        }
    }
}