using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace MyStoryMaker.Controllers
{
    public class StoryController : Controller
    {
        static string path = System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\Collages.xml");
        XDocument doc = XDocument.Load(path);
        static string paths = System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\StoryCollection.xml");
        XDocument collection1 = XDocument.Load(paths);
        static int collageId;

        public ActionResult Index(int id)
        {
            collageId = id;
            ViewBag.id = id;
            Models.Collage all = new Models.Collage();
            List<Models.Story> storyList = new List<Models.Story>();
            if (id == 0)
            {
                return View(all.storyList);
            }
            else
            {
                Models.Archive archive = new Models.Archive();

                List<int> styId = new List<int>();
                foreach (int a in archive.archiveList)
                {
                    string collection = System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\StoryCollection.xml");
                    XDocument docu = XDocument.Load(collection);

                    var cos = from s in doc.Elements("collages").Elements("collage") select s;
                    foreach (var co in cos)
                    {
                        if (co.Element("id").Value == a.ToString())
                        {
                            var ids = from s in co.Elements("storyId") select s;
                            foreach (var sid in ids)
                            {
                                styId.Add(Int32.Parse(sid.Value));
                            }
                        }
                    }
                }
                var clgs = from collage in collection1.Element("collages")
                                .Elements("collage")
                           select collage;
                foreach (var clg in clgs)
                {
                    
                    if (clg.Element("id").Value == id.ToString())
                    {
                        ViewBag.collageName = clg.Element("collageName").Value;
                        var styids = from story in clg.Elements("storyId")
                                  select story;
                        ViewBag.storyId = styids.First().Value;
                        var s_in_c = from sty in doc.Element("collages").Elements("collage").Elements("story")
                                    select sty;

                        foreach (var styid in styids)
                        {
                            foreach (var elem in s_in_c)
                            {
                                if (styid.Value == elem.Element("id").Value)
                                {
                                        Models.Story c = new Models.Story();
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
                        }

                        
                        return View(storyList);
                    }
                }
            }
            return View();
        }

        // GET: /Story/Details/5
        public ActionResult Details(int id)
        {
            try
            {
                ViewBag.id = collageId;
                Models.Collage collage = new Models.Collage();
                foreach (Models.Story story in collage.storyList)
                {
                    if (story.id == id)
                    {
                        return View(story);
                    }
                }
                return RedirectToAction("Index" + "/" + collageId);
            }
            catch
            {
                return RedirectToAction("Index" + "/" + collageId);
            }
        }

        // GET: /Story/Create
        [Authorize(Users = "deve")]
        public ActionResult Create()
        {
            ViewBag.id = collageId;
            return View();
        }

        // POST: /Story/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            Models.Collage collage = new Models.Collage();
            Models.Story story = new Models.Story();
            if (!doc.Element("collages").Elements("collage").Elements("story").Any())
            {
                story.id = 0;
            }
            else
            {
                story.id = (int)(from S in doc.Descendants("story")
                                   orderby (int)S.Element("id")
                                   descending
                                   select (int)S.Element("id")).FirstOrDefault() + 1;
            }
            story.storyCaption = collection["StoryCaption"];
            story.storyTime = collection["StoryTime"];
            story.storyOrder = collection["StoryOrder"];

            collage.Create(collageId, story);

            return RedirectToAction("Index"+"/"+collageId);

        }

        // GET: /Story/Edit/5
        [Authorize(Users = "deve")]
         public ActionResult Edit(int id)
         {
             try
             {
                 ViewBag.id = collageId;
                 Models.Collage collage = new Models.Collage();
                 foreach (Models.Story story in collage.storyList)
                 {
                     if (story.id == id)
                     {
                         return View(story);
                     }
                 }
                 return RedirectToAction("Index" + "/" + collageId);
             }
             catch
             {
                 return RedirectToAction("Index" + "/" + collageId);
             }
         }

         // POST: /Story/Edit/5
         // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
         // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
         [HttpPost]
         public ActionResult Edit(int id, FormCollection collection)
         {
             try
             {
                 ViewBag.id = collageId;
                 Models.Collage collage = new Models.Collage();
                 foreach (Models.Story story in collage.storyList)
                 {
                     if (story.id == id)
                     {
                         story.storyCaption = collection["StoryCaption"];
                         story.storyTime = collection["StoryTime"];
                         story.storyOrder = collection["StoryOrder"];
                         collage.Edit(collageId, story);
                         break;
                     }
                 }
                 return RedirectToAction("Index" + "/" + collageId);
             }
             catch
             {
                 return View();
             }
         }

         // GET: /Story/Delete/5
        [Authorize(Users = "admin")]
         public ActionResult Delete(int id)
         {
             try
             {
                 ViewBag.id = collageId;
                 Models.Collage collage = new Models.Collage();
                 foreach (Models.Story story in collage.storyList)
                 {
                     if (story.id == id)
                     {
                         return View(story);
                     }
                 }
                 return RedirectToAction("Index" + "/" + collageId);
             }
             catch
             {
                 return RedirectToAction("Index" + "/" + collageId);
             }
         }

         // POST: /Story/Delete/5
         [HttpPost]
         public ActionResult Delete(int id, FormCollection collection)
         {
             try
             {
                 ViewBag.id = collageId;
                 Models.Collage collage = new Models.Collage();
                 foreach (Models.Story story in collage.storyList)
                 {
                     if (story.id == id)
                     {
                         collage.storyList.Remove(story);
                         collage.Delete(story);
                         break;
                     }
                 }
                 return RedirectToAction("Index" + "/" + collageId);
             }
             catch
             {
                 return View();
             }
         }
    }
}
