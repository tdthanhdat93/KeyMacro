#pragma once
#include "Windows.h"

public ref class INPUT_Managed
{
protected:
	INPUT* m_pNativeData;

public:
	INPUT_Managed(System::UInt32 keyCode, System::UInt32 flags)
	{
		m_pNativeData = new INPUT();
		m_pNativeData->type = INPUT_KEYBOARD;
		m_pNativeData->ki.wVk = keyCode;
		m_pNativeData->ki.dwFlags = flags;
	}
	~INPUT_Managed()
	{
		delete m_pNativeData;
	}

	INPUT& ToUnmanaged()
	{
		return *m_pNativeData;
	}
};
