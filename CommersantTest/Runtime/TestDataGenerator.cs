using System;
using System.Text;
using System.Xml;
using System.IO;

using CommersantTest.RuntimeModel;

namespace CommersantTest.Runtime
{
    /// <summary>
    /// Представляет генератор тестового набора данных
    /// </summary>
    sealed class TestDataGenerator: IDataProcessor
    {        
        readonly int _testSize;
        readonly int _numRoots;
        readonly float _extraNodeFactor;

        public TestDataGenerator (int size, int numRoots, float extraNodeFactor)
        {
            _testSize = size;
            _numRoots = numRoots;
            _extraNodeFactor = extraNodeFactor;
        }

        public void ProcessData (string inPath, string outPath, bool validate = true)
        {
            Random rnd = new Random();
            int sz = (int)Math.Round(_testSize * _extraNodeFactor);

            using (FileStream f = File.OpenWrite(inPath))
            using (XmlWriter wr = XmlWriter.Create(f, new XmlWriterSettings() {ConformanceLevel = ConformanceLevel.Document, Indent = true, Encoding = Encoding.UTF8 }))
            {
                wr.WriteStartElement(null, "root", null);

                for (int i = 0; i < _testSize; ++i)
                {
                    wr.WriteStartElement(null, "item", null);
                    wr.WriteAttributeString(null, "id", null, rnd.Next(1, sz).ToString());
                    if (i >= _numRoots)
                        wr.WriteAttributeString(null, "parentId", null, rnd.Next(1, sz).ToString());

                    wr.WriteString(string.Format("Elem {0}", i + 1));
                    wr.WriteEndElement();
                }

                wr.WriteEndElement();
                wr.Flush();
            }
        }

        public int CyclesCount
        {
            get
            {
                return 0;
            }
        }

        public int MultiParentCount
        {
            get
            {
                return 0;
            }
        }
    }
}
