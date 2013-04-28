using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using frontend.Models;

namespace frontend.Controllers
{
    public class SnippetsController : ApiController
    {
        private CodeContext db = new CodeContext();

        // GET api/Snippets
        public IEnumerable<CodeSnippet> GetCodeSnippets()
        {
            return db.CodeSnippets.AsEnumerable();
        }

        // GET api/Snippets/5
        public CodeSnippet GetCodeSnippet(int id)
        {
            CodeSnippet codesnippet = db.CodeSnippets.Include(p => p.Result).Where(p => p.Id == id).FirstOrDefault(); 
            if (codesnippet == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return codesnippet;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}