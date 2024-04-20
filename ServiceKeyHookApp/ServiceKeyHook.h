#pragma once
#include <WinUser.h>
#include <string>


static void InitMapKeys();
static void SetKeyInput(INPUT* pInput, WORD keyCode, DWORD dwFlags = 0);
static std::string Value_8Bits_AsString(int num);
int shift_active();
int capital_active();
static BOOL IsInjectedKey(DWORD flags);
static BOOL InjectKey(DWORD keyCode);
static LRESULT KeyboardProc(int nCode, WPARAM wParam, LPARAM lParam);
static std::string GetLastErrorAsString();
static std::vector<INPUT> BindKey(DWORD keyCode, std::string keyMacros);