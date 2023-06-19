namespace PBIXInspectorLibrary
{
    internal class PbipFile : PbiFile, IDisposable
    {
        private bool disposedValue;
        private Stream _fileStream;

        public PbipFile(string pbiFilePath) : base(pbiFilePath)
        {
        }

        public override Stream GetEntryStream(string entryPath)
        {
            if (string.IsNullOrEmpty(entryPath)) return null;
            var filePath = Path.Combine(this.DirectoryPath, entryPath);
            if (!File.Exists(filePath)) { return null; }
            _fileStream = File.Open(filePath, FileMode.Open);
            return _fileStream;
        }

        protected void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    if (_fileStream != null)
                    {
                        _fileStream.Close();
                        _fileStream.Dispose();
                        _fileStream = null;
                    }
                }

                disposedValue = true;
            }
        }

        public override void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
