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
    public class CollageController : Controller
    {
        public ActionResult Index(Models.StoryCollection clg)
        {
            return View(clg.CollectionList);
        }


        public ActionResult Details(int id)
        {
            try
            {
                Models.StoryCollection StoryCollection = new Models.StoryCollection();
                foreach (Models.Collage collage in StoryCollection.CollectionList)
                {
                    if (collage.id == id)
                    {
                        return View(collage);
                    }
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }

        }

        // GET: /Collafe/Create
        [Authorize(Users = "deve")]
        public ActionResult Create(Models.Collage collage)
        {
            return View(collage);
        }

        // POST: /Collafe/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                Models.StoryCollection StoryCollection = new Models.StoryCollection();
                Models.Collage collage = new Models.Collage();

                string path = System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\StoryCollection.xml");
                XDocument doc = XDocument.Load(path);
                if(!doc.Element("collages").Elements("collage").Any())
                {
                    collage.id = 1;
                }
                else { 
                    collage.id = (int)(from S in doc.Descendants("collage")
                                   orderby (int)S.Element("id")
                                   descending
                                   select (int)S.Element("id")).FirstOrDefault()+1;
                }
                collage.collageName = collection["CollageName"];
                string checkResp = collection["checkResp"];
                collage.storyId = checkResp.Split(',').ToList();

                StoryCollection.CollectionList.Add(collage);
                StoryCollection.Create(collage);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: /Collafe/Edit/5
        [Authorize(Users = "deve")]
        public ActionResult Edit(int id)
        {
            try
            {
                Models.StoryCollection StoryCollection = new Models.StoryCollection();
                foreach (Models.Collage collage in StoryCollection.CollectionList)
                {
                    if (collage.id == id)
                    {
                        return View(collage);
                    }
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        // POST: /Collafe/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                Models.StoryCollection StoryCollection = new Models.StoryCollection();
                foreach (Models.Collage collage in StoryCollection.CollectionList)
                {
                    if (collage.id == id)
                    {
                        collage.collageName = collection["CollageName"];
                        string checkResp = collection["checkResp"];
                        collage.storyId = checkResp.Split(',').ToList();
                        StoryCollection.Edit(collage);
                        break;
                    }
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: /Collafe/Delete/5
        [Authorize(Users = "admin")]
        public ActionResult Delete(int id)
        {
            try
            {
                Models.StoryCollection StoryCollection = new Models.StoryCollection();
                foreach (Models.Collage collage in StoryCollection.CollectionList)
                {
                    if (collage.id == id)
                    {
                        return View(collage);
                    }
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        // POST: /Collafe/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                Models.StoryCollection StoryCollection = new Models.StoryCollection();
                foreach (Models.Collage collage in StoryCollection.CollectionList)
                {
                    if (collage.id == id)
                    {
                        StoryCollection.CollectionList.Remove(collage);
                        StoryCollection.Delete(collage);
                        break;
                    }
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


    }
}
