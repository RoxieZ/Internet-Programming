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

namespace MyStoryMaker.Controllers
{
    public class BlockController : Controller
    {
        static string path = System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\Collages.xml");
        XDocument doc = XDocument.Load(path);

        static int storyId;
        static int collageId;
        static int blockId;
        static List<Models.StoryBlock> blockList;

        // GET: /Block/
        public ActionResult Index(int id, int? clgId)
        {
            ViewBag.storyId = storyId = id;
            ViewBag.collageId = clgId;
            collageId = clgId ?? default(int);
            blockList = new List<Models.StoryBlock>();

            var stys = from story in doc.Element("collages")
                           .Elements("collage").Elements("story")
                       select story;

            foreach(var sty in stys)
            {
                if (sty.Element("id").Value == id.ToString())
                {
                    ViewBag.story = sty.Element("storyCaption").Value;
                    var blk = from block in sty.Elements("block")
                              orderby (int)block.Element("bolckOrder")
                              select block;

                    foreach(var elem in blk)
                    {
                        Models.StoryBlock c = new Models.StoryBlock();
                        c.id = Int32.Parse(elem.Element("id").Value);
                        c.blockCaption = elem.Element("blockCaption").Value;
                        c.blockContent = elem.Element("blockContent").Value;
                        c.bolckOrder = elem.Element("bolckOrder").Value;
                        if (elem.Element("imgPath") != null) {
                          c.imgPath = elem.Element("imgPath").Value;
                        }
                        blockList.Add(c);
                    }
                    return View(blockList);
                }
            }
            
            return View();
        }

        public ActionResult showImgView(int id, int? clgId)
        {
            ViewBag.storyId = id;
            storyId = id;    //?????
            ViewBag.collageId = clgId;
            collageId = clgId ?? default(int);
            List<Models.StoryBlock> blockList = new List<Models.StoryBlock>();

            //get the list of stories in each collage

            string pathc = System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\Collages.xml");
            XDocument doc = XDocument.Load(pathc);
            string paths = System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\StoryCollection.xml");
            XDocument collection = XDocument.Load(paths);

            Models.Collage c = new Models.Collage();
            List<Models.Story> stories = new List<Models.Story>();
            if (collageId == 0) //all stories in collage 0
            {
                stories = c.storyList;
            }
            else
            {
                // choose stories in other collage and add to List
                List<Models.Story> storyList = new List<Models.Story>();
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

            //get the id of each story in collage
            int[] stor = new int[stories.Count()];
            int i = 0;

            foreach (Models.Story story in stories)
            {
                stor[i] = story.id;
                i++;
            }
            if (id == stor[stor.Count() - 1])
            {
                ViewBag.run = stor[0];
            }
            else
            {
                for (int j = 0; j < stories.Count(); j++)
                {

                    if (id == stor[j])
                    {
                        ViewBag.run = stor[j + 1];
                    }

                }
            }

            //get information of each block in story
            var stys = from story in doc.Element("collages")
                           .Elements("collage").Elements("story")
                       orderby (int)story.Element("storyOrder")
                       select story;

                foreach (var sty in stys)
                {

                    if (sty.Element("id").Value == id.ToString())
                    {
                        ViewBag.story = sty.Element("storyCaption").Value;
                        ViewBag.storyTime = sty.Element("storyTime").Value + "000";
                        var blk = from block in sty.Elements("block")
                                  orderby (int)block.Element("bolckOrder")
                                  select block;

                        foreach (var elem in blk)
                        {
                            Models.StoryBlock cs = new Models.StoryBlock();
                            cs.id = Int32.Parse(elem.Element("id").Value);
                            cs.blockCaption = elem.Element("blockCaption").Value;
                            cs.blockContent = elem.Element("blockContent").Value;
                            cs.bolckOrder = elem.Element("bolckOrder").Value;
                            if (elem.Element("imgPath") != null)
                            {
                                cs.imgPath = elem.Element("imgPath").Value;
                            }
                            blockList.Add(cs);
                        }
                        return View(blockList);
                    }
                }

            return View();
        }

        public ActionResult SingleImgView(int id, int? clgId)
        {
            ViewBag.storyId = id;
            storyId = id;    //?????
            ViewBag.collageId = clgId;
            return View(blockList);
        }

        // GET: /Block/Details/5
        public ActionResult Details(int id, int? styId)
        {
            try
            {
                ViewBag.collageId = collageId;
                if (styId == null)
                {
                    ViewBag.storyId = storyId;
                }
                else {
                    ViewBag.storyId = styId;
                }
                ViewBag.blockId = id;
                Models.Story story = new Models.Story();
                foreach (Models.StoryBlock block in story.blockList)
                {
                    if (block.id == id)
                    {
                        return View(block);
                    }
                }
                return RedirectToAction("Index", new { id = storyId, clgId = collageId });
            }
            catch
            {
                return RedirectToAction("Index", new { id = storyId, clgId = collageId });
            }
        }

        // GET: /Block/Create
        [Authorize(Users = "deve")]
        public ActionResult Create()
        {
            ViewBag.collageId = collageId;
            ViewBag.storyId = storyId;
            return View();
        }


        //public ActionResult GetPic(string FolderPath)
        //{
        //    string path = Server.MapPath("~/Img/" + FolderPath);
        //    DirectoryInfo dir = new DirectoryInfo(path);
        //    List<string> result = new List<string>();
        //    foreach (FileInfo f in dir.GetFiles())
        //    {
        //        string pathes = "";
        //        pathes += f.Name;
        //        result.Add(pathes);
        //    }
        //    return Json(result,JsonRequestBehavior.AllowGet);
        //}

        // POST: /Block/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            ViewBag.storyId = storyId;
            ViewBag.collageId = collageId;
            Models.Story story = new Models.Story();
            Models.StoryBlock block = new Models.StoryBlock();
            if (!doc.Element("collages").Elements("collage").Elements("story").Elements("block").Any())
            {
                block.id = 0;
            }
            else
            {
                block.id = (int)(from S in doc.Descendants("block")
                                 orderby (int)S.Element("id")
                                 descending
                                 select (int)S.Element("id")).FirstOrDefault() + 1;
            }
            blockId = block.id;
            ViewBag.blockId = blockId;

            block.blockCaption = collection["BlockCaption"];
            block.blockContent = collection["BlockContent"];
            block.imgPath = ""+block.id;
            block.bolckOrder = collection["bolckOrder"];
            story.Create(storyId, block);

            return View("Upload");

        }


