using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace MissionskyOA.Core.Config
{
    public static class TemplateManager
    {
        private static List<Template> templates { get; set; }

        public static void Load(string path)
        {
            XmlDocument document = new XmlDocument();
            document.Load(path);
            templates = new List<Template>();
            var templateNodes = document.SelectNodes("templates/template");
            foreach (XmlNode templateNode in templateNodes)
            {
                Template template = new Template();
                template.Name = templateNode.Attributes["name"].Value;
                template.Content = templateNode.InnerText;
                templates.Add(template);
            }
        }

        public static Template GetTemplate(string name)
        {
            if (templates == null)
            {
                string fileName = Path.Combine(HttpRuntime.AppDomainAppPath, "App_Data", "templates.xml");
                Load(fileName);
            }

            return templates.FirstOrDefault(item => item.Name == name);
        }
    }
}
