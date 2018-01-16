
namespace CommersantTest.RuntimeModel
{
    /// <summary>
    /// Интерфейс обработчика Xml
    /// </summary>
    interface IDataProcessor
    {
        /// <summary>
        /// Обрабатывает файл с Xml, преобразуя в Xml-файл с древовидной структурой.
        /// </summary>
        /// <param name="inPath">Путь к файлу с исходным плоским Xml</param>
        /// <param name="outPath">Путь к файлу с выходным Xml</param>
        /// <param name="validate">Если true, то валидировать входной Xml по схеме</param>
        void ProcessData(string inPath, string outPath, bool validate = true);

        /// <summary>
        /// После обработки возвращает число узлов, ссылающихся сами на себя.
        /// </summary>
        int CyclesCount
        {
            get;
        }

        /// <summary>
        /// После обработки возвращает число узлов, которые одновременно принадлежат многим родителям.
        /// </summary>
        int MultiParentCount
        {
            get;
        }
    }
}
