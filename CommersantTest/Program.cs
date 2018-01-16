using System;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.ExceptionServices;
using System.Threading;
using CommersantTest.Runtime;
using CommersantTest.RuntimeModel;

namespace CommersantTest
{
    public class Program
    {        
        [STAThread]
        [HandleProcessCorruptedStateExceptions]
        static void Main (string[] args)
        {
            try
            {                
                //устанавливаем обработчики исключений
                AppDomain.CurrentDomain.UnhandledException += currentDomain_UnhandledException;
                TaskScheduler.UnobservedTaskException += taskScheduler_UnobservedTaskException;

                //выполняем приложение
                new Program().run(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            AppDomain.CurrentDomain.UnhandledException -= currentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException -= taskScheduler_UnobservedTaskException;
        }

        static void application_ThreadException (object sender, ThreadExceptionEventArgs e)
        {            
            Console.WriteLine(string.Format("Unhandled Thread Exception: {0}", e.Exception));
        }

        static void currentDomain_UnhandledException (object sender, UnhandledExceptionEventArgs e)
        {            
            Console.WriteLine(string.Format("Unhandled Domain Exception: {0}", e.ExceptionObject.ToString()));
        }

        static void taskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs arg)
        {
            arg.Exception.Handle((ex) =>
            {
                Console.WriteLine(string.Format("Unobserved task's exception: {0}", ex));
                return true;
            });
            arg.SetObserved();
        }

        void run (string[] args)
        {
            string cmd, pathIn;
            if (args.Length == 1) //передан путь ко входному Xml
            {
                cmd = ProcessorFactory.UseMapper;
                pathIn = Path.GetFullPath(saveGetParam(args[0]));                
            }
            else if (args.Length == 2) //переданы метод обратоки Xml и путь ко входному Xml
            {
                cmd = args[0];
                pathIn = Path.GetFullPath(saveGetParam(args[1]));
            }
            else
                throw new Exception(string.Format("Unrecognized command line. Examples:\r\n" +
                    "\tCommersantTest path - process file by Mapper processor;\r\n" +
                    "\tCommersantTest {0} path - process file by Mapper processor;\r\n" +
                    "\tCommersantTest {1} path - process file by Xslt processor\r\n", ProcessorFactory.UseMapper, ProcessorFactory.UseXslt)
                );

            
            //обрабатываем Xml выбранным методом
            var processor = new ProcessorFactory().CreateProcessor(cmd);
            processor.ProcessData(pathIn, Path.Combine(Path.GetDirectoryName(pathIn), "out.xml"));
            //отображаем отчёт
            writeReport(processor, Path.GetFileName(pathIn));            
        }        

        static void writeReport (IDataProcessor p, string name)
        {
            Console.WriteLine("File '{0}' has been processed:\r\n" +
                "\tCycles = {1}\r\n" +
                "\tMultiparent nodes = {2}\r\n", name, p.CyclesCount, p.MultiParentCount
            );
        }

        static string saveGetParam(string v)
        {
            if (v == null)
                return string.Empty;
            return v.Trim();
        }
    }
}
