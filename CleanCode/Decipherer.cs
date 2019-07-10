using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace CleanCode
{
    class Decipherer
    {
        public Decipherer()
        {
            _translatedCodes = new List<string>();
        }

        public Decipherer(string entryFile)
        {
            _translatedCodes = new List<string>();
            _file = entryFile;
            getCode();
        }

        private string _file;

        private string _code;

        private List<string> _codeNumbers;

        private List<string> _translatedCodes;

        private const int ROW_COUNT = 3;

        private const int CHAR_PER_NUMBER = 3;

        private const int CODE_LENGHT = 9;


        private static readonly ReadOnlyCollection<string> _codex =
            new ReadOnlyCollection<string>(new[]
            {
                " _ | ||_|",
                "     |  |",
                " _  _||_ ",
                " _  _| _|",
                "   |_|  |",
                " _ |_  _|",
                " _ |_ |_|",
                " _   |  |",
                " _ |_||_|",
                " _ |_| _|"
            });

        public void TranslateEntry(string entryFile)
        {
            _translatedCodes = new List<string>();
            _file = entryFile;
            getCode();
        }

        public void TranslateEntrys(string[] entryFiles)
        {
            foreach (string entryFile in entryFiles)
            {
                _translatedCodes = new List<string>();
                _file = entryFile;
                getCode();
            }
        }

        private void getCode()
        {
            int fileIterator;
            Console.WriteLine("Fetching data from " + _file);
            try
            {
                using (StreamReader sr = new StreamReader(_file))
                {
                    while (!sr.EndOfStream)
                    {
                        _code = "";
                        fileIterator = -1;
                        while (++fileIterator <= ROW_COUNT)
                            _code += sr.ReadLine() + ".";
                        parseCode();
                    }
                }
                if (_code != "")
                {
                    Console.WriteLine("All data decoded");
                    saveResult();
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        private bool checkFormat(List<string> codeRows)
        {
            bool isOk = true;
            if (codeRows.Count < 4)
                isOk = false;
            else 
                for (int rowIterator = 0; rowIterator < ROW_COUNT; ++rowIterator)
                    if (codeRows[rowIterator].Length != 27)
                        isOk = false;
            return isOk;
        }

        private void parseCode()
        {
            List<string> codeRows = _code.Split(".").ToList();
            if (!checkFormat(codeRows))
            {
                Console.WriteLine("Bad Format, jumping to next code or file");
                _code = "";
                return;
            }
            _codeNumbers = new List<string>();

            for (int rowIterator = 0; rowIterator < ROW_COUNT; ++rowIterator)
                for (int numberIterator = 0; numberIterator < (codeRows[rowIterator].Length - 1); numberIterator += CHAR_PER_NUMBER)
                {
                    if (rowIterator != 0)
                        _codeNumbers[numberIterator / CHAR_PER_NUMBER] += codeRows[rowIterator].Substring(numberIterator, CHAR_PER_NUMBER);
                    else
                        _codeNumbers.Add(codeRows[rowIterator].Substring(numberIterator, CHAR_PER_NUMBER));
                }
            decode();
        }

        private bool checkSum(string result)
        {
            int sum = 0;
            for (int numberIterator = 0; numberIterator < CODE_LENGHT; ++numberIterator)
            {
                sum += (result[numberIterator] - 48) * (CODE_LENGHT - numberIterator);
            }
            if (sum % 11 == 0)
                return false;
            return true;
        }

        private void decode()
        {
            string result = "";
            bool decodeError = false;

            for (int numberIterator = 0; numberIterator < _codeNumbers.Count; ++numberIterator)
            {
                for (int codexIterator = 0; codexIterator <= CODE_LENGHT; ++codexIterator)
                {
                    if (_codex[codexIterator] == _codeNumbers[numberIterator])
                        result += codexIterator.ToString();
                }
                if (numberIterator == result.Length)
                {
                    result += "?";
                    decodeError = true;
                }
            }
            if (decodeError)
                result += " ILL";
            else if (checkSum(result))
                result += " ERR";
            _translatedCodes.Add(result + "\n");
        }

        private void saveResult()
        {
            Console.WriteLine("Saving data in " + _file.Replace(".txt", "-output.txt"));
            try
            {
                using (StreamWriter outputFile = new StreamWriter(_file.Replace(".txt", "-output.txt")))
                {
                    foreach (string translatedCode in _translatedCodes)
                        outputFile.WriteLine(translatedCode);
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be written:");
                Console.WriteLine(e.Message);
            }
        }
    }
}
