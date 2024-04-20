#pragma once
#include <Windows.h>
#include <string>
#include <functional>
#include <map>
#include <vector>

#ifdef SERVICEKEYHOOK_EXPORTS
#define SERVICEKEYHOOK_API __declspec(dllexport)
#else
#define SERVICEKEYHOOK_API __declspec(dllimport)
#endif // SERVICEKEYHOOL_EXPORTS

//typedef void(*HookCallback)(const KBDLLHOOKSTRUCT&);
using HookCallback = void(*)(const KBDLLHOOKSTRUCT&) ;

namespace ServiceKeyHookAPI
{
	HHOOK _hhook = 0;
	std::map<DWORD, std::vector<INPUT>> _mapKeys;
	HookCallback _cbRecord;

	static BOOL IsInjectedKey(DWORD flags);
	static BOOL InjectKey(DWORD keyCode);
	static LRESULT KeyboardProc(int nCode, WPARAM wParam, LPARAM lParam);

	extern "C" SERVICEKEYHOOK_API void __stdcall StartHook();
	extern "C" SERVICEKEYHOOK_API void __stdcall StopHook();

	extern "C" SERVICEKEYHOOK_API void __stdcall RegisterRecordKey(HookCallback cbRecord);
	extern "C" SERVICEKEYHOOK_API void __stdcall UnRegisterRecordKey();

	extern "C" SERVICEKEYHOOK_API void __stdcall ReplayKeys(std::vector<INPUT>& keyInputs);
}