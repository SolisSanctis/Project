using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTI
{
    class Program
    {
        static void Main(string[] args)
        {
            Scanner scanner = new Scanner("C:/Users/The Flash/Desktop/NEWPRO/IZ/IZ/test.cl");  // создать сканер
            Parser parser = new Parser(scanner);   // создать парсер
            parser.Parse();                         // запустить парсер
            Console.WriteLine("Обнаружено ошибок: " + parser.errors.count);
        }
    }
}
