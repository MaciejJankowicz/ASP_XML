using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IO_3_2.Models;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Hosting;

namespace IO_3_2.Controllers
{
    public static class HttpPostedFileBaseExtensions
    {
        public const int ImageMinimumBytes = 512;

        public static bool IsImage(this HttpPostedFileBase postedFile)
        {
            //-------------------------------------------
            //  Check the image mime types
            //-------------------------------------------
            if (postedFile.ContentType.ToLower() != "image/jpg" &&
                        postedFile.ContentType.ToLower() != "image/jpeg" &&
                        postedFile.ContentType.ToLower() != "image/pjpeg" &&
                        postedFile.ContentType.ToLower() != "image/gif" &&
                        postedFile.ContentType.ToLower() != "image/x-png" &&
                        postedFile.ContentType.ToLower() != "image/png")
            {
                return false;
            }

            //-------------------------------------------
            //  Check the image extension
            //-------------------------------------------
            if (Path.GetExtension(postedFile.FileName).ToLower() != ".jpg"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".png"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".gif"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".jpeg")
            {
                return false;
            }

            //-------------------------------------------
            //  Attempt to read the file and check the first bytes
            //-------------------------------------------
            try
            {
                if (!postedFile.InputStream.CanRead)
                {
                    return false;
                }

                if (postedFile.ContentLength < ImageMinimumBytes)
                {
                    return false;
                }

                byte[] buffer = new byte[512];
                postedFile.InputStream.Read(buffer, 0, 512);
                string content = System.Text.Encoding.UTF8.GetString(buffer);
                if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            //-------------------------------------------
            //  Try to instantiate new Bitmap, if .NET will throw exception
            //  we can assume that it's not a valid image
            //-------------------------------------------

            try
            {
                using (var bitmap = new System.Drawing.Bitmap(postedFile.InputStream))
                {
                }
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                postedFile.InputStream.Position = 0;
            }

            return true;
        }
    }

    public class ContactsController : Controller
    {
        private ContactXMLHandler mHSTYDB = new ContactXMLHandler(Path.Combine(HostingEnvironment.MapPath("~/XMLDB"), "Contacts.xml"));
        //private IO_3_2Context db = new IO_3_2Context();

        // GET: Contacts
        public ActionResult Index()
        {
            return View(mHSTYDB.GetEntries());
            //return View(db.Contacts.ToList());
        }

        // GET: Contacts/Create
        public ActionResult Create()
        {
            ViewBag.Contacts = mHSTYDB.GetEntries();
            return View();
        }

        // POST: Contacts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AddContactViewModel contact)  //([Bind(Include = "Email,FirstName,LastName,EventType,Avatar")] AddContactViewModel contact)
        {
            if (ModelState.IsValid)
            {
                Contact cnt = new Contact();
                if (contact.Avatar == null || !contact.Avatar.IsImage())
                {
                    cnt.Avatar = "";
                }
                else
                {
                    string path = Path.Combine(
                        HostingEnvironment.MapPath("~/Uploads"),
                        Path.GetFileName(contact.Avatar.FileName));
                    contact.Avatar.SaveAs(path);
                    cnt.Avatar = Path.Combine("~/Uploads", Path.GetFileName(contact.Avatar.FileName));
                }
                cnt.Email = contact.Email;
                cnt.FirstName = contact.FirstName;
                cnt.LastName = contact.LastName;
                cnt.EventType = contact.EventType;

                //db.Contacts.Add(contact);
                //db.SaveChanges();
                mHSTYDB.Add(cnt);
                mHSTYDB.Save();
                return RedirectToAction("Index");
            }

            ViewBag.Contacts = mHSTYDB.GetEntries();
            return View(contact);
        }

        //// GET: Contacts/Details/5
        //public ActionResult Details(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Contact contact = db.Contacts.Find(id);
        //    if (contact == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(contact);
        //}

        //// GET: Contacts/Edit/5
        //public ActionResult Edit(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Contact contact = db.Contacts.Find(id);
        //    if (contact == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(contact);
        //}

        //// POST: Contacts/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Email,FirstName,LastName,EventType,Avatar")] Contact contact)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(contact).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(contact);
        //}

        //// GET: Contacts/Delete/5
        //public ActionResult Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Contact contact = db.Contacts.Find(id);
        //    if (contact == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(contact);
        //}

        //// POST: Contacts/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(string id)
        //{
        //    Contact contact = db.Contacts.Find(id);
        //    db.Contacts.Remove(contact);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
