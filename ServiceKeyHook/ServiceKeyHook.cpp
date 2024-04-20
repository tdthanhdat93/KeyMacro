#include "pch.h"
#include <iostream>
#include <Windows.h>
#include <string>
#include <map>
#include <vector>
#include "ServiceKeyHook.h"
#include "Utility.h"

using namespace ServiceKeyHookAPI;

void ServiceKeyHookAPI::StartHook()
{
	_hhook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardProc, NULL, 0);
	if (_hhook != NULL)
	{
		OutputDebugString(L"Installed Windows hook successfully\n");
	}
	else
	{
		std::string errMsg = GetLastErrorAsString();
		printf("Install Windows hook failed. Error message: %s", errMsg.c_str());
	}
}

SERVICEKEYHOOK_API void __stdcall ServiceKeyHookAPI::StopHook()
{
	BOOL bRet = UnhookWindowsHookEx(_hhook);
}

static LRESULT ServiceKeyHookAPI::KeyboardProc(int nCode, WPARAM wParam, LPARAM lParam)
{
	if (nCode == HC_ACTION)
	{
		KBDLLHOOKSTRUCT* info = (KBDLLHOOKSTRUCT*)lParam;
		if (_cbRecord != nullptr)
		{
			try
			{
				_cbRecord(*info);
			}
			catch(...)
			{ 
				OutputDebugString(L"Exception on _cbRecord(*info)");
			}
			return CallNextHookEx(NULL, nCode, wParam, lParam);
		}

		if (wParam == WM_KEYDOWN && !IsInjectedKey(info->flags))
		{
			if (InjectKey(info->vkCode))
			{
				CallNextHookEx(NULL, nCode, wParam, lParam);
				return 1;
			}
		}
	}
	return CallNextHookEx(NULL, nCode, wParam, lParam);
}

static BOOL ServiceKeyHookAPI::IsInjectedKey(DWORD flags)
{
	return flags & LLKHF_INJECTED;
}

static BOOL ServiceKeyHookAPI::InjectKey(DWORD keyCode)
{
	if (_mapKeys.find(keyCode) != _mapKeys.end())
	{
		for (auto& input : _mapKeys[keyCode])
		{
			UINT uSent = SendInput(1, &input, sizeof(INPUT));
		}

		return TRUE;
	}
	return FALSE;
}

void __stdcall ServiceKeyHookAPI::RegisterRecordKey(HookCallback cbRecord)
{
	ServiceKeyHookAPI::_cbRecord = cbRecord;
}

void ServiceKeyHookAPI::UnRegisterRecordKey()
{
	ServiceKeyHookAPI::_cbRecord = nullptr;
}

SERVICEKEYHOOK_API void __stdcall ServiceKeyHookAPI::ReplayKeys(std::vector<INPUT>& keyInputs)
{
	for (auto& input : keyInputs)
	{
		UINT uSent = SendInput(1, &input, sizeof(INPUT));
	}
}
