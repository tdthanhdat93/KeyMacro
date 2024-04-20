#pragma once
#include "Windows.h"
#include <vector>
#include "ConvertManagedData.h"

using namespace System;
using namespace System::Runtime::InteropServices;

namespace ServiceKeyHookWrapper {

	delegate void HookDelagateNative(const KBDLLHOOKSTRUCT&);
	public delegate void HookDelegateManaged(System::UInt32, System::UInt32);

	public ref class APIWrapper
	{
	private:

		static void HookHandler(const KBDLLHOOKSTRUCT& hookInfo);
		static HookDelegateManaged^ _hookDelegate;
		static HookDelagateNative^ _cbHook;
	public:
		static void StartHook();
		static void StopHook();
		static void StartMacro();
		static void StopMacro();

		static void StartRecord(HookDelegateManaged^ hookDelegate);
		static void EndRecord();
		static void ReplayKeys(System::Collections::Generic::List<INPUT_Managed^>^ listKeys);
		static void BindKey(System::UInt32 keyCode, System::Collections::Generic::List<INPUT_Managed^>^ macro);
		static void ClearMacroOfKey(System::UInt32 keyCode);
		static void ClearAllMacro();
	};
}

namespace ImportAPI
{
	[DllImportAttribute("ServiceKeyHook.dll")]
		extern "C" void __stdcall StartHook();

	[DllImportAttribute("ServiceKeyHook.dll")]
		extern "C" void __stdcall StopHook();
	
	[DllImportAttribute("ServiceKeyHook.dll")]
		extern "C" void __stdcall StartMacro();

	[DllImportAttribute("ServiceKeyHook.dll")]
		extern "C" void __stdcall StopMacro();

	[DllImportAttribute("ServiceKeyHook.dll")]
		extern "C" void __stdcall RegisterRecordKey(ServiceKeyHookWrapper::HookDelagateNative^ cbHook);

	[DllImportAttribute("ServiceKeyHook.dll")]
		extern "C" void __stdcall UnRegisterRecordKey();

	[DllImportAttribute("ServiceKeyHook.dll")]
		extern "C" void __stdcall ReplayKeys(std::vector<INPUT>&keyInputs);

	[DllImportAttribute("ServiceKeyHook.dll")]
		extern "C" void __stdcall BindKey(DWORD keyCode, std::vector<INPUT>&macro);

	[DllImportAttribute("ServiceKeyHook.dll")]
		extern "C" void __stdcall ClearMacroOfKey(DWORD keyCode);

	[DllImportAttribute("ServiceKeyHook.dll")]
		extern "C" void __stdcall ClearAllMacro();
}