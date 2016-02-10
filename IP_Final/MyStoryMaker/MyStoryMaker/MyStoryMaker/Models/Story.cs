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
    public class Story
    {
        string path;
        private XDocument doc;

        public int id { get; set; }

        [DisplayName("Story Title")]
        [Required(ErrorMessage = "Enter a stroy caption")]
        public string storyCaption { get; set; }

        [DisplayName("Story Duration")]
        [Required(ErrorMessage = "When the stroy happend")]
        public string storyTime { get; set; }

        [DisplayName("Story Position")]
        [Required(ErrorMessage = "Which position this story belongs to?")]
        public string storyOrder { get; set; }

        public List<StoryBlock> blockList{get;set;}

        public Story()
        {
            blockList = new List<StoryBlock>();
            path = HttpContext.Current.Server.MapPath("~\\App_Data\\Collages.xml");
            Fill_story();
        }

        private void Fill_story()
        {
            try
            {
                doc = XDocument.Load(path);

                var q = from blk in doc.Elements("collages").Elements("collage").Elements("story").Elements("block") select blk;
                foreach (var elem in q)
                {
                    StoryBlock c = new StoryBlock();
                    c.id = Int32.Parse(elem.Element("id").Value);
                    c.blockCaption = elem.Element("blockCaption").Value;
                    c.blockContent = elem.Element("blockContent").Value;
                    c.bolckOrder = elem.Element("bolckOrder").Value;
                    if (elem.Element("imgPath") == null)
                    {
                        ;
                    }
                    else { 
                     c.imgPath = elem.Element("imgPath").Value;
                    }
                    blockList.Add(c);
                }

            }
            catch
            {
                return;
            }
            return;
        }


        internal void Create(int id, StoryBlock block)
        {
            try
            {
                doc = XDocument.Load(path);

                doc.Element("collages")
                   .Elements("collage")
                   .Elements("story")
                   .Where(items => items.Element("id").Value == id.ToString()).FirstOrDefault()
                   .Add(new XElement("block",
                 new XElement("id", block.id),
                 new XElement("blockCaption", block.blockCaption),
                 new XElement("blockContent", block.blockContent),
                 new XElement("bolckOrder", block.bolckOrder)));
                if (block.imgPath != null)
                {
                    doc.Element("collages")
                   .Elements("collage")
                   .Elements("story")
                   .Elements("block")
                   .Where(items => items.Element("id").Value == block.id.ToString()).FirstOrDefault()
                   .Add(new XElement("imgPath", block.imgPath));
                }

                doc.Save(path);
            }
            catch
            {
                return;
            }
            return;
        }

        public void Edit(int storyId, StoryBlock block)
        {
            try
            {
                doc = XDocument.Load(path);

                var blks = from b in doc.Element("collages")
                   .Elements("collage")
                   .Elements("story")
                   .Elements("block")
                   select b;
                foreach(var blk in blks)
                {
                    if(blk.Element("id").Value == block.id.ToString()){
                        blk.Element("blockCaption").Value = block.blockCaption;
                        blk.Element("blockContent").Value = block.blockContent;
                        blk.Element("imgPath").Value = block.imgPath;
                        blk.Element("bolckOrder").Value = block.bolckOrder;

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

        public void Delete(StoryBlock block, int storyId)
        {
            try { 
                doc = XDocument.Load(path);

                doc.Element("collages")
                   .Elements("collage")
                   .Elements("story")
                   .Elements("block")
                   .Where(items => items.Element("id").Value == block.id.ToString()).FirstOrDefault()
                   .Remove();
                doc.Save(path);
                string imgPath = HttpContext.Current.Server.MapPath(@"~/Img/" + storyId + "/" +block.imgPath);
                if (File.Exists(imgPath)) { 
                    File.Delete(imgPath);
                }
             }
            catch
            {
                return;
            }
            return;
        }
        
    }
}