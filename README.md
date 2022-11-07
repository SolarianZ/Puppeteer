# Puppeteer

A graph based animation controller for Unity.

Still under development.


## Roadmap

Legends:
- 󠀥✅ Completed
- ▶️ In Progress
- 📅 Planned
- ❔ Undetermined
- 🔘 Canceled
- 🚫 Won't Support


### Animation Graph Editor

- Nodes
    - ✅ Pose Output Node(Graph Root Node)
    - ✅ Mixer Node
    - ✅ Layer Mixer Node
    - ✅ Clip Node
        - ✅ Optional Playback Speed
        - ✅ Optional Motion Time
    - ▶️ Blend Space 1D Node
    - ▶️ Blend Space 2D Node
    - ✅ Animation Script Node
    - ✅ Script Node
    - ✅ Sub Graph Node
    - ▶️ State Machine Node
    - ▶️ State Node(In State Machine)
    - 🔘 ~~Param Node~~ (Embed into PlayableNode)
    - 🚫 Pose Cache Node
    - 🚫 Mirror Pose Node
- State Machine
    - ▶️ Transition
        - Smooth Transition
        - Frozen Transition
- ❔ Light Theme UI
- ❔ Pose Preview
- ❔ Debug Mode


### Animation Graph Runtime

- Nodes
    - ✅ Clip Node
        - ✅ Optional Playback Speed
        - ✅ Optional Motion Time
    - ✅ Mixer Node
    - ✅ Layer Mixer Node
    - ▶️ Blend Space 1D Node
    - ▶️ Blend Space 2D Node
    - ✅ Animation Script Node
    - ✅ Script Node
    - ✅ Sub Graph Node
    - ▶️ State Machine Node
    - ▶️ State Node(In State Machine)
    - 🔘 ~~Param Node~~ (Embed into PlayableNode)
    - 🚫 Pose Cache Node
    - 🚫 Mirror Pose Node
- Runtime PlayableGraph Modification
    - 📅 Clip Replacement
    - 📅 BlendSpace1D Replacement
    - 📅 BlendSpace2D Replacement
    - 📅 SubGraph Replacement
- Events
    - 📅 Playable State Events
    - 📅 Custom Events


### Others

- ❔ IK
- ❔ Ragdoll
- ❔ Pose Matching
