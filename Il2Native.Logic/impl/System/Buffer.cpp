#include "CoreLib.h"

// Method : System.Buffer.BlockCopy(System.Array, int, System.Array, int, int)
void CoreLib::System::Buffer::BlockCopy(CoreLib::System::Array* src, int32_t srcOffset, CoreLib::System::Array* dst, int32_t dstOffset, int32_t count)
{
    throw 3221274624U;
}

// Method : System.Buffer.InternalBlockCopy(System.Array, int, System.Array, int, int)
void CoreLib::System::Buffer::InternalBlockCopy(CoreLib::System::Array* src, int32_t srcOffsetBytes, CoreLib::System::Array* dst, int32_t dstOffsetBytes, int32_t byteCount)
{
	if (byteCount <= 0)
	{
		return;
	}

	auto pSrc = &((__array<int8_t>*)src)->_data;
	auto pDest = &((__array<int8_t>*)dst)->_data;
	std::memcpy(pDest + dstOffsetBytes, pSrc + srcOffsetBytes, byteCount);
}

// Method : System.Buffer.IsPrimitiveTypeArray(System.Array)
bool CoreLib::System::Buffer::IsPrimitiveTypeArray(CoreLib::System::Array* array)
{
    throw 3221274624U;
}

// Method : System.Buffer._GetByte(System.Array, int)
uint8_t CoreLib::System::Buffer::_GetByte(CoreLib::System::Array* array, int32_t index)
{
    throw 3221274624U;
}

// Method : System.Buffer._SetByte(System.Array, int, byte)
void CoreLib::System::Buffer::_SetByte(CoreLib::System::Array* array, int32_t index, uint8_t value)
{
    throw 3221274624U;
}

// Method : System.Buffer._ByteLength(System.Array)
int32_t CoreLib::System::Buffer::_ByteLength(CoreLib::System::Array* array)
{
    throw 3221274624U;
}

// Method : System.Buffer.__Memmove(byte*, byte*, uint)
void CoreLib::System::Buffer::__Memmove(uint8_t* dest, uint8_t* src, uint32_t len)
{
    throw 3221274624U;
}