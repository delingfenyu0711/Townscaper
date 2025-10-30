Townscaper Project

Project Overview
This project is a grid - like grid generation and module placement system developed based on the Unity engine. It allows users to create grid structures and automatically place corresponding module components through interactive means.

Technical Environment
Unity Version: 2022.3.8f1c1

Rendering Pipeline: Universal Render Pipeline (URP 14.0.8)

Main Dependent Packages:

TextMeshPro 3.0.6

Timeline 1.7.5

Visual Scripting 1.9.0

Various Unity built - in modules (Physics, Audio, UI, etc.)

Project Structure

Townscaper/

├── Assets/

│   ├── Resources/          # Resource files

│   │   ├── Data/           # Data resources

│   │   ├── Fbx/            # FBX model resources

│   │   └── Materials/      # Material resources

│   ├── Scenes/             # Scene files

│   ├── Scripts/            # Script files

│   │   ├── GridGenerator/  # Scripts related to grid generation

│   │   └── Module/         # Scripts related to modules

│   └── Settings/           # Project setting files

├── Packages/               # Package management configuration

└── ProjectSettings/        # Project settings

Core Features

Grid Generation System:

Generate a hexagonal grid based on radius and height parameters.

Support grid smoothing processing.

Visualize grid vertices and edges.

Module Management System:

The ModuleLibrary manages different types of modules.

Automatically match and place appropriate modules according to the grid state.

Support module rotation and flipping.

Interactive Features:

Add/delete grid vertices by interacting with spheres.

Update the grid state and module display in real - time.

Usage Methods

Open the project in Unity 2022.3.8f1c1.

Load the SampleScene.

Adjust the GridGenerator component parameters in the Inspector panel:

Radius: Grid radius

Height: Grid height

CellHeight: Cell height

CellSize: Cell size

RelaxTimes: Number of grid smoothing times

Run the scene and use the following interactions:

Move the addSphere near the grid to add vertices.

Move the deleteSphere near the vertices to delete them.

Check isView, isLine, isBit to display different grid debugging information.

Script Description

GridGenerator.cs: The core class for grid generation and management.

Grid.cs: Definition of the grid data structure.

Module.cs: Definition of the module data structure.

ModuleLibrary.cs: The class for module library management.

Slot.cs: The class for managing module placement slots.

Other auxiliary classes (Vertex.cs, Quad.cs, Triangle.cs, etc.): Definitions of grid component elements.

Notes

Ensure to open the project with the specified version of Unity to avoid compatibility issues.

Module resources need to be placed in the Resources directory to ensure correct loading.

After adjusting the grid parameters, you may need to re - run the scene to make them take effect.
