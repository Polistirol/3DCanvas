namespace ParserLibrary.Interfaces
{
    public interface IParser
    {
        string Filename { get; set; }

        IProgramContext GetProgramContext();
    }
}