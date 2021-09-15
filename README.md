# Searching Algorithms in C# Windows Form

In this repo, we have 4 searching algorithms that are written in C# windows form:
* BFS
* DFS
* UCS
* A*

### Note!
Since the cost of each action is equivalent to another, BFS results are as same as the UCS results.

## How it works?
In the following, we will explain the application.  

### Main Page
The main page is as follows:<br/><br/>
![1](https://user-images.githubusercontent.com/30634010/133369441-399c770a-9955-4250-be9d-e5c16d8f056d.png "Main Page")<br/><br/>
The black cells are walls, the red cell is source state and the green cell is destination state.

#### ComboBoxes
As you see, we have two comboboxes:
* Algorithm: In this combobox, we can select the desired searching algorithm.
* Color: This combobox should be explained after "Handy Pattern" button. So, don't worry about that!

#### Buttons
Also we have four buttons:
* Generate Pattern: By pressing this button, we can generate new automatic patterns.
* Handy Pattern: By pressing this button, a white page will appear that we can draw our wall cells (that are black), source state (that is red), destination state (that is green) and clear cells (that are white). To do this, we use "Color" combobox to choose the color of each cell. You can see an example page of a handy pattern bellow:<br/><br/>
![2](https://user-images.githubusercontent.com/30634010/133373994-f9c8f9ef-86aa-4a1a-a67c-87b7c5c3fe72.png "Handy Pattern")<br/><br/>
* Search: When we chose the desired algorithm, by pressing this button, the search process will be done. After that, a blue path will be drawn from the red cell to the green cell. In this blue path, some little numbers are written that is the order of each cell from source to destination. The pink cells are visited states. You can see two examples bellow:<br/><br/>
![3](https://user-images.githubusercontent.com/30634010/133375275-a85baea7-66af-421c-bbcb-de2ddf8b89e3.png)<br/><br/>
![4](https://user-images.githubusercontent.com/30634010/133375281-b37ca472-40f5-4971-997f-7d16f617f004.png)<br/><br/>
* Clear: After each searching process, we should press this button to clear path (blue cells), visited states (pink cells) and order of each path cells.

#### Note!
If no paths exist, a message box will be shown as follows:<br/><br/>
![5](https://user-images.githubusercontent.com/30634010/133375492-7c6555ed-b387-4f64-a00f-f46bda44e4dc.png)

#### Some Statistics
For each searching process, we can see the "Time of Execution" and "Opened Nodes".
