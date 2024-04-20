// ConsoleApplication1_C++.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include <Windows.h>
#include <string>
#include <bitset>
#include <map>
#include <vector>
#include "ServiceKeyHook.h"

std::map<DWORD, std::vector<INPUT>> _mapKeys;

int main()
{
	HHOOK hhook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardProc, NULL, 0);
	if (hhook != NULL)
	{
		printf("Installed Windows hook successfully\n");

		printf("Init map keys...\n");
		InitMapKeys();
		printf("Ready!\n");

		BOOL bRet;
		MSG msg;
		while ((bRet = GetMessage(&msg, NULL, 0, 0)) != 0)
		{
			if (bRet == -1)
			{
				// handle the error and possibly exit
			}
			else
			{
				TranslateMessage(&msg);
				DispatchMessage(&msg);
			}
		}
	}
	else
	{
		std::string errMsg = GetLastErrorAsString();
		printf("Install Windows hook failed. Error message: %s", errMsg.c_str());
	}

}

static LRESULT KeyboardProc(int nCode, WPARAM wParam, LPARAM lParam)
{
	if (nCode == HC_ACTION)
	{
		std::string action;
		switch (wParam)
		{
		case WM_KEYDOWN:
			action = "DOWN";
			break;
		case WM_KEYUP:
			action = "UP";
			break;
		case WM_SYSKEYDOWN:
			action = "ALT down";
			break;
		case WM_SYSKEYUP:
			action = "ALT up";
			break;
		default:
			break;
		}

		KBDLLHOOKSTRUCT* info = (KBDLLHOOKSTRUCT*)lParam;
		BYTE lpKeyState[256];
		BOOL bRet = GetKeyboardState(lpKeyState);
		if (shift_active()) {
			//printf("[Shift] ON\n");
			lpKeyState[VK_SHIFT] = 0x80;
		}
		if (capital_active()) {
			lpKeyState[VK_CAPITAL] = 0x01;
		}

		WORD keyAscii[2];
		int chars = ToAscii(info->vkCode, info->scanCode, lpKeyState, keyAscii, 0);
		if (chars > 0)
		{
			printf("%#x (%c) %s\t\t| scancode: %s | flags: %s\n", info->vkCode, keyAscii[0], action.c_str(), Value_8Bits_AsString(info->scanCode).c_str(), Value_8Bits_AsString(info->flags).c_str());
		}
		else
		{
			printf("%#x %s\t\t| scancode: %s | flags: %s\n", info->vkCode, action.c_str(), Value_8Bits_AsString(info->scanCode).c_str(), Value_8Bits_AsString(info->flags).c_str());
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

static void InitMapKeys()
{
	_mapKeys['Q'] = BindKey('Q', "WWQR");
	_mapKeys['W'] = BindKey('W', "WWWR");
	_mapKeys['E'] = BindKey('E', "EEWR");
	_mapKeys['R'] = BindKey('R', "QWER");
	_mapKeys['T'] = BindKey('R', "EEQR");
	_mapKeys['A'] = BindKey('A', "QQQR");
	_mapKeys['S'] = BindKey('S', "EEER");
	_mapKeys['G'] = BindKey('G', "WWER");
	_mapKeys[VK_OEM_3] = BindKey(VK_OEM_3, "QQER");	//~`
	_mapKeys[VK_CAPITAL] = BindKey(VK_CAPITAL, "QQWR");
}

std::vector<INPUT> BindKey(DWORD keyCode, std::string keyMacros)
{
	std::vector<INPUT> bindKeys;
	INPUT input;

	for (char ch : keyMacros)
	{
		SetKeyInput(&input, ch);
		bindKeys.push_back(input);

		SetKeyInput(&input, ch, KEYEVENTF_KEYUP);
		bindKeys.push_back(input);
	}

	return bindKeys;
}

static void SetKeyInput(INPUT* pInput, WORD keyCode, DWORD dwFlags)
{
	ZeroMemory(pInput, sizeof(INPUT));
	pInput->type = INPUT_KEYBOARD;
	pInput->ki.wVk = keyCode;
	pInput->ki.dwFlags = dwFlags;
}

static BOOL IsInjectedKey(DWORD flags)
{
	return flags & LLKHF_INJECTED;
}

static BOOL InjectKey(DWORD keyCode)
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


//Returns the last Win32 error, in string format. Returns an empty string if there is no error.
static std::string GetLastErrorAsString()
{
	//Get the error message ID, if any.
	DWORD errorMessageID = ::GetLastError();
	if (errorMessageID == 0) {
		return std::string(); //No error message has been recorded
	}

	LPSTR messageBuffer = nullptr;

	//Ask Win32 to give us the string version of that message ID.
	//The parameters we pass in, tell Win32 to create the buffer that holds the message for us (because we don't yet know how long the message string will be).
	size_t size = FormatMessage(
		FORMAT_MESSAGE_ALLOCATE_BUFFER |
		FORMAT_MESSAGE_FROM_SYSTEM |
		FORMAT_MESSAGE_IGNORE_INSERTS,
		NULL,
		errorMessageID,
		MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
		(LPWSTR)&messageBuffer,
		0, NULL);

	//Copy the error message into a std::string.
	std::string message(messageBuffer, size);

	//Free the Win32's string's buffer.
	LocalFree(messageBuffer);

	return message;
}

static std::string Value_8Bits_AsString(int num) {
	std::bitset<8> x(num);
	return x.to_string();
}
int shift_active() {
	return GetKeyState(VK_LSHIFT) < 0 || GetKeyState(VK_RSHIFT) < 0;
}

int capital_active() {
	return (GetKeyState(VK_CAPITAL) & 1) == 1;
}




// Run program: Ctrl + F5 or Debug > Start Without Debugging menu
// Debug program: F5 or Debug > Start Debugging menu

// Tips for Getting Started: 
//   1. Use the Solution Explorer window to add/manage files
//   2. Use the Team Explorer window to connect to source control
//   3. Use the Output window to see build output and other messages
//   4. Use the Error List window to view errors
//   5. Go to Project > Add New Item to create new code files, or Project > Add Existing Item to add existing code files to the project
//   6. In the future, to open this project again, go to File > Open > Project and select the .sln file
