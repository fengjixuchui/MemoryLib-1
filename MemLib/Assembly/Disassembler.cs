using System;
using System.Collections.Generic;
using System.Linq;
using MemLibNative.Zydis;

namespace MemLib.Assembly {
    public sealed class Disassembler : IDisposable {
        private readonly Zydisasm m_Zydisasm;

        public DisasmSyntax Syntax {
            get => (DisasmSyntax)m_Zydisasm.Style;
            set => m_Zydisasm.Style = (ZydisasmStyle)value;
        }

        public DisasmMode Mode {
            get => (DisasmMode)m_Zydisasm.Mode;
            set => m_Zydisasm.Mode = (ZydisasmMode)value;
        }

        public DisasmAddressWidth AddressWidth {
            get => (DisasmAddressWidth)m_Zydisasm.AddressWidth;
            set => m_Zydisasm.AddressWidth = (ZydisasmAddressWidth)value;
        }

        public Disassembler() : this(Environment.Is64BitProcess) { }
        public Disassembler(bool use64Bit) {
            m_Zydisasm = new Zydisasm(use64Bit ? ZydisasmMode.Mode64 : ZydisasmMode.Mode32) {
                Style = ZydisasmStyle.Intel
            };
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
            if (!m_Zydisasm.Disassemble(data, address, out var insnList)) {
                instructions = null;
                return false;
            }
            instructions = new List<InstructionData>(insnList.Count);
            foreach (var instruction in insnList) {
                instructions.Add(new InstructionData {
                    Length = instruction.Length,
                    Address = new IntPtr(instruction.Address),
                    Instruction = instruction.Instruction,
                    Data = data.Skip(instruction.Offset).Take(instruction.Length).ToArray()
                });
            }
            return true;
        }

        #endregion

        #region DisassembleLine

        public InstructionData DisassembleLine(byte[] data) {
            return DisassembleLine(data, 0);
        }

        public InstructionData DisassembleLine(byte[] data, IntPtr address) {
            return DisassembleLine(data, address.ToInt64());
        }

        public InstructionData DisassembleLine(byte[] data, long address) {
            if (DisassembleLine(data, address, out var instruction))
                return instruction;
            return null;
        }

        public bool DisassembleLine(byte[] data, out InstructionData instruction) {
            return DisassembleLine(data, 0, out instruction);
        }

        public bool DisassembleLine(byte[] data, IntPtr address, out InstructionData instruction) {
            return DisassembleLine(data, address.ToInt64(), out instruction);
        }

        public bool DisassembleLine(byte[] data, long address, out InstructionData instruction) {
            if (!m_Zydisasm.DisassembleLine(data, address, out var insn)) {
                instruction = null;
                return false;
            }

            instruction = new InstructionData {
                Length = insn.Length,
                Address = new IntPtr(address),
                Instruction = insn.Instruction,
                Data = data.Take(insn.Length).ToArray()
            };
            return true;
        }

        #endregion

        #region IDisposable

        void IDisposable.Dispose() {
            GC.SuppressFinalize(this);
        }

        ~Disassembler() {
            ((IDisposable) this).Dispose();
        }

        #endregion
    }
}