using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web;
using System.IO;
using System.Net.Http.Headers;
using System.Xml.Linq;

namespace MyStoryMaker.Controllers
{
    public class FileUploadController : ApiController
    {
        public static string sId;
        public static string bId;
        public async Task<List<string>> PostAsync(int storyId, int blockId)
        {
            sId = storyId.ToString();
            bId = blockId.ToString();

            if (Request.Content.IsMimeMultipartContent())
            {
                string path = HttpContext.Current.Server.MapPath("~\\App_Data\\Collages.xml");
                XDocument doc;
                doc = XDocument.Load(path);

                string id = storyId.ToString();
                string folderName = HttpContext.Current.Server.MapPath(@"~/Img/" + id);

                if (!Directory.Exists(folderName))
                {
                    System.IO.Directory.CreateDirectory(folderName);
                    
                }
                
                string uploadPath = folderName;

                MyStreamProvider streamProvider = new MyStreamProvider(uploadPath);

                await Request.Content.ReadAsMultipartAsync(streamProvider);

                List<string> messages = new List<string>();
                foreach (var file in streamProvider.FileData)
                {
                    try 
                    { 
                        FileInfo fi = new FileInfo(file.LocalFileName);
                        messages.Add("File uploaded as " + fi.FullName + " (" + fi.Length + " bytes)");

                        XElement elem = doc.Element("collages")
                               .Elements("collage")
                               .Elements("story")
                               .Elements("block")
                               .Where(items => items.Element("id").Value == blockId.ToString()).FirstOrDefault();
                        if (elem.Element("imgPath") == null)
                        {
                            elem.Add(new XElement("imgPath", fi.Name));
                        }
                        else
                        {
                            elem.Element("imgPath").Value = fi.Name;
                        }
                        doc.Save(path);
                     }
                    catch
                    {
                        
                        return messages;
                    }
                }
                
                return messages;
            }
            else
            {
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Request!");
                throw new HttpResponseException(response);
            }
        }

        public class MyStreamProvider : MultipartFormDataStreamProvider
        {
            public MyStreamProvider(string uploadPath)
                : base(uploadPath)
            {

            }

            public override string GetLocalFileName(HttpContentHeaders headers)
            {
                string fileName = sId + "_" + bId + headers.ContentDisposition.FileName.Substring( headers.ContentDisposition.FileName.LastIndexOf("."));
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    fileName = Guid.NewGuid().ToString() + ".data";
                }
                return fileName.Replace("\"", string.Empty);
            }
        }
    }
}
