#pragma once
#include <string.h>
#include <Zydis/Zydis.h>

using namespace System;
using namespace Collections::Generic;
using namespace Runtime::InteropServices;

namespace MemLibNative {
	namespace Zydis {
		public enum class ZydisasmMode {
			Mode64 = ZYDIS_MACHINE_MODE_LONG_64,
			Mode32 = ZYDIS_MACHINE_MODE_LONG_COMPAT_32,
			Mode16 = ZYDIS_MACHINE_MODE_LONG_COMPAT_16
		};
		public enum class ZydisasmAddressWidth {
			Width16 = ZYDIS_ADDRESS_WIDTH_16,
			Width32 = ZYDIS_ADDRESS_WIDTH_32,
			Width64 = ZYDIS_ADDRESS_WIDTH_64
		};
		public enum class ZydisasmStyle {
			Att = ZYDIS_FORMATTER_STYLE_ATT,
			Intel = ZYDIS_FORMATTER_STYLE_INTEL,
			Masm = ZYDIS_FORMATTER_STYLE_INTEL_MASM  //Ignores address
		};
		public value struct ZydisasmInstruction sealed {
			array<Byte>^ Buffer;
			String^ Instruction;
			int Length;
			long long Address;
		};

		public ref class Zydisasm sealed {
		private:
			ZydisasmMode m_Mode;
			ZydisasmAddressWidth m_AddressWidth;
			ZydisasmStyle m_Style;
		public:
			explicit Zydisasm(const ZydisasmMode mode) {
				m_Mode = mode;
				switch (mode) {
				case ZydisasmMode::Mode64:
					m_AddressWidth = ZydisasmAddressWidth::Width64;
					break;
				case ZydisasmMode::Mode32:
					m_AddressWidth = ZydisasmAddressWidth::Width32;
					break;
				case ZydisasmMode::Mode16:
					m_AddressWidth = ZydisasmAddressWidth::Width16;
					break;
				default:
					throw gcnew ArgumentException(String::Format("Invalid mode '{0}'", mode), "mode");
				}
				m_Style = ZydisasmStyle::Intel;
			}
			Zydisasm(const ZydisasmMode mode, const ZydisasmAddressWidth addressWidth) {
				m_Mode = mode;
				m_AddressWidth = addressWidth;
				m_Style = ZydisasmStyle::Intel;
			}
			property ZydisasmMode Mode { ZydisasmMode get() { return m_Mode; } 	void set(ZydisasmMode mode) { m_Mode = mode; }}
			property ZydisasmAddressWidth AddressWidth { ZydisasmAddressWidth get() { return m_AddressWidth; } void set(ZydisasmAddressWidth width) { m_AddressWidth = width; }}
			property ZydisasmStyle Style { ZydisasmStyle get() { return m_Style; } void set(ZydisasmStyle style) { m_Style = style; }}
		public:
			List<ZydisasmInstruction>^ Disassemble(array<Byte>^ data) {
				return Disassemble(data, 0);
			}
			List<ZydisasmInstruction>^ Disassemble(array<Byte>^ data, IntPtr address) {
				return Disassemble(data, address.ToInt64());
			}
			bool Disassemble(array<Byte>^ data, [Out] List<ZydisasmInstruction>^% instructions) {
				return Disassemble(data, 0, instructions);
			}
			bool Disassemble(array<Byte>^ data, IntPtr address, [Out] List<ZydisasmInstruction>^% instructions) {
				return Disassemble(data, address.ToInt64(), instructions);
			}
			ZydisasmInstruction DisassembleLine(array<Byte>^ data) {
				return DisassembleLine(data, 0);
			}
			ZydisasmInstruction DisassembleLine(array<Byte>^ data, IntPtr address) {
				return DisassembleLine(data, address.ToInt64());
			}
			bool DisassembleLine(array<Byte>^ data, [Out] ZydisasmInstruction% instruction) {
				return DisassembleLine(data, 0, instruction);
			}
			bool DisassembleLine(array<Byte>^ data, IntPtr address, [Out] ZydisasmInstruction% instruction) {
				return DisassembleLine(data, address.ToInt64(), instruction);
			}

			List<ZydisasmInstruction>^ Disassemble(array<Byte>^ data, long long address) {
				const auto zydismode = static_cast<ZydisMachineMode>(m_Mode);
				const auto zydiswidth = static_cast<ZydisAddressWidth>(m_AddressWidth);
				const auto zydisstyle = static_cast<ZydisFormatterStyle>(m_Style);
				auto insnlist = gcnew List<ZydisasmInstruction>();

				ZydisDecoder decoder;
				if (!ZYAN_SUCCESS(ZydisDecoderInit(&decoder, zydismode, zydiswidth)))
					return insnlist;
				ZydisFormatter formatter;
				if (!ZYAN_SUCCESS(ZydisFormatterInit(&formatter, zydisstyle)))
					return insnlist;

				ZyanU64 runtime_address = address;
				ZyanUSize offset = 0;
				const ZyanUSize length = data->Length;
				ZydisDecodedInstruction instruction;

				pin_ptr<unsigned char> p_data = &data[0];
				auto buffer = new unsigned char[length];
				memcpy_s(buffer, length, p_data, length);

				while(ZYAN_SUCCESS(ZydisDecoderDecodeBuffer(&decoder, buffer + offset, length - offset, &instruction))) {
					char textbuffer[256];
					if (ZYAN_SUCCESS(ZydisFormatterFormatInstruction(&formatter, &instruction, textbuffer, sizeof textbuffer, runtime_address))) {
						ZydisasmInstruction insn;
						insn.Address = runtime_address;
						insn.Length = instruction.length;
						insn.Instruction = gcnew String(textbuffer);
						insn.Buffer = gcnew array<Byte>(instruction.length);
						pin_ptr<Byte> buf_tmp = &insn.Buffer[0];
						memcpy_s(buf_tmp, instruction.length, buffer + offset, instruction.length);
						insnlist->Add(insn);
					}
					offset += instruction.length;
					runtime_address += instruction.length;
				}

				delete[] buffer;
				return insnlist;
			}

			bool Disassemble(array<Byte>^ data, long long address, [Out] List<ZydisasmInstruction>^% instructions) {
				const auto zydismode = static_cast<ZydisMachineMode>(m_Mode);
				const auto zydiswidth = static_cast<ZydisAddressWidth>(m_AddressWidth);
				const auto zydisstyle = static_cast<ZydisFormatterStyle>(m_Style);
				
				ZydisDecoder decoder;
				if (!ZYAN_SUCCESS(ZydisDecoderInit(&decoder, zydismode, zydiswidth))) {
					instructions = nullptr;
					return false;
				}
				ZydisFormatter formatter;
				if (!ZYAN_SUCCESS(ZydisFormatterInit(&formatter, zydisstyle))) {
					instructions = nullptr;
					return false;
				}
				auto insnlist = gcnew List<ZydisasmInstruction>();

				ZyanU64 runtime_address = address;
				ZyanUSize offset = 0;
				const ZyanUSize length = data->Length;
				ZydisDecodedInstruction instruction;

				pin_ptr<unsigned char> p_data = &data[0];
				auto buffer = new unsigned char[length];
				memcpy_s(buffer, length, p_data, length);

				while (ZYAN_SUCCESS(ZydisDecoderDecodeBuffer(&decoder, buffer + offset, length - offset, &instruction))) {
					char textbuffer[256];
					if (ZYAN_SUCCESS(ZydisFormatterFormatInstruction(&formatter, &instruction, textbuffer, sizeof textbuffer, runtime_address))) {
						ZydisasmInstruction insn;
						insn.Address = runtime_address;
						insn.Length = instruction.length;
						insn.Instruction = gcnew String(textbuffer);
						insn.Buffer = gcnew array<Byte>(instruction.length);
						pin_ptr<Byte> buf_tmp = &insn.Buffer[0];
						memcpy_s(buf_tmp, instruction.length, buffer + offset, instruction.length);
						insnlist->Add(insn);
					} else {
						delete[] buffer;
						instructions = nullptr;
						return false;
					}
					offset += instruction.length;
					runtime_address += instruction.length;
				}

				delete[] buffer;
				instructions = insnlist;
				return true;
			}

			ZydisasmInstruction DisassembleLine(array<Byte>^ data, long long address) {
				const auto zydismode = static_cast<ZydisMachineMode>(m_Mode);
				const auto zydiswidth = static_cast<ZydisAddressWidth>(m_AddressWidth);
				const auto zydisstyle = static_cast<ZydisFormatterStyle>(m_Style);
				ZydisasmInstruction insn;

				ZydisDecoder decoder;
				if (ZYAN_SUCCESS(ZydisDecoderInit(&decoder, zydismode, zydiswidth))) {
					ZydisDecodedInstruction instruction;
					pin_ptr<unsigned char> p_data = &data[0];
					if (ZYAN_SUCCESS(ZydisDecoderDecodeBuffer(&decoder, p_data, data->Length, &instruction))) {
						ZydisFormatter formatter;
						if (ZYAN_SUCCESS(ZydisFormatterInit(&formatter, zydisstyle))) {
							char buffer[256];
							if (ZYAN_SUCCESS(ZydisFormatterFormatInstruction(&formatter, &instruction, buffer, 256, address))) {
								insn.Length = instruction.length;
								insn.Address = address;
								insn.Instruction = gcnew String(buffer);
								insn.Buffer = gcnew array<Byte>(instruction.length);
								Buffer::BlockCopy(data, 0, insn.Buffer, 0, instruction.length);
								return insn;
							}
						}
					}
				}
				return insn;
			}
		
			bool DisassembleLine(array<Byte>^ data, long long address, [Out] ZydisasmInstruction% instruction) {
				const auto zydismode = static_cast<ZydisMachineMode>(m_Mode);
				const auto zydiswidth = static_cast<ZydisAddressWidth>(m_AddressWidth);
				const auto zydisstyle = static_cast<ZydisFormatterStyle>(m_Style);
				ZydisasmInstruction insn;

				ZydisDecoder decoder;
				if (ZYAN_SUCCESS(ZydisDecoderInit(&decoder, zydismode, zydiswidth))) {
					ZydisDecodedInstruction zinstruction;
					pin_ptr<unsigned char> p_data = &data[0];
					if (ZYAN_SUCCESS(ZydisDecoderDecodeBuffer(&decoder, p_data, data->Length, &zinstruction))) {
						ZydisFormatter formatter;
						if (ZYAN_SUCCESS(ZydisFormatterInit(&formatter, zydisstyle))) {
							char buffer[256];
							if (ZYAN_SUCCESS(ZydisFormatterFormatInstruction(&formatter, &zinstruction, buffer, 256, address))) {
								insn.Length = zinstruction.length;
								insn.Address = address;
								insn.Instruction = gcnew String(buffer);
								insn.Buffer = gcnew array<Byte>(zinstruction.length);
								Buffer::BlockCopy(data, 0, insn.Buffer, 0, zinstruction.length);
								instruction = insn;
								return true;
							}
						}
					}
				}
				return false;
			}
		};
	}
}
