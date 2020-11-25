# MazeInWpf
This Repo contains a Mazegame written in C# with WPF. It can be played by the User with WASD or multiple algorithms can calculate the path to the finish. Panning and zooming are also available
to the user with the mouse.

The console displays the Time it took to calculate the path and how many steps are in the solution. For better accuracy each algorithm gets it's own thread.

The mazegame itself is rendered in a `writeablebitmap` for performance. Rendering each block as a rectangle is not greatly scalable due to high object counts.

Following things will be added later:

1. Autotrack for player
2. Devtool
3. Console to debugging Window (Statistics of algorithms)
4. Colorpicker
5. Mapeditor
6. Multiple maze generation algorithms
