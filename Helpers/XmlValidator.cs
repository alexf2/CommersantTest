using System;
using System.Xml;
using System.Text;
using System.Xml.Schema;

namespace Ifx.FoundationHelpers.General
{
    public sealed class XmlValidator
    {
        XmlReaderSettings _settings;
        bool _res;
        StringBuilder _bld = new StringBuilder();
        readonly string _lineSep;

        public XmlValidator (XmlReader schema, string sep)
        {
            _settings = new XmlReaderSettings();
            //_settings.XmlResolver = _resolver;
            _settings.ValidationType = ValidationType.Schema;
            _settings.ValidationFlags = XmlSchemaValidationFlags.ProcessInlineSchema | XmlSchemaValidationFlags.ReportValidationWarnings;
            _settings.ConformanceLevel = System.Xml.ConformanceLevel.Document;
            _settings.ValidationEventHandler += schemaValidationCallback;
            _settings.Schemas.Add(string.Empty, schema);
            schema.Dispose();
            _lineSep = sep;
        }

        public Tuple<bool, string> Validate (string path)
        {
            _res = true;
            _bld.Clear();

            using (XmlReader sr = XmlReader.Create(path, _settings))
                while (sr.Read()) ;

            return new Tuple<bool, string>(_res, _bld.ToString());
        }

        void schemaValidationCallback (object sender, ValidationEventArgs e)
        {
            if (_bld.Length > 0)
                _bld.Append(_lineSep);
            _bld.Append(e.Message);
            _res = false;
        }
    }
}
