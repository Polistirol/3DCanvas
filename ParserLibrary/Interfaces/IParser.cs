namespace ParserLibrary.Interfaces
{
    public interface IParser
    {
        string PartProgramFilePath { get; set; }

        IProgramContext GetProgramContext();

        void ReadFile(string partProgramFilePath);
    }
}