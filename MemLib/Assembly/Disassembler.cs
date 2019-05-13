using System;
using System.Collections.Generic;
using MemLibNative.Capstone;

namespace MemLib.Assembly {
    public sealed class Disassembler : IDisposable {
        private readonly CapstoneEngine m_Engine;

        public DisasmSyntax Syntax {
            get => (DisasmSyntax)m_Engine.Syntax;
            set => m_Engine.Syntax = (CapstoneSyntax)value;
        }

        public DisasmMode Mode {
            get => (DisasmMode)m_Engine.Mode;
            set => m_Engine.Mode = (CapstoneMode)value;
        }
        
        public Disassembler() : this(Environment.Is64BitProcess) { }
        public Disassembler(bool use64Bit) {
            m_Engine = new CapstoneEngine(use64Bit ? CapstoneMode.Mode64 : CapstoneMode.Mode32);
        }

        #region Disassemble

        public List<InstructionData> Disassemble(byte[] data) {
            return Disassemble(data, 0);
        }

        public List<InstructionData> Disassemble(byte[] data, IntPtr address) {
            return Disassemble(data, address.ToInt64());
        }

        public List<InstructionData> Disassemble(byte[] data, long address) {
            if (Disassemble(data, address, out var instructions))
                return instructions;
            return new List<InstructionData>();
        }

        public bool Disassemble(byte[] data, out List<InstructionData> instructions) {
            return Disassemble(data, 0, out instructions);
        }

        public bool Disassemble(byte[] data, IntPtr address, out List<InstructionData> instructions) {
            return Disassemble(data, address.ToInt64(), out instructions);
        }

        public bool Disassemble(byte[] data, long address, out List<InstructionData> instructions) {
            if (!m_Engine.Disassemble(data, address, out var insnList)) {
                instructions = null;
                return false;
            }
            instructions = new List<InstructionData>(insnList.Count);
            foreach (var insn in insnList) {
                instructions.Add(new InstructionData {
                    Size = (int)insn.Size,
                    Address = new IntPtr(insn.Address),
                    Mnemonic = insn.Mnemonic,
                    OpString = insn.OpString,
                    Bytes = insn.Bytes
                });
            }
            return true;
        }

        #endregion

        #region DisassembleSingle

        public InstructionData DisassembleSingle(byte[] data) {
            return DisassembleSingle(data, 0);
        }

        public InstructionData DisassembleSingle(byte[] data, IntPtr address) {
            return DisassembleSingle(data, address.ToInt64());
        }

        public InstructionData DisassembleSingle(byte[] data, long address) {
            if (DisassembleSingle(data, address, out var instruction))
                return instruction;
            return null;
        }

        public bool DisassembleSingle(byte[] data, out InstructionData instruction) {
            return DisassembleSingle(data, 0, out instruction);
        }

        public bool DisassembleSingle(byte[] data, IntPtr address, out InstructionData instruction) {
            return DisassembleSingle(data, address.ToInt64(), out instruction);
        }

        public bool DisassembleSingle(byte[] data, long address, out InstructionData instruction) {
            if (!m_Engine.DisassembleSingle(data, address, out var insn)) {
                instruction = null;
                return false;
            }

            instruction = new InstructionData {
                Size = (int)insn.Size,
                Address = new IntPtr(insn.Address),
                Mnemonic = insn.Mnemonic,
                OpString = insn.OpString,
                Bytes = insn.Bytes
            };
            return true;
        }

        #endregion

        #region IDisposable

        void IDisposable.Dispose() {
            m_Engine?.Dispose();
            GC.SuppressFinalize(this);
        }

        ~Disassembler() {
            ((IDisposable) this).Dispose();
        }

        #endregion
    }
}