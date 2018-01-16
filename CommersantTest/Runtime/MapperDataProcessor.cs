using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

using CommersantTest.RuntimeModel;
using Ifx.FoundationHelpers.General;

namespace CommersantTest.Runtime
{
    /// <summary>
    /// Представляет процессор Xml, использующий мэппер для преобразования плоского Xml в древовидный.
    /// </summary>
    sealed class MapperDataProcessor : IDataProcessor
    {
        /// <summary>
        /// Используется для хранения данных об элементе item. Для экономии памяти будем использовать struct, а не class.
        /// </summary>
        struct Item
        {
            public string Id;
            public string Text;
            public List<string> Children;
        };

        //тут все собранные из входного Xml уникальные item
        readonly Dictionary<string, Item> _mapper = new Dictionary<string, Item>();
        //тут отслеживаем был ли item добавлен в Children, чтобы исключить дублирование
        readonly HashSet<string> _addedToAncestor = new HashSet<string>();

        //контекст для SAX-парсера
        Item _current;
        string _currParentId;
        bool _isInItem;

        //статистика по ошибкам
        int _cyclesCount, _multiParentCount;

        public void ProcessData (string inPath, string outPath, bool validate = true)
        {
            //1) валидируем исходный Xml по схеме XSD
            if (validate)
                validateXml(inPath);

            //2) создаём мэппинг элементов item
            populateMapper(inPath); 

            //3) генерируем по созданному мэппингу древовидный Xml
            generateXml(outPath); 
        }

        public int CyclesCount
        {
            get
            {
                return _cyclesCount;
            }
        }

        public int MultiParentCount
        {
            get
            {
                return _multiParentCount;
            }
        }

        void generateXml (string path)
        {
            if (File.Exists(path))
                File.Delete(path);

            using (FileStream f = File.OpenWrite(path))
            using (XmlWriter wr = XmlWriter.Create(f, new XmlWriterSettings(){ConformanceLevel = ConformanceLevel.Document, Indent = true, Encoding = Encoding.UTF8}))
            {
                wr.WriteStartElement(null, "root", null);

                foreach (var rootItem in getRoots())
                    generateNode(wr, rootItem);

                wr.WriteEndElement();
                wr.Flush();
            }
        }

        void generateNode(XmlWriter wr, Item it)
        {
            wr.WriteStartElement(null, "item", null);

            wr.WriteAttributeString(null, "id", null, it.Id);
            if (!string.IsNullOrEmpty(it.Text))
                wr.WriteAttributeString(null, "text", null, it.Text);

            if (it.Children != null)
                foreach (var chId in it.Children)
                    generateNode(wr, _mapper[chId]);

            wr.WriteEndElement();
        }

        IEnumerable<Item> getRoots ()
        {
            foreach (var id in _mapper.Keys)
                if (!_addedToAncestor.Contains(id))
                    yield return _mapper[id];
        }

        void populateMapper(string inPath)
        {
            _mapper.Clear();
            _addedToAncestor.Clear();
            _currParentId = null;
            _cyclesCount = 0;
            _multiParentCount = 0;

            using (FileStream f = File.OpenRead(inPath))
            using (XmlReader rd = XmlReader.Create(new StreamReader(f)))
            {
                while (rd.Read())
                {
                    switch (rd.NodeType)
                    {
                        case XmlNodeType.Element:
                            startElement(rd.LocalName, rd);
                            if (rd.IsEmptyElement) //если у тэга нет тела, то XmlNodeType.EndElement не встретиться и закрытие выполняем сразу
                                closeElement(rd.LocalName);
                            break;

                        case XmlNodeType.EndElement:
                            closeElement(rd.LocalName);
                            break;

                        case XmlNodeType.Text:
                            addValue(rd.Value);
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        #region SAX methods
        void addValue (string val)
        {
            if (_isInItem)
            {
                _current.Text = val;
            }
        }
        void startElement (string name, XmlReader rd)
        {
            if (name.Equals("item"))
            {
                _current = new Item();
                _current.Id = rd.GetAttribute("id").Trim();
                _currParentId =  rd.GetAttribute("parentId");
                if (_currParentId != null)
                    _currParentId = _currParentId.Trim();
                _isInItem = true;
            }
        }
        void closeElement (string name)
        {
            if (name.Equals("item"))
            {
                _isInItem = false;

                if (_mapper.ContainsKey(_current.Id))
                {
                    Item tmp = _mapper[_current.Id];
                    if (string.IsNullOrEmpty(tmp.Text))
                        tmp.Text = _current.Text;
                    _mapper[_current.Id] = tmp;
                }
                else
                {
                    _mapper.Add(_current.Id, _current);
                }

                if (string.IsNullOrEmpty(_currParentId))
                    return;

                if (_current.Id == _currParentId)
                    ++_cyclesCount;
                else if (_addedToAncestor.Contains(_current.Id))
                    ++_multiParentCount;
                else
                {
                    _addedToAncestor.Add(_current.Id);

                    if (!_mapper.ContainsKey(_currParentId))
                        _mapper[_currParentId] = new Item(){Id = _currParentId};

                    Item tmp = _mapper[ _currParentId ];
                    if (tmp.Children == null)
                    {
                        tmp.Children = new List<string>();
                        tmp.Children.Add(_current.Id);
                        _mapper[_currParentId] = tmp;
                    }
                    else
                        tmp.Children.Add(_current.Id);
                }
            }
        }
        #endregion SAX methods

        void validateXml (string inPath)
        {
            var v = new XmlValidator(
                XmlReader.Create(
                new StringReader(
                    ResourceHelpers.GetStringResource("CommersantSchema.xsd", GetType().Assembly.GetName().Name + ".Resources")
            )), "\r\n");

            var res = v.Validate(inPath);

            if (!res.Item1)
                throw new Exception(string.Format("Xml '{0}' doesn't match the schema: {1}", inPath, res.Item2));
        }
    }
}
