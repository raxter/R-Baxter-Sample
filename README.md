Richard Baxter Code Samples
=======

This project contains some code samples of Richard Baxter

A short overview
-----------

### Serializer
Uses reflection to serialise data of a generic class instance into JSON data. Once serialized it can be deserialised back into an instance of that class. It reads each property and recursively builds up JSON representations. Similarly for deserialization. *NOTE* the SimpleJSON library is external and I did not write this.

### Ditering Image Effect (working example in Unity project)
Creates a dithered effect when applied to a camera. For each fragment/pixel and samples a random point near it. If the colour is similar enough to the source pixel's colour, then it uses the nearby colour instead. The shader has comtrols for the distance of the random sampling, how much dithering occurs, and what the colour cutoff lmit is.

### FSM - Finite State Machine
A (slightly over-engineered) finite state machine with generic type as the states (usually I use an enum). Has global or state specific callbacks for state changes as well as an Update callback that can be run each Update, FixedUpdate frame, or even every few seconds and will call a specified update function of the state that it is in.

### BeizerTools
A set of Beizer curve/path calculation tools. The code in general use tool but was made for a particle system so that it would follow a predefined bezier path.

### IntVector2/IntVector3
Similar usage to the Vector2 class but integer based instead of float. Includes expected arithmetic operators for ease of use and hashing function.

### Splat Shader (working example in Unity project)
Blends 3 different textures using an RGB splat texture to determine the blend weights.

### Extensions
A collection of extension functions that I've found useful. Includes general, easy to use code for summing values of a lists, finding angles, object instantiation and destruction and so on

