///////////////////////////////////////////////////////////////////////////
// FileController.cs - Demonstrates how to start a file handling service //
//                                                                       //
// Jim Fawcett, CSE686 - Internet Programming, Spring 2014               //
///////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Threading.Tasks;

namespace FilesAgain.Controllers
{
    public class FileController : ApiController
    {
        //----< GET api/File - get list of available files >---------------

        public IEnumerable<string> Get()
        {
          // available files
          string path = System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\DownLoad");
          string[] files = Directory.GetFiles(path);
          for (int i = 0; i < files.Length; ++i)
            files[i] = Path.GetFileName(files[i]);
          return files;
        }

        //----< GET api/File?fileName=foobar.txt&open=true >---------------
        //----< attempt to open or close FileStream >----------------------
      
        public HttpResponseMessage Get(string fileName, string open)
        {
          string sessionId;
          var response = new HttpResponseMessage();
          Models.Session session = new Models.Session();

          CookieHeaderValue cookie = Request.Headers.GetCookies("session-id").FirstOrDefault();
          if (cookie == null)
          {
            sessionId = session.incrSessionId();
            cookie = new CookieHeaderValue("session-id", sessionId);
            cookie.Expires = DateTimeOffset.Now.AddDays(1);
            cookie.Domain = Request.RequestUri.Host;
            cookie.Path = "/";
          }
          else
          {
            sessionId = cookie["session-id"].Value;
          }
          try
          {
            FileStream fs;
            string path = System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\");
            if (open == "download")  // attempt to open requested fileName
            {
              path = path + "DownLoad";
              string currentFileSpec = path + "\\" + fileName;
              fs = new FileStream(currentFileSpec, FileMode.Open);
              session.saveStream(fs, sessionId);
            }
            else if(open == "upload")
            {
              path = path + "UpLoad";
              string currentFileSpec = path + "\\" + fileName;
              fs = new FileStream(currentFileSpec, FileMode.OpenOrCreate);
              session.saveStream(fs, sessionId);
            }
            else  // close FileStream
            {
              fs = session.getStream(sessionId);
              session.removeStream(sessionId);
              fs.Close();
            }
            response.StatusCode = (HttpStatusCode)200;
          }
          catch
          {
            response.StatusCode = (HttpStatusCode)400;
          }
          finally  // return cookie to save current sessionId
          {
            response.Headers.AddCookies(new CookieHeaderValue[] { cookie });
          }
          return response;
        }

        //----< GET api/File?blockSize=2048 - get a block of bytes >-------

        public HttpResponseMessage Get(int blockSize)
        {
          // get FileStream and read block

          Models.Session session = new Models.Session();
          CookieHeaderValue cookie = Request.Headers.GetCookies("session-id").FirstOrDefault();
          string sessionId = cookie["session-id"].Value;
          FileStream down = session.getStream(sessionId);
          byte[] Block = new byte[blockSize];
          int bytesRead = down.Read(Block, 0, blockSize);
          if(bytesRead < blockSize)  // compress block
          {
            byte[] returnBlock = new byte[bytesRead];
            for (int i = 0; i < bytesRead; ++i)
              returnBlock[i] = Block[i];
            Block = returnBlock;
          }
          // make response message containing block and cookie
          
          HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.OK);
          message.Headers.AddCookies(new CookieHeaderValue[] { cookie });
          message.Content = new ByteArrayContent(Block);
          return message;
        }

        // POST api/file
        public HttpResponseMessage Post(int blockSize)
        {
          Task<byte[]> task = Request.Content.ReadAsByteArrayAsync();
          byte[] Block = task.Result;
          Models.Session session = new Models.Session();
          CookieHeaderValue cookie = Request.Headers.GetCookies("session-id").FirstOrDefault();
          string sessionId = cookie["session-id"].Value;
          FileStream up = session.getStream(sessionId);
          up.Write(Block, 0, Block.Count());
          HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.OK);
          message.Headers.AddCookies(new CookieHeaderValue[] { cookie });
          return message;
        }

        // PUT api/file/5
        public void Put(int id, [FromBody]string value)
        {
          string debug = "debug";
        }

        // DELETE api/file/5
        public void Delete(int id)
        {
          string debug = "debug";
        }
    }
}
