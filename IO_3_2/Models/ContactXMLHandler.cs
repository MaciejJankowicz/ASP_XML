using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace IO_3_2.Models
{
    public class ContactXMLHandler
    {
        string XMLPath;
        XDocument xContacts;

        public ContactXMLHandler(string XMLPath)
        {
            XmlReader xRead;
            this.XMLPath = XMLPath;

            if (!File.Exists(XMLPath))
            {
                CreateXML();
            }

            try
            {
                xRead = XmlReader.Create(XMLPath);
                try
                {
                    XElement xEle = XElement.Load(xRead);
                    xRead.Close();
                }
                catch (Exception)
                {
                    xRead.Close();
                    CreateXML();
                }
            }
            catch (Exception)
            {
                CreateXML();
            }
            
            xContacts = XDocument.Load(XMLPath);           
        }

        public void CreateXML()
        {
            XNamespace cntNM = "urn:lst-cnt:cnt";
            //Contact cnt = new Contact { FirstName = "aaa", LastName = "bbb", Email = "ccc@gmail.com", Avatar = "file.jpg", EventType = EventType.Kurs };

            XDocument xDoc = new XDocument(
                        new XDeclaration("1.0", "UTF-16", null),
                        new XElement("Contacts"));

            StringWriter sw = new StringWriter();
            XmlWriter xWrite = XmlWriter.Create(sw);
            xDoc.Save(xWrite);
            xWrite.Close();

            // Save to Disk
            xDoc.Save(XMLPath);
        }

        public void Add(Contact cnt)
        {
            xContacts.Element("Contacts").Add(new XElement("Contact",
                                new XElement("FirstName", cnt.FirstName),
                                new XElement("LastName", cnt.LastName),
                                new XElement("Email", cnt.Email),
                                new XElement("Avatar", cnt.Avatar),
                                new XElement("EventType", cnt.EventType)));
        }

        public void Save()
        {
            xContacts.Save(XMLPath);
        }

        public List<Contact> GetEntries()
        {
            List<Contact> entries = new List<Contact>();

            foreach (var item in xContacts.Descendants("Contact"))
            {

                entries.Add(new Contact
                {
                    FirstName = item.Element("FirstName").Value,
                    LastName = item.Element("LastName").Value,
                    Email = item.Element("Email").Value,
                    Avatar = item.Element("Avatar").Value,
                    EventType = Contact.StringToEnum(item.Element("EventType").Value)
                });
            }

            //return entries.Count > 0 ? entries : null;
            return entries;
        }
       
    }
}