# VisPr Windows Desktop Recorder
The recorder is a separate programm that can be launched on a runtime and then assist users to quickly record the steps of a process. Since the runtime is an ASP.Net application the runtime cannot record desktop interactions (specifically Keyboard input) directly. It requires a windows application to have an event queue. On top of this UiA2 and UiA3 backends need an application that uses .Net Framework 4 (or newer) to properly recognize keyboard input. 
The recorder and runtime communicate over named pipes.
 
