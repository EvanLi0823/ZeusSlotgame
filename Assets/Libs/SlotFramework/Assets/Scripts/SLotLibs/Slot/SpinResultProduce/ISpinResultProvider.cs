public interface ISpinResultProvider
{
    string Name { get; set; }
    object Decode();
}
