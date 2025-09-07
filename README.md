# Unity Dynamic Weather System with VFX Graph

![tornadoHomePage-compressed](https://github.com/user-attachments/assets/531a0bcd-a09f-41b6-9dee-04d80df3a0c0)

This repository contains a portfolio project demonstrating the creation of a dynamic, real-time weather system in Unity. The project's centerpiece is a stylized tornado, complemented by a full suite of atmospheric effects including wind, rain, and lightning. The entire system is driven by Unity's Visual Effect Graph and orchestrated by a central C# controller, allowing for smooth transitions from calm conditions to a full-blown storm.

**For a detailed, step-by-step breakdown of the project's development and learning outcomes, please visit the [Wiki Page](https://github.com/HangyBoi/Piece-of-Dynamic-Weather/wiki).**

---

## Features

*   **Dynamic Weather Control:** A single master slider (`stormIntensity`) in the central `WeatherOrchestrator` script controls every aspect of the storm in real-time.
*   **Stylized Tornado:** A multi-layered vortex effect built with VFX Graph, featuring a custom 3D model, materials powered by Shader Graph, and a C# script that applies physics-based attraction forces to surrounding objects.
*   **Atmospheric VFX Suite:**
    *   **Versatile Wind:** Directional and circular wind trails built on a performant leader-follower particle system.
    *   **GPU-Accelerated Rain:** A high-density rain effect that responds to storm intensity.
    *   **Procedural Lightning:** Two layers of lightning effects repurposed from the core wind system.
    *   **Ambient Dust:** A localized dust cloud that adds texture and grounds the tornado.
*   **Modular and Organized:** The project is structured with clear separation between VFX assets, control scripts, and scene setup, making it easy to understand and navigate.

![SliderTornado-ezgif com-optimize](https://github.com/user-attachments/assets/21b61795-3a99-4737-8b3b-7a1f524ba7c9)

## Technologies Used

*   **Engine:** Unity 6.1 (6000.1.14f1) (URP)
*   **Core Graphics:** Visual Effect Graph, Shader Graph
*   **Scripting:** C#
*   **3D Modeling:** Blender (for the custom tornado mesh)
*   **Animation Helper:** iTween (for tornado movement)

## How to Use

1.  **Clone the Repository:**
    ```bash
    git clone https://github.com/HangyBoi/Piece-of-Dynamic-Weather.git
    ```
2.  **Open in Unity Hub:** Open the project using Unity Hub with the specified Unity version (or newer). The editor will import the project and its dependencies.
3.  **Explore the Main Scene:** The primary showcase scene can be found at `Assets/Scenes/MainScene.unity` (or your scene's actual path).
4.  **Interact with the System:**
    *   Select the `_WeatherOrchestrator` GameObject in the Hierarchy.
    *   In the Inspector, find the `Storm Intensity` slider under the "Master Control" header.
    *   Press Play in the editor.
    *   Drag the `Storm Intensity` slider from 0 to 1 to see the entire weather system transition dynamically.

## Acknowledgements

This project would not have been possible without the incredible tutorials and insights from the online VFX community. A special thank you to the following creators and resources for their invaluable guidance:

*   **Gabriel Aguiar:** His in-depth tutorials on YouTube were the primary source of knowledge for advanced VFX Graph techniques, particularly for creating complex vortex and wind effects. [Gabriel Aguiar's YouTube Channel](https://www.youtube.com/@GabrielAguiarProd)
*   **Unity Official Resources:** The official Unity documentation and example projects provided a solid foundation for understanding the core principles of VFX Graph.
*   **The broader VFX community on YouTube and Unity Forums:** Countless smaller tutorials and forum posts helped solve specific technical hurdles throughout the development process.
