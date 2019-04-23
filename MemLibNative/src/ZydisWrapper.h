#pragma once
#include "Zydis/Zydis.h"

namespace MemLibNative {
	namespace Zydis {
		using namespace System;
		using System::Runtime::InteropServices::OutAttribute;

		public ref class ZydisWrapper : IDisposable {
		public:
			ZydisWrapper() { m_IsDisposed = false; }
			!ZydisWrapper() { this->~ZydisWrapper(); }
			virtual ~ZydisWrapper() {
				if (m_IsDisposed) return;
				m_IsDisposed = true;
			}
		private:
			bool m_IsDisposed;
		public:
			bool Test(array<Byte>^ data, [Out] String^% value) {
				ZydisDecoder mDecoder;
				if (!ZYAN_SUCCESS(ZydisDecoderInit(&mDecoder, ZYDIS_MACHINE_MODE_LONG_64, ZYDIS_ADDRESS_WIDTH_64))) {
					Console::WriteLine("ZydisDecoderInit");
					return false;
				}

				ZydisDecodedInstruction instruction;
				pin_ptr<unsigned char> p_data = &data[0];
				if(!ZYAN_SUCCESS(ZydisDecoderDecodeBuffer(&mDecoder, p_data, data->Length, &instruction))) {
					Console::WriteLine("ZydisDecoderDecodeBuffer");
					return false;
				}

				ZydisFormatter formatter;
				if (!ZYAN_SUCCESS(ZydisFormatterInit(&formatter, ZYDIS_FORMATTER_STYLE_INTEL))) {
					Console::WriteLine("ZydisFormatterInit");
					return false;
				}

				char buffer[260];
				if(!ZYAN_SUCCESS(ZydisFormatterFormatInstruction(&formatter, &instruction, buffer, 260, -1))) {
					Console::WriteLine("ZydisFormatterFormatInstruction");
					return false;
				}

				value = gcnew String(buffer);
				return true;
			}
		};
	}
}
