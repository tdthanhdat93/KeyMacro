#include "pch.h"

#include "ServiceKeyHookWrapper.h"

void ServiceKeyHookWrapper::APIWrapper::HookHandler(const KBDLLHOOKSTRUCT& hookInfo)
{
	_hookDelegate(hookInfo.vkCode, hookInfo.flags);
}

void ServiceKeyHookWrapper::APIWrapper::StartHook()
{
	ImportAPI::StartHook();
}

void ServiceKeyHookWrapper::APIWrapper::StopHook()
{
	ImportAPI::StopHook();
}

void ServiceKeyHookWrapper::APIWrapper::StartMacro()
{
	ImportAPI::StartMacro();
}

void ServiceKeyHookWrapper::APIWrapper::StopMacro()
{
	ImportAPI::StopMacro();
}

void ServiceKeyHookWrapper::APIWrapper::StartRecord(HookDelegateManaged^ hookDelegate)
{
	_hookDelegate = hookDelegate;
	_cbHook = gcnew HookDelagateNative(APIWrapper::HookHandler);
	ImportAPI::RegisterRecordKey(_cbHook);
}

void ServiceKeyHookWrapper::APIWrapper::EndRecord()
{
	ImportAPI::UnRegisterRecordKey();
}

void ServiceKeyHookWrapper::APIWrapper::ReplayKeys(System::Collections::Generic::List<INPUT_Managed^>^ listKeys)
{
	std::vector<INPUT> inputs;
	for each (INPUT_Managed^ key in listKeys)
	{
		inputs.push_back(key->ToUnmanaged());
	}
	ImportAPI::ReplayKeys(inputs);
}

void ServiceKeyHookWrapper::APIWrapper::BindKey(System::UInt32 keyCode, System::Collections::Generic::List<INPUT_Managed^>^ macro)
{
	std::vector<INPUT> macroNative;
	for each (INPUT_Managed ^ key in macro)
	{
		macroNative.push_back(key->ToUnmanaged());
	}
	ImportAPI::BindKey(keyCode, macroNative);
}

void ServiceKeyHookWrapper::APIWrapper::ClearMacroOfKey(System::UInt32 keyCode)
{
	ImportAPI::ClearMacroOfKey(keyCode);
}

void ServiceKeyHookWrapper::APIWrapper::ClearAllMacro()
{
	ImportAPI::ClearAllMacro();
}

