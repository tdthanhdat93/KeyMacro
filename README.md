# KeyMacro
## Description
The project aims to create an application that allows users to bind keyboard inputs to macros using a low-level key hook implemented with [`SetWindowsHookEx(WH_KEYBOARD_LL)`](https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowshookexw).</br>
It consists of 3 modules:
1. Core library (ServiceKeyHook.dll) - C++
   - Implemented in C++, this library is responsible for installing the key hook, handling keystroke events, and injecting key input with defined macros.
   - The core library provides the functionality to intercept keyboard input at a low level, allowing for precise control over keystroke events.
2. Wrapper library (ServiceKeyHookWrapper.dll) - C++/CLI
   - Written in C++/CLI, the wrapper library acts as a bridge between the UI application (C#) and the core library (C++).
   - It facilitates communication between the managed and unmanaged code components, enabling seamless integration of the core functionality into the user interface.
   - The wrapper library abstracts the complexities of the core library, providing a managed interface that can be easily accessed by the UI application.
3.  UI application - C#/WPF
    - Developed using the WPF framework in C#, the UI application follows the MVVM (Model-View-ViewModel) pattern to ensure separation of concerns and maintainability.
    - The application provides a user-friendly interface for users to define macros by recording key inputs, selecting which keys are bound to macros, and managing multiple profiles.
    - Users can interact with the UI to configure and customize macro bindings, set up profiles for different use cases, and manage their macro configurations efficiently.
    - The UI application communicates with the wrapper library to access the functionality exposed by the core library, allowing users to create and manage macros seamlessly.
  
## Getting started
1. Clone the repo [KeyMacro](https://github.com/tdthanhdat93/KeyMacro.git).
2. Open `KeyMacro.sln` by Visual Studio.
3. Build solution

## How to use
1. By default, the project build output at folder `Bin`
2. Run app `KeyMacroApp.exe`
3. First need to add new profile
   ![image](https://github.com/tdthanhdat93/KeyMacro/assets/104155665/8b709837-9e79-409c-8c51-f1981105d16e)
   
4. Select keyboard button to bind macro</br>
   ![image](https://github.com/tdthanhdat93/KeyMacro/assets/104155665/adc778da-79de-49c8-a062-e90b2c1e7a8b)
   ![image](https://github.com/tdthanhdat93/KeyMacro/assets/104155665/df281317-5176-4f6c-a36c-4377a666f4b1)
   ![image](https://github.com/tdthanhdat93/KeyMacro/assets/104155665/a25504c4-9d90-457b-b5de-4045907607a4)


   
