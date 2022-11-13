# Overview

It's imperfect, but here's a simple rhythm based combat prototype. I built this as part of a potentially larger project that I'm working on. The goal was to create an action combat system that required only two inputs. I think it turned out pretty well. This prototype is no-frills. To turn it into an actual game, a lot more code will be needed. But the mechanics are mostly there, and that is good enough for now.

[Software Demo Video](https://youtu.be/ydJ8uxLp4bU)

# Development Environment

I wrote this prototype using the Unity Engine. Familiarity with the engine is assumed and necessary to get the project running or to use it in any substantial way.
The necessary packages are pretty bare-bones. I used the new input system, but even that isn't technically necessary if you have your own input system. You will need to change the privacy of a few controller member functions.

I focused on using Actions and Coroutines to decouple components from one another. I wasn't super strict about it, but doing so will allow for me to make the components easier to mix and match.

# Useful Websites

{Make a list of websites that you found helpful in this project}
* [Unity Engine Website](https://unity.com/)
* [Events & Delegates in Unity](https://gamedevbeginner.com/events-and-delegates-in-unity/)
* [Unity's New Input System](https://gamedevbeginner.com/input-in-unity-made-easy-complete-guide-to-the-new-system/)
