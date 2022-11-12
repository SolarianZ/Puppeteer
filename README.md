# Puppeteer

A graph based animation controller for Unity.

**This package is not finished and has not been carefully tested. It may be good for reference but not good for production!**

![Animation Mixer Graph Example](./Documents~/imgs/img_sample_animation_mixer_graph.png) 

![Animation State Machine Graph Example](./Documents~/imgs/img_sample_animation_state_machine_graph.png) 

## Status

Legends:
- 󠀥✅ Completed
- ▶️ In Progress
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
    - ❔ Clip Replacement
    - ❔ BlendSpace1D Replacement
    - ❔ BlendSpace2D Replacement
    - ❔ SubGraph Replacement
- Events
    - ❔ Playable State Events
    - ❔ Custom Events


### Others

- ❔ IK
- ❔ Ragdoll
- ❔ Pose Matching
