using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;

namespace frontend.Models
{
    public class CodeController : Controller
    {
        private const string QueueName = "ProcessingQueue";
        private QueueClient Client;
        private CodeContext db = new CodeContext();

        //
        // GET: /Code/

        public ActionResult Index()
        {
            return View(db.CodeSnippets.ToList());
        }

        //
        // GET: /Code/Details/5

        public ActionResult Details(int id = 0)
        {
            CodeSnippet codesnippet = db.CodeSnippets.Include(p => p.Result).Where(p => p.Id == id).FirstOrDefault();
            if (codesnippet == null)
            {
                return HttpNotFound();
            }

            Request.AcceptTypes.Contains("application/json");
            return View(codesnippet);
        }

        //
        // GET: /Code/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Code/Create

        [HttpPost]
        public ActionResult Create(CodeSnippet codesnippet)
        {
            if (ModelState.IsValid)
            {
                db.CodeSnippets.Add(codesnippet);
                db.SaveChanges();

                if (Client == null)
                {
                    // TODO: move somewhere
                    // Create the queue if it does not exist already
                    string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
                    var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
                    if (!namespaceManager.QueueExists(QueueName))
                    {
                        namespaceManager.CreateQueue(QueueName);
                    }

                    // Initialize the connection to Service Bus Queue
                    Client = QueueClient.CreateFromConnectionString(connectionString, QueueName);
                }

                BrokeredMessage message = new BrokeredMessage(codesnippet);
                Client.Send(message);

                return RedirectToAction("Details", new { Id=codesnippet.Id });
            }

            return View(codesnippet);
        }

        //
        // GET: /Code/Edit/5

        public ActionResult Edit(int id = 0)
        {
            CodeSnippet codesnippet = db.CodeSnippets.Find(id);
            if (codesnippet == null)
            {
                return HttpNotFound();
            }
            return View(codesnippet);
        }

        //
        // POST: /Code/Edit/5

        [HttpPost]
        public ActionResult Edit(CodeSnippet codesnippet)
        {
            if (ModelState.IsValid)
            {
                db.Entry(codesnippet).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(codesnippet);
        }

        //
        // GET: /Code/Delete/5

        public ActionResult Delete(int id = 0)
        {
            CodeSnippet codesnippet = db.CodeSnippets.Find(id);
            if (codesnippet == null)
            {
                return HttpNotFound();
            }
            return View(codesnippet);
        }

        //
        // POST: /Code/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CodeSnippet codesnippet = db.CodeSnippets.Find(id);
            db.CodeSnippets.Remove(codesnippet);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}