        // GET: /Block/Edit/5
        [Authorize(Users = "deve")]
        public ActionResult Edit(int id)
        {
            try
            {
                ViewBag.collageId = collageId;
                ViewBag.blockId = id;
                ViewBag.storyId = storyId;
                Models.Story story = new Models.Story();
                foreach (Models.StoryBlock block in story.blockList)
                {
                    if (block.id == id)
                    {
                        return View(block);
                    }
                }
                return RedirectToAction("Index", new { id = storyId, clgId = collageId });
            }
            catch
            {
                return RedirectToAction("Index", new { id = storyId, clgId = collageId });
            }
        }

        // POST: /Block/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                ViewBag.blockId = id;
                ViewBag.collageId = collageId;
                ViewBag.storyId = storyId;
                Models.Story story = new Models.Story();
                foreach (Models.StoryBlock block in story.blockList)
                {
                    if (block.id == id)
                    {
                        block.blockCaption = collection["BlockCaption"];
                        block.blockContent = collection["BlockContent"];
                        block.bolckOrder = collection["bolckOrder"];
                        story.Edit(storyId, block);
                        break;
                    }
                }
                return RedirectToAction("Index", new { id = storyId, clgId = collageId });
            }
            catch
            {
                return View();
            }
        }

        // GET: /Block/Delete/5
        [Authorize(Users = "admin")]
        public ActionResult Delete(int id)
        {
            try
            {
                ViewBag.collageId = collageId;
                ViewBag.blockId = id;
                ViewBag.storyId = storyId;
                Models.Story story = new Models.Story();
                foreach (Models.StoryBlock block in story.blockList)
                {
                    if (block.id == id)
                    {
                        return View(block);
                    }
                }
                return RedirectToAction("Index", new { id = storyId, clgId = collageId });
            }
            catch
            {
                return RedirectToAction("Index", new { id = storyId, clgId = collageId });
            }
        }

        // POST: /Block/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                Models.Story story = new Models.Story();
                foreach (Models.StoryBlock block in story.blockList)
                {
                    if (block.id == id)
                    {
                        story.blockList.Remove(block);
                        story.Delete(block, storyId);
                        break;
                    }
                }
                return RedirectToAction("Index", new { id = storyId, clgId = collageId });
            }
            catch
            {
                return View();
            }
        }
    }
}
