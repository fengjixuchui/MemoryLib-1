#pragma once
#include <msclr/marshal.h>
#include <keystone/keystone.h>

using namespace System;
using namespace Runtime::InteropServices;

namespace MemLibNative {
	namespace Keystone {
		public enum class KsMode {
			Mode16 = 2, // 16-bit mode
			Mode32 = 4, // 32-bit mode
			Mode64 = 8, // 64-bit mode
		};

		[Flags]
		public enum class KsSyntax {
			Intel = 1, // X86 Intel syntax - default on X86 (KS_OPT_SYNTAX).
			Att = 2, // X86 ATT asm syntax (KS_OPT_SYNTAX).
			Nasm = 4, // X86 Nasm syntax (KS_OPT_SYNTAX).
			//Masm = 8, // X86 Masm syntax (KS_OPT_SYNTAX) - unsupported yet.
			Gas = 16, // X86 GNU GAS syntax (KS_OPT_SYNTAX).
			Radix16 = 32, // All immediates are in hex format (i.e 12 is 0x12)
		};

		public ref class KeystoneEngine sealed : IDisposable {
		public:
			KeystoneEngine() : KeystoneEngine{KsMode::Mode64} {}

			explicit KeystoneEngine(KsMode mode) {
				m_IsDisposed = false;
				const auto ksmode = static_cast<ks_mode>(mode);
				const pin_ptr<ks_engine*> engine = &m_Engine;
				ks_open(KS_ARCH_X86, ksmode, engine);
				m_Syntax = KsSyntax::Intel;
			}

			!KeystoneEngine() { this->~KeystoneEngine(); }

			virtual ~KeystoneEngine() {
				if (!m_IsDisposed) {
					m_IsDisposed = true;
					ks_close(m_Engine);
				}
			}

		private:
			bool m_IsDisposed;
			ks_engine* m_Engine;
			KsSyntax m_Syntax;
		public:
			property KsSyntax Syntax
			{
				KsSyntax get() { return m_Syntax; }
				void set(KsSyntax value) {
					m_Syntax = value;
					ks_option(m_Engine, KS_OPT_SYNTAX, static_cast<size_t>(value));
				}
			}

			int GetLastError() {
				return static_cast<int>(ks_errno(m_Engine));
			}

			String^ GetLastErrorString() {
				return gcnew String(ks_strerror(ks_errno(m_Engine)));
			}

			bool Assemble(String^ source, [Out] array<Byte>^% buffer) {
				return Assemble(source, IntPtr::Zero, buffer);
			}

			bool Assemble(String^ source, const Int64 address, [Out] array<Byte>^% buffer) {
				return Assemble(source, IntPtr(address), buffer);
			}

			bool Assemble(String^ source, IntPtr address, [Out] array<Byte>^% buffer) {
				auto marshalcontext = gcnew msclr::interop::marshal_context();
				const auto marshal_source = marshalcontext->marshal_as<const char*>(source);

				unsigned char* encode;
				size_t size;
				size_t count;

				if (ks_asm(m_Engine, marshal_source, address.ToInt64(), &encode, &size, &count) != KS_ERR_OK) {
					ks_free(encode);
					return false;
				}

				buffer = gcnew array<Byte>(static_cast<int>(size));
				const pin_ptr<Byte> p_buffer = &buffer[0];
				memcpy(p_buffer, encode, size);

				ks_free(encode);
				return true;
			}

			array<Byte>^ Assemble(String^ source) {
				return Assemble(source, IntPtr::Zero);
			}

			array<Byte>^ Assemble(String^ source, const Int64 address) {
				return Assemble(source, IntPtr(address));
			}

			array<Byte>^ Assemble(String^ source, IntPtr address) {
				auto marshalcontext = gcnew msclr::interop::marshal_context();
				const auto marshal_source = marshalcontext->marshal_as<const char*>(source);

				unsigned char* encode;
				size_t size;
				size_t count;

				if (ks_asm(m_Engine, marshal_source, address.ToInt64(), &encode, &size, &count) != KS_ERR_OK) {
					ks_free(encode);
					return nullptr;
				}

				auto buffer = gcnew array<Byte>(static_cast<int>(size));
				const pin_ptr<Byte> p_buffer = &buffer[0];
				memcpy(p_buffer, encode, size);

				ks_free(encode);
				return buffer;
			}
		};
	}
}
