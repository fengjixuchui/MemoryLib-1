#include "AsmTkWrapper.h"

void rm_cr(std::string& s) {
	for (auto p = s.find('\r'); p != std::string::npos; p = s.find('\r'))
		s.erase(p, 1);
}

using namespace asmjit;
using namespace asmtk;

MemLibNative::AsmTk::ASMTKPARSE MemLibNative::AsmTk::asmtk_parse_asm(const char* source,
	unsigned char* outbuffer,
	unsigned long long address,
	bool use64Bit) {
	ASMTKPARSE parse;
	parse.address = address;
	parse.x64 = use64Bit;

	if (!source || !outbuffer) {
		parse.status = ASMTKPARSE_ERROR;
		return parse;
	}

	CodeInfo code_info(parse.x64 ? ArchInfo::kTypeX64 : ArchInfo::kTypeX86, 0, parse.address);
	CodeHolder code;
	auto error = code.init(code_info);
	if (error != kErrorOk) {
		strcpy_s(parse.error, DebugUtils::errorAsString(error));
		parse.status = ASMTKPARSE_ERROR;
		return parse;
	}

	X86Assembler a(&code);
	AsmParser p(&a);

	std::string ssource(source);
	std::istringstream sstream(ssource);
	std::string line;
	auto linecounter = 1;

	while (std::getline(sstream, line, '\n')) {
		error = p.parse(line.c_str());
		if (error != kErrorOk) {
			rm_cr(line);
			std::ostringstream errorstring;
			errorstring << DebugUtils::errorAsString(error) << ": \"" << line << "\" (line: " << linecounter << ")";
			strcpy_s(parse.error, errorstring.str().c_str());
			parse.status = ASMTKPARSE_ERROR;
			return parse;
		}
		linecounter++;
	}

	code.sync();

	auto& buffer = code.getSectionEntry(0)->getBuffer();
	parse.dest_size = static_cast<unsigned int>(buffer.getLength());
	memcpy(outbuffer, buffer.getData(), parse.dest_size);

	return parse;
}
