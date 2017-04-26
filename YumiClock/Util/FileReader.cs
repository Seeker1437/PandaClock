using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;


namespace YumiClock.Util
{
    public class FileReaderLine
    {
        public FileReaderLine(string line, string file)
        {
            Value = line;
            File = file != null ? Path.GetFullPath(file) : null;
        }

        public void SetValue(string value)
        {
            this.ParsedValue = value;
        }

        public string ParsedValue { get; private set; }
        public string Value { get; private set; }

        public string File { get; private set; }
    }

    public class FileReader : IEnumerable<FileReaderLine>, IDisposable
    {
        private readonly string _filePath;
        private readonly string _relativePath;
        private readonly StreamReader _streamReader;

        public FileReader(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File '" + filePath + "' not found.");

            _filePath = filePath;
            _relativePath = Path.GetDirectoryName(Path.GetFullPath(filePath));

            _streamReader = new StreamReader(filePath);
        }

        public FileReader(Stream stream)
        {
            _streamReader = new StreamReader(stream);
        }

        public int CurrentLine { get; protected set; }

        public void Dispose()
        {
            _streamReader.Close();
        }

        public IEnumerator<FileReaderLine> GetEnumerator()
        {
            string line;

            // Until EOF
            while ((line = _streamReader.ReadLine()) != null)
            {
                CurrentLine++;

                line = line.Trim();

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (line.Length < 2 || line[0] == '!' || line[0] == ';' || line[0] == '#' || line.StartsWith("//") ||
                    line.StartsWith("--"))
                    continue;

                bool require = false, divert = false;
                if (line.StartsWith("include ") || (require = line.StartsWith("require ")) ||
                    (divert = line.StartsWith("divert ")))
                {
                    var fileName = line.Substring(line.IndexOf(' ')).Trim(' ', '"');
                    var includeFilePath = Path.Combine((!fileName.StartsWith("/") ? _relativePath : ""),
                        fileName.TrimStart('/'));

                    if (includeFilePath != _filePath)
                    {
                        if (File.Exists(includeFilePath))
                        {
                            using (var fr = new FileReader(includeFilePath))
                            {
                                foreach (var incLine in fr)
                                    yield return incLine;
                            }

                            if (divert)
                                yield break;
                        }
                        else if (require)
                        {
                            throw new FileNotFoundException("Required file '" + includeFilePath + "' not found.");
                        }
                    }

                    continue;
                }

                yield return new FileReaderLine(line, _filePath);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
