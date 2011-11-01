using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace RecursiveCleaner.Tests.Helpers
{
    class XmlSample
    {
        public string Content { get; set; }

        public XmlSample(string content)
        {
            Content = content;
        }

        public XmlReader Read()
        {
            var reader = XmlReader.Create(new StringReader(Content));
            reader.Read();
            return reader;
        }
    }
}
