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
		static void StartRecord(HookDelegateManaged^ hookDelegate);
		static void EndRecord();
		static void ReplayKeys(System::Collections::Generic::List<INPUT_Managed^>^ listKeys);
	};
}

namespace ImportAPI
{
	[DllImportAttribute("ServiceKeyHook.dll")]
		extern "C" void __stdcall StartHook();

	[DllImportAttribute("ServiceKeyHook.dll")]
		extern "C" void __stdcall StopHook();

	[DllImportAttribute("ServiceKeyHook.dll")]
		extern "C" void __stdcall RegisterRecordKey(ServiceKeyHookWrapper::HookDelagateNative^ cbHook);

	[DllImportAttribute("ServiceKeyHook.dll")]
		extern "C" void __stdcall UnRegisterRecordKey();

	[DllImportAttribute("ServiceKeyHook.dll")]
		extern "C" void __stdcall ReplayKeys(std::vector<INPUT>&keyInputs);
}