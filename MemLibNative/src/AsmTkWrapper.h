#pragma once
#include <asmtk/asmtk.h>
#include <sstream>
#include <msclr/marshal.h>

namespace MemLibNative {
	namespace AsmTk {

		enum ASMTKPARSE_STATUS {
			ASMTKPARSE_ERROR = 0,
			ASMTKPARSE_OK = 1
		};

		struct ASMTKPARSE {
			bool x64;
			unsigned long long address;
			unsigned int dest_size;
			char error[256];
			ASMTKPARSE_STATUS status;

			ASMTKPARSE() : x64(true), address(0), dest_size(0), error{ 0 }, status(ASMTKPARSE_OK){}
		};

		ASMTKPARSE asmtk_parse_asm(const char* source, unsigned char* outbuffer, unsigned long long address = 0, bool use64Bit = true);

		using namespace System;
		using System::Runtime::InteropServices::OutAttribute;

		public ref class AsmTkWrapper : IDisposable {
		public:
			AsmTkWrapper() { m_IsDisposed = false; }
			!AsmTkWrapper() { this->~AsmTkWrapper(); }
			virtual ~AsmTkWrapper() {
				if (m_IsDisposed) return;
				m_IsDisposed = true;
			}
		private:
			bool m_IsDisposed;
		public:
			bool Test(String^ source, [Out] array<Byte>^% value) {

				auto marshalcontext = gcnew msclr::interop::marshal_context();
				const auto marshal_source = marshalcontext->marshal_as<const char*>(source);
				unsigned char outbuff[1024];

				const auto parse = asmtk_parse_asm(marshal_source, outbuff, 0, true);
				if (parse.status == ASMTKPARSE_ERROR) {
					Console::WriteLine(msclr::interop::marshal_as<String^>(parse.error));
					return false;
				}

				value = gcnew array<Byte>(parse.dest_size);
				const pin_ptr<Byte> p_value = &value[0];
				memcpy(p_value, outbuff, parse.dest_size);

				return true;
			}
		};
	}
}
