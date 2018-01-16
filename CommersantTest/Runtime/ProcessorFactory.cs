using System;
using CommersantTest.RuntimeModel;
using Ifx.FoundationHelpers.General;

namespace CommersantTest.Runtime
{
    sealed class ProcessorFactory
    {
        public const string UseMapper = "/m";
        public const string UseXslt = "/x";
        const string GenerateTest = "/g";

        public IDataProcessor CreateProcessor (string type)
        {
            switch (type.ToLower())
            {
                case UseMapper:
                    return new MapperDataProcessor();
                case UseXslt:
                    return new XsltDataProcessor();
                case GenerateTest:
                    return new TestDataGenerator(ConfigHelper.GetAppSetO<int>("test-size", 100), ConfigHelper.GetAppSetO<int>("num-roots", 1), ConfigHelper.GetAppSetO<float>("extra-nodes-factor", 1));
            }

            throw new Exception(string.Format("Processor type '{0}' is not defined"));
        }
    }
}
