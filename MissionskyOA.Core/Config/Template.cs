using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionskyOA.Core.Config
{
    public class Template
    {
        public string Name { get; set; }

        public string Content { get; set; }

        public string Translate(params object[] args)
        {
            return string.Format(this.Content, args);
        }
    }
}
