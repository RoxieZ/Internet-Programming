using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace MyStoryMaker.Controllers
{
    public class PrepDownloadController : Controller
    {
        public static List<Models.Story> stories;
        //
        // GET: /PrepDownload/
        public FilePathResult Index(int collageId, string api="")
        {
            string path = System.Web.HttpContext.Current.Server.MapPath("~\\Img\\");

            string pathc = System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\Collages.xml");
            XDocument doc = XDocument.Load(pathc);
            string paths = System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\StoryCollection.xml");
            XDocument collection = XDocument.Load(paths);

            Models.Collage c = new Models.Collage();
            stories = new List<Models.Story>();
            if (collageId == 0) //all stories
            {
                stories = c.storyList;
            }
            else
            {
                List<Models.Story> storyList = new List<Models.Story>();
                // choose stories in collage and add to List
                var clgs = from collage in collection.Element("collages")
                                .Elements("collage")
                           select collage;
                foreach (var clg in clgs)
                {
                    if (clg.Element("id").Value == collageId.ToString())
                    {
                        ViewBag.collageName = clg.Element("collageName").Value;
                        var styids = from story in clg.Elements("storyId")
                                     select story;

                        var s_in_c = from sty in doc.Element("collages").Elements("collage").Elements("story")
                                     select sty;

                        foreach (var styid in styids)
                        {
                            foreach (var elem in s_in_c)
                            {
                                if (styid.Value == elem.Element("id").Value)
                                {
                                    Models.Story st = new Models.Story();
                                    st.id = Int32.Parse(elem.Element("id").Value);
                                    st.storyCaption = elem.Element("storyCaption").Value;
                                    st.storyTime = elem.Element("storyTime").Value;
                                    st.storyOrder = elem.Element("storyOrder").Value;

                                    storyList.Add(st);

                                }
                            }
                        }


                        stories = storyList;
                    }
                }
            }

            List<string> zipFiles = new List<string>();
            foreach (Models.Story story in stories)
            {
                string htmlcontent = htmlBuilder(story);
            
                using (FileStream fs = new FileStream(path+story.id+"\\"+story.storyCaption+".html", FileMode.Create))
                {
                    using (StreamWriter w = new StreamWriter(fs, Encoding.UTF8))
                    {
                        w.WriteLine(htmlcontent);
                    }
                }

                string storyFolder = path + story.id;
                DirectoryInfo dir = new DirectoryInfo(storyFolder);
                foreach (FileInfo f in dir.GetFiles())
                {
                    zipFiles.Add(f.FullName);
                }

            }
            if (api == "api")
            {
                FileZipper.ZipFiles(zipFiles.ToArray(), path + "collage\\" + collageId + ".zip");
                return null;
            }
            else if (api == "archive")
            {
                FileZipper.ZipFiles(zipFiles.ToArray(), path + "archive\\" + collageId + ".zip");
                return Archive(collageId);
            }
            else {
                FileZipper.ZipFiles(zipFiles.ToArray(), path + "collage\\" + collageId + ".zip");
                return Download(collageId);
            }
        }
        [Authorize (Users="admin")]
        public FilePathResult Archive(int collageId)
        {
            Models.Archive archive = new Models.Archive();
            string path = System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\Archive.xml");
            XDocument doc = XDocument.Load(path);
            archive.Do_archive(collageId);
                
            return null;
        }
        [Authorize(Users = "admin")]
        public FilePathResult Unarchive(int collageId)
        {
            Models.Archive archive = new Models.Archive();
            string path = System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\Archive.xml");
            XDocument doc = XDocument.Load(path);
            archive.Unarchive(collageId);

            return null;
        }

        public string htmlBuilder(Models.Story s)
        {
            string collages = System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\Collages.xml");
            XDocument doc = XDocument.Load(collages);

            var stys = from story in doc.Element("collages")
                           .Elements("collage").Elements("story")
                       orderby (int)story.Element("storyOrder")
                       select story;

            //get link for next story
            int[] a = new int[stories.Count()]; 
            int link = 0;
            string href = "";
            int i = 0;
            foreach(Models.Story story in stories){
                a[i] = story.id;
                i++;
            }
            if (s.id == a[stories.Count - 1])
            {
                link = a[0];
            }
            else
            {
                for (int j = 0; j < stories.Count(); j++)
                {
                    if (s.id == a[j])
                    {
                        link = a[j + 1];
                    }
                }
            }
            foreach (Models.Story story in stories) 
            {
                if (story.id == link)
                {
                    href = story.storyCaption;
                }
            }

            //form html for each story
            foreach (var sty in stys) 
            {

                if (sty.Element("id").Value == s.id.ToString())
                {
                    var blk = from block in sty.Elements("block")
                              orderby (int)block.Element("bolckOrder")
                              select block;

                    StringBuilder sb = new StringBuilder();
                    sb.Append("<h2>");
                    sb.Append(sty.Element("storyCaption").Value);
                    sb.Append("</h2>");
                    sb.Append("<a href='");
                    sb.Append(href);
                    sb.Append(".html'>");
                    sb.Append("Next Story");
                    sb.Append("</a>");
                    sb.Append("<br/>");

                    foreach (var elem in blk)
                    {
                        Models.StoryBlock c = new Models.StoryBlock();
                        c.blockContent = elem.Element("blockContent").Value;
                        if (elem.Element("imgPath") != null)
                        {
                            c.imgPath = elem.Element("imgPath").Value;
                        }

                        sb.Append("<img style='width:1024ps; height: 768px' src='");
                        sb.Append(c.imgPath);
                        sb.Append("'/>");
                        sb.Append("<p>");
                        sb.Append(c.blockContent);
                        sb.Append("</p>");

                    }
                    return sb.ToString();
                }
            }
            return "";
        }

        public FilePathResult Download(int collageId)
        {
            string serverPath = System.Web.HttpContext.Current.Server.MapPath("~\\Img\\collage\\" + collageId + ".zip");
            return File(serverPath, "application/zip", Path.GetFileName(serverPath));
        }
	}
}