using System;

using CommersantTest.RuntimeModel;


namespace CommersantTest.Runtime
{
    sealed class XsltDataProcessor : IDataProcessor
    {
        public void ProcessData (string inPath, string outPath, bool validate = true)
        {
            throw new Exception("XSLT processor is not implemented");                
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
