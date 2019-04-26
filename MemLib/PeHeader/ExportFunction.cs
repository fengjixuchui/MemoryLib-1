namespace MemLib.PeHeader {
    public class ExportFunction {
        public string Name { get; }
        public int RelativeAddress { get; }
        public int Ordinal { get; }

        public ExportFunction(string name, int address, int ordinal) {
            Name = name;
            RelativeAddress = address;
            Ordinal = ordinal;
        }

        public override string ToString() => $"RelativeAddress: {RelativeAddress:X}, Ordinal: {Ordinal:X}, Name: {Name}";
    }
}