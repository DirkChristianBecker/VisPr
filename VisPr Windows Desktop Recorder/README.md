# VisPr² Windows Desktop Recorder
The recorder is a separate programm that can be launched on a runtime and then assist users to quickly record the steps of a process. Since the runtime is an ASP.Net application the runtime cannot record desktop interactions (specifically Keyboard input) directly, since only proper windows application start and maintain an event loop. Even though you can start an eventloop inside an ASP.Net application, UiA2 and UiA3 backends need an application that uses .Net Framework 4 (or newer) to properly recognize keyboard input. To resolve these version-, framework- and backend-issues the recorder exists as an independent application to allow users to record interactions with destkop applications.
The recorder and runtime communicate over named pipes.
 
